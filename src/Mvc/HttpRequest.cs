using Domainify.Domain;

namespace Domainify.AspMvc
{
    /// <summary>
    /// Represents an HTTP request configuration with details such as HTTP method, request body, and API-specific parameters.
    /// </summary>
    public class HttpRequest
    {
        /// <summary>
        /// Gets the HTTP method of the request (e.g., GET, POST, PUT, PATCH, DELETE).
        /// </summary>
        public HttpMethod HttpMethod { get; private set; }

        /// <summary>
        /// Gets or sets the request body associated with the HTTP request.
        /// </summary>
        public BaseRequest? Request { get; private set; }

        /// <summary>
        /// Gets the application context for the API.
        /// </summary>
        public string ApplicationContext { get; private set; }

        /// <summary>
        /// Gets the API version.
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Gets the main collection resource for the API.
        /// </summary>
        public string CollectionResource { get; private set; }

        /// <summary>
        /// Gets the parameter identifying a specific item in the collection (if applicable).
        /// </summary>
        public object? CollectionItemParameter { get; private set; }

        /// <summary>
        /// Gets the sub-collection resource within the main collection.
        /// </summary>
        public string SubCollectionResource { get; private set; }

        /// <summary>
        /// Gets the name of the action to be performed by the API request.
        /// </summary>
        public string ActionName { get; private set; }

        /// <summary>
        /// Gets the query parameters as a raw string.
        /// </summary>
        public string QueryParametersString { get; private set; }

        /// <summary>
        /// Gets the query parameters as an object of type <see cref="QueryParameters"/>.
        /// </summary>
        public QueryParameters? QueryParameters { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequest"/> class with specified configuration details.
        /// </summary>
        /// <param name="httpMethod">The HTTP method of the request.</param>
        /// <param name="request">The request body associated with the HTTP request.</param>
        /// <param name="applicationContext">The application context for the API.</param>
        /// <param name="version">The API version.</param>
        /// <param name="collectionResource">The main collection resource for the API.</param>
        /// <param name="collectionItemParameter">The parameter identifying a specific item in the collection.</param>
        /// <param name="subCollectionResource">The sub-collection resource within the main collection.</param>
        /// <param name="actionName">The name of the action to be performed by the API request.</param>
        /// <param name="queryParametersString">The query parameters as a raw string.</param>
        /// <param name="queryParameters">The query parameters as an object of type <see cref="QueryParameters"/>.</param>
        public HttpRequest(
            HttpMethod httpMethod,
            BaseRequest? request = null,
            string applicationContext = "",
            string version = "",
            string collectionResource = "",
            object? collectionItemParameter = null,
            string subCollectionResource = "",
            string actionName = "",
            string queryParametersString = "",
            QueryParameters? queryParameters = null)
        {
            HttpMethod = httpMethod;
            Request = request;
            ApplicationContext = applicationContext;
            Version = version;
            CollectionResource = collectionResource;
            CollectionItemParameter = collectionItemParameter;
            SubCollectionResource = subCollectionResource;
            ActionName = actionName;
            QueryParametersString = queryParametersString;
            QueryParameters = queryParameters;
        }
    }
}
