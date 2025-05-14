using Server.Core.Entities;

namespace Server.API.PostModel
{
    public class RolePostModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
