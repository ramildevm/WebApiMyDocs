using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiMyDocs.Models
{
    public partial class Photo
    {
        public Guid Id { get; set; }
        public byte[] Image { get; set; }
        public string Image64 { get; set; }
        public Guid CollectionId { get; set; }
        public DateTime? UpdateTime { get; set; }
        [JsonIgnore]
        public virtual Item Collection { get; set; }
    }
}
