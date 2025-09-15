using DisprzTraining.Business.Services;
using DisprzTraining.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DisprzTraining.Utils
{
    public static class ConfigureDependenciesExtension
    {
        public static void ConfigureDependencyInjections(this IServiceCollection services)
        {
            // Register repositories
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            
            // Register services
            services.AddScoped<IAppointmentService, AppointmentService>();
            
            // Register AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));
        }
    }
}
