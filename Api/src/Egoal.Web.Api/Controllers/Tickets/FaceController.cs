using Egoal.Application.Services.Dto;
using Egoal.Mvc.ModelBinding;
using Egoal.Tickets;
using Egoal.Tickets.Dto;
using Egoal.Upload;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Egoal.Web.Api.Controllers.Tickets
{
    public class FaceController : TmsControllerBase
    {
        private readonly FaceAppService _faceAppService;

        public FaceController(FaceAppService faceAppService)
        {
            _faceAppService = faceAppService;
        }

        [Route("/Api/Ticket/EnrollFaceAsync")]
        [HttpPost]
        [DisableFormValueModelBinding]
        public async Task<long> EnrollFaceAsync()
        {
            var input = new EnrollFaceInput();

            var formValueProvider = await UploadHelper.UploadAsync(Request, async (stream, ext) =>
            {
                using (var targetStream = new System.IO.MemoryStream())
                {
                    await stream.CopyToAsync(targetStream);

                    input.Photo = targetStream.ToArray();
                }
            });

            await UpdateModelAsync(input, string.Empty, formValueProvider);

            return await _faceAppService.EnrollFaceAsync(input);
        }

        [Route("/Api/Ticket/DeleteFaceAsync")]
        [HttpPost]
        public async Task DeleteFaceAsync(EntityDto<long> input)
        {
            await _faceAppService.DeleteFaceAsync(input.Id);
        }
    }
}
