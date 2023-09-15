using System.Text;

var builder = WebApplication.CreateBuilder();
var services = builder.Services;
services.AddTransient<ICalcService, CalcService>();
var app = builder.Build();

app.Run(async context =>
{
    var calcService = app.Services.GetService<ICalcService>();
    var sb = new StringBuilder();
    sb.Append($"22 + 33: {calcService?.Add(22, 33)}\n");
    sb.Append($"23 - 6: {calcService?.Subtract(23, 6)}\n");
    sb.Append($"5 * 6: {calcService?.Mylt(5, 6)}\n");
    sb.Append($"5 / 6: {calcService?.Divide(5.0, 6.0)}\n");
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
