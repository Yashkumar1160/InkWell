namespace InkWell.Comment.DTOs
{
    public class ErrorResponseDTO
    {
        public string Timestamp { get; set; }
        public int Status { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
        public string Path { get; set; }
    }
}
