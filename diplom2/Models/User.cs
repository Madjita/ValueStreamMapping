using System;
using System.Text.Json.Serialization;

namespace auntification.Models
{
    public enum UserRole
    {
        Free = 1,
        Busy = 2
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }

        public UserRole UserRole { get; set; }

        public string Email { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
    }
}
