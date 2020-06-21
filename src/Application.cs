﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Markdig;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blog
{
    public class Application : Autofac.Module
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services => services.AddAutofac())
                .UseStartup<Startup>()
                .Build()
                .Run();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ArticleStore>().AsSelf();
            builder.RegisterInstance(new MarkdownPipelineBuilder()
                .UseAutoIdentifiers()
                .UseAutoLinks()
                .UseFootnotes()
                .UsePipeTables()
                .UseSmartyPants()
                .Build())
                .As<MarkdownPipeline>();
        }
    }

    public class ProdModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new MemoryCache(new MemoryCacheOptions()))
                .As<IMemoryCache>();
        }
    }

    public class DevelopmentModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new NoOpCache())
                .As<IMemoryCache>();
        }
    }

    public class Startup
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public Startup(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddRazorOptions(options =>
                {
                    options.ViewLocationFormats.Add("/src/Views/{0}.cshtml");
                });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<Application>();

            if (webHostEnvironment.IsDevelopment())
            {
                builder.RegisterModule<DevelopmentModule>();
            }
            else
            {
                builder.RegisterModule<ProdModule>();
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            if (webHostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
