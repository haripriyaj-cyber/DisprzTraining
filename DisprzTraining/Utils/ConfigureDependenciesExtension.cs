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
            services.AddScoped<IUserRepository, UserRepository>();
            
            // Register services
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IUserService, UserService>();
            
            // Register AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));
        }
    }
}
