using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthOneParser.Dto
{
    public class Patient
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string PostalName { get; set; }
        public DateTime? BirthDate { get; set; }
        public Sex Sex { get; set; }
    }
}
