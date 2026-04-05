using API.Entities;
using API.Helpers;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface ILikedRepository
    {
        Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId);
        Task<PaginatedResult<Member>> GetMemberLikes(LikesParams likesParams);
        Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId);
        void DeleteLike(MemberLike like);
        void AddLike(MemberLike like);

        Task<bool> SaveAllChanges();

    }
}
