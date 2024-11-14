using BusinessLayer.Container;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using TraversalProject.CQRS.Handlers.DestinationHandlers;
using TraversalProject.Mapping.AutoMapperProfile;
using TraversalProject.Models;

namespace TraversalProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            
            builder.Services.AddLogging(x =>
            {
                x.ClearProviders();
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddDebug();
            });
            builder.Services.AddScoped<GetAllDestinationQueryHandler>();
            builder.Services.AddScoped<GetDestinationByIdQueryHandler>();
            builder.Services.AddScoped<CreateDestinationCommandHandler>();
            builder.Services.AddScoped<RemoveDestinationCommandHandler>();
            builder.Services.AddScoped<UpdateDestinationCommandHandler>();

            builder.Services.AddMediatR(typeof(Program));
            builder.Services.AddDbContext<Context>();
            builder.Services.CustomerValidator();
            builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<Context>()
                .AddErrorDescriber<CustomIdentityValidator>().AddEntityFrameworkStores<Context>();
            builder.Services.AddControllersWithViews().AddFluentValidation();
            builder.Services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder().
                RequireAuthenticatedUser().
                Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });
            builder.Services.AddMvc();
            builder.Services.AddAutoMapper(typeof(MapProfile));
            builder.Services.ContainerDependencies();
            builder.Services.AddHttpClient();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            ILoggerFactory loggerFactory=app.Services.GetRequiredService<ILoggerFactory>();
            var path =Directory.GetCurrentDirectory();
            loggerFactory.AddFile($"{path}\\Logs\\Log1.txt");

            app.UseStatusCodePagesWithReExecute("/ErrorPage/Error404", "?code={0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Default}/{action=Index}/{id?}");

           
            
                app.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
            
          
            
                app.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
            
            app.Run();
        }
    }
}
