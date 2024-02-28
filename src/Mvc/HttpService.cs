using Newtonsoft.Json;
using System.Text;

namespace Domainify.AspMvc
{
    /// <summary>
    /// Represents a service for making HTTP requests and handling API interactions.
    /// </summary>
    public class HttpService
    {
        private readonly string _applicationContext = string.Empty;
        private readonly string _version = string.Empty;
        private readonly string _collectionResource = string.Empty;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Represents a delegate for handling API request modifications.
        /// </summary>
        /// <param name="request">The HttpRequestMessage to be modified.</param>
        public delegate void ApiRequestDelegate(HttpRequestMessage request);

        private ApiRequestDelegate? apiRequestDelegation = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpService"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance used for making HTTP requests.</param>
        /// <param name="applicationContext">The application context for the API.</param>
        /// <param name="version">The API version.</param>
        /// <param name="collectionResource">The main collection resource of the API.</param>
        public HttpService(
            HttpClient httpClient,
            string applicationContext = "",
            string version = "",
            string collectionResource = "")
        {
            _httpClient = httpClient;
            _applicationContext = applicationContext;
            _version = version;
            _collectionResource = collectionResource;
        }

        /// <summary>
        /// Adds a delegate for modifying API requests before sending.
        /// </summary>
        /// <param name="delegation">The delegate to be added.</param>
        /// <returns>The current instance of <see cref="HttpService"/>.</returns>
        public HttpService AddApiRequestDelegation(ApiRequestDelegate delegation)
        {
            apiRequestDelegation += delegation;
            return this;
        }

