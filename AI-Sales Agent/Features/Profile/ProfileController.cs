using AI_Sales_Agent.Abstractions;
using AI_Sales_Agent.Features.Profile.ChangePassword;
using AI_Sales_Agent.Features.Profile.GetProfile;
using AI_Sales_Agent.Features.Profile.UpdateProfile;
using AI_Sales_Agent.Features.Profile.UploadProfilePicture;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AI_Sales_Agent.Features.Profile
{
    [ApiController]
    [Authorize]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly ISender _sender;

        public ProfileController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult<ProfileResponseDto>> GetProfile(CancellationToken cancellationToken)
        {
            var profile = await _sender.Send(new GetProfileQuery(), cancellationToken);
            return Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdateProfileCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return ToActionResult(result);
        }

        [HttpPost("picture")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadProfilePicture([FromForm] UploadProfilePictureRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new UploadProfilePictureCommand(request.File), cancellationToken);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResult.Failure(result.Message));
            }
            return Ok(new { message = result.Message, profilePictureUrl = result.ProfilePictureUrl });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return ToActionResult(result);
        }

        private static IActionResult ToActionResult(ApiResult result)
        {
            return result.Succeeded ? new OkObjectResult(result) : new BadRequestObjectResult(result);
        }
    }

    public class UploadProfilePictureRequest
    {
        public required IFormFile File { get; set; }
    }
}
