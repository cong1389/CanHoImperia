using System.Collections.Generic;
using App.Core.Utils;
using App.Domain.Entities.Other;
using App.Domain.Interfaces.Services;

namespace App.Service.Other
{
	public interface ILandingPageService : IBaseService<LandingPage>
	{
		IEnumerable<LandingPage> PagedList(SortingPagingBuilder sortBuider, Paging page);
	}
}