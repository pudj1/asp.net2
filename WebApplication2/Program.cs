using System.Text;

var builder = WebApplication.CreateBuilder();
var services = builder.Services;
services.AddSingleton<ICalcService, CalcService>();
services.AddSingleton<ITimeService, TimeService>();
var app = builder.Build();

app.Run(async context =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    var calcService = app.Services.GetService<ICalcService>();
    var timeService = app.Services.GetService<ITimeService>();
    var sb = new StringBuilder();
    sb.Append($"22 + 33: {calcService?.Add(22, 33)}<br>");
    sb.Append($"23 - 6: {calcService?.Subtract(23, 6)}<br>");
    sb.Append($"5 * 6: {calcService?.Mylt(5, 6)}<br>");
    sb.Append($"5 / 6: {calcService?.Divide(5.0, 6.0)}<br>");
    sb.Append($"{timeService.GetTime()}");
    await context.Response.WriteAsync(sb.ToString());
});

app.Run();

interface ICalcService { 
    T Add<T>(T a, T b);
    T Subtract<T>(T a, T b);
    T Mylt<T>(T a, T b);
    T Divide<T>(T a, T b);
}

class CalcService : ICalcService
{
    public T Add<T>(T a, T b)
    {
        return (dynamic)a + (dynamic)b;
    }

    public T Divide<T>(T a, T b)
    {
        return (dynamic)a / (dynamic)b;
    }

    public T Mylt<T>(T a, T b)
    {
        return (dynamic)a * (dynamic)b;
    }

    public T Subtract<T>(T a, T b)
    {
        return (dynamic)a - (dynamic)b;
    }
}

interface ITimeService
{
    string GetTime();
}

class TimeService : ITimeService
{
    public string GetTime()
    {
        DateTime currentTime = DateTime.Now;

        string timeOfDay;
        if (currentTime.Hour >= 12 && currentTime.Hour < 18)
        {
            timeOfDay = "зараз день";
        }
        else if (currentTime.Hour >= 18 && currentTime.Hour < 24)
        {
            timeOfDay = "зараз вечір";
        }
        else if (currentTime.Hour >= 0 && currentTime.Hour < 6)
        {
            timeOfDay = "зараз ніч";
        }
        else
        {
            timeOfDay = "зараз ранок";
        }

        return timeOfDay;
    }
}
