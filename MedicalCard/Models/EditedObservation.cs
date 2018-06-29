using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalCard.Models
{
    public class EditedObservation : IMapableResource<Observation, EditedObservation>
    {
        public EditedObservation MapFromResource(Observation original)
        {
            throw new NotImplementedException();
        }

        public Observation MapToResource()
        {
            throw new NotImplementedException();
        }

        public bool TryMergeWithResource(Observation original)
        {
            throw new NotImplementedException();
        }

        [StringLength(20, ErrorMessage = "Maksymalna długość znaków wynosi 20")]
        [Required(ErrorMessage = "Nazwa obserwacji jest obowiązkowa")]
        public string Name { get => name; set => name = value; }
        [Required(ErrorMessage = "Opis/wartość obserwacji jest obowiązkowa")]
        public string Value { get => value; set => this.value = value; }
        [Required(ErrorMessage = "Kod obserwacji jest obowiązkowy")]
        public string Code { get => code; set => code = value; }
        public string PatientID { get => patientID; set => patientID = value; }

        private string name;
        private string value;
        private string code;
        private string patientID;
    }
}
