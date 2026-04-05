using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository(AppDbContext context) : ILikedRepository
    {

        public void AddLike(MemberLike like)
        {
            context.MemberLikes.Add(like);
        }

        public void DeleteLike (MemberLike like)
        {
            context.MemberLikes.Remove(like);
        }

        public async Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId)
        {
            return await context.MemberLikes
                .Where(x => x.SourceMemberId == memberId)
                .Select(x => x.TargetMemberId)
                .ToListAsync();
        }

        public async Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId)
        {
            return await context.MemberLikes
                .FirstOrDefaultAsync(x =>
                x.SourceMemberId == sourceMemberId &&
                x.TargetMemberId == targetMemberId);
        }

        public async Task<PaginatedResult<Member>> GetMemberLikes(LikesParams likesParams)
        {
            var query = context.MemberLikes.AsQueryable();

            IQueryable<Member> membersQuery;

            if (likesParams.Predicated == "liked")
            {
                membersQuery = query
                    .Where(x => x.SourceMemberId == likesParams.MemberId)
                    .Select(x => x.TargetMember);
            }
            else if (likesParams.Predicated == "likedBy")
            {
                membersQuery = query
                    .Where(x => x.TargetMemberId == likesParams.MemberId)
                    .Select(x => x.SourceMember);
            }
            else
            {
                var likeIds = await GetCurrentMemberLikeIds(likesParams.MemberId);

                membersQuery = query
                    .Where(x => x.TargetMemberId == likesParams.MemberId
                        && likeIds.Contains(x.SourceMemberId))
                    .Select(x => x.SourceMember);
            }

            return await PaginationHelper.CreateAsync(
                membersQuery,
                likesParams.PageNumber,
                likesParams.PageSize
            );
        }
        public async Task<bool> SaveAllChanges()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
