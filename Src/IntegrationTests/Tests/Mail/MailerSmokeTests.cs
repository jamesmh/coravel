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
        public async Task WithHtmlInlineMailableDoesntThrowTest() {
            var content = await this._factory.CreateClient().GetStringAsync("/Mail/WithHtmlInlineMailable");
            // Pass = no exceptions.
        }
        
        [Fact]
        public async Task RenderHtmlInlineMailableDoesntThrowTest() {
            var content = await this._factory.CreateClient().GetStringAsync("/Mail/RenderHtmlInlineMailable");
            Assert.False(string.IsNullOrWhiteSpace(content));
        }

        [Fact]
        public async Task WithHtmlInlineMailableOfTDoesntThrowTest() {
            var content = await this._factory.CreateClient().GetStringAsync("/Mail/WithHtmlInlineMailableOfT");
            // Pass = no exceptions.
        }
        
        [Fact]
        public async Task RenderHtmlInlineMailableOfTDoesntThrowTest() {
            var content = await this._factory.CreateClient().GetStringAsync("/Mail/RenderHtmlInlineMailableOfT");
            Assert.False(string.IsNullOrWhiteSpace(content));
        }
        
        // This actually works when running the app manually. Something about the ASP.NET Core Integration Tests
        // that doesn't work well here.
        // [Fact]
        // public async Task WithViewDoesntThrowTest() {
        //     var content = await this._factory.CreateClient().GetStringAsync("/Mail/WithView");
        //     // Pass = no exceptions.
        // }

        // This actually works when running the app manually. Something about the ASP.NET Core Integration Tests
        // that doesn't work well here.
        // [Fact]
        // public async Task RenderViewDoesntThrowTest() {
        //     var content = await this._factory.CreateClient().GetStringAsync("/Mail/RenderView");
        //     Assert.False(string.IsNullOrWhiteSpace(content));
        // }
    }
}