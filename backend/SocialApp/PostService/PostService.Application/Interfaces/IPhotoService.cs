using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace PostService.Application.Interfaces
{
	public class PhotoUploadResult
	{
		public string Url { get; set; } = string.Empty;
		public string PublicId { get; set; } = string.Empty;
	}

	public interface IPhotoService
	{
		Task<PhotoUploadResult?> UploadPhotoAsync(IFormFile file);
		Task<bool> DeletePhotoAsync(string publicId);
	}
}
