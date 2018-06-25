﻿using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Scheduler;
using Rocket.API.User;
using Rocket.Core.User;

namespace Rocket.Console.Properties
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterSingletonType<ITaskScheduler, SimpleTaskScheduler>();
            container.RegisterSingletonType<IHost, ConsoleHost>();
            container.RegisterSingletonType<IUserManager, StdConsoleUserManager>("host", "stdconsole");
        }
    }
}