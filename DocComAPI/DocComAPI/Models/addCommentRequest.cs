namespace DocComAPI.Models
{
    public class addCommentRequest
    {

        public Guid poster { get; set; }

        public Guid subject { get; set; }

        public string content { get; set; }

    }
}
