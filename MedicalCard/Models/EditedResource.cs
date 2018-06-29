using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalCard.Models
{
    public abstract class EditedResource<T, G> : IComparableResource<T>, IMapableResource<T, G> where T : Resource, new()
    {
        public abstract T CompareResources(T original);
        public abstract G MapFromResource(T original);
        public abstract T MapToResource();

        public abstract bool TryMergeWithResource(T original);

        public string Id { get => id; set => id = value; }

        private string id;
    }
}
