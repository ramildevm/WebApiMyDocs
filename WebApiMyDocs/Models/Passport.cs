using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiMyDocs.Models
{
    public partial class Passport
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; }
        public string Fio { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string ResidencePlace { get; set; }
        public string ByWhom { get; set; }
        public string DivisionCode { get; set; }
        public DateTime? GiveDate { get; set; }
        [JsonProperty("FacePhoto64")]
        public string FacePhoto { get; set; }
        [JsonProperty("PhotoPage164")]
        public string PhotoPage1 { get; set; }
        [JsonProperty("PhotoPage264")]
        public string PhotoPage2 { get; set; }
        public DateTime? UpdateTime { get; set; }
        [JsonIgnore]
        public virtual Item IdNavigation { get; set; }
    }
}
