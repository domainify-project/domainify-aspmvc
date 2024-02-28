using Domainify.Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Web;

namespace Domainify.AspMvc
{
    /// <summary>
    /// Base abstract class for API controllers, extending the functionality of the standard MVC Controller.
    /// </summary>
    public abstract class ApiController : Controller
    {
        /// <summary>
        /// Executes an asynchronous action and returns a NoContent result upon success.
        /// </summary>
        /// <param name="action">The asynchronous action to be executed.</param>
        /// <returns>A NoContent result upon successful execution.</returns>
        [NonAction]
        public async Task<IActionResult> View(Func<Task> action)
        {
            try
            {
                await action();
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex is ErrorException
                    && ((ErrorException)ex).Error.Issues.OfType<NoEntityWasFound>().Any())
                    return NotFound();

                throw;
            }
        }

        /// <summary>
        /// Executes an asynchronous function and returns the result as an ActionResult upon success.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="action">The asynchronous function to be executed.</param>
        /// <returns>An ActionResult containing the result of the asynchronous function.</returns>
        [NonAction]
        public async Task<ActionResult<T>> View<T>(Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                if (ex is ErrorException
                    && ((ErrorException)ex).Error.Issues.OfType<NoEntityWasFound>().Any())
                    return NotFound();

                throw;
            }
        }

        /// <summary>
        /// Deserializes the query string parameters into an object of type TRequest.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request object to be deserialized.</typeparam>
        /// <returns>An instance of TRequest populated with values from the query string.</returns>
        [NonAction]
        public TRequest GetRequest<TRequest>()
        {
            var dict = HttpUtility.ParseQueryString(HttpContext.Request.QueryString.ToString());
            string json = JsonConvert.SerializeObject(dict.Cast<string>()
                .ToDictionary(k => k, v => dict[v]));
            return JsonConvert.DeserializeObject<TRequest>(json)!;
        }
    }
}
