using Egoal.Application.Services;
using Egoal.Domain.Repositories;
using Egoal.Extensions;
using Egoal.Face;
using Egoal.Face.Dto;
using Egoal.IO;
using Egoal.Runtime.Session;
using Egoal.Settings;
using Egoal.Tickets.Dto;
using Egoal.TicketTypes;
using Egoal.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Egoal.Tickets
{
    public class FaceAppService : ApplicationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ISession _session;
        private readonly ParkOptions _parkOptions;
        private readonly ITicketSaleDomainService _ticketSaleDomainService;
        private readonly ITicketSaleRepository _ticketSaleRepository;
        private readonly IRepository<TicketSalePhoto, long> _ticketSalePhotoRepository;
        private readonly IRepository<TicketSalePhotoQueque, long> _ticketSalePhotoQueueRepository;

        public FaceAppService(
            IServiceProvider serviceProvider,
            IHostingEnvironment hostingEnvironment,
            ISession session,
            IOptions<ParkOptions> options,
            ITicketSaleDomainService ticketSaleDomainService,
            ITicketSaleRepository ticketSaleRepository,
            IRepository<TicketSalePhoto, long> ticketSalePhotoRepository,
            IRepository<TicketSalePhotoQueque, long> ticketSalePhotoQueueRepository)
        {
            _serviceProvider = serviceProvider;
            _hostingEnvironment = hostingEnvironment;
            _session = session;
            _parkOptions = options.Value;
            _ticketSaleDomainService = ticketSaleDomainService;
            _ticketSaleRepository = ticketSaleRepository;
            _ticketSalePhotoRepository = ticketSalePhotoRepository;
            _ticketSalePhotoQueueRepository = ticketSalePhotoQueueRepository;
        }

        public async Task<long> EnrollFaceAsync(EnrollFaceInput input)
        {
            IFaceService faceService = _serviceProvider.GetRequiredService<IFaceService>();
            var validateOutput = await faceService.ValidateFaceAsync(new ValidateFaceInput { Photo = input.Photo });

            var ticketSale = await _ticketSaleRepository.FirstOrDefaultAsync(input.TicketId);
            if (ticketSale == null)
            {
                throw new UserFriendlyException($"TicketId:{input.TicketId}不存在");
            }

            if (!await _ticketSaleDomainService.AllowEnrollFaceAsync(ticketSale))
            {
                throw new UserFriendlyException("此门票已不支持登记人脸");
            }

            var surplusQuantity = await _ticketSaleDomainService.GetSurplusNumAsync(ticketSale);
            if (surplusQuantity <= 0)
            {
                throw new UserFriendlyException("可用数量不足");
            }
            if (ticketSale.CheckTypeId == CheckType.家庭套票)
            {
                surplusQuantity = surplusQuantity * ticketSale.GetCheckNum();
            }

            var photoQuantity = await _ticketSaleDomainService.GetPhotoQuantityAsync(ticketSale);
            if (surplusQuantity <= photoQuantity)
            {
                throw new UserFriendlyException("已登记人脸");
            }

            var ticketSalePhoto = ticketSale.MapToTicketSalePhoto();
            ticketSalePhoto.Photo = input.Photo;
            ticketSalePhoto.PhotoTemplate = validateOutput.Template;
            ticketSalePhoto.RegSourceId = input.RegSource;

            var fileName = await SaveFaceToDirectoryAsync(input.Photo);
            ticketSalePhoto.PhotoUrl = _parkOptions.WebApiUrl.UrlCombine(fileName);

            var id = await _ticketSalePhotoRepository.InsertAndGetIdAsync(ticketSalePhoto);

            var queue = new TicketSalePhotoQueque();
            queue.OpTypeId = FaceRegOpType.添加;
            queue.TicketSalePhotoId = id;
            queue.TicketTypeId = ticketSale.TicketTypeId;
            queue.CTime = ticketSalePhoto.Ctime;
            await _ticketSalePhotoQueueRepository.InsertAsync(queue);

            ticketSale.PhotoBindFlag = true;
            ticketSale.PhotoBindTime = DateTime.Now;
            ticketSale.PhotoBindType = (int)input.RegSource;
            if (!ticketSale.MemberId.HasValue)
            {
                ticketSale.MemberId = _session.MemberId;
            }

            return id;
        }

        private async Task<string> SaveFaceToDirectoryAsync(byte[] photo)
        {
            var directory = TicketSalePhoto.GetWebSavePath(_hostingEnvironment.ContentRootPath);
            DirectoryHelper.CreateIfNotExists(directory);
            var fileName = $"{Path.GetRandomFileName()}.jpeg";
            var filePath = Path.Combine(directory, fileName);
            using (var ms = new MemoryStream(photo))
            {
                using (var file = File.Create(filePath))
                {
                    await ms.CopyToAsync(file);
                }
            }

            return TicketSalePhoto.FaceDirectory.UrlCombine(fileName);
        }

        public async Task DeleteFaceAsync(long id)
        {

            var photo = await _ticketSalePhotoRepository.GetAll()
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();
            if (photo == null)
            {
                throw new UserFriendlyException("所选人脸不存在");
            }

            TicketSale ticketSale = await _ticketSaleRepository.FirstOrDefaultAsync(t => t.Id == photo.TicketId.Value);
            if (ticketSale == null)
            {
                throw new UserFriendlyException("对应门票不存在");
            }
            if (ticketSale.TicketStatusId != TicketStatus.已售)
            {
                throw new UserFriendlyException("此门票已用，不能删除人脸");
            }

            var photoQuantity = await _ticketSaleDomainService.GetPhotoQuantityAsync(ticketSale);
            if (photoQuantity == 1)
            {
                ticketSale.PhotoBindFlag = false;
                ticketSale.PhotoBindTime = null;
                ticketSale.PhotoBindType = null;
            }

            var queue = new TicketSalePhotoQueque();
            queue.OpTypeId = FaceRegOpType.删除;
            queue.TicketSalePhotoId = id;
            queue.TicketTypeId = photo.TicketTypeId;
            queue.CTime = DateTime.Now;
            await _ticketSalePhotoQueueRepository.InsertAsync(queue);

            await _ticketSalePhotoRepository.DeleteAsync(photo);
        }
    }
}
