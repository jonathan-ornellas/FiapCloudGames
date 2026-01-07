namespace FiapCloudGames.Lambda.Functions;

using Amazon.Lambda.Core;
using FiapCloudGames.Shared.Models;
using FiapCloudGames.Shared.Events;
using FiapCloudGames.Lambda.Services;
using Newtonsoft.Json;

public class NotificationFunction
{
    private readonly IEmailService _emailService;
    private readonly string _recipientEmail;

    public NotificationFunction()
    {
        var senderEmail = Environment.GetEnvironmentVariable("Email__SenderEmail") ?? "noreply@fiapcloudgames.com";
        var recipientEmail = Environment.GetEnvironmentVariable("Email__RecipientEmail") ?? "jonathan.nnt@hotmail.com";
        var awsRegion = Environment.GetEnvironmentVariable("Email__AwsRegion") ?? "us-east-1";
        
        _recipientEmail = recipientEmail;
        _emailService = new EmailService(senderEmail, recipientEmail, awsRegion);
    }

    public async Task<string> HandlePaymentNotificationAsync(PaymentProcessedEvent @event, ILambdaContext context)
    {
        context.Logger.LogLine($"Processing payment notification for payment: {@event.PaymentId}");

        try
        {
            await _emailService.SendPaymentNotificationAsync(
                recipientEmail: _recipientEmail,
                userId: @event.UserId,
                gameId: @event.GameId,
                amount: @event.Amount,
                paymentStatus: @event.Status
            );

            var result = new
            {
                success = true,
                message = "Payment notification sent successfully",
                paymentId = @event.PaymentId,
                recipientEmail = _recipientEmail,
                timestamp = DateTime.UtcNow
            };

            context.Logger.LogLine($"Notification sent: {JsonConvert.SerializeObject(result)}");
            return JsonConvert.SerializeObject(result);
        }
        catch (Exception ex)
        {
            context.Logger.LogLine($"Error sending notification: {ex.Message}");
            return JsonConvert.SerializeObject(new
            {
                success = false,
                error = ex.Message,
                paymentId = @event.PaymentId
            });
        }
    }

    public async Task<string> HandleGamePurchaseAsync(GamePurchasedEvent @event, ILambdaContext context)
    {
        context.Logger.LogLine($"Processing game purchase notification for user: {@event.UserId}");

        try
        {
            await _emailService.SendPaymentNotificationAsync(
                recipientEmail: _recipientEmail,
                userId: @event.UserId.ToString(),
                gameId: @event.GameId.ToString(),
                amount: @event.Amount,
                paymentStatus: "Completed"
            );

            var result = new
            {
                success = true,
                message = "Purchase notification sent successfully",
                userId = @event.UserId,
                gameId = @event.GameId,
                recipientEmail = _recipientEmail,
                timestamp = DateTime.UtcNow
            };

            context.Logger.LogLine($"Purchase notification sent: {JsonConvert.SerializeObject(result)}");
            return JsonConvert.SerializeObject(result);
        }
        catch (Exception ex)
        {
            context.Logger.LogLine($"Error sending purchase notification: {ex.Message}");
            return JsonConvert.SerializeObject(new
            {
                success = false,
                error = ex.Message,
                userId = @event.UserId
            });
        }
    }
}
