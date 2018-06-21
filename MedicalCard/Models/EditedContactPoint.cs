using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Hl7.Fhir.Model.ContactPoint;

namespace MedicalCard.Models
{
    public class EditedContactPoint : IMapableResource<ContactPoint, EditedContactPoint>
    {
        public override bool Equals(object obj)
        {
            if (obj is ContactPoint point)
            {
                return point != null &&
                    point.System.HasValue &&
                    system == point.System.Value &&
                    contactValue == point.Value;
            }
            return false;
        }

        public EditedContactPoint MapFromResource(ContactPoint original)
        {
            this.system = original.System ?? ContactPointSystem.Other;
            this.contactValue = original.Value;
            this.contactDetails = original.ValueElement?.Value ?? string.Empty;
            return this;
        }

        public ContactPoint MapToResource()
        {
            return new ContactPoint()
            {
                System = this.system,
                Value = this.contactValue,
                ValueElement = new FhirString(this.contactDetails)
            };

        }

        private ContactPointSystem system;
        private string contactValue;
        private string contactDetails;
    }
    public static class ContactPointExtensions
    {
        public static List<EditedContactPoint> MapTelecom(this List<ContactPoint> contact)
        {
            return contact.Select(
                x => new EditedContactPoint().MapFromResource(x)).ToList();
        }
    }
}
