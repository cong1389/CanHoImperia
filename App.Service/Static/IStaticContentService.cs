using System.Collections.Generic;
using App.Core.Utils;
using App.Domain.Entities.Data;
using App.Domain.Interfaces.Services;

namespace App.Service.Static
{
	public interface IStaticContentService : IBaseService<StaticContent>
	{
		StaticContent GetById(int id, bool isCache = true);

		IEnumerable<StaticContent> GetBySeoUrl(string seoUrl, bool isCache = true);

		IEnumerable<StaticContent> PagedList(SortingPagingBuilder sortBuider, Paging page);

		IEnumerable<StaticContent> PagedListByMenu(SortingPagingBuilder sortBuider, Paging page);

	    IEnumerable<StaticContent> GetEnableOrDisables(bool enable = true, bool isCache = true);

	}
}