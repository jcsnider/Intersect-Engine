using System.Reflection;

using Intersect.Plugins.Helpers;
using Intersect.Plugins.Interfaces;

using JetBrains.Annotations;

namespace Intersect.Plugins.Contexts
{

    public abstract class
        PluginContext<TPluginContext, TLifecycleHelper> : IPluginContext<TPluginContext, TLifecycleHelper>
        where TPluginContext : IPluginContext<TPluginContext> where TLifecycleHelper : ILifecycleHelper
    {

        [NotNull] private Plugin Plugin { get; }

        /// <inheritdoc />
        public Assembly Assembly => Plugin.Reference.Assembly;

        /// <inheritdoc />
        public PluginConfiguration Configuration => Plugin.Configuration;

        /// <inheritdoc />
        public IEmbeddedResourceHelper EmbeddedResources { get; }

        ILifecycleHelper IPluginContext.Lifecycle => Lifecycle;

        /// <inheritdoc cref="IPluginContext.Lifecycle" />
        public abstract TLifecycleHelper Lifecycle { get; }

        /// <inheritdoc />
        public ILoggingHelper Logging => Plugin.Logging;

        /// <inheritdoc />
        public IManifestHelper Manifest => Plugin.Manifest;

        protected PluginContext([NotNull] Plugin plugin)
        {
            Plugin = plugin;
            EmbeddedResources = new EmbeddedResourceHelper(Assembly);
        }

        /// <inheritdoc />
        public TConfiguration GetTypedConfiguration<TConfiguration>() where TConfiguration : PluginConfiguration =>
            Configuration as TConfiguration;

    }

}
