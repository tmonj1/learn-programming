using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // MVCサービスを登録
            services.AddControllers();

            // 認証サービスをDIに登録し、Bearerをデフォルトスキームとして設定
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    // IdP (OP)
                    options.Authority = "http://localhost:5000";

                    // Metadataアドレスにhttpsは不要の設定
                    options.RequireHttpsMetadata = false;   // dev only

                    options.Audience = "api1";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Audientをチェックしない
                        ValidateAudience = false
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            // 認証ミドルウェアをパイプラインに注入、全APIコールで認証が実行される
            app.UseAuthentication();

            // 認可ミドルウェアをパイプラインに注入
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
