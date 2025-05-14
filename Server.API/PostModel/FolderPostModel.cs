namespace Server.API.PostModel
{
    public class FolderPostModel
    {
        public int UserId { get; set; } 
        public string Name { get; set; }
        public int? ParentFolderId { get; set; }
    }
}
