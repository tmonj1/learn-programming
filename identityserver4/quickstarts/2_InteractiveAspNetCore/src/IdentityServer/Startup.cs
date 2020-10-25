// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IdentityServerHost.Quickstart.UI;

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
            // 認証画面使用時(=MVC使用時)、以下の2行をuncommentする
            services.AddControllersWithViews();

            // IdentityServerサービスをDIに登録、メモリデータストアをセットアップ。
            var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes) // dev only
                .AddInMemoryClients(Config.Clients)     // dev only
                .AddTestUsers(TestUsers.Users);         // dev only

            // 起動時に一時的な署名キーファイル(tempkey.rsa)を生成 
            builder.AddDeveloperSigningCredential();    // dev only
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 認証画面使用時(=MVC使用時)、以下の2行をuncommentする
            app.UseStaticFiles();
            app.UseRouting();

            // IdPとして動作させる
            app.UseIdentityServer();

            // 認証画面使用時(=MVC使用時)、以下の2行をuncommentする
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
