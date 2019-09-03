namespace XService.Driver
{
    /// <summary>
    /// Provides environment exit codes for Driver level processes
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class DriverExitCodes
    {
        public static DriverExitCodes Success = new DriverExitCodes(0);

        public static DriverExitCodes DriverConfigurationFault = new DriverExitCodes(10);

        /// <summary>
        /// The exit code value
        /// </summary>
        /// <value></value>
        public int Value { get; }

        /// <summary>
        /// Private constructor to prevent external instantiation
        /// </summary>
        /// <param name="value"></param>
        private DriverExitCodes(int value) {
            Value = value;
        }

        /// <summary>
        /// Allow for implicit conversion to int
        /// </summary>
        /// <param name="code"></param>
        public static implicit operator int(DriverExitCodes code) {
            return code.Value;
        }
    }
}
