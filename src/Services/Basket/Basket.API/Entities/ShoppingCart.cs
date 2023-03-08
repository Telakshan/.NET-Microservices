﻿namespace Basket.API.Entities;

public class ShoppingCart
{
    public ShoppingCart()
    {
    }

    public ShoppingCart(string username)
    {
        UserName = username;
    }

    public decimal TotalPrice
    {
        get
        {
            return Items.Select(x => x.Price).Sum();
        }
    }

    public string? UserName { get; set; }
    public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
}

