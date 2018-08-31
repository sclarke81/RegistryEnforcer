using System;
using System.ServiceProcess;

namespace RegistryEnforcer
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            if (Environment.UserInteractive)
            {
                var enforcer = new RegistryEnforcer();
                enforcer.TestStartupAndStop();
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new RegistryEnforcer()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}