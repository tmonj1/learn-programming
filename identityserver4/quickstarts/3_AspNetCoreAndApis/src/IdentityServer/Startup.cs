// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IdentityServerHost.Quickstart.UI;
using IdentityServer4;
using Microsoft.IdentityModel.Tokens;

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

            services.AddAuthentication()
                .AddGoogle("Google", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = "201086464310-lt608ok9383aie9q881p96q4pc0qlutj.apps.googleusercontent.com";
                    options.ClientSecret = "f9lWE2XpdAY3Bn925fiffVhL";
                })
                .AddOpenIdConnect("oidc", "Demo IdentityServer", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                    options.SaveTokens = true;

                    options.Authority = "https://demo.identityserver.io/";
                    options.ClientId = "interactive.confidential";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });
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
