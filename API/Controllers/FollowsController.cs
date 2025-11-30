using System.Security.Claims;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpPost("toggle-Follow/{targetMemberId}")]
        public async Task<ActionResult> ToggleFollow(int targetMemberId)
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (memberId == null) return BadRequest("no id found in token");
            int sourceUserId = int.Parse(memberId);

            var targetMember = await unitOfWork.MemberRepository.GetMemberByIdAsync(targetMemberId);
            if (targetMember == null) return NotFound("Target member not found");
            
            var existingFollow = await unitOfWork.FollowRepository.GetFollowAsync(sourceUserId, targetMemberId);
            if (existingFollow == null)
            {
                var newFollow = new Entities.MemberFollow
                {
                    SourceMemberId = sourceUserId,
                    TargetMemberId = targetMemberId
                };
                unitOfWork.FollowRepository.AddFollow(newFollow);
                if (await unitOfWork.Complete())
                    return Ok(new { message = "Followed successfully" });
                return BadRequest("Failed to follow member");
            }
            else
            {
                unitOfWork.FollowRepository.RemoveFollow(existingFollow);
                if (await unitOfWork.Complete())
                    return Ok(new { message = "Unfollowed successfully" });
                return BadRequest("Failed to unfollow member");
            }
        }

        [HttpGet("Follows-Ids")]
        public async Task<ActionResult<PaginatedResult<int>>> GetFollowsIds([FromQuery] FollowParams followParams)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null) return BadRequest("no id found in token");
            int memberId = int.Parse(id);

            try
            {
                var follows = await unitOfWork.FollowRepository.GetAllFollowsIdsAsync(memberId, followParams);
                return Ok(follows);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Follows-Members")]
        public async Task<ActionResult<PaginatedResult<Member>>> GetFollowsMembers([FromQuery] FollowParams followParams)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null) return BadRequest("no id found in token");
            int memberId = int.Parse(id);

            try
            {
                var follows = await unitOfWork.FollowRepository.GetAllFollowsAsync(memberId, followParams);
                return Ok(follows);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    } 
}