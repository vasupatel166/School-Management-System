using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using DotNetEnv;

namespace Schoolnest
{
    public class Global : System.Web.HttpApplication
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["schoolnestConnectionString"].ConnectionString;

        //public static string ConnectionString;
        protected void Application_Start(object sender, EventArgs e)
        {
            // Load the .env file
            //Env.Load();

            // Retrieve the connection string from the environment variables
            //ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");



        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}