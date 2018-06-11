using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace MedicalCard.Helpers
{
    public static class Converter
    {
        public static string ResourceToString(ResourceType resource)
        {
            switch (resource)
            {
                case ResourceType.MedicationRequest:
                    return "MedicationRequest";
                case ResourceType.Patient:
                    return "Patient";
                case ResourceType.Medication:
                    return "Medication";
                case ResourceType.Observation:
                    return "Observation";
                default:
                    return "None";
            }
        }
    }
}
