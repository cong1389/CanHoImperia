using System.Web.Configuration;
using System.Web.Mvc;

namespace App.Front.Models
{
	public class PartialCacheAttribute : OutputCacheAttribute
	{
		public PartialCacheAttribute(string cacheProfileName)
		{
			var item =
			    ((OutputCacheSettingsSection) WebConfigurationManager.GetSection("system.web/caching/outputCacheSettings"))
			    .OutputCacheProfiles[cacheProfileName];
			Duration = item.Duration;
			VaryByParam = item.VaryByParam;
		}
	}
}