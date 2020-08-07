using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Egoal.Payment.ABCPay
{
    public static class ABCPayModule
    {
        public static void AddABCPayModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<PayService>();
            services.AddScoped<ABCPayApi>();

            services.Configure<ABCPayOptions>(configuration);
        }
    }
}
