using AI_Sales_Agent.Domain;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace AI_Sales_Agent.Features.Profile.UploadProfilePicture
{
    public class UploadProfilePictureCommandHandler : IRequestHandler<UploadProfilePictureCommand, UploadProfilePictureResult>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _environment;

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public UploadProfilePictureCommandHandler(
            ICurrentUserService currentUserService,
            UserManager<User> userManager,
            IWebHostEnvironment environment)
        {
            _currentUserService = currentUserService;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<UploadProfilePictureResult> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId is not { } userId)
            {
                return new UploadProfilePictureResult(false, "User is not authenticated.");
            }

            var file = request.File;
            if (file == null || file.Length == 0)
            {
                return new UploadProfilePictureResult(false, "No file uploaded.");
            }

            if (file.Length > MaxFileSizeBytes)
            {
                return new UploadProfilePictureResult(false, "File size exceeds the 5 MB limit.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                return new UploadProfilePictureResult(false, "Unsupported file format. Allowed formats: .jpg, .jpeg, .png, .webp, .gif");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                return new UploadProfilePictureResult(false, "User not found.");
            }

            // Create target folder in wwwroot/uploads/profiles/
            var webRootPath = _environment.WebRootPath;
            if (string.IsNullOrEmpty(webRootPath))
            {
                webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            var uploadsFolder = Path.Combine(webRootPath, "uploads", "profiles");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Remove existing profile picture file if present
            if (!string.IsNullOrWhiteSpace(user.ProfilePictureUrl))
            {
                var relativePath = user.ProfilePictureUrl.TrimStart('/');
                var existingFilePath = Path.Combine(webRootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(existingFilePath))
                {
                    try { File.Delete(existingFilePath); } catch { /* Ignore if locked */ }
                }
            }

            // Generate unique filename
            var fileName = $"{userId}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var relativeUrl = $"/uploads/profiles/{fileName}";
            user.ProfilePictureUrl = relativeUrl;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return new UploadProfilePictureResult(false, "Failed to save profile picture URL.");
            }

            return new UploadProfilePictureResult(true, "Profile picture uploaded successfully.", relativeUrl);
        }
    }
}
