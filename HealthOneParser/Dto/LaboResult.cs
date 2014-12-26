using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthOneParser.Dto
{
    public class LaboResult
    {
        public string TestCode { get; set; }
        public string TestName { get; set; }
        public string ReferenceValues { get; set; }
        public string Unit { get; set; }
        public string Code { get; set; }
        public string Result { get; set; }
    }
}
