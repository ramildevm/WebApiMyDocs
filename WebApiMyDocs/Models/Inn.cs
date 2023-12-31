﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiMyDocs.Models
{
    public partial class Inn
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public string Fio { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public DateTime? RegistrationDate { get; set; }
        [JsonProperty("PhotoPage164")]
        public string PhotoPage1 { get; set; }
        public DateTime? UpdateTime { get; set; }

        [JsonIgnore]
        public virtual Item IdNavigation { get; set; }
    }
}
