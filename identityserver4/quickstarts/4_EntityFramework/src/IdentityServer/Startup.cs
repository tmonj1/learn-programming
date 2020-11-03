// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Reflection;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using IdentityServerHost.Quickstart.UI;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using System.Security.Claims;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Collections.Generic;

namespace IdentityServer
{
    //
    // アクセストークンにクレームを追加
    //
    public class DefaultClientClaimsAdder : ICustomTokenRequestValidator
    {
        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            // こうするとIDトークンにnameやfamily_nameなど追加の属性がIDトークンに設定される
            // (が、IDトークンが肥大化するのでやめたほうが良い。必要に応じてUserInfoから取得するのがよい)
            context.Result.ValidatedRequest.Client.AlwaysIncludeUserClaimsInIdToken = true;

            // こうやってもIDトークンには値はなぜか設定されない・・・
            context.Result.ValidatedRequest.Subject.Claims.Append(new Claim("test1", "test1-body"));

            // 以下のコードでアクセストークンにクレームを追加できる(IDトークンではない)
            context.Result.ValidatedRequest.Client.AlwaysSendClientClaims = true;
            context.Result.ValidatedRequest.ClientClaims.Add(new Claim("testtoken", "testbody"));
            return Task.FromResult(0);
        }
    }

    //
    // Identity情報としてカスタムのクレームを追加するときに本サービスを使用する。
    //
    // `GetProfileDataAsync`メソッドで必要なクレームを追加する。本メソッドは以下のケースで呼ばれる
    //   * アクセストークンの取得
    //   * IDトークンの取得
    //   * UserInfoエンドポイントが呼ばれたとき
    //
    // どのケースで呼ばれたかは`ProfileDataRequestContext`のCallerで判定可能。
    //
    // `services.AddTransient<IProfileService, MyProfileService>();`でサービス登録する。
    // 重要な注：
    //  ただし、これをするとgiven_nameなどのprofile情報が取れなくなる。
    //  おそらく`GetProfileDataAsync`メソッドは（追加したいクレームだけでなく）クライアントに返す全クレームを
    //  設定する必要がある。
    //
    // 参考：
    // * https://stackoverflow.com/questions/43894146/identityserver4-add-custom-default-claim-to-client-principal-for-client-credent
    // * https://tnakamura.hatenablog.com/entry/2018/10/18/how-to-change-authentication-of-identityserver4
    //
    // 追加した情報は単純にIDトークンを取得しただけでは取れない。
    public class MyProfileService : IProfileService
    {
        public MyProfileService()
        { }

        //
        // カスタムのprofile情報を返すときに
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var claims = new List<Claim>()
            {
                new Claim("TestKey", "TestValue")
            };
            context.IssuedClaims.AddRange(claims);

            return Task.CompletedTask;
        }

        //
        // トークンが取得可能なユーザーのとき、`context.IsActive = true`を設定
        //
        public Task IsActiveAsync(IsActiveContext context)
        {
            // await base.IsActiveAsync(context);
            return Task.CompletedTask;
        }
    }

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

            // IdentityServerサービスをDIに登録、データストアをセットアップ
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            const string connectionString = @"User ID=idpadmin;Password=idpadmin0!;Data Source=localhost;Initial Catalog=master;Application Name=is4;Connection Lifetime=60;Pooling=true;Min Pool Size=10;Max Pool Size=100";

            var builder = services.AddIdentityServer()
                .AddTestUsers(TestUsers.Users)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                });

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

            // CustomTokenRequestValidatorサービスの追加
            services.AddTransient<ICustomTokenRequestValidator, DefaultClientClaimsAdder>();

            // IDトークンのカスタムクレームを取得するようにする
            // 重要な注：これを有効化するとprofile情報が取れなくなる
            // (書き方としては、builder.AddProfileService<MyProfileService>()でも良い)
            services.AddTransient<IProfileService, MyProfileService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            // Identityデータベースの構築は1回だけ実行すればOK
            // InitializeDatabase(app);

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

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.IdentityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.ApiScopes)
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
