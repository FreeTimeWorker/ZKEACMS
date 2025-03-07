/* http://www.zkea.net/ 
 * Copyright (c) ZKEASOFT. All rights reserved. 
 * http://www.zkea.net/licenses */

using Easy.Cache;
using Easy.Encrypt;
using Easy.Extend;
using Easy.Logging;
using Easy.MetaData;
using Easy.Modules.DataDictionary;
using Easy.Modules.MutiLanguage;
using Easy.Modules.Role;
using Easy.Modules.User.Models;
using Easy.Modules.User.Service;
using Easy.Mvc.Authorize;
using Easy.Mvc.Plugin;
using Easy.Mvc.RazorPages;
using Easy.Mvc.StateProviders;
using Easy.Mvc.ValueProvider;
using Easy.Net;
using Easy.Notification;
using Easy.Notification.Queue;
using Easy.Options;
using Easy.RepositoryPattern;
using Easy.RuleEngine;
using Easy.RuleEngine.RuleProviders;
using Easy.RuleEngine.Scripting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Easy
{
    public static class Builder
    {
        public static void UseEasyFrameWork(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<RazorViewEngineOptions>, PluginRazorViewEngineOptionsSetup>());

            //services.Replace(ServiceDescriptor.Transient<IControllerActivator, Mvc.Controllers.ServiceBasedControllerActivator>());
            //services.TryAddEnumerable(ServiceDescriptor.Transient<IActionDescriptorProvider, ActionDescriptorProvider>());
            services.TryAddSingleton<IPluginLoader, Loader>();


            services.TryAddScoped<IAuthorizer, DefaultAuthorizer>();

            services.TryAddTransient<ICookie, Cookie>();
            services.TryAddTransient<IUserService, UserService>();
            services.TryAddTransient<IRoleService, RoleService>();
            services.TryAddTransient<IUserRoleRelationService, UserRoleRelationService>();
            services.TryAddTransient<IPermissionService, PermissionService>();
            services.TryAddTransient<IDataDictionaryService, DataDictionaryService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.TryAddTransient<IEncryptService, EncryptService>();
            services.AddSingleton<IOnModelCreating, EntityFrameWorkModelCreating>();

            services.AddTransient<IViewRenderService, ViewRenderService>();
            services.AddTransient<INotificationManager, NotificationManager>();
            services.AddTransient<INotificationProvider, EmailNotificationProvider>();
            services.AddTransient<INotificationProvider, RazorEmailNotificationProvider>();
            services.AddTransient<IEmailNotification, EmailNotificationProvider>();
            services.AddTransient<IEmailQueue>(provider => null);
            services.AddTransient<IPluginLoader, Loader>();
            services.AddTransient<IRuleManager, RuleManager>();
            services.AddTransient<IRuleProvider, CommonMethodsRuleProvider>();
            services.AddTransient<IRuleProvider, ValueOfRuleProvider>();
            services.AddTransient<IRuleProvider, DateRuleProvider>();
            services.AddTransient<IRuleProvider, MoneyRuleProvider>();
            services.AddTransient<IScriptExpressionEvaluator, ScriptExpressionEvaluator>();
            services.AddTransient<IWebClient, WebClient>().AddHttpClient();

            services.AddScoped<ILocalize, Localize>();

            services.AddMemoryCache();
            services.AddTransient(typeof(ICacheManager<>), typeof(CacheManager<>));
            services.AddSingleton<ISignals, Signals>();

            services.AddScoped<IAuthorizationHandler, RolePolicyRequirementHandler>();
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            services.ConfigureStateProvider<CurrentCustomerStateProvider>();
            services.ConfigureStateProvider<CurrentUserStateProvider>();
            services.ConfigureStateProvider<HostingEnvironmentStateProvider>();

            services.ConfigureMetaData<UserEntity, UserMetaData>();
            services.ConfigureMetaData<DataDictionaryEntity, DataDictionaryEntityMetaData>();
            services.ConfigureMetaData<LanguageEntity, LanguageEntityMetaData>();
            services.ConfigureMetaData<Permission, PermissionMetaData>();
            services.ConfigureMetaData<RoleEntity, RoleMetaData>();
            services.ConfigureMetaData<UserRoleRelation, UserRoleRelationMetaData>();
            services.ConfigureMetaData<SmtpSetting, SmtpSettingMetaData>();


            services.Configure<CDNOption>(configuration.GetSection("CDN"));
            services.Configure<CultureOption>(configuration.GetSection("Culture"));

            services.AddDataProtection();
            //Share persistkeys for distributed deployment
            //You can create a new implementation of 'IXmlRepository' to store the persistkeys for sharing, like Redis or other database
            //FileSystemXmlRepository:
            //https://github.com/dotnet/aspnetcore/blob/master/src/DataProtection/DataProtection/src/Repositories/FileSystemXmlRepository.cs
            //services.AddDataProtection().SetApplicationName("ZKEACMS").PersistKeysToFileSystem(new DirectoryInfo("PersistKeys"));
        }

        public static void ConfigureMetaData<TEntity, TMetaData>(this IServiceCollection service)
            where TMetaData : ViewMetaData<TEntity>
            where TEntity : class
        {
            service.AddSingleton<ViewMetaData<TEntity>, TMetaData>();
        }

        public static IEnumerable<IPluginStartup> LoadAvailablePlugins(this IServiceCollection services)
        {
            var pluginStartup = ActivatorUtilities.GetServiceOrCreateInstance<IPluginLoader>(services.BuildServiceProvider());
            return pluginStartup.LoadEnablePlugins(services);
        }

        public static IApplicationBuilder UsePluginStaticFile(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<PluginStaticFileMiddleware>();
            return builder;
        }
        public static void AddFileLog(this ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>());
        }
    }
}