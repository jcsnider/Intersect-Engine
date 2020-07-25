using CommandLine;

using Intersect.Factories;
using Intersect.Plugins.Helpers;
using Intersect.Plugins.Interfaces;

using JetBrains.Annotations;

using System;
using System.Reflection;

namespace Intersect.Plugins.Contexts
{

    public sealed class PluginBootstrapContext : IPluginBootstrapContext
    {

        public sealed class Factory : IFactory<IPluginBootstrapContext>
        {

            [NotNull] private readonly string[] mArgs;

            [NotNull] private readonly Parser mParser;

            public Factory([NotNull] string[] args, [NotNull] Parser parser)
            {
                mArgs = args;
                mParser = parser;
            }

            /// <inheritdoc />
            public IPluginBootstrapContext Create(params object[] args)
            {
                if (args.Length < 1)
                {
                    throw new ArgumentException($@"Need to provide an instance of {nameof(IManifestHelper)}.");
                }

                if (!(args[0] is Plugin plugin))
                {
                    throw new ArgumentException($@"First argument needs to be non-null and of type {nameof(Plugin)}.");
                }

                return new PluginBootstrapContext(mArgs, mParser, plugin);
            }

        }

        [NotNull] private Plugin Plugin { get; }

        /// <inheritdoc />
        public ICommandLineHelper CommandLine { get; }

        /// <inheritdoc />
        public Assembly Assembly => Plugin.Reference.Assembly;

        /// <inheritdoc />
        public PluginConfiguration Configuration => Plugin.Configuration;

        /// <inheritdoc />
        public IEmbeddedResourceHelper EmbeddedResources { get; }

        /// <inheritdoc />
        public ILoggingHelper Logging => Plugin.Logging;

        /// <inheritdoc />
        public IManifestHelper Manifest => Plugin.Manifest;

        private PluginBootstrapContext(
            [NotNull] string[] args,
            [NotNull] Parser parser,
            [NotNull] Plugin plugin
        )
        {
            Plugin = plugin;

            CommandLine = new CommandLineHelper(plugin.Logging.Plugin, args, parser);
            EmbeddedResources = new EmbeddedResourceHelper(Assembly);
        }

        /// <inheritdoc />
        public TConfiguration GetTypedConfiguration<TConfiguration>()
            where TConfiguration : PluginConfiguration => Configuration as TConfiguration;

    }

}
