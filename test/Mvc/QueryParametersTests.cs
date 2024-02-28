using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domainify.AspMvc;

namespace Domainify.Test.Mvc
{
    [TestClass]
    public class QueryParametersTests
    {
        [TestMethod]
        public void AddParameter_ShouldAddOrUpdateParameter()
        {
            // Arrange
            var queryParameters = new QueryParameters();

            // Act
            queryParameters.AddParameter("param1", "value1");

            // Assert
            queryParameters.GetQueryparameters().Should().Be("param1=value1");
        }

        [TestMethod]
        public void AddParameter_ShouldNotAddParameterIfValueIsNull()
        {
            // Arrange
            var queryParameters = new QueryParameters();

            // Act
            queryParameters.AddParameter("param1", null);

            // Assert
            queryParameters.GetQueryparameters().Should().BeEmpty();
        }

        [TestMethod]
        public void AddParameter_ShouldUpdateExistingParameter()
        {
            // Arrange
            var queryParameters = new QueryParameters();

            // Act
            queryParameters.AddParameter("param1", "value1");
            queryParameters.AddParameter("param1", "updatedValue");

            // Assert
            queryParameters.GetQueryparameters().Should().Be("param1=updatedValue");
        }

        [TestMethod]
        public void GetQueryparameters_ShouldReturnFormattedQueryString()
        {
            // Arrange
            var queryParameters = new QueryParameters();

            // Act
            queryParameters.AddParameter("param1", "value1");
            queryParameters.AddParameter("param2", "value2");

            // Assert
            queryParameters.GetQueryparameters().Should().Be("param1=value1&param2=value2");
        }

        [TestMethod]
        public void GetQueryparameters_ShouldEscapeSpecialCharacters()
        {
            // Arrange
            var queryParameters = new QueryParameters();

            // Act
            queryParameters.AddParameter("param1", "value 1");
            queryParameters.AddParameter("param2", "value&2");

            // Assert
            queryParameters.GetQueryparameters().Should().Be("param1=value%201&param2=value%262");
        }
    }
}
