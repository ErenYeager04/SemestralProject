using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SemestralProject.Entities
{
    internal class UserTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        // relationships
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
    }
}
