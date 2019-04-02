using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Options;
using Ncs.Prototype.Web.Web3.Models;
using Newtonsoft.Json;

namespace Ncs.Prototype.Web.Web3.Services
{
    public class PartService : IPartService
    {

        private readonly HttpClient _httpClient;
        private readonly Options.WebApiSettings _webApiSettings;

        public string BearerToken { get; set; }

        public PartService(HttpClient httpClient, IOptions<Options.WebApiSettings> webApiSettings)
        {
            _httpClient = httpClient;
            _webApiSettings = webApiSettings.Value;
        }

        public async Task<IEnumerable<PartViewModel>> GetPartsJsonAsync()
        {
            string url = $"{_webApiSettings.BaseUrl}/api/{_webApiSettings.PartsApiGetJson}";
            var results = await CallHttpClientAsync<Models.PartViewModel>(url);

            return results;
        }

        public async Task<IEnumerable<PartViewModel>> GetPartsAuthorizedAsync()
        {
            string url = $"{_webApiSettings.BaseUrl}/api/{_webApiSettings.PartsApiGetAuthorized}";
            var results = await CallHttpClientAsync<Models.PartViewModel>(url);

            return results;
        }

        public async Task<HtmlString> GetPartsHtmlAsync()
        {
            string url = $"{_webApiSettings.BaseUrl}/api/{_webApiSettings.PartsApiGetHtml}";
            var results = await CallHttpClientHtmlStringAsync(url);

            return results;
        }

        public async Task<IEnumerable<PartViewModel>> GetPartsXmlAsync()
        {
            string url = $"{_webApiSettings.BaseUrl}/api/{_webApiSettings.PartsApiGetXml}";
            var results = await CallHttpClientXmlAsync<PartsData>(url);

            var response = (from a in results
                            select new PartViewModel()
                            {
                                Id = a.Id,
                                Name = a.Name,
                                Description = a.Description
                            }
                            );

            return response;
        }

        private async Task<IEnumerable<T>> CallHttpClientAsync<T>(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            if (!string.IsNullOrEmpty(BearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);
            }

            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<IEnumerable<T>>(responseString);

            return result;
        }

        private async Task<HtmlString> CallHttpClientHtmlStringAsync(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            if (!string.IsNullOrEmpty(BearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);
            }

            request.Headers.Add("Accept", "application/html");

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            return new HtmlString(responseString);
        }

        private async Task<IEnumerable<T>> CallHttpClientXmlAsync<T>(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            if (!string.IsNullOrEmpty(BearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);
            }

            request.Headers.Add("Accept", "application/xml");

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var serializer = new XmlSerializer(typeof(T[]));

            using (TextReader reader = new StringReader(responseString))
            {
                var result = (IEnumerable<T>)serializer.Deserialize(reader);

                return result;
            }
        }

    }
}
