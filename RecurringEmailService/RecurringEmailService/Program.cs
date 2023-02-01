using Hangfire;
using SendGrid;
using SendGrid.Extensions.DependencyInjection;
using SendGrid.Helpers.Mail;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSendGrid(options => options.ApiKey = "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");

builder.Services.AddHangfire(configuration =>
    configuration.UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseInMemoryStorage());

builder.Services.AddHangfireServer();

var app = builder.Build();

app.MapHangfireDashboard("");

await app.StartAsync();
RecurringJob.AddOrUpdate<EmailJob>(emailJob => emailJob.SendEmail(), "*/15 * * * * *");
await app.WaitForShutdownAsync();

public class EmailJob
{
    private readonly ILogger<EmailJob> logger;
    private readonly ISendGridClient sendGridClient;

    public EmailJob(ILogger<EmailJob> logger, ISendGridClient sendGridClient)
    {
        this.logger = logger;
        this.sendGridClient = sendGridClient;
    }

    public async Task SendEmail()
    {
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("luiskevin.escudero@bosonit.com", "kevin"),
            Subject = "A Recurring Email using Twilio SendGrid",
            PlainTextContent = "Hello and welcome to the world of periodic emails with Hangfire and SendGrid. "
        };
        msg.AddTo(new EmailAddress("luiskevin.escudero@bosonit.com", "prueba"));

        var response = await sendGridClient.SendEmailAsync(msg);

        if (response.IsSuccessStatusCode) logger.LogInformation("Email queued successfully!");
        else logger.LogError("Failed to queue email");
    }

}