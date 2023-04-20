#if !Mini
namespace MSCLoader
{
    /// <summary>
    /// Base class for console commands
    /// </summary>
    public abstract class ConsoleCommand
	{
        internal static ConsoleController cc;

		/// <summary>
		/// The name of the ConsoleCommand (What the user will have to type in console to trigger the command). Cannot contain spaces!
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// The help message that will be displayed for the command when the user types "help"
		/// </summary>
		public abstract string Help { get; }

        /// <summary>
        /// Show this command in help screen.
        /// (Default true).
        /// </summary>
        public virtual bool ShowInHelp => true;

        /// <summary>
        /// Alternate command name that does the same thing. Cannot contain spaces!
        /// </summary>
        public virtual string Alias => null;

        /// <summary>
        /// The function that will get called when the command is ran.
        /// </summary>
        /// <param name="args">The arguments the user passed after the command.</param>
        public abstract void Run(string[] args);


        /// <summary>
        /// Adds a console command.
        /// </summary>
        /// <param name="cmd">The instance of the command to add.</param>
        public static void Add(ConsoleCommand cmd)
		{
            if(string.IsNullOrEmpty(cmd.Alias))
                cc.RegisterCommand(cmd.Name.ToLower(), cmd.Run, cmd.Help, cmd.ShowInHelp);
            else
                cc.RegisterCommand(cmd.Name.ToLower(), cmd.Run, cmd.Help, cmd.Alias.ToLower(), cmd.ShowInHelp);

        }
    }
}
#endif