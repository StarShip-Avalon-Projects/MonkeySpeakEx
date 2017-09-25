using System;
using System.IO;
using System.Reflection;

namespace Monkeyspeak
{
    /// <summary>
    /// Monkey Speak Engine options
    /// </summary>
    public class Options
    {
        #region Public Constructors

        /// <summary>
        /// default options preloaded
        /// </summary>
        public Options()
        {
            CanOverrideTriggerHandlers = false;
            StringBeginSymbol = '{';
            StringEndSymbol = '}';
            VariableDeclarationSymbol = '%';
            CommentSymbol = "*";
            TriggerLimit = 6000;
            VariableCountLimit = 1000;
            StringLengthLimit = Int32.MaxValue;
            TimerLimit = 100;
            Version = Assembly.GetExecutingAssembly().GetName().Version;
            DefaultExecutePath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location).ToString();
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// If set to TRUE then an existing TriggerHandler can be overridden by newer TriggerHandler
        /// <para>
        /// Default: false
        /// </para>
        /// </summary>
        public bool CanOverrideTriggerHandlers { get; set; }

        /// <summary>
        /// Comment literal symbol
        /// <para>
        /// Default: *
        /// </para>
        /// </summary>
        public string CommentSymbol { get; set; }

        /// <summary>
        /// Default File path for executing assemblies such as
        /// <see cref="Monkeyspeak.Libraries.IO"/>
        /// </summary>
        public string DefaultExecutePath { get; set; }

        /// <summary>
        /// Beginning string literal symbol
        /// <para>
        /// Default: {
        /// </para>
        /// </summary>
        public char StringBeginSymbol { get; set; }

        /// <summary>
        /// Ending string literal symbol
        /// <para>
        /// Default: }
        /// </para>
        /// </summary>
        public char StringEndSymbol { get; set; }

        /// <summary>
        /// Used to limit the maximum length of a string
        /// <para>
        /// Default: Int32.MaxValue
        /// </para>
        /// </summary>
        public int StringLengthLimit { get; set; }

        /// <summary>
        /// Used to limit the maximum amount of timers per execution
        /// <para>
        /// Default: 100
        /// </para>
        /// </summary>
        public int TimerLimit { get; set; }

        /// <summary>
        /// Used to limit the maximum amount of triggers per page
        /// <para>
        /// Default: 6000
        /// </para>
        /// </summary>
        public int TriggerLimit { get; set; }

        /// <summary>
        /// Used to limit the maximum amount of variables per page
        /// <para>
        /// Default: 1000
        /// </para>
        /// </summary>
        public int VariableCountLimit { get; set; }

        /// <summary>
        /// Variable literal symbol
        /// <para>
        /// Default: %
        /// </para>
        /// </summary>
        public char VariableDeclarationSymbol { get; set; }

        /// <summary>
        /// Current assembly version used internally by the compiler to handle versioning
        /// </summary>
        public Version Version { get; internal set; }

        #endregion Public Properties
    }
}