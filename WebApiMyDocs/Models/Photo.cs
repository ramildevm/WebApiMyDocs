using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiMyDocs.Models
{
    public partial class Photo
    {
        public Guid Id { get; set; }
        public byte[] Image { get; set; }
        public Guid CollectionId { get; set; }
        public DateTime? UpdateTime { get; set; }

        public virtual Item Collection { get; set; }
    }
}
