using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domainify.AspMvc;

namespace Domainify.Test.Mvc
{
    [TestClass]
    public class HttpServiceTests
    {
        [TestMethod]
        public void RefineUriSegment_Tests()
        {
            // Arrange
            var httpService = new HttpService(new HttpClient(), "TaskManager", "v1", "Project");

            // Act && Assert
            httpService.RefineUriSegment("//segment/").Should().NotBeNull();
            httpService.RefineUriSegment("//segment/").Should().Be("segment");
            httpService.RefineUriSegment("/segment/").Should().Be("segment");
            httpService.RefineUriSegment("segment///").Should().Be("segment");
            httpService.RefineUriSegment("///segment").Should().Be("segment");
            httpService.RefineUriSegment("  ///segment/ ").Should().Be("segment");
        }

        [TestMethod]
        public void FinalizeUrlSegment_Tests()
        {
            // Arrange
            var httpService = new HttpService(new HttpClient(), "TaskManager", "v1", "Project");

            // Act && Assert
            httpService.FinalizeUrlSegment("//segment/").Should().NotBeNull();
            httpService.FinalizeUrlSegment("//segment/").Should().Be("/segment");
            httpService.FinalizeUrlSegment("/segment/").Should().Be("/segment");
            httpService.FinalizeUrlSegment("segment///").Should().Be("/segment");
            httpService.FinalizeUrlSegment("///segment").Should().Be("/segment");
            httpService.FinalizeUrlSegment("  ///segment/ ").Should().Be("/segment");
        }
    }
}
