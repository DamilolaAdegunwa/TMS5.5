using Egoal.Email.MailKit;
using Egoal.Invoice.GuangDongBaiWangJiuBin;
using Egoal.Net.Http;
using Egoal.Payment.ABCPay;
using Egoal.Payment.Alipay;
using Egoal.Payment.SaobePay;
using Egoal.Payment.WeChatPay;
using Egoal.Redis;
using Egoal.ShortMessage.Huyi;
using Egoal.WeChat;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;

namespace Egoal.Web.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddKernelModule();
            services.AddBackgroundJob();
            services.AddAspNetCoreModule();
            services.AddEntityFrameworkCoreModule();
            services.AddDbContext(Configuration.GetConnectionString(AppConsts.ConnectionStringName));
            services.AddModelModule();
            services.AddApplicationModule(Configuration);
            services.AddDomainModule();
            services.AddRepositoryModule();

            services.AddWeChatModule(Configuration);
            services.AddWeChatPayModule();
            services.AddAlipayModule(Configuration);
            services.AddSaobePayModule(Configuration);
            services.AddABCPayModule(Configuration);

            services.AddHuyiShortMessage(Configuration);

            services.AddMailKitEmail(Configuration);

            services.AddGuangDongBaiWangJiuBinInvoice(Configuration);

            services.AddRedisLock(Configuration);

            services.AddMemoryCache();
            services.AddCors();
            services.AddMvc(mvcOptions =>
            {
                mvcOptions.AddCustomFilters();
                mvcOptions.AddCustomModelBinders();
            })
            .AddXmlSerializerFormatters()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSignalR();
            services.AddHttpClient<HttpService>(c => c.Timeout = TimeSpan.FromSeconds(AppConsts.HttpTimeoutSeconds))
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new SocketsHttpHandler
                    {
                        ConnectTimeout = TimeSpan.FromSeconds(AppConsts.HttpTimeoutSeconds),
                        SslOptions = new SslClientAuthenticationOptions
                        {
                            RemoteCertificateValidationCallback = (sender, certificate, chain, errors) => true
                        }
                    };
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v5.5", new Info { Title = "WebApi接口文档", Version = "v5.5" });

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = "apiKey"
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } }
                });

                c.CustomSchemaIds((type) => type.FullName);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v5.5/swagger.json", "WebApi接口文档 v5.5");
                });
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();

            //app.UseHttpsRedirection();

            app.UseCors(builder =>
            {
                builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetPreflightMaxAge(TimeSpan.FromDays(30));

                if (env.IsDevelopment())
                {
                    builder.AllowAnyOrigin();
                }
                else
                {
                    builder.WithOrigins(Configuration.GetValue<string>("ClientWhiteList").Split(',', StringSplitOptions.RemoveEmptyEntries));
                }
            });

            app.UseNotification();

            app.UseStaticFiles();
            app.MapWhen(
                httpContext =>
                {
                    var path = httpContext.Request.Path.Value;
                    return path != "/" && !path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase);
                },
                builder =>
                {
                    var options = new RewriteOptions();
                    options.AddRewrite("^[aA]dmin/(.*)", "/admin/index.html", true);
                    options.AddRewrite("^[hH]andset/(.*)", "/handset/index.html", true);
                    //options.AddRewrite("^[mM]obile/(.*)", "/mobile/index.html", true);
                    //options.AddRewrite("^[wW]eb[sS]ale/(.*)", "/websale/index.html", true);
                    builder.UseRewriter(options);

                    builder.UseStaticFiles();
                });

            app.UseMvc();

            applicationLifetime.ApplicationStarted.Register(new Action<object>(OnStarted), app.ApplicationServices);
            applicationLifetime.ApplicationStopping.Register(new Action<object>(OnStopping), app.ApplicationServices);
        }

        private void OnStarted(object state)
        {
            var serviceProvider = state as IServiceProvider;

            ApplicationModule.Start(serviceProvider);
            RedisModule.Start(serviceProvider);
            WeChatModule.Start(serviceProvider);
        }

        private void OnStopping(object state)
        {
            var serviceProvider = state as IServiceProvider;

            RedisModule.Stop(serviceProvider);
        }
    }
}
