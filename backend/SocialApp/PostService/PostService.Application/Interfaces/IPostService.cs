using System;
using System.Collections.Generic;
using System.Text;
using PostService.Application.DTO;

namespace PostService.Application.Interfaces
{
	public interface IPostService
	{
		Task<IEnumerable<PostResponseDto>> GetAllPostsAsync();
		Task<PostResponseDto?> GetPostByIdAsync(Guid id);
		Task<IEnumerable<PostResponseDto>> GetPostsByUserIdAsync(Guid userId);
		Task<PostResponseDto> CreatePostAsync(CreatePostDto dto, Guid userId, string username);
		Task<PostResponseDto?> UpdatePostAsync(Guid id, UpdatePostDto dto, Guid userId);
		Task<bool> DeletePostAsync(Guid id, Guid userId);
	}
}
