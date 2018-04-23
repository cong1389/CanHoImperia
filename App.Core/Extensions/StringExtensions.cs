using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using HtmlAgilityPack;

namespace App.Core.Extensions
{
    public static class StringExtensions
    {
        public const string CarriageReturnLineFeed = "\r\n";
        public const string Empty = "";
        public const char CarriageReturn = '\r';
        public const char LineFeed = '\n';
        public const char Tab = '\t';
        

        #region String extensions
        

        [Obsolete("The 'removeTags' parameter is not supported anymore. Use the parameterless method instead.")]
        public static string RemoveHtml(this string source, ICollection<string> removeTags)
        {
            return RemoveHtml(source);
        }

        public static string RemoveHtml(this string source)
        {
            if (source.IsEmpty())
                return string.Empty;

            var doc = new HtmlDocument()
            {
                OptionOutputOriginalCase = true,
                OptionFixNestedTags = true,
                OptionAutoCloseOnEnd = true,
                OptionDefaultStreamEncoding = Encoding.UTF8
            };

            doc.LoadHtml(source);
            var nodes = doc.DocumentNode.Descendants().Where(n =>
               n.NodeType == HtmlNodeType.Text &&
               n.ParentNode.Name != "script" &&
               n.ParentNode.Name != "style" &&
               n.ParentNode.Name != "svg");

            var sb = new StringBuilder();
            foreach (var node in nodes)
            {
                var text = node.InnerText;
                if (text.HasValue())
                {
                    sb.AppendLine(node.InnerText);
                }
            }

            return sb.ToString().HtmlDecode();
        }

        #endregion

        [DebuggerStepThrough]
        public static string HtmlDecode(this string value)
        {
            return HttpUtility.HtmlDecode(value);
        }

        [DebuggerStepThrough]
        public static string NullEmpty(this string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }

        [DebuggerStepThrough]
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        [DebuggerStepThrough]
        public static bool IsCaseInsensitiveEqual(this string value, string comparing)
        {
            return string.Compare(value, comparing, StringComparison.OrdinalIgnoreCase) == 0;
        }

        [DebuggerStepThrough]
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        [DebuggerStepThrough]
        public static string EmptyNull(this string value)
        {
            return (value ?? string.Empty).Trim();
        }

		[DebuggerStepThrough]
		public static string EnsureEndsWith(this string value, string endWith)
		{
			if (value.Length >= endWith.Length)
			{
				if (string.Compare(value, value.Length - endWith.Length, endWith, 0, endWith.Length, StringComparison.OrdinalIgnoreCase) == 0)
					return value;

				string trimmedString = value.TrimEnd(null);

				if (string.Compare(trimmedString, trimmedString.Length - endWith.Length, endWith, 0, endWith.Length, StringComparison.OrdinalIgnoreCase) == 0)
					return value;
			}

			return value + endWith;
		}

	    [DebuggerStepThrough]
	    public static string FormatInvariant(this string format, params object[] objects)
	    {
		    return string.Format(CultureInfo.InvariantCulture, format, objects);
	    }

	    [DebuggerStepThrough]
	    public static string[] SplitSafe(this string value, string separator)
	    {
		    if (string.IsNullOrEmpty(value))
			    return new string[0];

		    // do not use separator.IsEmpty() here because whitespace like " " is a valid separator.
		    // an empty separator "" returns array with value.
		    if (separator == null)
		    {
			    separator = "|";

			    if (value.IndexOf(separator) < 0)
			    {
				    if (value.IndexOf(';') > -1)
				    {
					    separator = ";";
				    }
				    else if (value.IndexOf(',') > -1)
				    {
					    separator = ",";
				    }
				    else if (value.IndexOf(Environment.NewLine) > -1)
				    {
					    separator = Environment.NewLine;
				    }
			    }
		    }

		    return value.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
	    }
	}
}
