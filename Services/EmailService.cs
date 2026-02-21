using System.Net;
using System.Net.Mail;
using System.Text;
using GooseShop.Models;

namespace GooseShop.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config) => _config = config;

    /// <summary>
    /// Підтвердження нового замовлення (з можливим паролем)
    /// </summary>
    public async Task SendOrderConfirmationEmail(Order order, string? tempPassword = null)
    {
        var itemsHtml = new StringBuilder();
        foreach (var item in order.OrderItems)
        {
            string photoUrl = !string.IsNullOrEmpty(item.Product?.ImageUrl)
                ? item.Product.ImageUrl
                : "images/default-goose.png";

            itemsHtml.Append($@"
            <tr>
                <td style='padding: 10px; border-bottom: 1px solid #eee;'>
                    <img src='https://yourdomain.com/{photoUrl}' width='50' style='border-radius:5px; vertical-align:middle; margin-right:10px;' />
                    <b>{item.Product?.Name}</b>
                </td>
                <td style='padding: 10px; border-bottom: 1px solid #eee; text-align: center;'>{item.Quantity} шт.</td>
                <td style='padding: 10px; border-bottom: 1px solid #eee; text-align: right;'>{item.PriceAtPurchase:N0} грн</td>
            </tr>");
        }

        string accountInfo = "";
        if (!string.IsNullOrEmpty(tempPassword))
        {
            accountInfo = $@"
            <div style='background-color: #fff3cd; border: 1px solid #ffeeba; padding: 15px; margin-top: 20px; border-radius: 5px;'>
                <h4 style='margin-top: 0; color: #856404;'>Для вас створено акаунт! 🦢</h4>
                <p>Логін: <b>{order.Email}</b><br/>
                Тимчасовий пароль: <span style='background: #eee; padding: 2px 5px;'>{tempPassword}</span></p>
            </div>";
        }

        string body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; border: 1px solid #ddd; padding: 20px;'>
            <h2 style='color: #0d6efd;'>Дякуємо за покупку, {order.CustomerName}! 🦢</h2>
            <p>Ваше замовлення <b>№{order.Id}</b> успішно створено.</p>
            <table style='width: 100%; border-collapse: collapse;'>
                <tr style='background: #f8f9fa;'>
                    <th style='padding: 10px; text-align: left;'>Товар</th>
                    <th style='padding: 10px;'>К-сть</th>
                    <th style='padding: 10px; text-align: right;'>Ціна</th>
                </tr>
                {itemsHtml}
            </table>
            <h3 style='text-align: right; color: #0d6efd;'>Разом: {order.TotalAmount:N0} грн</h3>
            {accountInfo} 
        </div>";

        await SendEmailAsync(order.Email, $"Замовлення №{order.Id} прийнято! GooseShop", body);
    }

    /// <summary>
    /// Повідомлення про готовність замовлення (викликається з адмінки)
    /// </summary>
    public async Task SendOrderReadyEmail(Order order)
    {
        var subject = $"Ваше замовлення №{order.Id} готове! 🦢";
        var body = $@"
        <div style='font-family: Arial, sans-serif; border: 1px solid #eee; padding: 20px; max-width: 600px;'>
            <h2 style='color: #28a745;'>Чудові новини!</h2>
            <p>Привіт, <b>{order.CustomerName}</b>!</p>
            <p>Ваше замовлення №{order.Id} вже виготовлено та готове до відправлення.</p>
            <p><b>Сума до сплати:</b> {order.TotalAmount:N0} грн</p>
            <p>Дякуємо, що обрали GooseShop!</p>
            <hr>
            <small>Деталі в <a href='https://yourdomain.com/profile'>особистому кабінеті</a>.</small>
        </div>";

        await SendEmailAsync(order.Email, subject, body);
    }

    /// <summary>
    /// Універсальний внутрішній метод для відправки пошти
    /// </summary>
    // ... (весь ваш попередній код без змін)

    /// <summary>
    /// Універсальний внутрішній метод для відправки пошти
    /// </summary>
    // ПЕРЕЙМЕНОВАНО З SendEmailInternalAsync НА SendEmailAsync
    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var mailSettings = _config.GetSection("MailSettings");

        string host = mailSettings["Host"] ?? "";
        string fromMail = mailSettings["Mail"] ?? "";
        string password = mailSettings["Password"] ?? "";
        int port = int.Parse(mailSettings["Port"] ?? "587");

        using var message = new MailMessage()
        {
            From = new MailAddress(fromMail, "GooseShop 🦢"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        message.To.Add(toEmail);

        using var smtp = new SmtpClient(host, port)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromMail, password)
        };

        await smtp.SendMailAsync(message);
    }
}
