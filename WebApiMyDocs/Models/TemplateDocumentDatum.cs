﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiMyDocs.Models
{
    public partial class TemplateDocumentDatum
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public Guid TemplateObjectId { get; set; }
        public Guid TemplateDocumentId { get; set; }
        public DateTime? UpdateTime { get; set; }

        [JsonIgnore]
        public virtual TemplateDocument TemplateDocument { get; set; }
        [JsonIgnore]
        public virtual TemplateObject TemplateObject { get; set; }
    }
}
