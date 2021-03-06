﻿using ArkBot.ViewModel;
using Autofac;
using Autofac.Core;
using Autofac.Integration.SignalR;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.IO;

namespace ArkBot.WebApi
{
    public class WebApiStartup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.DependencyResolver = new AutofacWebApiDependencyResolver(Workspace.Container);
            config.Formatters.Add(new BrowserJsonFormatter());

            var hubConfig = new HubConfiguration { EnableDetailedErrors = true };
            hubConfig.Resolver = Workspace.Container.Resolve<IDependencyResolver>();

            appBuilder.UseAutofacMiddleware(Workspace.Container);
            appBuilder.UseAutofacWebApi(config);
            appBuilder.UseCompressionModule();
            appBuilder.UseCors(CorsOptions.AllowAll);
            appBuilder.UseWebApi(config);
            appBuilder.MapSignalR(hubConfig);
            //appBuilder.UseFileServer(new FileServerOptions
            //{
            //    FileSystem = new PhysicalFileSystem(@"WebApi\Static\"),
            //    RequestPath = new PathString("/app"),
            //});
        }
    }

    public class BrowserJsonFormatter : JsonMediaTypeFormatter
    {
        public BrowserJsonFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            //SerializerSettings.Formatting = Formatting.Indented;
        }

        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);
            headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
    }
}
