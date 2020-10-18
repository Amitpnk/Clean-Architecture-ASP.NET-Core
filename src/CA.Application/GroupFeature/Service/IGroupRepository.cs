using System.Threading.Tasks;

namespace CA.Application.GroupFeature.Service
{
    public interface IGroupRepository //: IGenericRepositoryAsync<Group>
    {
        Task<bool> IsUniqueGroupNameAsync(string barcode);
    }
}
