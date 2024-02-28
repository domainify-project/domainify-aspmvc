using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Net;
using Domainify.AspMvc;

namespace Domainify.Test.Mvc
{
    public class TestObject
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    [TestClass]
    public class HttpResponseTests
    {
        [TestMethod]
        public async Task ReadAsResultAsync_ShouldDeserializeContentIntoType()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            var responseData = new { Name = "John", Age = 30 };
            httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(responseData));
            var xhttpResponse = new HttpResponse(httpResponseMessage);

            // Act
            var result = await xhttpResponse.ReadAsResultAsync<TestObject>();

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(responseData.Name);
            result.Age.Should().Be(responseData.Age);
        }

        [TestMethod]
        public async Task ReadAsResultAsync_ShouldHandleNullContent()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponseMessage.Content = null;
            var xhttpResponse = new HttpResponse(httpResponseMessage);

            // Act
            var result = await xhttpResponse.ReadAsResultAsync<TestObject>();

            // Assert
            result.Should().BeNull();
        }
    }
}
