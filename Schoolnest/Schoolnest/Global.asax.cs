using System;
using System.IO;
using DotNetEnv;

namespace Schoolnest
{
    public class Global : System.Web.HttpApplication
    {
        public static string ConnectionString { get; private set; }

        protected void Application_Start(object sender, EventArgs e)
        {
            try
            {
                // Get the base directory of the application
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                // Build the path to the .env file dynamically
                string envFilePath = Path.Combine(baseDirectory, @"..\..\Schoolnest\Schoolnest\.env");

                // Resolve the full path to ensure it points to the correct location
                string fullEnvFilePath = Path.GetFullPath(envFilePath);

                // Load the .env file
                Env.Load(@fullEnvFilePath);

                // Retrieve the connection string from the environment variables
                ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

                if (string.IsNullOrEmpty(ConnectionString))
                {
                    throw new Exception("Connection string not found in environment variables.");
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading environment variables: {ex.Message}");
            }
        }

        protected void Session_Start(object sender, EventArgs e) { }

        protected void Application_BeginRequest(object sender, EventArgs e) { }

        protected void Application_AuthenticateRequest(object sender, EventArgs e) { }

        protected void Application_Error(object sender, EventArgs e) { }

        protected void Session_End(object sender, EventArgs e) { }

        protected void Application_End(object sender, EventArgs e) { }
    }
}
