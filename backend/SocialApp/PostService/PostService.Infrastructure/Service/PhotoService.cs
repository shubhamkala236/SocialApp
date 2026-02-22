using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PostService.Application.Interfaces;
using PostService.Application.Settings;

namespace PostService.Infrastructure.Service
{
	public class PhotoService : IPhotoService
	{
		private readonly Cloudinary _cloudinary;

		public PhotoService(IOptions<CloudinarySettings> config)
		{
			var acc = new Account(
				config.Value.CloudName,
				config.Value.ApiKey,
				config.Value.ApiSecret
			);
			_cloudinary = new Cloudinary(acc);
		}

		public async Task<PhotoUploadResult?> UploadPhotoAsync(IFormFile file)
		{
			if (file.Length <= 0) return null;

			await using var stream = file.OpenReadStream();

			var uploadParams = new ImageUploadParams
			{
				File = new FileDescription(file.FileName, stream),
				Folder = "social_app/posts",
				Transformation = new Transformation()
					.Width(1200).Height(630)
					.Crop("limit")           // won't upscale, just limits max size
					.Quality("auto")
					.FetchFormat("auto")     // serves webp to supported browsers
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
			var deleteParams = new DeletionParams(publicId);
			var result = await _cloudinary.DestroyAsync(deleteParams);
			return result.Result == "ok";
		}
	}
}