using BlazorWasmLoker.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;

namespace BlazorWasmLoker.Utility
{
    public static class AppSettingConfig
    {
        public static IServiceCollection AddAppSettingService(this IServiceCollection services, string apiEndPoint)
        {
            services.AddHttpClient<LokerService>(x => { x.BaseAddress = new Uri(apiEndPoint); });
            services.AddHttpClient<MotivationService>(x => { x.BaseAddress = new Uri(apiEndPoint); });
            services.AddHttpClient<SettingService>(x => { x.BaseAddress = new Uri(apiEndPoint); });
            services.AddHttpClient<UserService>(x => { x.BaseAddress = new Uri(apiEndPoint); });
            return services;
        }
    }
}
