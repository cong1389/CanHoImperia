using System.Collections.Generic;
using App.Core.Utils;
using App.Domain.Interfaces.Services;
using App.Domain.Orders;

namespace App.Service.Orders
{
    public interface IOrderItemService : IBaseService<OrderItem>
	{
		OrderItem GetById(int id);

		IEnumerable<OrderItem> PagedList(SortingPagingBuilder sortBuider, Paging page);
	}
}