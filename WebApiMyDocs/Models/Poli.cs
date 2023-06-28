using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiMyDocs.Models
{
    public partial class Poli
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public string Fio { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public byte[] PhotoPage1 { get; set; }
        public byte[] PhotoPage2 { get; set; }
        public string PhotoPage164 { get; set; }
        public string PhotoPage264 { get; set; }
        public string ValidUntil { get; set; }
        public DateTime? UpdateTime { get; set; }

        [JsonIgnore]
        public virtual Item IdNavigation { get; set; }
    }
}
