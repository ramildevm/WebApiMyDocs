using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiMyDocs.Models
{
    public partial class User
    {
        public User()
        {
            Items = new HashSet<Item>();
            Templates = new HashSet<Template>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }
        [JsonIgnore]
        public byte[] Photo { get; set; }
        public string Photo64 { get; set; }
        public string AccessCode { get; set; }
        public DateTime? UpdateTime { get; set; }

        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<Template> Templates { get; set; }
    }
}
