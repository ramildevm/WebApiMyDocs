using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiMyDocs.Models
{
    public partial class TemplateObject
    {
        public TemplateObject()
        {
            TemplateDocumentData = new HashSet<TemplateDocumentDatum>();
        }

        public Guid Id { get; set; }
        public int Position { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public Guid TemplateId { get; set; }
        public DateTime? UpdateTime { get; set; }

        [JsonIgnore]
        public virtual Template IdNavigation { get; set; }
        [JsonIgnore]
        public virtual ICollection<TemplateDocumentDatum> TemplateDocumentData { get; set; }
    }
}
