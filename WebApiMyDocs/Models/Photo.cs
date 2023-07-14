using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiMyDocs.Models
{
    public partial class Photo
    {
        public Guid Id { get; set; }
        [JsonProperty("Image64")]
        public string Image { get; set; }
        public Guid CollectionId { get; set; }
        public DateTime? UpdateTime { get; set; }
        [JsonIgnore]
        public virtual Item Collection { get; set; }
    }
}
