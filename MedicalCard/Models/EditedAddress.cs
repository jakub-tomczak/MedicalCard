using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalCard.Models
{
    public class EditedAddress : IMapableResource<Address, EditedAddress>
    {
        [Required]
        public string Country { get => country; set => country = value; }
        [Required]
        public string City { get => city; set => city = value; }
        [Required]
        public string AddressLine { get => addressLine; set => addressLine = value; }
        [Required]
        public string PostalCode { get => postalCode; set => postalCode = value; }

        public EditedAddress MapFromResource(Address original)
        {
            country = original.Country;
            city = original.City;
            addressLine = original.Line.Aggregate((x, y) => $"{x} {y}");
            postalCode = original.PostalCode;
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

        private string country;
        private string city;
        private string addressLine;
        private string postalCode;

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
