using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;

namespace TextCopy;

public static class ServiceExtensions
{
	public static void InjectMockClipboard(this IServiceCollection services)
	{
		Guard.AgainstNull(services, "services");
		services.AddSingleton((Func<IServiceProvider, IClipboard>)((IServiceProvider _) => new MockClipboard()));
	}

	public static void InjectClipboard(this IServiceCollection services)
	{
		Guard.AgainstNull(services, "services");
		services.AddSingleton((Func<IServiceProvider, IClipboard>)delegate(IServiceProvider provider)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser")))
			{
				Type type = Type.GetType("Microsoft.JSInterop.IJSRuntime, Microsoft.JSInterop", throwOnError: false);
				if (type == null)
				{
					throw new Exception("Running in Blazor but could not resolve JSInterop type.");
				}
				object service = provider.GetService(type);
				if (service == null)
				{
					throw new Exception("Running in Blazor but could not get the JSInterop instance from the IServiceProvider.");
				}
				return new BlazorClipboard(service);
			}
			return new Clipboard();
		});
	}
}
