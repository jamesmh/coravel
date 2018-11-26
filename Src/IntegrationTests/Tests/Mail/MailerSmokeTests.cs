using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.Mail
{
    public class FileLogMailerSmokeTests : IClassFixture<WebApplicationFactory<TestMvcApp.Startup>>
    {
        private readonly WebApplicationFactory<TestMvcApp.Startup> _factory;

        public FileLogMailerSmokeTests(WebApplicationFactory<TestMvcApp.Startup> factory)
        {            
            this._factory = factory;
        }

        [Fact]
        public async Task WithHtmlDoesntThrowTest() {
            var content = await this._factory.CreateClient().GetStringAsync("/Mail/WithHtml");
            // Pass = no exceptions.
        }
        
        [Fact]
        public async Task RenderHtmlDoesntThrowTest() {
            var content = await this._factory.CreateClient().GetStringAsync("/Mail/RenderHtml");
            Assert.False(string.IsNullOrWhiteSpace(content));
        }
        
        [Fact]
        public async Task WithViewDoesntThrowTest() {
            var content = await this._factory.CreateClient().GetStringAsync("/Mail/WithView");
            // Pass = no exceptions.
        }
        
        [Fact]
        public async Task RenderViewDoesntThrowTest() {
            var content = await this._factory.CreateClient().GetStringAsync("/Mail/RenderView");
            Assert.False(string.IsNullOrWhiteSpace(content));
        }
    }
}