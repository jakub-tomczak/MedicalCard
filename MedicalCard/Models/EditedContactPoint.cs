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
            if (original != null)
            {
                this.system = original.System ?? ContactPointSystem.Other;
                this.contactValue = original.Value;
                this.contactDetails = original.ValueElement?.Value ?? string.Empty;
            }
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

        public bool TryMergeWithResource(ContactPoint original)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            var hashCode = 869453999;
            hashCode = hashCode * -1521134295 + system.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(contactValue);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(contactDetails);
            return hashCode;
        }

        public ContactPointSystem System { get => system; set => system = value; }
        public string ContactValue { get => contactValue; set => contactValue = value; }
        [Required]
        [Display(Name = "Numer telefonu")]
        [StringLength(12, ErrorMessage = "Długość pola wynosi 9-13 znaków.")]
        [RegularExpression(@"\d{9,13}", ErrorMessage = "Numer telefonu musi mieć od 9 do 13 znaków. (bez spacji)")]
        public string ContactDetails { get => contactDetails; set => contactDetails = value; }

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
