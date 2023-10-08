using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder();

        var app = builder.Build();

        app.UseStatusCodePagesWithReExecute("/error/{0}");

        app.Map("/get-data", app1 => app1.Run(async context =>
        {
            string format = "yyyy-MM-ddTHH:mm";
            DateTime.TryParseExact(context.Request.Form["datetime"],
                format,
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out DateTime resulted_date);
            context.Response.Cookies.Append("name", context.Request.Form["value"],
                new CookieOptions
                {
                    Expires = resulted_date.AddHours(3)
                });
            context.Response.Redirect("/");
        }));
        app.Map("/clear-data", app => app.Run(async context =>
        {
            context.Response.Cookies.Delete("name");
            context.Response.Redirect("/");
        }));

        app.Run(async context =>
        {
            StringBuilder sb = new StringBuilder();
            app.Logger.LogInformation("2");
            context.Response.ContentType = "text/html; charset=utf-8";
            if (context.Request.Cookies.TryGetValue("name", out var name))
            {
                sb.Clear();
                sb.Append($"Hello {name}!\r\n");
                sb.Append($"<a href=\"clear-data\"><button action=\"clear-data\" method=\"post\" type=\"button\"> Clear data </a>");
                await context.Response.WriteAsync(sb.ToString());
            }
            else if (context.Request.Path != "/get-data" || context.Request.Path != "/clear-data")
            {
                sb.Append("<!DOCTYPE html>\r\n" +
                    "<html>\r\n" +
                    "<head>\r\n" +
                    "    <title>Форма для введення значення та дати/часу</title>\r\n" +
                    "</head>\r\n" +
                    "<body>\r\n" +
                    "    <h1>Форма для введення ім'я та дати/часу</h1>\r\n" +
                    "    <form action=\"get-data\" method=\"post\">\r\n" +
                    "        <label for=\"value\">ім'я:</label>\r\n" +
                    "        <input type=\"text\" id=\"value\" name=\"value\" required><br><br>\r\n" +
                    "        \r\n        <label for=\"datetime\">Дата і час:</label>\r\n" +
                    "        <input type=\"datetime-local\" id=\"datetime\" name=\"datetime\" ><br><br>\r\n" +
                    "        \r\n        <input type=\"submit\" value=\"Відправити\">\r\n" +
                    "    </form>\r\n" +
                    "</body>\r\n" +
                    "</html>");
                await context.Response.WriteAsync(sb.ToString());
            }
        });


        app.Run();
    }
}