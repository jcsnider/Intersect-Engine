using Intersect.Plugins.Interfaces;
using Intersect.Plugins.Manifests.Types;

using Semver;

namespace Intersect.Examples.ClientPlugin
{
    /// <summary>
    /// Defines a plugin manifest in code rather than an embedded manifest.json file.
    /// </summary>
    public struct Manifest : IManifestHelper
    {
        // ReSharper disable once AssignNullToNotNullAttribute This will not be null.
        /// <inheritdoc />
        public string Name => typeof(Manifest).Namespace;

        // ReSharper disable once AssignNullToNotNullAttribute This will not be null.
        /// <inheritdoc />
        public string Key => typeof(Manifest).Namespace;

        /// <inheritdoc />
        public SemVersion Version => new SemVersion(1);

        /// <inheritdoc />
        public Authors Authors => "Ascension Game Dev <admin@ascensiongamedev.com> (https://github.com/AscensionGameDev/Intersect-Engine)";

        /// <inheritdoc />
        public string Homepage => "https://github.com/AscensionGameDev/Intersect-Engine";
    }
}
