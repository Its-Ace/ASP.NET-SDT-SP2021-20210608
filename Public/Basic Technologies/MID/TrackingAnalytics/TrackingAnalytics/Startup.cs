using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TrackingAnalytics.Infrastructure.Context;
using TrackingAnalytics.middlewares;


namespace TrackingAnalytics
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
			services.AddControllersWithViews();
			string mysqlString = Configuration.GetConnectionString("DBconnection");

			services.AddDbContextPool<RecordContext>(options => options.UseMySql(mysqlString, ServerVersion.AutoDetect(mysqlString)));
			services.AddTransient(typeof(RecordContext), typeof(RecordContext));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			//Configure middle ware here using IApplication builder

			/* Used For running the middleware
			 * app.Run() (To add the middleware)
			 * Now,What is this Run function
			 * 
			 * public static void Run(this IApplicationBuilder app, RequestDelegate handler)
			 * Takes a delegate
			 * 
			 * Now, What is a delegate 
			 * 
			 * For now we will use it in our custom middlewares 
			 */

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			//Here below app.UseFunctionnames are Middlewares their patteren is quite important we cannot 
			//Change their positions 

			app.UseHttpsRedirection();

			// ...
			// get real IP from reverse proxy

			app.Use(async (context, next) =>
			{
				context.Response.OnStarting(() =>
				{//Important comments
					// if Antiforgery hasn't already set this header
					if (string.IsNullOrEmpty(context.Response.Headers["X-Frame-Options"]))
					{
						// do not allow to put your website pages into frames (prevents clickjacking)
						context.Response.Headers.Add("X-Frame-Options", "DENY");
					}
					// check MIME types (prevents MIME-based attacks)
					context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
					// hide server information
					context.Response.Headers.Add("Server", "ololo");
					// allow to load scripts only from listed sources
					//context.Response.Headers.Add("Content-Security-Policy", "default-src 'self' *.google-analytics.com; style-src 'self' 'unsafe-inline'; script-src 'self' 'unsafe-inline'");
					return Task.FromResult(0);
				});

				await next();
			});

			app.UseAnalytics();
			// ...

			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
