using AutoMapper;
using AutoMapper.EquivalencyExpression;
using HGV.Warhorn.Api;
using HGV.Warhorn.Api.Data;
using HGV.Warhorn.Api.Profiles;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Startup))]


namespace HGV.Warhorn.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddDbContext<PuzzleContext>();
            builder.Services.AddAutoMapper(Startup.ConfigAutoMapper, typeof(PuzzleContext).Assembly);
        }

        public static void ConfigAutoMapper(IServiceProvider sp, IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<PuzzleProfile>();
            cfg.AddCollectionMappers();
            cfg.UseEntityFrameworkCoreModel<PuzzleContext>(sp);
        }
    }

    
}
