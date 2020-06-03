using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

namespace Coravel.Mailer.Mail.Renderers
{
    public class RazorRenderer
    {
        private IRazorViewEngine _viewEngine;
        private ITempDataProvider _tempDataProvider;
        private IServiceProvider _serviceProvider;
        private string _logoSrc;
        private string _companyName;
        private string _companyAddress;
        private string _primaryColor;

        public RazorRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IConfiguration config)
        {
            this._viewEngine = viewEngine;
            this._tempDataProvider = tempDataProvider;
            this._serviceProvider = serviceProvider;

            this._logoSrc = config?.GetValue<string>("Coravel:Mail:LogoSrc");
            this._companyName = config?.GetValue<string>("Coravel:Mail:CompanyName");
            this._companyAddress = config?.GetValue<string>("Coravel:Mail:CompanyAddress");
            this._primaryColor = config?.GetValue<string>("Coravel:Mail:PrimaryColor");
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
            viewBag.LogoSrc = this._logoSrc;
            viewBag.CompanyName = this._companyName;
            viewBag.CompanyAddress = this._companyAddress;
            viewBag.PrimaryColor = this._primaryColor;
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