using Egoal.Application.Services;
using Egoal.Face.Dto;
using Egoal.Net.Wcf;
using Egoal.UI;
using Face;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Egoal.Face
{
    public class FaceService : ApplicationService, IFaceService
    {
        private readonly IWcfProxy<IZkLiveFaceService> _clientProxy;
        private readonly FaceOptions _options;

        public FaceService(IOptions<FaceOptions> options)
        {
            _options = options.Value;

            _clientProxy = new WcfProxy<IZkLiveFaceServiceChannel>(_options.FaceServerUrl);
        }

        public async Task<ValidateFaceOutput> ValidateFaceAsync(ValidateFaceInput input)
        {
            var request = new ZKLiveFaceServiceRequest();
            request.Method = "Score";
            request.ImageData = input.Photo;

            var response = await _clientProxy.Execute(client => client.HandlerZKLiveFaceRequestAsync(request));
            if (response == null)
            {
                throw new UserFriendlyException("人脸验证失败");
            }

            if (!response.Result.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
            {
                throw new UserFriendlyException(response.ErrorDesc);
            }

            return new ValidateFaceOutput { Template = response.PhotoTemplate };
        }
    }
}
