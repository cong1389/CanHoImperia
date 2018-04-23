using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using App.Core.Extensions;
using App.Core.Utils;
using App.Framework.Templating.Liquid;
using App.Service.Addresses;
using App.Service.Common;
using DotLiquid;
using Newtonsoft.Json;

namespace App.Framework.Templating.Filters
{
	public static class AdditionalFilters
	{
		private static LiquidTemplateEngine GetEngine()
		{
			return (LiquidTemplateEngine)Template.FileSystem;
		}

		#region Common Filters

		public static object Default(object input, object value)
		{
			return input ?? value;
		}

		public static string Json(object input)
		{
			if (input == null)
				return null;

			return JsonConvert.SerializeObject(input, new JsonSerializerSettings
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			});
		}

		public static string FormatAddress(Context context, object input)
		{
			if (input == null)
				return null;

			var engine = GetEngine();
            //var countryService = engine.Services.Resolve<ICountryService>();

		    var addressService = DependencyResolver.Current.GetService<IAddressService>();
            //var addressService = engine.Services.Resolve<IAddressService>();

			//Country country = null;

			// We know that we converted Address entity to a dictionary.

			//if (input is IDictionary<string, object> dict)
			//{
			//	country = countryService.GetCountryById(dict.Get("CountryId").Convert<int?>().GetValueOrDefault());
			//}
			//else if (input is IIndexable lq)
			//{
			//	country = countryService.GetCountryById(lq["CountryId"].Convert<int?>().GetValueOrDefault());
			//}

			//return addressService.FormatAddress(input, country?.AddressFormat, context.FormatProvider);
		    return string.Empty;
		}

		#endregion

		#region Array Filters

		public static IEnumerable Uniq(IEnumerable input)
		{
			if (input == null)
				return null;

			return input.Cast<object>().Distinct();
		}

		public static IEnumerable Compact(IEnumerable input)
		{
			if (input == null)
				return null;

			return input.Cast<object>().Where(x => x != null);
		}

		public static IEnumerable Reverse(IEnumerable input)
		{
			if (input == null)
				return null;

			return input.Cast<object>().Reverse();
		}

		#endregion
        
		#region Url Filters

		#endregion

		#region Html Filters

		public static string ScriptTag(string input)
		{
			return String.Format("<script src=\"{0}\"></script>", input);
		}

		public static string StylesheetTag(string input)
		{
			return String.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" media=\"all\" />", input);
		}

		public static string ImgTag(string input, string alt = "", string css = "")
		{
			return input == null ? null : GetImageTag(input, alt, css);
		}

		private static string GetImageTag(string src, string alt, string css)
		{
			return String.Format("<img src=\"{0}\" alt=\"{1}\" class=\"{2}\" />", src, alt, css);
		}

		#endregion
	}
}
