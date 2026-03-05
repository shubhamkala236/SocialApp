using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using UserService.Application;
using UserService.Application.Interfaces;

namespace UserService.Infrastructure.Services
{
	public class PhotoService : IPhotoService
	{
		private readonly Cloudinary _cloudinary;

		public PhotoService(IOptions<CloudinarySettings> config)
		{
			var acc = new Account(
				config.Value.CloudName,
				config.Value.ApiKey,
				config.Value.ApiSecret);
			_cloudinary = new Cloudinary(acc);
		}

		public async Task<PhotoUploadResult?> UploadPhotoAsync(IFormFile file)
		{
			if (file.Length <= 0) return null;

			await using var stream = file.OpenReadStream();

			var uploadParams = new ImageUploadParams
			{
				File = new FileDescription(file.FileName, stream),
				Folder = "social_app/avatars",
				Transformation = new Transformation()
					.Width(300).Height(300)
					.Crop("fill")
					.Gravity("face")
					.Quality("auto")
					.FetchFormat("auto")
			};

			var result = await _cloudinary.UploadAsync(uploadParams);
			if (result.Error is not null) return null;

			return new PhotoUploadResult
			{
				Url = result.SecureUrl.ToString(),
				PublicId = result.PublicId
			};
		}

		public async Task<bool> DeletePhotoAsync(string publicId)
		{
			var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId));
			return result.Result == "ok";
		}
	}
}
