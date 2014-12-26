using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthOneParser.Dto
{
    public class Labo : IAdministrative
    {
        public Labo()
        {
            Results = new List<LaboResult>();
        }

        public string ProtocolNumber { get; set; }
        public string Identification { get; set; }
        public Patient Patient { get; set; }
        public string RequestorId { get; set; }
        public DateTime? RequestDate { get; set; }
        public ProtocolStatus Status { get; set; }
        public Mutuality Mutuality { get; set; }

        public IList<LaboResult> Results { get; set; }
    }
}
