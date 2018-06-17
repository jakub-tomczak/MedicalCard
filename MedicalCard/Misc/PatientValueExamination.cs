using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalCard.Misc
{
    public class PatientValueExamination : PatientExamination
    {
        decimal value;
        string unit;

        public decimal Value { get => value; set => this.value = value; }
        public string Unit { get => unit; set => unit = value; }
    }
}
