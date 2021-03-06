﻿using Hl7.Fhir.Model;
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
            client = new FhirClient(ServiceRootUrl);
        }

        public IEnumerable<T> GetItems<T>(out Bundle bundle, int limit = 10) where T : Resource, new()
        {
            var res = client.Search<T>(pageSize: limit);
            bundle = res;
            return res.Entry.Select(x => x.Resource).Cast<T>();
        }

        public bool UpdateItem<T>(T item) where T : Resource, new()
        {
            return client.Update<T>(item) != null;
        }

        public List<T> SearchItemsWithParameters<T>(string key, string value, int entriesLimit = SearchLimit) where T : Resource, new()
        {
            return SearchItemsWithParameters<T>(new List<Tuple<string, string>>() { new Tuple<string, string>(key, value) }, entriesLimit);
        }
        public List<T> SearchItemsWithParameters<T>(List<Tuple<String, String>> searchParameters, int entriesLimit = SearchLimit) where T : Resource, new()
        {
            var parameters = new SearchParams();
            foreach (var item in searchParameters)
            {
                parameters.Add(item.Item1, item.Item2);
            }

            var result = client.Search<T>(parameters);
            int counter = 0;
            var outList = new List<T>();
            while (result != null && counter < entriesLimit)
            {
                outList.AddRange(result.Entry?.Select(x => x.Resource).Cast<T>());
                counter = outList.Count;
                result = client.Continue(result);
            }
            if (counter <= entriesLimit)
            {
                return outList;
            }
            //fetched too many items
            return outList.Take(entriesLimit).ToList();
        }

        public bool AddItem<T>(T item) where T : Resource, new()
        {
            return client.Create<T>(item) != null;
        }

        public bool RemoveItem<T>(T item) where T : Resource, new()
        {
            try
            {
                client.Delete(string.Format(ServerBase, item.Id));
                return true;
            }
            catch
            {
                return false;
            }
        }

        private const string ServerBase = "http://localhost:8080/baseDstu3/Patient/{0}";

        public T GetItem<T>(string id) where T : Resource, new()
        {
            try
            {
                var resource = client.SearchById<T>(id);
                return resource.Entry?.FirstOrDefault()?.Resource as T;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool TryGetPage(Bundle bundle, PageDirection direction, out Bundle requestedPage)
        {
            try
            {
                requestedPage = client.Continue(bundle, direction);
                return requestedPage != null;
            }
            catch (FhirOperationException)
            {
                requestedPage = null;
                return false;
            }
        }

        private FhirClient client;
        private const string ServiceRootUrl = "http://localhost:8080/baseDstu3";
        private const int SearchLimit = 300;
    }
}
