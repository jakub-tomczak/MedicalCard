using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalCard.Misc
{
    public class PatientExamination
    {
        DateTime? date;
        string name;
        string description;
        string code;

        public DateTime? Date { get => date; set => date = value; }
        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }
        public string Code { get => code; set => code = value; }

    }
}
