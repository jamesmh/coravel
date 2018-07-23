using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Coravel.Mail.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;

namespace Coravel.Mail.Renderers
{
    public class RazorRenderer : IRazorRenderer    
    {
        private IRazorViewEngine _viewEngine;
        private ITempDataProvider _tempDataProvider;
        private IServiceProvider _serviceProvider;
        private IConfiguration _config;

        public RazorRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IConfiguration config)
        {
            this._viewEngine = viewEngine;
            this._tempDataProvider = tempDataProvider;
            this._serviceProvider = serviceProvider;
            this._config = config;
        }

        public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
        {
            var actionContext = GetActionContext();
            var view = FindView(actionContext, viewName);

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    new ViewDataDictionary<TModel>(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary())
                    {
                        Model = model
                    },
                    new TempDataDictionary(
                        actionContext.HttpContext,
                        this._tempDataProvider),
                    output,
                    new HtmlHelperOptions());

                    this.BindConfigurationToViewBag(viewContext.ViewBag);

                await view.RenderAsync(viewContext);

                return output.ToString();
            }
        }

        private void BindConfigurationToViewBag(dynamic viewBag)
        {
            string logoSrc = this._config.GetValue<string>("Coravel:Mail:LogoSrc");
            string companyName = this._config.GetValue<string>("Coravel:Mail:CompanyName");
            string companyAddress = this._config.GetValue<string>("Coravel:Mail:CompanyAddress");

            void IfNotEmpty(string str, Action a){
                if(!string.IsNullOrWhiteSpace(str))
                    a();
            }

            IfNotEmpty(logoSrc, () => viewBag.LogoSrc = logoSrc);
            IfNotEmpty(companyName, () => viewBag.CompanyName = companyName);
            IfNotEmpty(companyAddress, () => viewBag.CompanyAddress = companyAddress);
        }

        private IView FindView(ActionContext actionContext, string viewName)
        {
            var getViewResult = this._viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
            if (getViewResult.Success)
            {
                return getViewResult.View;
            }

            var findViewResult = this._viewEngine.FindView(actionContext, viewName, isMainPage: true);
            if (findViewResult.Success)
            {
                return findViewResult.View;
            }

            var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
            var errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations)); ;

            throw new InvalidOperationException(errorMessage);
        }

        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = this._serviceProvider;
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }    
}