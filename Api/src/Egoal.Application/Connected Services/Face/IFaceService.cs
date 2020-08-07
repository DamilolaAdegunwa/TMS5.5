using Egoal.Face.Dto;
using System.Threading.Tasks;

namespace Egoal.Face
{
    public interface IFaceService
    {
        Task<ValidateFaceOutput> ValidateFaceAsync(ValidateFaceInput input);
    }
}
