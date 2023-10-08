using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Globalization;
using System.Net;
using System.Text;
public class FileLogger : ILogger, IDisposable
{
    string filePath;
    static object _lock = new object();

    public FileLogger(string path)
    {
        filePath = path;
    }
    public IDisposable BeginScope<TState>(TState state)
    {
        return this;
    }
    public void Dispose() { }
    public bool IsEnabled(LogLevel logLevel)
    {
        //return logLevel == LogLevel.Trace;
        return true;
    }
    public void Log<TState>(LogLevel logLevel, EventId eventId,
    TState state, Exception? exception, Func<TState, Exception?, string>
   formatter)
    {
        lock (_lock)
        {
            File.AppendAllText(filePath, formatter(state, exception) +
           Environment.NewLine);
        }
    }
}

public class FileLoggerProvider : ILoggerProvider
{
    string path;
    public FileLoggerProvider(string path)
    {
        this.path = path;
    }
    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(path);
    }
    public void Dispose() { }
}
public static class FileLoggerExtensions
{
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string
   filePath)
    {
        builder.AddProvider(new FileLoggerProvider(filePath));
        return builder;
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory(),
            "logger.txt"));

        var app = builder.Build();

        app.Environment.EnvironmentName = "Production";
        app.UseExceptionHandler("/Error");
        app.Map("/error", app1 => app1.Run(async context =>
        {
            app.Logger.LogInformation($"Path: {context.Request.Path} " +
                $"Time:{DateTime.Now.ToLongTimeString()} " +
                $"Error:Year can only be beetween 1 and 10000");
            context.Response.Redirect("/er");
        }));
        app.Map("/get-data", app1 => app1.Run(async context =>
        {
            string format = "yyyy-MM-ddTHH:mm";
            if (DateTime.TryParseExact(context.Request.Form["datetime"],
                format,
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out DateTime resulted_date))
            {
                context.Response.Cookies.Append("name", context.Request.Form["value"],
                    new CookieOptions
                    {
                        Expires = resulted_date.AddHours(3)
                    });
                context.Response.Redirect("/");
            }
            else
            {
                throw new Exception("Year can only be beetween 1 and 10000");
            }
        }));
        app.Map("/clear-data", app => app.Run(async context =>
        {
            context.Response.Cookies.Delete("name");
            context.Response.Redirect("/");
        }));

        app.Run(async context =>
        {
            StringBuilder sb = new StringBuilder();
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
                    "    <title></title>\r\n" +
                    "</head>\r\n" +
                    "<body>\r\n" +
                    "    <h1>Enter your name and date to expires</h1>\r\n" +
                    "    <form action=\"get-data\" method=\"post\">\r\n" +
                    "        <label for=\"value\">name:</label>\r\n" +
                    "        <input type=\"text\" id=\"value\" name=\"value\" required><br><br>\r\n");
                if (context.Request.Path == "/er") {
                    sb.Append("<span style=\"color:red;\">Year can only be beetween 1 and 10000</span><br>\r\n");
                }
                sb.Append(
                    "        <label for=\"datetime\">date:</label>\r\n" +
                    "        <input type=\"datetime-local\" id=\"datetime\" name=\"datetime\" ><br><br>\r\n" +
                    "        \r\n        <input type=\"submit\" value=\"send\">\r\n" +
                    "    </form>\r\n" +
                    "</body>\r\n</html>");
                    

                await context.Response.WriteAsync(sb.ToString());
            }
        });


        app.Run();
    }
}