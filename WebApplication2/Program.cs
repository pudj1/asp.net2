using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
builder.Configuration
   .AddJsonFile("config.json")
   .AddJsonFile("config2.json")
   .AddXmlFile("config.xml")
   .AddIniFile("config.ini");
app.MapGet("/", (IConfiguration appConfig) => {
    var appleemployees = int.Parse(appConfig["apple:employees"]);
    var googleemployees = int.Parse(appConfig["Google-employees-number"]);
    var microsoftemployees = int.Parse(appConfig["Microsoft-employees-number"]);
    var mydata = $"\n\nname:{appConfig["name"]}, surname:{appConfig["surname"]}";
    if (appleemployees > microsoftemployees)
    {
        if (appleemployees > googleemployees)
        {
            return $"apple:{appleemployees}" + mydata;
        }
    }
    else if (googleemployees > microsoftemployees)
    {
        return $"google:{googleemployees}" + mydata;
    }
    else {
        return $"microsoft:{microsoftemployees}" + mydata;
    }
    return "";
    });
app.Run();

