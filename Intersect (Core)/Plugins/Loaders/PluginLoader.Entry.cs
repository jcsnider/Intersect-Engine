using Intersect.Logging;
using Intersect.Reflection;

using JetBrains.Annotations;

using System;
using System.Linq;
using System.Reflection;

namespace Intersect.Plugins.Loaders
{
    internal sealed partial class PluginLoader
    {
        private static bool IsPluginEntryType([NotNull] Type type)
        {
            // Abstract, interface and generic types are not valid virtual manifest types.
            if (type.IsAbstract || type.IsInterface || type.IsGenericType)
            {
                return false;
            }

            if (!typeof(IPluginEntry).IsAssignableFrom(type))
            {
                return false;
            }

            var constructor = type.GetConstructor(Array.Empty<Type>());
            if (constructor != null)
            {
                return true;
            }

            Log.Debug($"'{type.Name}' is missing a default constructor.");
            return false;
        }

        private static PluginReference CreatePluginReference([NotNull] Assembly assembly)
        {
            try
            {
                var assemblyTypes = assembly.GetTypes();
                var entryType = assemblyTypes.FirstOrDefault(IsPluginEntryType);

                if (entryType == null)
                {
                    throw new ArgumentNullException(
                        nameof(entryType), 
                        $@"Unable to find entry type in {assembly.FullName}."
                    );
                }

                var entryTypeConstructor = entryType.GetConstructor(Array.Empty<Type>());
                if (entryTypeConstructor == null)
                {
                    throw new InvalidOperationException(
                        $@"Entry type {entryType.FullName} does not have a generic constructor."
                    );
                }

                var configurationBaseType = typeof(PluginConfiguration);
                var configurationType = assembly.FindDefinedSubtypesOf(configurationBaseType).FirstOrDefault() ?? configurationBaseType;

                return new PluginReference(assembly, configurationType, entryType);
            }
            catch (Exception exception)
            {
                Log.Warn(exception);
                return null;
            }
        }
    }
}
