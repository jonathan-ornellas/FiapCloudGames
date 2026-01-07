using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Amazon.Lambda.Core;

namespace FiapCloudGames.Lambda.Services;

public interface IEmailService
{
    Task SendPaymentNotificationAsync(string recipientEmail, string userId, string gameId, decimal amount, string paymentStatus);
}

public class EmailService : IEmailService
{
    private readonly IAmazonSimpleEmailService _sesClient;
    private readonly string _senderEmail;
    private readonly string _recipientEmail;
    private readonly string _awsRegion;

    public EmailService(string senderEmail = "noreply@fiapcloudgames.com", string recipientEmail = "jonathan.nnt@hotmail.com", string awsRegion = "us-east-1")
    {
        _senderEmail = senderEmail;
        _recipientEmail = recipientEmail;
        _awsRegion = awsRegion;
        _sesClient = new AmazonSimpleEmailServiceClient(Amazon.RegionEndpoint.GetBySystemName(awsRegion));
    }

    public async Task SendPaymentNotificationAsync(string recipientEmail, string userId, string gameId, decimal amount, string paymentStatus)
    {
        try
        {
            var htmlBody = GeneratePaymentEmailHtml(userId, gameId, amount, paymentStatus);
            var textBody = GeneratePaymentEmailText(userId, gameId, amount, paymentStatus);

            var sendRequest = new SendEmailRequest
            {
                Source = _senderEmail,
                Destination = new Destination { ToAddresses = new List<string> { recipientEmail } },
                Message = new Message
                {
                    Subject = new Content($"Payment Notification - {paymentStatus}"),
                    Body = new Body
                    {
                        Html = new Content(htmlBody),
                        Text = new Content(textBody)
                    }
                }
            };

            var response = await _sesClient.SendEmailAsync(sendRequest);
            LambdaLogger.Log($"Email sent successfully. MessageId: {response.MessageId}");
        }
        catch (Exception ex)
        {
            LambdaLogger.Log($"Error sending email: {ex.Message}");
            throw;
        }
    }

    private string GeneratePaymentEmailHtml(string userId, string gameId, decimal amount, string paymentStatus)
    {
        return $@"
            <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; border: 1px solid #ddd; }}
                        .footer {{ text-align: center; padding: 10px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Payment Notification</h1>
                        </div>
                        <div class='content'>
                            <p>Hello,</p>
                            <p>Your payment has been <strong>{paymentStatus}</strong>.</p>
                            <table border='1' cellpadding='10'>
                                <tr>
                                    <td><strong>User ID:</strong></td>
                                    <td>{userId}</td>
                                </tr>
                                <tr>
                                    <td><strong>Game ID:</strong></td>
                                    <td>{gameId}</td>
                                </tr>
                                <tr>
                                    <td><strong>Amount:</strong></td>
                                    <td>${amount:F2}</td>
                                </tr>
                                <tr>
                                    <td><strong>Status:</strong></td>
                                    <td>{paymentStatus}</td>
                                </tr>
                            </table>
                            <p>Thank you for your purchase!</p>
                        </div>
                        <div class='footer'>
                            <p>&copy; 2026 FIAP Cloud Games. All rights reserved.</p>
                        </div>
                    </div>
                </body>
            </html>";
    }

    private string GeneratePaymentEmailText(string userId, string gameId, decimal amount, string paymentStatus)
    {
        return $@"Payment Notification

Your payment has been {paymentStatus}.

User ID: {userId}
Game ID: {gameId}
Amount: ${amount:F2}
Status: {paymentStatus}

Thank you for your purchase!

© 2026 FIAP Cloud Games. All rights reserved.";
    }
}
