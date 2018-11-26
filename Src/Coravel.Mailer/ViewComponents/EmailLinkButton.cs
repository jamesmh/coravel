using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Coravel.Mailer.ViewComponents
{
    public class EmailLinkButton : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(
            string text, string url, string backgroundColor = null, string textColor = null
        )
        {
            ViewBag.Text = text;
            ViewBag.url = url;
            ViewBag.BackgroundColor = backgroundColor ?? "#539be2";
            ViewBag.TextColor = textColor ?? "#ffffff";
            var view = View();
            return await Task.FromResult(view);
        }
    }
}