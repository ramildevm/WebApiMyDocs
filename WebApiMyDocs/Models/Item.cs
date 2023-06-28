using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiMyDocs.Models
{
    public partial class Item
    {
        public Item()
        {
            Photos = new HashSet<Photo>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        [JsonIgnore]
        public byte[] Image { get; set; }
        public string Image64 { get; set; }
        public int Priority { get; set; }
        public int IsHidden { get; set; }
        public DateTime DateCreation { get; set; }
        public Guid FolderId { get; set; }
        public int UserId { get; set; }
        public DateTime? UpdateTime { get; set; }

        public virtual User User { get; set; }
        public virtual CreditCard CreditCard { get; set; }
        public virtual Inn Inn { get; set; }
        public virtual Passport Passport { get; set; }
        public virtual Poli Poli { get; set; }
        public virtual Snil Snil { get; set; }
        public virtual TemplateDocument TemplateDocument { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
    }
}
