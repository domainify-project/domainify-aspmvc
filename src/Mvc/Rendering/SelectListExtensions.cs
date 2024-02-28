using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;

namespace Domainify.AspMvc
{
    /// <summary>
    /// Provides extension methods for converting a collection to a list of SelectListItem for use in dropdown lists.
    /// </summary>
    public static class SelectListExtensions
    {
        /// <summary>
        /// Converts a collection to a list of SelectListItem, using expressions to specify the value and text properties.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <typeparam name="TKey">The type of the property used as the 'value' for SelectListItem.</typeparam>
        /// <typeparam name="TValue">The type of the property used as the 'text' for SelectListItem.</typeparam>
        /// <param name="items">The collection of items to convert to SelectListItem.</param>
        /// <param name="value">Expression specifying the property to be used as the 'value' for SelectListItem.</param>
        /// <param name="text">Expression specifying the property to be used as the 'text' for SelectListItem.</param>
        /// <returns>A list of SelectListItem generated from the collection.</returns>
        public static List<SelectListItem> ToSelectList<T, TKey, TValue>(
            this IEnumerable<T> items,
            Expression<Func<T, TKey>> value,
            Expression<Func<T, TValue>> text)
        {
            var valueFunc = value.Compile();
            var textFunc = text.Compile();

            return items.Select(item => new SelectListItem
            {
                Value = valueFunc(item)!.ToString(),
                Text = textFunc(item)!.ToString()
            }).ToList();
        }
    }
}
