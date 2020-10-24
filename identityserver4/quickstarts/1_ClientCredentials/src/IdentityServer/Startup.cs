// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            //services.AddControllersWithViews();

            // IdentityServerサービスをDIに登録、メモリデータストアをセットアップ。
            var builder = services.AddIdentityServer()
                .AddInMemoryApiScopes(Config.ApiScopes) // dev only
                .AddInMemoryClients(Config.Clients);    // dev only

            // 起動時に一時的な署名キーファイル(tempkey.rsa)を生成 
            builder.AddDeveloperSigningCredential();    // dev only
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add MVC
            //app.UseStaticFiles();
            //app.UseRouting();

            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            //app.UseAuthorization();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapDefaultControllerRoute();
            //});
        }
    }
}
