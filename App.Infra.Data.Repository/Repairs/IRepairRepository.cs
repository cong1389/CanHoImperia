using System.Collections.Generic;
using App.Core.Utils;
using App.Domain.Entities.Data;
using App.Domain.Interfaces.Repository;

namespace App.Infra.Data.Repository.Repairs
{
    public interface IRepairRepository : IRepositoryBase<Repair>
	{
		IEnumerable<Repair> PagedList(Paging page);

		IEnumerable<Repair> PagedSearchList(SortingPagingBuilder sortBuider, Paging page);
	}
}