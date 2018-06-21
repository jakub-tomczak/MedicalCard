using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalCard.Models
{
    interface IMapableResource<T, G>
    {
        /// <summary>
        /// Allows unpacking Hl7 model resource to an editable resource.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        G MapFromResource(T original);
        T MapToResource();
    }
}
