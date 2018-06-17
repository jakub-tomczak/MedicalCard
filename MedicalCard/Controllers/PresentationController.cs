using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using MedicalCard.Helpers;
using MedicalCard.Misc;
using MedicalCard.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MedicalCard.Controllers
{
    public class PresentationController : Controller
    {
        public IActionResult Index()
        {
            return View("ViewPatient");
        }

        public IActionResult ViewPatient(string id, string startDate = null, string endDate = null)
        {
            if (!DataValidator.TryValidateString(id, out string idValidated, 4, 50))
            {
                ViewBag.Error = true;
                ViewBag.ErrorMessage = $"Błąd parsowania id : `{id.Substring(30)}` nie jest poprawnym ciagiem.";
                return View();
            }
            var searchParameters = new List<Tuple<string, string>>();

            DateTime startDateValidated = DateTime.MinValue,
                endDateValidated = DateTime.Now;
            if (!string.IsNullOrEmpty(startDate) && DataValidator.TryValidateDateTime(startDate, out startDateValidated))
            {
                searchParameters.Add(new Tuple<string, string>("date", startDateValidated.ToString(">=yyyy-MM-dd")));
                ViewBag.startDate = startDateValidated.ToString("dd-MMMM-yyyy");
            }

            if (!string.IsNullOrEmpty(endDate) && DataValidator.TryValidateDateTime(endDate, out endDateValidated))
            {
                searchParameters.Add(new Tuple<string, string>("date", endDateValidated.ToString("<=yyyy-MM-dd")));
                ViewBag.endDate = endDateValidated.ToString("dd-MMMM-yyyy");
            }

            var resourceGetter = new ResourceGetter();
            var person = resourceGetter.GetItem<Patient>(id);

            if (person == null)
            {
                return View();
            }

            searchParameters.Add(new Tuple<string, string>("subject", id));
            var patientsObservations = resourceGetter.SearchItemsWithParameters<Observation>(searchParameters);
            var patientsMedicamentsRequests = resourceGetter.SearchItemsWithParameters<MedicationRequest>("subject", id);   //use only id search parameter

            var valuableExaminations = new Dictionary<string, List<PatientValueExamination>>();
            var valueObservations = patientsObservations.Where(x => x.Value is SimpleQuantity || x.Component.Count > 0);
            var timelineObservations = patientsObservations.Where(x => !(x.Value is SimpleQuantity) && x.Component.Count == 0).
                Select(x =>
                {
                    DateTimeOffset? issued = x.Issued;
                    return new TimelineObject()
                    {
                        Date = issued.HasValue ? x.Issued.Value.DateTime : DateTime.MinValue,
                        Header = x.Code.Coding.FirstOrDefault()?.Display ?? DefaultObservationHeader,
                        Description = x.Value.ToString(),
                        Code = x.Code.Coding.FirstOrDefault()?.Code ?? DefaultCodeNumber,
                        EventType = TimelineEvent.ObservationMisc
                    };
                });

            var timelineMedications = patientsMedicamentsRequests.
                Where(x =>
                {
                    //filter dates here seems date in MedicationRequest search parameters doesn't affect results
                    var dateTime = x.AuthoredOnElement.ToDateTime() ?? DateTime.Now;
                    return dateTime >= startDateValidated && dateTime <= endDateValidated;
                }).
                Select(x =>
                {
                    var medication = x.Medication as CodeableConcept;
                    return new TimelineObject()
                    {
                        Date = (x.AuthoredOnElement.ToDateTime() ?? DateTime.MinValue),
                        Header = "Prośba o lek",
                        Description = (medication.Text ?? "Nieznana nazwa leku"),
                        Code = medication.Coding.FirstOrDefault().Code ?? DefaultCodeNumber,
                        EventType = TimelineEvent.MedicationRequest
                    };
                });

            //var valueSingleObservations = patientsObservations.Where(x => x.Value is SimpleQuantity).
            //    Select(x => new PatientValueExamination()
            //    {
            //        Code = x.Code.Coding.FirstOrDefault()?.Code ?? DefaultCodeNumber,
            //        Name = x.Code.Text,
            //        Date = x.Issued.Value.Date,
            //        Value = (x.Value as SimpleQuantity).Value.Value
            //    }).GroupBy(x => x.Code);


            foreach (var observation in valueObservations)
            {
                List<PatientValueExamination> examinations = TryGetObservationValue(observation);

                foreach (var examination in examinations)
                {
                    var code = examination.Code;
                    if (!valuableExaminations.ContainsKey(code))
                    {
                        valuableExaminations.Add(code, new List<PatientValueExamination>() { examination });
                    }
                    else
                    {
                        valuableExaminations[code].Add(examination);
                    }
                }
            }

            var patientSummary = new PatientSummary()
            {
                Data = person,
                Observations = patientsObservations,
                MedicationRequests = patientsMedicamentsRequests
            };
            //add dict for graphs
            patientSummary.SetPatientValueExaminations(valuableExaminations);
            //add timeline objects and sort them
            patientSummary.AddTimelineObjects(timelineObservations.Concat(timelineMedications).ToList());

            return View(patientSummary);
        }

        private List<PatientValueExamination> TryGetObservationValue(Observation observation)
        {
            var value = new List<PatientValueExamination>();
            if (observation.Value is SimpleQuantity simple)
            {
                value.Add(new PatientValueExamination()
                {
                    Code = observation.Code.Coding.FirstOrDefault()?.Code ?? DefaultCodeNumber,
                    Name = observation.Code.Text,
                    Date = observation.Issued.Value.Date,
                    Value = simple.Value.Value
                });
            }
            else
            {
                foreach (var item in observation.Component)
                {
                    value.Add(new PatientValueExamination()
                    {
                        Code = item.Code.Coding.FirstOrDefault()?.Code ?? DefaultCodeNumber,
                        Description = item.Code.Text,
                        Name = observation.Code.Text,
                        Date = observation.Issued.Value.Date
                    });
                }
            }
            return value;
        }
        private const string DefaultCodeNumber = "0000";
        private const string DefaultObservationHeader = "badanie";
    }
}