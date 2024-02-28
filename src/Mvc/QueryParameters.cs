using System.Text;

namespace Domainify.AspMvc
{
    /// <summary>
    /// Represents a collection of query parameters for constructing query strings in HTTP requests.
    /// </summary>
    public class QueryParameters
    {
        private Dictionary<string, string> queryParams = new Dictionary<string, string>();

        /// <summary>
        /// Adds or updates a parameter with the specified name and value.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter. If null, the parameter will not be added or updated.</param>
        /// <returns>The current instance of <see cref="QueryParameters"/>.</returns>
        public QueryParameters AddParameter(string name, object value)
        {
            if(value != null)
            {
                if (queryParams.ContainsKey(name))
                {
                    // If the parameter with the same name already exists, update its value.
                    queryParams[name] = value.ToString()!;
                }
                else
                {
                    queryParams.Add(name, value.ToString()!);
                }
            }

            return this;
        }

        /// <summary>
        /// Gets the query parameters as a formatted string suitable for appending to a URL.
        /// </summary>
        /// <returns>A formatted query string.</returns>
        public string GetQueryparameters()
        {
            StringBuilder queryString = new StringBuilder();
            foreach (var param in queryParams)
            {
                queryString.Append(Uri.EscapeDataString(param.Key));
                queryString.Append("=");
                queryString.Append(Uri.EscapeDataString(param.Value));
                queryString.Append("&");
            }

            // Remove the trailing "&" if there are parameters
            if (queryString.Length > 0)
            {
                // Remove the last character (which is an extra '&')
                queryString.Length--; 
            }

            return  queryString.ToString();
        }
    }
}
