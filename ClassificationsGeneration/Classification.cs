using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassificationsGeneration
{
    internal class Classification
    {
        public Classification(string code, string description)
        {
            Code = code;
            Description = description;
        }
        public string Code { get; set; }
        public string Description { get; set; }

        public Guid? Guid { get; set; }
    }
}
