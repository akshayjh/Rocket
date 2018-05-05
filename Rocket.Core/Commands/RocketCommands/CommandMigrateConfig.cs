﻿using System;
using System.IO;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.Core.Configuration;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandMigrateConfig : ICommand
    {
        public string Name => "MigrateConfig";
        public string[] Aliases => null;
        public string Description => "Migrates configs from one format to another.";
        public string Permission => "Rocket.MigrateConfig";
        public string Syntax => "[<from> <to> <path>]";
        public ISubCommand[] ChildCommands { get; }
        public bool SupportsCaller(Type commandCaller)
        {
            return typeof(IConsoleCommandCaller).IsAssignableFrom(commandCaller);
        }

        public void Execute(ICommandContext context)
        {
            if (context.Parameters.Length != 0 && context.Parameters.Length != 3)
            {
                throw new CommandWrongUsageException();
            }

            var configProviders = context.Container.ResolveAll<IConfiguration>().ToArray();

            if (context.Parameters.Length == 0)
            {
                context.Caller.SendMessage(GetConfigTypes(configProviders));
                context.SendUsage();
                return;
            }

            var from = context.Parameters.Get<string>(0);
            var to = context.Parameters.Get<string>(1);
            var path = context.Parameters.GetArgumentLine(2);

            if (from.Equals(to, StringComparison.OrdinalIgnoreCase))
            {
                context.Caller.SendMessage("\"from\" and \"to\" can not be the same config type!");
                return;
            }

            IConfiguration fromProvider =
                configProviders.FirstOrDefault(c => c.Name.Equals(from, StringComparison.OrdinalIgnoreCase));

            if (fromProvider == null)
            {
                throw new CommandWrongUsageException($"\"{from}\" is not a valid config type. " + GetConfigTypes(configProviders));
            }

            IConfiguration toProvider =
                configProviders.FirstOrDefault(c => c.Name.Equals(to, StringComparison.OrdinalIgnoreCase));


            if (toProvider == null)
            {
                throw new CommandWrongUsageException($"\"{to}\" is not a valid config type. " + GetConfigTypes(configProviders));
            }

            var workingDir = Path.GetDirectoryName(path);
            var fileName = Path.GetFileName(path);

            ConfigurationContext cc = new ConfigurationContext(workingDir, fileName);

            fromProvider.Load(cc);
            toProvider.ConfigurationContext = cc;

            toProvider.LoadEmpty();

            IConfigurationElement currentFromNode = fromProvider;
            IConfigurationElement currentToNode = fromProvider;

            while (true)
            {
                bool done = false;

                foreach (var fromChild in currentFromNode)
                {
                    var toChild = currentToNode.CreateSection(fromChild.Key, fromChild.Type);
                    currentToNode[fromChild.Key].Set(fromChild.Get());

                    if (currentFromNode.Type == SectionType.Object && !currentFromNode.GetChildren().Any())
                    {
                        done = true;
                        break;
                    }

                    currentFromNode = fromChild;
                    currentToNode = toChild;
                }

                if (done)
                    break;
            }

            context.Caller.SendMessage("Configuration was successfully migrated.");
        }

        private string GetConfigTypes(IConfiguration[] configProviders)
        {
            return "Available config types: " + string.Join(", ", configProviders.Select(c => c.Name).ToArray());
        }
    }
}