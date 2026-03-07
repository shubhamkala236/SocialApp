export interface PostInteraction {
  postId: string;
  likesCount: number;
  isLiked: boolean;
  isSaved: boolean;
}

export interface LikeResult {
  isLiked: boolean;
  likesCount: number;
}

export interface SavePostRequest {
  postId: string;
  postTitle: string;
  postContent: string;
  postUsername: string;
  postImageUrl?: string;
  postCreatedAt: string;
}

export interface SavedPost {
  postId: string;
  postTitle: string;
  postContent: string;
  postUsername: string;
  postImageUrl?: string;
  postCreatedAt: string;
  savedAt: string;
}