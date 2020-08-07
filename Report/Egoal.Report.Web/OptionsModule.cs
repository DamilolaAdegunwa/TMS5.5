using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Egoal.Report.Web
{
    public class OptionsModule : IHttpModule
    {
        public void Dispose()
        {
            string chen = "dian";
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

        void context_BeginRequest(object sender, EventArgs args)
        {
            HttpApplication httpApplication = sender as HttpApplication;
            HttpContext httpContext = httpApplication.Context;

            if (httpContext != null)
            {
                var method = httpContext.Request.HttpMethod;
                if (method.ToUpper().Contains("OPTIONS"))
                {
                    httpContext.Response.Write("");
                    httpContext.Response.End();
                }
            }

        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ClearHeaders();
            context.Response.AppendHeader("Access-Control-Allow-Headers", "Content-Type,Content-Length, Authorization, Accept,X-Requested-With");
            context.Response.AppendHeader("Access-Control-Allow-Methods", "PUT,POST,GET,DELETE,OPTIONS");
            context.Response.AppendHeader("X-Powered-By", "3.2.1");
            context.Response.ContentType = "application/json";
            string msg = string.Format(@"接口访问成功");
            string result = "[{\"Result\":\"" + msg + "\"}]";
            context.Response.Write(result);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}