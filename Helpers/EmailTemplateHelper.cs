using RetailOrdering.API.Models;

namespace RetailOrdering.API.Helpers;

public static class EmailTemplateHelper
{
    public static string OrderConfirmation(Order order, User user)
    {
        var itemRows = string.Join("", order.OrderItems.Select(i =>
            $"<tr><td>{i.Product?.Name}</td><td>{i.Quantity}</td>" +
            $"<td>Rs. {i.UnitPrice:F2}</td></tr>"));

        return $"""
            <html><body style="font-family:Arial,sans-serif;color:#333">
              <h2 style="color:#1E3A5F">Order Confirmed!</h2>
              <p>Hi {user.FullName}, your order #{order.OrderId} has been placed.</p>
              <table border="1" cellpadding="8" cellspacing="0" width="100%">
                <tr style="background:#1E3A5F;color:#fff">
                  <th>Item</th><th>Qty</th><th>Price</th>
                </tr>
                {itemRows}
                <tr><td colspan="2"><b>Total</b></td>
                    <td><b>Rs. {order.TotalAmount:F2}</b></td></tr>
              </table>
              <p>Delivery to: {order.ShippingAddress}</p>
              <p style="color:#888;font-size:12px">Thank you for ordering with us!</p>
            </body></html>
            """;
    }
}
