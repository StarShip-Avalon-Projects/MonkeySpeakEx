namespace Monkeyspeak.Libraries
{
    /// <summary>
    /// Provides IDE Debug MonkeySpeak Lines
    /// </summary>
    internal class Debug : AbstractBaseLibrary
    {
        #region Public Constructors

        /// <summary>
        /// Initialize Cause and effect
        /// <para>
        /// <see cref="WhenBreakpointHit">(0:10000)</see> $amp; <see cref="CreateBreakPoint">(5:10000)</see>
        /// </para>
        /// </summary>
        public Debug()
        {
            //(0:10000) when a debug breakpoint is hit,
            Add(TriggerCategory.Cause, 10000, WhenBreakpointHit,
                "(0:10000) when a debug breakpoint is hit,");

            //(5:10000) create a debug breakpoint here,
            Add(TriggerCategory.Effect, 10000, CreateBreakPoint,
                "(5:10000) create a debug breakpoint here,");
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// (5:10000) create a debug breakpoint here,
        /// </summary>
        /// <param name="reader">
        /// <see cref="TriggerReader"/>
        /// </param>
        /// <returns>
        /// </returns>
        private bool CreateBreakPoint(TriggerReader reader)
        {
            if (System.Diagnostics.Debugger.Launch())
            {
                System.Diagnostics.Debugger.NotifyOfCrossThreadDependency();
                System.Diagnostics.Debugger.Break();
                reader.Page.Execute(10000);
            }
            else RaiseError("Debugger could not be attached.");

            return true;
        }

        /// <summary>
        /// (0:10000) when a debug breakpoint is hit
        /// </summary>
        /// <param name="reader">
        /// <see cref="TriggerReader"/>
        /// </param>
        /// <returns>
        /// </returns>
        private bool WhenBreakpointHit(TriggerReader reader)
        {
            return true;
        }

        #endregion Private Methods
    }
}