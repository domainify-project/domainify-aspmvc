using Newtonsoft.Json;

namespace Domainify.AspMvc
{
    /// <summary>
    /// Represents an HTTP response with additional functionality for reading the response content as a result.
    /// </summary>
    public class HttpResponse
    {
        /// <summary>
        /// Gets the underlying HttpResponseMessage associated with the HTTP response.
        /// </summary>
        public HttpResponseMessage HttpResponseMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponse"/> class with the specified HttpResponseMessage.
        /// </summary>
        /// <param name="httpResponseMessage">The HttpResponseMessage associated with the HTTP response.</param>
        public HttpResponse(HttpResponseMessage httpResponseMessage)
        {
            HttpResponseMessage = httpResponseMessage;
        }

        /// <summary>
        /// Reads the response content asynchronously and deserializes it into an object of type TResult.
        /// </summary>
        /// <typeparam name="TResult">The type of the result object.</typeparam>
        /// <returns>An object of type TResult representing the deserialized response content.</returns>
        public async Task<TResult> ReadAsResultAsync<TResult>()
        {
            // Read the response content as a string
            string content = await HttpResponseMessage.Content.ReadAsStringAsync();

            // Deserialize the content into an object of type TResult
            return JsonConvert.DeserializeObject<TResult>(content)!;
        }
    }
}
