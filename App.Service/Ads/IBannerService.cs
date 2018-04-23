using System.Collections.Generic;
using App.Core.Utils;
using App.Domain.Entities.Ads;
using App.Domain.Interfaces.Services;

namespace App.Service.Ads
{
    public interface IBannerService : IBaseService<Banner>
    {
        Banner GetById(int id, bool isCache = true);

        IEnumerable<Banner> PagedList(SortingPagingBuilder sortBuider, Paging page);
    }
}