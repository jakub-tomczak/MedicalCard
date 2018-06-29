using Hl7.Fhir.Model;
using MedicalCard.Misc;
using System.Collections.Generic;

namespace MedicalCard.ViewModels
{
    /// <summary>
    /// Class used to summarize patient's data.
    /// </summary>
    public class PatientSummary
    {
        public List<MedicationStatement> Medicaments { get => medicaments; set => medicaments = value; }
        public List<Observation> Observations { get => observations; set => observations = value; }
        public Patient Data { get => data; set => data = value; }
        public List<MedicationRequest> MedicationRequests { get => medicationRequests; set => medicationRequests = value; }
        public Dictionary<string, List<PatientValueExamination>> ValueExaminations { get => valueExaminations; }
        public List<TimelineObject> TimelineObjects { get => timelineObjects; set => timelineObjects = value; }

        public void AddTimelineObjects(List<TimelineObject> timelineObjects)
        {
            this.timelineObjects.AddRange(timelineObjects);
            this.timelineObjects.Sort((x, y) => x.Date.CompareTo(y.Date));
        }
        public void SetPatientValueExaminations(Dictionary<string, List<PatientValueExamination>> patientValueExaminations)
        {
            this.valueExaminations = patientValueExaminations;
            foreach (var item in valueExaminations)
            {
                item.Value.Sort((x, y) => x.Date.Value.CompareTo(y.Date.Value));
            }
        }

        private Patient data;
        private List<Observation> observations;
        private List<MedicationStatement> medicaments;
        private List<MedicationRequest> medicationRequests;
        private Dictionary<string, List<PatientValueExamination>> valueExaminations;
        private List<TimelineObject> timelineObjects = new List<TimelineObject>();
    }
}
