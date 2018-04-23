using System.Collections.Generic;
using App.Core.Utils;
using App.Domain.Interfaces.Repository;
using App.Domain.Shippings;

namespace App.Infra.Data.Repository.ShippingMethods
{
    public interface IShippingMethodRepository : IRepositoryBase<ShippingMethod>
	{
		ShippingMethod GetById(int id);

		IEnumerable<ShippingMethod> PagedList(Paging page);

		IEnumerable<ShippingMethod> PagedSearchList(SortingPagingBuilder sortBuider, Paging page);
	}
}