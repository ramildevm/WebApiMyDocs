using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiMyDocs.Models
{
    public partial class Template
    {
        public Template()
        {
            TemplateDocuments = new HashSet<TemplateDocument>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public DateTime? UpdateTime { get; set; }

        public virtual User User { get; set; }
        public virtual TemplateObject TemplateObject { get; set; }
        public virtual ICollection<TemplateDocument> TemplateDocuments { get; set; }
    }
}
