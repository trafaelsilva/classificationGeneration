// See https://aka.ms/new-console-template for more information

using ClassificationsGeneration;
using Google.Apis.Services;
using Google.Apis.Translate.v2;
using Google.Cloud.Translation.V2;
using Microsoft.Extensions.Configuration;
using System.Globalization;

var EntityName = "ReminderOption";
var classificationData = new List<Classification>();

classificationData.Add(new Classification("ReminderCodes.None", "None"));
classificationData.Add(new Classification("ReminderCodes.Tomorrow", "Tomorrow"));
classificationData.Add(new Classification("ReminderCodes.In3Days", "In 3 Days"));
classificationData.Add(new Classification("ReminderCodes.NextWeek", "Next Week"));
classificationData.Add(new Classification("ReminderCodes.NextMonth", "Next Month"));
classificationData.Add(new Classification("ReminderCodes.FiveDaysBeforeDue", "5 Days Before Due"));

var languages = new List<string>() { "en", "pt", "ja" };
IConfiguration config = new ConfigurationBuilder()
    .AddUserSecrets<Classification>()
    .Build();
var apikey = config["ApiKey"];

var service = new TranslateService(new BaseClientService.Initializer { ApiKey = apikey });
var client = new TranslationClientImpl(service, TranslationModel.ServiceDefault);
var fileName = "ClassificationsTranslations.txt";
var order = 1;
using (StreamWriter writer = new StreamWriter(fileName))
{
    foreach (var c in classificationData)
    {
        var guid = Guid.NewGuid();
        c.Guid = guid;
        writer.WriteLine($"new {EntityName}() {{ Id = new Guid(\"{c.Guid}\"), Code = {c.Code}, Description = \"{c.Description}\", OriginalDisplayValue = \"{c.Description}\", DisplayOrder = {order}, OriginalDisplayOrder = {order}, IsActive = true, Created = DateTime.Parse(\"{DateTime.Now.ToString("dd-MMMM-yyyy", DateTimeFormatInfo.InvariantInfo)}\"), CreatedBy = Audit.System, Modified = DateTime.Parse(\"{DateTime.Now.ToString("dd-MMMM-yyyy", DateTimeFormatInfo.InvariantInfo)}\"), ModifiedBy = Audit.System, IsSystemClassification = true }},");
        ++order;
    }

    writer.WriteLine("");
    writer.WriteLine("TRANSLATIONS");
    writer.WriteLine("");

    foreach (var c in classificationData)
    {
        foreach (var l in languages)
        {
            var translatedText = client.TranslateText(c.Description, l).TranslatedText;
            writer.WriteLine($"new ClassificationTranslation() {{ Id = new Guid(\"{Guid.NewGuid()}\"), EntityId = new Guid(\"{c.Guid}\"), LanguageCode = \"{l}\", DisplayValue = \"{translatedText}\" }},");
        }
    }
}

Console.WriteLine($"Done - please check file {fileName}");
Console.ReadLine();