        /// <summary>
        /// Sends an HTTP request asynchronously and returns the response as an <see cref="HttpResponse"/>.
        /// </summary>
        /// <param name="httpRequest">The HTTP request details.</param>
        /// <returns>An <see cref="HttpResponse"/> representing the HTTP response.</returns>
        public async Task<HttpResponse> SendAsync(HttpRequest httpRequest)
        {
            var requestUri = GetRequestUri(
                httpRequest.ApplicationContext,
                httpRequest.Version,
                httpRequest.CollectionResource,
                httpRequest.CollectionItemParameter,
                httpRequest.SubCollectionResource,
                httpRequest.ActionName,
                httpRequest.QueryParametersString,
                httpRequest.QueryParameters);

            var httpRequestMessage = new HttpRequestMessage(
                httpRequest.HttpMethod,
                requestUri);

            if (apiRequestDelegation != null)
                apiRequestDelegation!.Invoke(httpRequestMessage);

            if (httpRequest.Request != null)
            {
                string jsonBody = JsonConvert.SerializeObject(httpRequest.Request);
                httpRequestMessage.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

            httpResponseMessage.EnsureSuccessStatusCode();

            return new HttpResponse(httpResponseMessage);
        }

        /// <summary>
        /// Sends an HTTP request asynchronously and deserializes the response into a specified result type.
        /// </summary>
        /// <typeparam name="TResult">The type to deserialize the response into.</typeparam>
        /// <param name="httpRequest">The HTTP request details.</param>
        /// <returns>The deserialized result of type <typeparamref name="TResult"/>.</returns>
        public async Task<TResult> SendAndReadAsResultAsync<TResult>(HttpRequest httpRequest)
        {
            var response = await SendAsync(httpRequest);
            return await response.ReadAsResultAsync<TResult>();
        }

        /// <summary>
        /// Constructs the request URI for an HTTP request based on the provided parameters and API conventions.
        /// Refer to: https://medium.com/@nadinCodeHat/rest-api-naming-conventions-and-best-practices-1c4e781eb6a5
        /// </summary>
        /// <param name="applicationContext">The application context for the API.</param>
        /// <param name="version">The API version.</param>
        /// <param name="collectionResource">The main collection resource of the API.</param>
        /// <param name="collectionItemParameter">An optional parameter for identifying a specific item in the collection.</param>
        /// <param name="subCollectionResource">An optional sub-collection resource within the main collection.</param>
        /// <param name="actionName">An optional action name for the API request.</param>
        /// <param name="queryParametersString">An optional string representing query parameters.</param>
        /// <param name="queryParameters">An optional instance of <see cref="QueryParameters"/> for handling query parameters.</param>
        /// <returns>The constructed request URI for the HTTP request.</returns>
        internal string GetRequestUri(
            string applicationContext = "",
            string version = "",
            string collectionResource = "",
            object? collectionItemParameter = null,
            string subCollectionResource = "",
            string actionName = "",
            string queryParametersString = "",
            QueryParameters? queryParameters = null)
        {
            string path = string.Empty;

            // Process application context
            applicationContext = RefineUriSegment(applicationContext);
            applicationContext = string.IsNullOrEmpty(applicationContext) ? _applicationContext : applicationContext;
            if (!string.IsNullOrEmpty(applicationContext))
                path += FinalizeUrlSegment(applicationContext);

            // Process version
            version = RefineUriSegment(version);
            version = string.IsNullOrEmpty(version) ? _version : version;
            if (!string.IsNullOrEmpty(version))
                path += FinalizeUrlSegment(version);

            // Process main collection resource
            collectionResource = RefineUriSegment(collectionResource);
            collectionResource = string.IsNullOrEmpty(collectionResource) ? _collectionResource : collectionResource;
            if (!string.IsNullOrEmpty(collectionResource))
            {
                path += FinalizeUrlSegment(collectionResource);

                // Handle collection item parameter
                if (string.IsNullOrEmpty(actionName)
                    && collectionItemParameter != null)
                {
                    var collectionParameterValue = collectionItemParameter.ToString();
                    collectionParameterValue = RefineUriSegment(collectionParameterValue!);
                    if (!string.IsNullOrEmpty(collectionParameterValue))
                        path += FinalizeUrlSegment(collectionParameterValue);
                }
            }

            // Process sub-collection resource
            subCollectionResource = RefineUriSegment(subCollectionResource);
            if (!string.IsNullOrEmpty(subCollectionResource))
                path += FinalizeUrlSegment(subCollectionResource);

            // Process action name
            actionName = RefineUriSegment(actionName);
            if (!string.IsNullOrEmpty(actionName))
            {
                path += FinalizeUrlSegment(actionName);

                // Handle collection item parameter for action
                if (collectionItemParameter != null)
                {
                    var collectionParameterValue = collectionItemParameter.ToString();
                    collectionParameterValue = RefineUriSegment(collectionParameterValue!);
                    if (!string.IsNullOrEmpty(collectionParameterValue))
                        path += FinalizeUrlSegment(collectionParameterValue);
                }
            }

            // Process query parameters
            var resultOfQueryParameters = string.Empty;
            if (!string.IsNullOrEmpty(queryParametersString))
            {
                resultOfQueryParameters += queryParametersString;
            }

            if (queryParameters != null)
            {
                resultOfQueryParameters += queryParameters.GetQueryparameters();
            }

            if (!string.IsNullOrEmpty(resultOfQueryParameters) && resultOfQueryParameters.Trim().Substring(0, 1) != "?")
                resultOfQueryParameters = "?" + resultOfQueryParameters;

            path += resultOfQueryParameters;

            return path;
        }

        /// <summary>
        /// Refines the given URI segment by trimming leading and trailing spaces,
        /// removing a trailing slash if present, and removing a leading slash if present.
        /// </summary>
        /// <param name="segment">The URI segment to be refined.</param>
        /// <returns>A refined URI segment.</returns>
        internal string RefineUriSegment(string segment)
        {
            segment = segment.Trim();
            segment = segment.TrimStart('/');
            segment = segment.TrimEnd('/');
            return segment;
        }

        /// <summary>
        /// Finalizes the given URI segment by adding a leading slash.
        /// </summary>
        /// <param name="segment">The URI segment to be finalized.</param>
        /// <returns>A finalized URI segment with a leading slash.</returns>
        internal string FinalizeUrlSegment(string segment)
        {
            segment = RefineUriSegment(segment);
            segment = "/" + segment;
            return segment;
        }
    }
}
