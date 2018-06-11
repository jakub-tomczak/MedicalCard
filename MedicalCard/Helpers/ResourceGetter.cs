using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalCard.Helpers
{
    public class ResourceGetter
    {
        public ResourceGetter()
        {
            client = new FhirClient(serviceRootUrl);
        }

        public IEnumerable<T> GetItems<T>(int limit = 10) where T : Resource, new()
        {
            var res = client.Search<T>(pageSize: limit);
            return res.Entry.Where(x => x.Resource is T).Select(x => x.Resource).Cast<T>();
        }

        public void UpdateItem<T>(T item) where T : Resource, new()
        {
            client.Update<T>(item);
        }
        public T GetItem<T>(ResourceType type, string id) where T : Resource, new()
        {
            var res = client.Read<T>($"{Converter.ResourceToString(type)}/{id}");
            return res;
        }
        private FhirClient client;
        //public Bundle FetchData()
        //{
        //    var FhirClient = new FhirClient(serviceRootUrl);
        //    try
        //    {
        //        //Attempt to send the resource to the server endpoint                
        //        UriBuilder UriBuilderx = new UriBuilder
        //        {
        //            Path = Converter.ResourceToString[Type]
        //        };
        //        Resource ReturnedResource = FhirClient.InstanceOperation(UriBuilderx.Uri, "everything");

        //        if (ReturnedResource is Bundle ReturnedBundle)
        //        {
        //            return ReturnedBundle;
        //            //Console.WriteLine("Received: " + ReturnedBundle.Total + " results, the resources are: ");
        //            //foreach (var Entry in ReturnedBundle.Entry)
        //            //{
        //            //    Console.WriteLine(string.Format("{0}/{1}", Entry.Resource.TypeName, Entry.Resource.Id));
        //            //}
        //        }
        //        else
        //        {
        //            throw new FhirOperationException("Operation call must return a bundle resource", System.Net.HttpStatusCode.BadRequest);
        //        }
        //    }
        //    catch (Hl7.Fhir.Rest.FhirOperationException FhirOpExec)
        //    {
        //        throw;
        //        //Process any Fhir Errors returned as OperationOutcome resource
        //        Console.WriteLine();
        //        Console.WriteLine("An error message: " + FhirOpExec.Message);
        //        Console.WriteLine();
        //        string xml = Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(FhirOpExec.Outcome);
        //        //XDocument xDoc = XDocument.Parse(xml);
        //        //Console.WriteLine(xDoc.ToString());
        //    }
        //    catch (Exception GeneralException)
        //    {
        //        throw new FhirOperationException(GeneralException.Message, System.Net.HttpStatusCode.BadRequest);
        //        Console.WriteLine();
        //        Console.WriteLine("An error message: " + GeneralException.Message);
        //        Console.WriteLine();
        //    }
        //}

        private static string serviceRootUrl = "http://localhost:8080/baseDstu3";
    }
}
