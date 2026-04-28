namespace TSG_Website.DTOs
{
	public class CreateBlogPostDto
	{
		public string Title { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
		public string? ImageUrl { get; set; }
	}
}