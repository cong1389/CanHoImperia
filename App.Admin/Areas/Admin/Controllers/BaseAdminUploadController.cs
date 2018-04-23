using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using App.Core.Caching;
using App.Service.Language;

namespace App.Admin.Controllers
{
    public class BaseAdminUploadController : Controller
    {
        private readonly ICacheManager _cacheManager;

        protected virtual void AddLocales<TLocalizedPropertyViewModelLocal>(ILanguageService languageService, IList<TLocalizedPropertyViewModelLocal> locales) where TLocalizedPropertyViewModelLocal : ILocalizedModelLocal
        {
            AddLocales(languageService, locales, null);
        }

        protected virtual void AddLocales<TLocalizedPropertyViewModelLocal>(ILanguageService languageService, IList<TLocalizedPropertyViewModelLocal> locales, Action<TLocalizedPropertyViewModelLocal, int> configure) where TLocalizedPropertyViewModelLocal : ILocalizedModelLocal
        {
            foreach (var language in languageService.FindBy(x => x.Status == 1))
            {
                var locale = Activator.CreateInstance<TLocalizedPropertyViewModelLocal>();
                locale.LanguageId = language.Id;
                if (configure != null)
                {
                    configure.Invoke(locale, locale.LanguageId);
                }
                locales.Add(locale);
            }
        }

        public int PageSize
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["ItemsPerPage"] ?? "10");
            }
        }

        protected string Key
        {
            get
            {
                return ViewData["Key"].ToString();
            }
            set
            {
                ViewData["Key"] = value;
            }
        }

        public BaseAdminUploadController(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;

            _cacheManager.Clear();
            //ICacheManager ICacheManager = DependencyResolver.Current.GetService<ICacheManager>();
        }
    }
}