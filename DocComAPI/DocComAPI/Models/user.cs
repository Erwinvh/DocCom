using System.Text.Json.Serialization;

namespace DocComAPI.Models
{
    public class user
    {

        public Guid id { get; set; }

        public string username { get; set; }

        public string email { get; set; }

        public int securityLevel { get; set; }

        [JsonIgnore]
        public string password { get; set; }

    }
}
