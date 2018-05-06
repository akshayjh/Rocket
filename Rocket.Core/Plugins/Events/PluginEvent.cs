﻿using Rocket.API.Eventing;
using Rocket.API.Plugins;

namespace Rocket.Core.Plugins.Events
{
    public abstract class PluginEvent : Event
    {
        protected PluginEvent(IPluginManager pluginManager, IPlugin plugin, bool global = true) : base(global)
        {
            PluginManager = pluginManager;
            Plugin = plugin;
        }

        protected PluginEvent(IPluginManager pluginManager, IPlugin plugin,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(executionTarget, global)
        {
            PluginManager = pluginManager;
            Plugin = plugin;
        }

        protected PluginEvent(IPluginManager pluginManager, IPlugin plugin, string name = null,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(name, executionTarget, global)
        {
            Plugin = plugin;
        }

        protected PluginEvent(IPluginManager pluginManager, IPlugin plugin) : this(pluginManager, plugin, true) { }
        public IPluginManager PluginManager { get; }
        public IPlugin Plugin { get; }
    }
}