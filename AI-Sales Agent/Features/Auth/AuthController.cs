using AI_Sales_Agent.Abstractions;
using AI_Sales_Agent.Features.Auth.Account;
using AI_Sales_Agent.Features.Auth.ForgotPassword;
using AI_Sales_Agent.Features.Auth.Login;
using AI_Sales_Agent.Features.Auth.Logout;
using AI_Sales_Agent.Features.Auth.Register;
using AI_Sales_Agent.Features.Auth.RefreshToken;
using AI_Sales_Agent.Features.Auth.ResetPassword;
using AI_Sales_Agent.Features.Auth.VerifyEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI_Sales_Agent.Features.Auth
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return ToActionResult(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResult>> Login(LoginCommand command, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(command, cancellationToken));
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResult>> RefreshToken(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(command, cancellationToken));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return ToActionResult(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return ToActionResult(result);
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] Guid userId, [FromQuery] string token, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new VerifyEmailCommand(userId, token), cancellationToken);
            return ToActionResult(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new LogoutCommand(), cancellationToken);
            return ToActionResult(result);
        }

        [Authorize]
        [HttpGet("account")]
        public async Task<ActionResult<AccountResponse>> Account(CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new GetAccountQuery(), cancellationToken));
        }

        private static IActionResult ToActionResult(ApiResult result)
        {
            return result.Succeeded ? new OkObjectResult(result) : new BadRequestObjectResult(result);
        }
    }
}
