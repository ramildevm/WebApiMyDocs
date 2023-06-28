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
        public byte[] FacePhoto { get; set; }
        public byte[] PhotoPage1 { get; set; }
        public byte[] PhotoPage2 { get; set; }
        public string FacePhoto64 { get; set; }
        public string PhotoPage164 { get; set; }
        public string PhotoPage264 { get; set; }
        public DateTime? UpdateTime { get; set; }
        [JsonIgnore]
        public virtual Item IdNavigation { get; set; }
    }
}
