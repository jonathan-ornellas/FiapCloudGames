namespace FiapCloudGames.Lambda.Functions;

using Amazon.Lambda.Core;
using FiapCloudGames.Shared.Models;
using Newtonsoft.Json;

public class NotificationFunction
{
    public async Task<string> HandlePaymentNotificationAsync(PaymentProcessedEvent @event, ILambdaContext context)
    {
        context.Logger.LogLine($"Processing payment notification for payment: {@event.PaymentId}");

        try
        {
            var notification = new
            {
                PaymentId = @event.PaymentId,
                UserId = @event.UserId,
                GameId = @event.GameId,
                Amount = @event.Amount,
                Status = @event.Status,
                Timestamp = @event.Timestamp
            };

            context.Logger.LogLine($"Notification: {JsonConvert.SerializeObject(notification)}");

            await Task.Delay(100);

            return JsonConvert.SerializeObject(new { success = true, message = "Notification sent" });
        }
        catch (Exception ex)
        {
            context.Logger.LogLine($"Error: {ex.Message}");
            throw;
        }
    }

    public async Task<string> HandleGamePurchaseAsync(GamePurchasedEvent @event, ILambdaContext context)
    {
        context.Logger.LogLine($"Processing game purchase notification for user: {@event.UserId}");

        try
        {
            var notification = new
            {
                UserId = @event.UserId,
                GameId = @event.GameId,
                Amount = @event.Amount,
                Timestamp = @event.Timestamp
            };

            context.Logger.LogLine($"Purchase Notification: {JsonConvert.SerializeObject(notification)}");

            await Task.Delay(100);

            return JsonConvert.SerializeObject(new { success = true, message = "Purchase notification sent" });
        }
        catch (Exception ex)
        {
            context.Logger.LogLine($"Error: {ex.Message}");
            throw;
        }
    }
}
