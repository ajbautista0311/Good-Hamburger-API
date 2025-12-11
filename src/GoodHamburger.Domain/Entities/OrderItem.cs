using System;
using System.Collections.Generic;
using System.Text;

namespace GoodHamburger.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Type { get; set; } = string.Empty;

        public Order Order { get; set; } = null!;
    }
}
