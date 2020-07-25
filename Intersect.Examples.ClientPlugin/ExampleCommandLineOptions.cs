using CommandLine;

using JetBrains.Annotations;

namespace Intersect.Examples.ClientPlugin
{

    /// <summary>
    /// Example immutable command line options structure.
    /// </summary>
    public struct ExampleCommandLineOptions
    {

        [UsedImplicitly]
        public ExampleCommandLineOptions(bool exampleFlag, int exampleVariable)
        {
            ExampleFlag = exampleFlag;
            ExampleVariable = exampleVariable;
        }

        /// <summary>
        /// Flag that is true if the application was started with --example-flag
        /// </summary>
        [Option("example-flag", Default = false, Required = false)]
        public bool ExampleFlag { get; }

        /// <summary>
        /// Integer argument that corresponds to --example-variable
        /// </summary>
        [Option("example-variable", Default = 100, Required = false)]
        public int ExampleVariable { get; }

    }

}
