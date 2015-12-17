using System;
using System.Collections.Generic;

namespace HealthOneParser.Dto
{
    interface IAdministrative
    {
        string Identification { get; set; }
        Mutuality Mutuality { get; set; }
        Patient Patient { get; set; }
        string ProtocolNumber { get; set; }
        DateTime? RequestDate { get; set; }
        string RequestorId { get; set; }
        ProtocolStatus Status { get; set; }

        IDictionary<int, IList<string>> ParserErrors { get; set; }
    }
}
