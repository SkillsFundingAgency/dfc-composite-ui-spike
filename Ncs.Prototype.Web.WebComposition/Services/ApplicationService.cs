using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Options;

namespace Ncs.Prototype.Web.WebComposition.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly HttpClient _httpClient;
        private readonly Dto.ApplicationDto _thisCompositeApplication;

        public ApplicationService(HttpClient httpClient, IOptions<Dto.ApplicationDto> thisCompositeApplication)
        {
            _httpClient = httpClient;
            _thisCompositeApplication = thisCompositeApplication.Value;
        }

        public Dto.ApplicationDto Application { get; set; }
        public string BearerToken { get; set; }
        public ClaimsPrincipal User { get; set; }
        public string RequestBaseUrl { get; set; }

        public (bool IsHealthy, string UnHealthyClue) Health { get; private set; }

        public async Task GetEntrypointMarkUpAsync(Models.PageViewModel pageViewModel)
        {
            await GetMarkUpAsync(Application.EntrypointUrl, pageViewModel);
        }

        public async Task GetApplicationMarkUpAsync(string data, Models.PageViewModel pageViewModel)
        {
            await GetMarkUpAsync(Application.RootUrl + data, pageViewModel);
        }

        public async Task PostApplicationMarkUpAsync(string url, KeyValuePair<string, string>[] formParameters, Models.PageViewModel pageViewModel)
        {
            var postTask = BuildPostMarkUpAsync(url, formParameters);
            var otherRegionsTasks = LoadOtherRegionsAsync(pageViewModel);

            await Task.WhenAll(postTask, otherRegionsTasks);

            if (postTask.IsCompletedSuccessfully)
            {
                pageViewModel.ApplicationMarkup = postTask.Result;
            }
            else
            {
                pageViewModel.ApplicationMarkup = BuildServiceUnavailableResponse();
            }

        }

        public async Task HeathCheckAsync()
        {
            string responseString = string.Empty;
            var response = await _httpClient.GetAsync(Application.HealthCheckUrl);

            if (response.IsSuccessStatusCode)
            {
                responseString = await response.Content.ReadAsStringAsync();
            }
            else
            {
                responseString = $"{response.ReasonPhrase} ({(int)response.StatusCode})";
            }

            Health = (IsHealthy: response.IsSuccessStatusCode, UnHealthyClue: responseString);
        }

        private async Task GetMarkUpAsync(string url, Models.PageViewModel pageViewModel)
        {
            var applicationTask = BuildApplicationMarkUpAsync(url);
            var otherRegionsTasks = LoadOtherRegionsAsync(pageViewModel);

            await Task.WhenAll(applicationTask, otherRegionsTasks);

            if (applicationTask.IsCompletedSuccessfully)
            {
                pageViewModel.ApplicationMarkup = applicationTask.Result;
            }
            else
            {
                pageViewModel.ApplicationMarkup = BuildServiceUnavailableResponse();
            }
        }

        private async Task LoadOtherRegionsAsync(Models.PageViewModel pageViewModel)
        {
            Task<HtmlString> CreateTask(List<Task<HtmlString>> tasks, string url, bool show = true)
            {
                if (show && !string.IsNullOrEmpty(url))
                {
                    var task = BuildApplicationMarkUpAsync(url);

                    tasks.Add(task);

                    return task;
                }

                return null;
            }

            HtmlString GetMarkupResult(Task<HtmlString> task)
            {
                if (task != null)
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        return task.Result;
                    }
                    else
                    {
                        return BuildServiceUnavailableResponse();
                    }
                }

                return null;
            }

            // create any required tasks
            var allTasks = new List<Task<HtmlString>>();
            var sidebarTask = CreateTask(allTasks, Application.SidebarUrl, Application.ShowSideBar);
            var appNavBarTask = CreateTask(allTasks, Application.AppNavBarUrl);
            var breadcrumbsTask = CreateTask(allTasks, Application.BreadcrumbsUrl);
            var personalisationTask = CreateTask(allTasks, Application.PersonalisationUrl, User.Identity.IsAuthenticated);
            var backButtonTask = CreateTask(allTasks, Application.BackButtonUrl);

            // await all tasks to complete
            await Task.WhenAll(allTasks.ToArray());

            // get the task results as markup
            pageViewModel.SidebarMarkup = GetMarkupResult(sidebarTask);
            pageViewModel.AppNavBarMarkup = GetMarkupResult(appNavBarTask);
            pageViewModel.BreadcrumbsMarkup = GetMarkupResult(breadcrumbsTask);
            pageViewModel.PersonalisationMarkup = GetMarkupResult(personalisationTask);
            pageViewModel.BackButtonMarkup = GetMarkupResult(backButtonTask);
        }

        private async Task<HtmlString> BuildApplicationMarkUpAsync(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            if (!string.IsNullOrEmpty(BearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);
            }

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            responseString = RewriteResponseUrls(responseString);

            return new HtmlString(responseString);
        }

        private async Task<HtmlString> BuildPostMarkUpAsync(string url, KeyValuePair<string, string>[] formParameters)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(formParameters)
            };

            if (!string.IsNullOrEmpty(BearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);
            }

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            responseString = RewriteResponseUrls(responseString);

            return new HtmlString(responseString);
        }

        private HtmlString BuildServiceUnavailableResponse()
        {
            var result = new StringBuilder();

            result.Append(@"<h2 class=""govuk-error-summary__title"">");
            result.Append("Service Unavailable");
            result.Append("</h2>");
            result.Append(@"<p class=""govuk-body"">");
            result.Append("Please try again later.");
            result.Append("</p>");

            return new HtmlString(result.ToString());
        }

        private string RewriteResponseUrls(string responseString)
        {
            var attributeNames = new string[] { "href", "action" };
            var quoteChars = new char[] { '"', '\'' };

            foreach (var attributeName in attributeNames)
            {
                foreach (var quoteChar in quoteChars)
                {
                    var fromUrlPrefixes = new string[] { $@"{attributeName}={quoteChar}/", $@"{attributeName}={quoteChar}{Application.RootUrl}/" };
                    string toUrlPrefix = $@"{attributeName}={quoteChar}{RequestBaseUrl}/{_thisCompositeApplication.ChildRoutePrefix}/"; 

                    foreach (var fromUrlPrefix in fromUrlPrefixes)
                    {
                        if (responseString.Contains(fromUrlPrefix, StringComparison.InvariantCultureIgnoreCase))
                        {
                            responseString = responseString.Replace(fromUrlPrefix, toUrlPrefix, StringComparison.InvariantCultureIgnoreCase);
                        }
                    }
                }
            }

            return responseString;
        }

    }
}
