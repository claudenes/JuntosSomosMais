using JSM.Application.Interfaces;
using JSM.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JSM.Infra.IoC
{
    public static class DependencyInjection
    {
       
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var myhandlers = AppDomain.CurrentDomain.Load("JSM.Application");

            return services;
        }
        
        public static IServiceCollection AddServices(this IServiceCollection services) 
        {
            services.AddScoped<IDataLoaderService, DataLoaderService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserTransformerService, UserTransformerService>();

            return services;
        }
    }
    
}
