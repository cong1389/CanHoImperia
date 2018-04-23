using System.Collections.Generic;
using System.Text;
using App.Core.Caching;
using App.Core.Utils;
using App.Domain.Entities.Ads;
using App.Infra.Data.Common;
using App.Infra.Data.Repository.Ads;
using App.Infra.Data.UOW.Interfaces;

namespace App.Service.Ads
{
    public class BannerService : BaseService<Banner>, IBannerService
    {
        private const string CacheKey = "db.Banner.{0}";
        private readonly ICacheManager _cacheManager;

        private readonly IBannerRepository _bannerRepository;

        public BannerService(IUnitOfWork unitOfWork, IBannerRepository bannerRepository, ICacheManager cacheManager) : base(unitOfWork, bannerRepository)
        {
            _bannerRepository = bannerRepository;
            _cacheManager = cacheManager;
        }

        public Banner GetById(int id, bool isCache = true)
        {
            Banner banner;

            if (isCache)
            {
                var sbKey = new StringBuilder();
                sbKey.AppendFormat(CacheKey, "GetById");
                sbKey.Append(id);

                var key = sbKey.ToString();
                banner = _cacheManager.Get<Banner>(key);
                if (banner == null)
                {
                    banner = _bannerRepository.GetById(id);
                    _cacheManager.Put(key, banner);
                }
            }
            else
            {
                banner = _bannerRepository.GetById(id);
            }            

            return banner;
        }

        public IEnumerable<Banner> PagedList(SortingPagingBuilder sortbuBuilder, Paging page)
        {
            return _bannerRepository.PagedSearchList(sortbuBuilder, page);
        }
    }
}