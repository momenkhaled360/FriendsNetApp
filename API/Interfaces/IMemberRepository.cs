using API.Entities;
using API.Helpers;
using System.Collections.Generic;

namespace API.Interfaces
{
    public interface IMemberRepository
    {
        void Update(Member member);
        Task<bool> SaveAllAsync();
        //Task<IReadOnlyList<Member>> GetMembersAsync();
        Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams);
        Task<Member?> GetMemberByIdAsync(string id);
        Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId);
        Task<Member?> GetMemberForUpdate(string id);
    }
}
