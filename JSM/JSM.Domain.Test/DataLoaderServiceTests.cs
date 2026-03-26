using JSM.Application.Services;
using Moq;
using System.Net;

namespace JSM.Domain.Test
{
    public class DataLoaderServiceTests
    {
        // Minimal delegating handler to return predetermined responses per request URL
        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

            public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
            {
                _responder = responder;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_responder(request));
            }
        }

        [Fact]
        public async Task LoadUsersAsync_WhenCsvAndJsonReturned_TransformsBothSources()
        {
            // Arrange
            var csvContent = "Id,Name,Email\n1,John Doe,john@example.com";
            var jsonContent = "{\"Results\":[{}]}"; // shape expected by RootObject -> Results

            var handler = new FakeHttpMessageHandler(req =>
            {
                var uri = req.RequestUri.ToString();
                if (uri.Contains("input-backend.csv"))
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(csvContent)
                    };
                }
                else if (uri.Contains("input-backend.json"))
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(jsonContent)
                    };
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            var httpClient = new HttpClient(handler, disposeHandler: true);

            // Mock the transformer. This assumes the real `UserTransformerService` methods are mockable
            // (virtual or otherwise mockable). Adjust if your implementation differs.
            var transformerMock = new Mock<UserTransformerService>();
         //transformerMock.Setup(t => t.TransformCSV(It.IsAny<object>()))
         //                  .Returns(new UserDto { Name = "FromCsv" });
         //   transformerMock.Setup(t => t.TransformJSON(It.IsAny<object>()))
         //                  .Returns(new UserDto { Name = "FromJson" });   

            var service = new DataLoaderService(httpClient, transformerMock.Object);

            // Act
            var users = await service.LoadUsersAsync();

            // Assert
            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            //Assert.Contains(users, u => u.Name == "FromCsv");
            //Assert.Contains(users, u => u.Name == "FromJson");

            transformerMock.Verify(t => t.TransformCSV(It.IsAny<object>()), Times.Once);
            transformerMock.Verify(t => t.TransformJSON(It.IsAny<object>()), Times.Once);
        }
    }
}