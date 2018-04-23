using System.Linq;
using App.Domain.Entities.Orders;

namespace App.Service.Orders
{
    public class OrderTotalCalculationService : IOrderTotalCalculationService
    {
        public decimal GetCurrentCartSubTotal(IOrderedEnumerable<ShoppingCartItem> cart)
        {
            var subTotal = decimal.Zero;

            if (!cart.Any())
            {
                return subTotal;
            }

            subTotal = (decimal) (from mul in cart
                select mul.Quantity * mul.CustomerEnteredPrice).Sum();

            return subTotal;
        }
    }
}
