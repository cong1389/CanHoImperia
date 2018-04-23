using System;
using App.Core.Caching;
using App.Service.LocaleStringResource;

namespace App.Service.Common
{
    public class CommonServices : ICommonServices
    {
        private readonly Lazy<IWorkContext> _workContext;

        private readonly Lazy<ILocaleStringResourceService> _localization;

        private readonly Lazy<ICacheManager> _cacheManager;

        public CommonServices(Lazy<IWorkContext> workContext, Lazy<ILocaleStringResourceService> localization, Lazy<ICacheManager> cacheManager)
        {
            _workContext = workContext;
            _localization = localization;
            _cacheManager = cacheManager;
        }

        public IWorkContext WorkContext => _workContext.Value;

        public ILocaleStringResourceService Localization => _localization.Value;

        public ICacheManager Cache => _cacheManager.Value;
    }
}
