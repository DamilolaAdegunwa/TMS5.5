using Egoal.Domain.Repositories;
using Egoal.Domain.Uow;
using Egoal.Extensions;
using Egoal.Redis;
using Egoal.Threading.BackgroundWorkers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Egoal.Tickets
{
    public class ClearFaceWorker : PeriodicBackgroundWorkerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly RedisManager _redisManager;
        private readonly IServiceProvider _serviceProvider;

        public ClearFaceWorker(
            ILogger<ClearFaceWorker> logger,
            IHostingEnvironment hostingEnvironment,
            RedisManager redisManager,
            IServiceProvider serviceProvider)
            : base(logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _redisManager = redisManager;
            _serviceProvider = serviceProvider;

            Period = TimeSpan.FromMinutes(10);
        }

        protected override async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            var now = DateTime.Now;
            if (now.Hour != 4) return;

            var directory = TicketSalePhoto.GetWebSavePath(_hostingEnvironment.ContentRootPath);
            if (!Directory.Exists(directory)) return;

            var today = now.ToString(DateTimeExtensions.DateFormat);

            var database = _redisManager.GetDatabase();
            string key = "LastClearFaceDate";
            var lastDate = await database.StringGetAsync(key);
            if (lastDate == today) return;

            using (var scope = _serviceProvider.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var ticketSalePhotoRepository = serviceProvider.GetRequiredService<IRepository<TicketSalePhoto, long>>();

                List<string> photoUrls = null;

                var unitOfWorkManager = serviceProvider.GetRequiredService<IUnitOfWorkManager>();
                using (var uow = unitOfWorkManager.Begin())
                {
                    photoUrls = await ticketSalePhotoRepository.GetAll()
                    .Where(t => t.PhotoUrl != null)
                    .Select(t => t.PhotoUrl)
                    .ToListAsync();

                    await uow.CompleteAsync();
                }

                List<string> photos = new List<string>();
                foreach (var photoUrl in photoUrls)
                {
                    if (photoUrl.IsNullOrEmpty()) continue;

                    var parties = photoUrl.Split(new[] { TicketSalePhoto.FaceDirectory }, StringSplitOptions.RemoveEmptyEntries);
                    if (parties.Length != 2) continue;

                    photos.Add(parties[1].TrimStart('/'));
                }

                var files = Directory.GetFiles(directory);
                foreach (var file in files)
                {
                    var parties = file.Split(new[] { TicketSalePhoto.FaceDirectory }, StringSplitOptions.RemoveEmptyEntries);
                    if (parties.Length != 2) continue;

                    var fileName = parties[1].TrimStart('\\');
                    if (!photos.Any(p => p == fileName))
                    {
                        File.Delete(file);
                    }
                }
            }

            await database.StringSetAsync(key, today);
        }
    }
}
