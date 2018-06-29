using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MedicalCard.Models
{
    public class EditedAddress : IMapableResource<Address, EditedAddress>
    {
        [Required(ErrorMessage = "Nazwa kraju jest wymagana!")]
        [Display(Name = "Wpisz nazwę kraju")]
        [StringLength(15, ErrorMessage = "Maksymalna długośc pola wynosi 15 znaków.")]
        public string Country { get => country; set => country = value; }

        [Required(ErrorMessage = "Nazwa miasta jest wymagane!")]
        [Display(Name = "Wpisz nazwę miasta")]
        [StringLength(30, ErrorMessage = "Maksymalna długośc pola wynosi 30 znaków.")]
        public string City { get => city; set => city = value; }

        [Required(ErrorMessage = "Nazwa i numer ulicy są wymagane!")]
        [Display(Name = "Linia adresu")]
        [StringLength(30, ErrorMessage = "Maksymalna długośc pola wynosi 30 znaków.")]
        public string AddressLine { get => addressLine; set => addressLine = value; }

        [Required(ErrorMessage = "Kod pocztowy jest wymagany!")]
        [RegularExpression(@"^\d{2}(-\d{3}|\d{2,4})$", ErrorMessage = "Kod pocztowy musi być w formacie xx-xxx lub być liczbą od 2 do 5 znaków.")]
        [Display(Name = "Kod pocztowy")]
        public string PostalCode { get => postalCode; set => postalCode = value; }

        public EditedAddress MapFromResource(Address original)
        {
            if (original != null)
            {
                country = original.Country;
                city = original.City;
                addressLine = original.Line.Aggregate((x, y) => $"{x} {y}");
                postalCode = original.PostalCode;
            }
            return this;
        }

        public Address MapToResource()
        {
            return new Address()
            {
                Country = country,
                City = city,
                Line = addressLine.Split(' '),
                PostalCode = postalCode
            };
        }

        public override bool Equals(object obj)
        {
            if (obj is Address address)
            {
                return address != null &&
                       Country == address.Country &&
                       City == address.City &&
                       AddressLine == address.Line.Aggregate((x, y) => $"{x} {y}") &&
                       PostalCode == address.PostalCode;
            }
            return false;
        }

        public bool TryMergeWithResource(Address original)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            var hashCode = -1244059517;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(country);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(city);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(addressLine);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(postalCode);
            return hashCode;
        }

        private string country;
        private string city;
        private string addressLine;
        private string postalCode;
    }
    public static class AddressExtension
    {
        public static List<EditedAddress> MapAddress(this List<Address> addresses)
        {
            return addresses.Select(
                x => new EditedAddress().MapFromResource(x)).ToList();
        }
    }
}
