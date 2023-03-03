using System;
using Functions101.Models.Toons;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Snoopy.Function.StartUp))]
namespace Snoopy.Function
{
    public class StartUp : FunctionsStartup {
        public override void Configure(IFunctionsHostBuilder builder) {
            string connStr = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");

            builder.Services.AddDbContext<SchoolContext>(
              options => SqlServerDbContextOptionsExtensions.UseSqlServer(options, connStr));

        }
    }
}
