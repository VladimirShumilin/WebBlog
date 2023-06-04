using Microsoft.AspNetCore.Mvc.Testing;

namespace WebBlog.IntegrationTests.IntegrationTests
{
    public class Basic : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;


        public Basic(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }


        [Theory]
        [InlineData("/")]
        public async Task GetEndpointtReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            if (await client.GetAsync(url) is HttpResponseMessage response)
            {
                // Assert
                response.EnsureSuccessStatusCode(); // Status Code 200-299
                Assert.Equal("text/html; charset=utf-8",
                    response.Content.Headers.ContentType?.ToString());
            }
        }
       
    }
}