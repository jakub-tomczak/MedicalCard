using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalCard.Models
{
    public class EditedMedication : IMapableResource<Medication, EditedMedication>
    {
        public EditedMedication MapFromResource(Medication original)
        {
            throw new NotImplementedException();
        }

        public Medication MapToResource()
        {
            throw new NotImplementedException();
        }

        public bool TryMergeWithResource(Medication original)
        {
            throw new NotImplementedException();
        }
        public string Name { get => name; set => name = value; }
        public string Code { get => code; set => code = value; }
        public string Value { get => value; set => this.value = value; }

        private string name;
        private string code;
        private string value;
        private string patientID;
    }
}
