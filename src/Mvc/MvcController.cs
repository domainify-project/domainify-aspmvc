using Domainify.Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Web;

namespace Domainify.AspMvc
{
    /// <summary>
    /// Specifies the error handling mode for the XMvcController class.
    /// </summary>
    public enum ErrorHandlingMode
    {
        /// <summary>
        /// Handle all errors globally.
        /// </summary>
        HandleAllErrorsGlobally,

        /// <summary>
        /// Do not handle domain errors globally; only technical errors will be handled globally.
        /// Notice: Logical and validation and invariant errors are related to the domain.
        /// </summary>
        NoHandleDomainErrorsGlobally,

        /// <summary>
        /// Do not handle any errors globally.
        /// </summary>
        NoHandleErrorsGlobally
    }

    /// <summary>
    /// Represents an abstract MVC controller with extended  of the standard MVC Controller and error handling capabilities.
    /// </summary>
    public abstract class MvcController : Controller
    {
        /// <summary>
        /// Executes an asynchronous action and returns a NoContent result upon success.
        /// </summary>
        /// <param name="action">The asynchronous action to be executed.</param>
        /// <param name="errorHandlingMode">The error handling mode to determine how errors should be handled.</param>
        /// <returns>A NoContent result upon successful execution.</returns>
        [NonAction]
        public async Task<IActionResult> View(Func<Task> action,
            ErrorHandlingMode errorHandlingMode = ErrorHandlingMode.HandleAllErrorsGlobally)
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

                if (errorHandlingMode == ErrorHandlingMode.HandleAllErrorsGlobally)
                    throw;

                if (errorHandlingMode == ErrorHandlingMode.NoHandleErrorsGlobally ||
                    (errorHandlingMode == ErrorHandlingMode.NoHandleDomainErrorsGlobally &&
                     (((ErrorException)ex).Error.ErrorType == ErrorType.Logical ||
                     ((ErrorException)ex).Error.ErrorType == ErrorType.Validation ||
                     ((ErrorException)ex).Error.ErrorType == ErrorType.Invariant)))
                {
                    var devError = ErrorHelper.GetDevError(ex);
                    if (AppEnvironment.State == EnvironmentState.Production)
                        return StatusCode(500, (Error)devError);
                    return StatusCode(500, devError);
                }

                throw;
            }
        }

        /// <summary>
        /// Executes an asynchronous function and returns the result as an ActionResult upon success.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="action">The asynchronous function to be executed.</param>
        /// <param name="errorHandlingMode">The error handling mode to determine how errors should be handled.</param>
        /// <returns>An ActionResult containing the result of the asynchronous function.</returns>
        [NonAction]
        public async Task<ActionResult<T>> View<T>(Func<Task<T>> action,
            ErrorHandlingMode errorHandlingMode = ErrorHandlingMode.HandleAllErrorsGlobally)
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

                if (errorHandlingMode == ErrorHandlingMode.HandleAllErrorsGlobally)
                    throw;

                if (errorHandlingMode == ErrorHandlingMode.NoHandleErrorsGlobally ||
                    (errorHandlingMode == ErrorHandlingMode.NoHandleDomainErrorsGlobally &&
                     (((ErrorException)ex).Error.ErrorType == ErrorType.Logical ||
                     ((ErrorException)ex).Error.ErrorType == ErrorType.Validation ||
                     ((ErrorException)ex).Error.ErrorType == ErrorType.Invariant)))
                {
                    var devError = ErrorHelper.GetDevError(ex);
                    if (AppEnvironment.State == EnvironmentState.Production)
                        return StatusCode(500, (Error)devError);
                    return StatusCode(500, devError);
                }

                throw;
            }
        }

        /// <summary>
        /// Executes an asynchronous action and returns an error object if any domain errors occur.
        /// </summary>
        /// <param name="action">The asynchronous action to be executed.</param>
        /// <returns>An error object representing domain errors or null if no domain errors occur.</returns>
        [NonAction]
        public async Task<Error?> CatchDomainErrors(Func<Task> action)
        {
            try
            {
                await action();
                return null;
            }
            catch (Exception ex)
            {
                var devError = ErrorHelper.GetDevError(ex);
                if (AppEnvironment.State == EnvironmentState.Production)
                    return (Error)devError;
                return devError;
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
