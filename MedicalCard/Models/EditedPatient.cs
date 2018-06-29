using Hl7.Fhir.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MedicalCard.Models
{
    public class EditedPatient : EditedResource<Patient, EditedPatient>
    {
        public EditedPatient()
        {
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
            original.Address.Add(address.MapToResource());

            //contact
            original.Telecom.Add(telecom.MapToResource());
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
            var name = original.Name.LastOrDefault();

            if (name != null)
            {
                this.givenName = name.Given.Aggregate((x, y) => $"{x} {y}");
                this.familyName = name.Family;
            }
            this.telecom = new EditedContactPoint().MapFromResource(original.Telecom.LastOrDefault());
            this.address = new EditedAddress().MapFromResource(original.Address.LastOrDefault());
            this.gender = original.Gender ?? AdministrativeGender.Unknown;

            return this;
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
            newPatient.Telecom.Add(telecom.MapToResource());
            return newPatient;
        }

        public override bool TryMergeWithResource(Patient original)
        {
            var patient = MapToResource();
            if (original.Id != patient.Id)
                return false;

            original.Name.AddRange(patient.Name);
            //remove the last one and add the new one
            if (original.Address.Any())
                original.Address.RemoveAt(original.Address.Count - 1);
            original.Address.Add(patient.Address.First());
            //the same for the telecom
            if (original.Telecom.Any())
                original.Telecom.RemoveAt(original.Telecom.Count - 1);
            original.Telecom.Add(patient.Telecom.First());

            original.BirthDateElement = patient.BirthDateElement;
            original.Gender = patient.Gender;
            return true;
        }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Data urodzenia jest wymagana")]
        [Display(Name = "Data urodzenia", Prompt = "Podaj datę urodzenia")]
        [DataType(DataType.DateTime)]
        public DateTime BirthDate { get => birthDate; set => birthDate = value; }

        [Required(ErrorMessage = "To pole jest wymagane!")]
        [StringLength(30, ErrorMessage = "Maksymalna długośc pola wynosi 30 znaków.")]
        [Display(Name = "Imię")]
        public string GivenName { get => givenName; set => givenName = value; }

        [Required(ErrorMessage = "Nazwisko jest wymagane!")]
        [StringLength(30, ErrorMessage = "Maksymalna długośc pola wynosi 30 znaków.")]
        [Display(Name = "Nazwisko rodowe")]
        public string FamilyName { get => familyName; set => familyName = value; }

        [Display(Name = "Płeć")]
        [Required(ErrorMessage = "Wybierz płeć")]
        public AdministrativeGender Gender { get => gender; set => gender = value; }

        public EditedAddress Address { get => address; set => address = value; }
        public EditedContactPoint Telecom { get => telecom; set => telecom = value; }


        private DateTime birthDate;
        private string givenName;
        private string familyName;
        private AdministrativeGender gender;
        private EditedAddress address;
        private EditedContactPoint telecom;
    }
}
