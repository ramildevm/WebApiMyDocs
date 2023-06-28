using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiMyDocs.Models
{
    public partial class TemplateDocument
    {
        public TemplateDocument()
        {
            TemplateDocumentData = new HashSet<TemplateDocumentDatum>();
        }

        public Guid Id { get; set; }
        public Guid TemplateId { get; set; }
        public DateTime? UpdateTime { get; set; }

        [JsonIgnore]
        public virtual Item IdNavigation { get; set; }
        [JsonIgnore]
        public virtual Template Template { get; set; }
        [JsonIgnore]
        public virtual ICollection<TemplateDocumentDatum> TemplateDocumentData { get; set; }
    }
}
