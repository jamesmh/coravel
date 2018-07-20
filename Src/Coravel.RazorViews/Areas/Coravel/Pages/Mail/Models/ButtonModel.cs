namespace Coravel.RazorViews.Areas.Coravel.Pages.Mail.Models
{
    public class ButtonModel
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public string BackgroundColorHex { get; set; }

        public ButtonModel(string text, string url, string backgroundHex = null)
        {
            this.Text = text;
            this.Url = url;
            this.BackgroundColorHex = backgroundHex;
        }
    }
}