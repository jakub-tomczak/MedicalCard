using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace MedicalCard.ViewModels
{
    public class PatientSummary
    {
        public List<MedicationStatement> Medicaments { get => medicaments; set => medicaments = value; }
        public List<Observation> Observations { get => observations; set => observations = value; }
        public Patient Data { get => data; set => data = value; }

        private Patient data;
        private List<Observation> observations;
        private List<MedicationStatement> medicaments;
    }
}
