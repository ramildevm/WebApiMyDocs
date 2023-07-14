using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiMyDocs.Models
{
    public partial class CreditCard
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public string Fio { get; set; }
        public string ExpiryDate { get; set; }
        public int? Cvv { get; set; }
        [JsonProperty("PhotoPage164")]
        public string PhotoPage1 { get; set; }
        public DateTime? UpdateTime { get; set; }

        [JsonIgnore]
        public virtual Item IdNavigation { get; set; }
    }
}
