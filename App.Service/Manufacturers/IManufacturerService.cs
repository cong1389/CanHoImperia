using System.Collections.Generic;
using App.Core.Utils;
using App.Domain.Entities.Data;
using App.Domain.Interfaces.Services;

namespace App.Service.Manufacturers
{
	public interface IManufacturerService : IBaseService<Manufacturer>
	{
		IEnumerable<Manufacturer> PagedList(SortingPagingBuilder sortBuider, Paging page);

		IEnumerable<Manufacturer> PagedListByMenu(SortingPagingBuilder sortBuider, Paging page);
	}
}