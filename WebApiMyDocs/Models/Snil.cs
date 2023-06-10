using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiMyDocs.Models
{
    public partial class Snil
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public string Fio { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public byte[] PhotoPage1 { get; set; }
        public DateTime? UpdateTime { get; set; }

        public virtual Item IdNavigation { get; set; }
    }
}
