using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController(ILikedRepository likedRepository) : BaseApiController
    {
        [HttpPost("{targetMemberId}")]
        public async Task<ActionResult> ToggleLike(string targetMemberId)
        {
            var sourceMemberId = User.GetMemberId();
            
            if(sourceMemberId == targetMemberId) return BadRequest();

            var existingLike =  await likedRepository.GetMemberLike(sourceMemberId, targetMemberId);
            if(existingLike == null)
            {
                var like = new MemberLike
                {
                    SourceMemberId = sourceMemberId,
                    TargetMemberId = targetMemberId,
                };

                likedRepository.AddLike(like);
            }
            else
            {
                likedRepository.DeleteLike(existingLike);
            }

            var result = await likedRepository.SaveAllChanges();

            if (result) return Ok();

            return BadRequest("No changes were made");
        }

        [HttpGet("list")]
        public async Task<IReadOnlyList<string>> GetCurrentMemberLikeIds()
        {
            return await likedRepository.GetCurrentMemberLikeIds(User.GetMemberId());
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<Member>>> GetMemberLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.MemberId = User.GetMemberId();
            var members = await likedRepository.GetMemberLikes(likesParams);
            return Ok(members);
        }
    }
}
