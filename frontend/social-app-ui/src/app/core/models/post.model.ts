export interface Post {
  id: string;
  title: string;
  content: string;
  userId: string;
  username: string;
  imageUrl?: string;
  createdAt: string;
  updatedAt?: string;
  userAvatarUrl?: string;
}

export interface CreatePostRequest {
  title: string;
  content: string;
  image?: File;
}

export interface UpdatePostRequest {
  title: string;
  content: string;
  image?: File;
}