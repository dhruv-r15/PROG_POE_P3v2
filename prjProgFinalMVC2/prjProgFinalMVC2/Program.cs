using prjProgFinalMVC2.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Polly.Extensions.Http;
using Polly;

namespace prjProgFinalMVC2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder);
            var app = builder.Build();
            ConfigureMiddleware(app);
            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllersWithViews()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true);

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddLogging();
            builder.Services.AddMemoryCache();
            ConfigureHttpClients(builder);
            ConfigureAuthentication(builder);
            ConfigureSession(builder);
            builder.Services.AddResponseCaching();
        }

        private static void ConfigureHttpClients(WebApplicationBuilder builder)
        {
            var baseUrl = builder.Configuration["ApiSettings:BaseUrl"]
                ?? throw new InvalidOperationException("API Base URL not configured");

            builder.Services.AddHttpClient("BaseClient", client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddPolicyHandler(GetRetryPolicy(builder.Configuration));

            RegisterServices(builder.Services);
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IAuthService>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("BaseClient");
                var configuration = sp.GetRequiredService<IConfiguration>();
                return new AuthService(client, configuration);
            });

            services.AddScoped<IModuleService>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("BaseClient");
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                return new ModuleService(client, httpContextAccessor);
            });

            services.AddScoped<IClaimService>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("BaseClient");
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var logger = sp.GetRequiredService<ILogger<ClaimService>>();
                return new ClaimService(client, httpContextAccessor, logger);
            });


            services.AddScoped<IDocumentService>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("BaseClient");
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var logger = sp.GetRequiredService<ILogger<DocumentService>>();
                return new DocumentService(client, httpContextAccessor, logger);
            });


            services.AddScoped<ILecturerService>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("BaseClient");
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                return new LecturerService(client, httpContextAccessor);
            });
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(IConfiguration configuration)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        private static void ConfigureAuthentication(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                });
        }

        private static void ConfigureSession(WebApplicationBuilder builder)
        {
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx => ctx.Context.Response.Headers.Append(
                    "Cache-Control", "public, max-age=31536000")
            });

            app.UseSession();
            app.UseResponseCaching();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }
    }
}









