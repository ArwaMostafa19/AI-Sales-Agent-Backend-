using AI_Sales_Agent.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AI_Sales_Agent.Features.Profile.UploadProfilePicture
{
    public record UploadProfilePictureCommand(IFormFile File) : IRequest<UploadProfilePictureResult>;

    public record UploadProfilePictureResult(bool Succeeded, string Message, string? ProfilePictureUrl = null);
}
