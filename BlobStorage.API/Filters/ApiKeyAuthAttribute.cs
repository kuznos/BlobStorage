using BlobStorage.Domain.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace BlobStorage.API.Filters
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
	{
		private const string ApiKeyHeaderName = "ApiKey";
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey))
			{
				context.Result = new UnauthorizedResult();
				return;
			}
			Domain.Enums.AppEnvironment myEnv = Domain.Enums.AppEnvironment.Production;
			if (context.HttpContext.Request.Host.Value.ToLower().Contains("dev") == true || context.HttpContext.Request.Host.Value.ToLower().Contains("localhost") == true)
			{
				myEnv = Domain.Enums.AppEnvironment.Dev;
			}


			var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
			//var apikey = configuration.GetValue<string>("BlobStorageApiKey");
			var apikey = Global_Helper.GetAzureBlobStorageAPIAuthPasswordFromAzureKeyVault(myEnv);
			//

			if (!apikey.Equals(potentialApiKey))
			{
				
				context.Result = new UnauthorizedResult();
				return;
			}
			
			await next();

		}
	}
}
