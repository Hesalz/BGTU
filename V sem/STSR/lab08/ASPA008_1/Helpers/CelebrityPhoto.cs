using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;

namespace ASPA008_1.Helpers
{
    public class MiniString : IHtmlContent
    {
        private readonly string _html;

        public MiniString(string html)
        {
            _html = html;
        }
        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            writer.Write(_html);
        }
        public override string ToString() => _html;
    }
    public static class CelebrityHelpers
    {
        public static MiniString CelebrityPhoto(this IHtmlHelper html, int id, string title, string src)
        {
            string onclick = $"location.href='/{id}'";
            string onload =
                "let h = this.height; let w = this.width;" +
                "let k = this.naturalWidth / this.naturalHeight;" +
                "if (h != 0 && w == 0) { this.width = k * h; }" +
                "if (h == 0 && w != 0) { this.height = w / k; }";

            string result = $"<img " +
                $"id='{id}' " +
                $"class='celebrity-photo' " +
                $"title='{title}' " +
                $"src='{src}' " +
                $"onclick='{onclick}' " +
                $"onload='{onload}'" +
                $"/>";

            return new MiniString(result);
        }
    }
}
