using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalCard.Models
{
    interface IComparableResource<T>
    {
        /// <summary>
        /// Compares oryginal resource and edited data create the sum of these two instances.
        /// </summary>
        /// <returns></returns>
        T CompareResources(T original);
    }
}
