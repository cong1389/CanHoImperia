using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using App.Service.Language;

namespace App.Admin.Controllers
{
    public class BaseAdminController : Controller
	{
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

	    protected string RenderRazorViewToString(string viewName, object model)
		{
			var str=string.Empty;
            try
            {
                ViewData.Model = model;
                using (var stringWriter = new StringWriter())
                {
                    var viewEngineResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                    var viewContext = new ViewContext(ControllerContext, viewEngineResult.View, ViewData, TempData, stringWriter);
                    viewEngineResult.View.Render(viewContext, stringWriter);
                    viewEngineResult.ViewEngine.ReleaseView(ControllerContext, viewEngineResult.View);
                    str = stringWriter.GetStringBuilder().ToString();
                }
            }
            catch
            {
            }

			return str;
		}
	}
}