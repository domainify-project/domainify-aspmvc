using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using Domainify.AspMvc;

namespace Domainify.Test.Mvc
{
    public class YourItemType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [TestClass]
    public class SelectListExtensionsTests
    {
        [TestMethod]
        public void ToSelectList_ShouldConvertItemsToSelectList()
        {
            // Arrange
            var items = new List<YourItemType>
            {
                new YourItemType { Id = 1, Name = "Item 1" },
                new YourItemType { Id = 2, Name = "Item 2" },
                new YourItemType { Id = 3, Name = "Item 3" }
            };

            Expression<Func<YourItemType, int>> valueExpression = item => item.Id;
            Expression<Func<YourItemType, string>> textExpression = item => item.Name;

            // Act
            var result = SelectListExtensions.ToSelectList(items, valueExpression, textExpression);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(items.Count);

            for (var i = 0; i < items.Count; i++)
            {
                result[i].Value.Should().Be(items[i].Id.ToString());
                result[i].Text.Should().Be(items[i].Name);
            }
        }
    }
}
