using Server.Core.Entities;

namespace Server.API.PostModel
{
    public class PermissionPostModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Role> Roles { get; set; }
    }
}
