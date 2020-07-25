using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Plugins;
using Intersect.Client.Plugins.Interfaces;
using Intersect.Plugins;

using JetBrains.Annotations;

using System;
using System.Diagnostics;
using System.Threading;

namespace Intersect.Examples.ClientPlugin
{

    /// <summary>
    /// Demonstrates basic plugin functionality for the client.
    /// </summary>
    public class ExampleClientPluginEntry : ClientPluginEntry
    {

        [UsedImplicitly] private Mutex mMutex;

        private GameTexture mButtonTexture;

        /// <inheritdoc />
        public override void OnBootstrap(IPluginBootstrapContext context)
        {
            context.Logging.Application.Info(
                $@"{nameof(ExampleClientPluginEntry)}.{nameof(OnBootstrap)} writing to the application log!"
            );

            context.Logging.Plugin.Info(
                $@"{nameof(ExampleClientPluginEntry)}.{nameof(OnBootstrap)} writing to the plugin log!"
            );

            mMutex = new Mutex(true, "testplugin", out var createdNew);
            if (!createdNew)
            {
                Environment.Exit(-1);
            }

            var exampleCommandLineOptions = context.CommandLine.ParseArguments<ExampleCommandLineOptions>();
            if (!exampleCommandLineOptions.ExampleFlag)
            {
                context.Logging.Plugin.Warn("Client wasn't started with the start-up flag!");
            }

            context.Logging.Plugin.Info(
                $@"{nameof(exampleCommandLineOptions.ExampleVariable)} = {exampleCommandLineOptions.ExampleVariable}"
            );
        }

        /// <inheritdoc />
        public override void OnStart(IClientPluginContext context)
        {
            context.Logging.Application.Info(
                $@"{nameof(ExampleClientPluginEntry)}.{nameof(OnStart)} writing to the application log!"
            );

            context.Logging.Plugin.Info(
                $@"{nameof(ExampleClientPluginEntry)}.{nameof(OnStart)} writing to the plugin log!"
            );

            mButtonTexture = context.ContentManager.LoadEmbedded<GameTexture>(
                context, ContentTypes.Interface, "Assets/join-our-discord.png"
            );

            context.Lifecycle.LifecycleChangeState += HandleLifecycleChangeState;
        }

        /// <inheritdoc />
        public override void OnStop(IClientPluginContext context)
        {
            context.Logging.Application.Info(
                $@"{nameof(ExampleClientPluginEntry)}.{nameof(OnStop)} writing to the application log!"
            );

            context.Logging.Plugin.Info(
                $@"{nameof(ExampleClientPluginEntry)}.{nameof(OnStop)} writing to the plugin log!"
            );
        }

        private void HandleLifecycleChangeState(
            [NotNull] IClientPluginContext context,
            [NotNull] LifecycleChangeStateArgs lifecycleChangeStateArgs
        )
        {
            Debug.Assert(mButtonTexture != null, nameof(mButtonTexture) + " != null");

            var activeInterface = context.Lifecycle.Interface;
            if (activeInterface == null)
            {
                return;
            }

            var button = activeInterface.Create<Button>("DiscordButton");
            Debug.Assert(button != null, nameof(button) + " != null");

            var discordInviteUrl = context.GetTypedConfiguration<ExamplePluginConfiguration>()?.DiscordInviteUrl;
            button.Clicked += (sender, args) =>
            {
                if (string.IsNullOrWhiteSpace(discordInviteUrl))
                {
                    context.Logging.Plugin.Error($@"DiscordInviteUrl configuration property is null/empty/whitespace.");
                    return;
                }

                Process.Start(discordInviteUrl);
            };

            button.SetImage(mButtonTexture, mButtonTexture.Name, Button.ControlState.Normal);
            button.SetSize(mButtonTexture.GetWidth(), mButtonTexture.GetHeight());
            button.CurAlignments?.Add(Alignments.Bottom);
            button.CurAlignments?.Add(Alignments.Right);
            button.ProcessAlignments();
        }

    }

}
