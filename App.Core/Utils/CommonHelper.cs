using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using App.Core.Extensions;

namespace App.Core.Utils
{
	public class CommonHelper
	{
		public static bool IsTruthy(object value)
		{
			if (value == null)
				return false;

			switch (value)
			{
				case string x:
					return x.HasValue();
				case bool x:
					return x == true;
				case DateTime x:
					return x > DateTime.MinValue;
				case TimeSpan x:
					return x > TimeSpan.MinValue;
				case Guid x:
					return x != Guid.Empty;
				case IComparable x:
					return x.CompareTo(0) != 0;
				case IEnumerable<object> x:
					return x.Any();
				case IEnumerable x:
					return x.GetEnumerator().MoveNext();
			}

			if (value.GetType().IsNullable(out var wrappedType))
			{
				return IsTruthy(Convert.ChangeType(value, wrappedType));
			}

			return true;
		}

	    public static string MapPath(string path, bool findAppRoot = true)
	    {
	        if (HostingEnvironment.IsHosted)
	        {
	            return HostingEnvironment.MapPath(path);
	        }
	        else
	        {
	            // not hosted. For example, running in unit tests or EF tooling
	            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
	            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');

	            var testPath = Path.Combine(baseDirectory, path);

	            return testPath;
	        }
        }
    }
}
