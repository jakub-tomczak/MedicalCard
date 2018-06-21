using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MedicalCard.Models
{
    public class EditedPatient : EditedResource<Patient, EditedPatient>
    {
        public EditedPatient()
        {
            this.telecom = new List<EditedContactPoint>();
        }
        public EditedPatient(string id)
            : this()
        {
            this.Id = id;
        }

        public override Patient CompareResources(Patient original)
        {
            //name
            var name = original.Name.FirstOrDefault();
            var editedName = new HumanName
            {
                Given = this.givenName.Split(' '),
                Family = this.familyName
            };

            if (original.Name.Any())
            {
                original.Name[0] = editedName;
            }
            else
            {
                original.Name.Add(editedName);
            }
            //birth
            if (original.BirthDateElement == null)
            {
                original.BirthDateElement = new Date(birthDate.Year, birthDate.Month, birthDate.Day);
            }
            //gender
            original.Gender = this.gender;
            //address
            if (original.Address.Any())
            {
                original.Address[0] = address.MapToResource();
            }
            else
            {
                original.Address.Add(address.MapToResource());
            }
            //contact
            original.Telecom.Clear();
            original.Telecom.AddRange(telecom.Select(x => x.MapToResource()));
            return original;
        }

        public override EditedPatient MapFromResource(Patient original)
        {
            this.Id = original.Id;
            var birthDate = original.BirthDateElement.ToDateTime();
            if (birthDate.HasValue)
            {
                this.birthDate = birthDate.Value;
            }
            var name = original.Name.FirstOrDefault();

            if (name != null)
            {
                this.givenName = name.Given.Aggregate((x, y) => $"{x} {y}");
                this.familyName = name.Family;
            }
            this.telecom = original.Telecom.MapTelecom();
            this.address = new EditedAddress().MapFromResource(original.Address.FirstOrDefault());
            this.gender = original.Gender ?? AdministrativeGender.Unknown;

            return this;
        }

        public void AddContact(EditedContactPoint contactPoint)
        {
            telecom.Add(contactPoint);
        }

        public override Patient MapToResource()
        {
            var newPatient = new Patient
            {
                Id = Id
            };
            //name
            newPatient.Name.Add(new HumanName
            {
                Given = this.givenName.Split(' '),
                Family = this.familyName
            });
            //birth
            newPatient.BirthDateElement = new Date(birthDate.Year, birthDate.Month, birthDate.Day);
            //gender
            newPatient.Gender = this.gender;
            //address
            newPatient.Address.Add(address.MapToResource());
            //contact
            newPatient.Telecom.AddRange(telecom.Select(x => x.MapToResource()));
            return newPatient;
        }

        [Display(Name = "Data urodzenia", Prompt = "Podaj datę urodzenia")]
        [DataType(DataType.DateTime)]
        public DateTime BirthDate { get => birthDate; set => birthDate = value; }
        [Required(ErrorMessage = "To pole jest wymagane!")]
        public string GivenName { get => givenName; set => givenName = value; }
        [Required(ErrorMessage = "Nazwisko jest wymagane!")]
        public string FamilyName { get => familyName; set => familyName = value; }
        public AdministrativeGender Gender { get => gender; set => gender = value; }
        [Required]
        public EditedAddress Address { get => address; set => address = value; }

        public List<EditedContactPoint> Telecom { get => telecom; }

        private DateTime birthDate;
        private string givenName;
        private string familyName;
        private AdministrativeGender gender;
        private EditedAddress address;
        private List<EditedContactPoint> telecom;
    }
}
