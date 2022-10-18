using Microsoft.AspNetCore.StaticFiles;

namespace BlobStorage.Domain.Extensions
{
	public static class FileExtensions
	{
		private static readonly FileExtensionContentTypeProvider Provider = new FileExtensionContentTypeProvider();
		public static string GetContentType(this string filename)
		{
			if (!Provider.TryGetContentType(filename, out var contenttype))
			{
				contenttype = "application/octet-stream";
			}
			return contenttype;
		}


		public static string GetPolicyTypeBySymbol(this string symbol)
		{
			string result = "";
			var b = false;
			string tempsymbol = symbol.ToLower().Replace(".pdf", "").Replace(".jpg", "").Replace(".png", "").Replace(".jpeg", "").Replace(".gif", "");


			if (tempsymbol.Trim().StartsWith("7.19"))
			{
				b = true;
			}
			if (tempsymbol.Trim().StartsWith("19."))
			{
				b = true;
			}
			if (tempsymbol.Trim().StartsWith("9.19"))
			{
				b = true;
			}
			if (tempsymbol.Trim().StartsWith("8.19"))
			{
				b = true;
			}
			if (b == false)
			{
				if (tempsymbol.Contains(".") == true)
				{
					result = "NonMotor";
				}
				else
				{
					result = "-";
				}
			}
			else
			{
				result = "Motor";
			}

			return result;

		}

	}
}