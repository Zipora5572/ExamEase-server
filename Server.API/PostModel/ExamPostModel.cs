using Server.Core.Entities;

namespace Server.API.PostModel
{
    public class ExamPostModel
    {

        public IFormFile File { get; set; }
        public int UserId { get; set; }
        public int? FolderId { get; set; }
        public string Lang { get; set; }

    }
}
