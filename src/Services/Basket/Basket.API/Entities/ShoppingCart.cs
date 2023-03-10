namespace Basket.API.Entities;

public class ShoppingCart
{
    public ShoppingCart()
    {
    }

    public ShoppingCart(string username)
    {
        UserName = username;
    }

    public decimal TotalPrice => Items.Select(x => x.Price * x.Quantity).Sum();
    public string? UserName { get; set; }
    public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
}

