namespace MSCLoader
{
    /// <summary>
    /// Allows for Mods to easily add console commands.
    /// </summary>
    /// <example><code source="Examples.cs" region="ConsoleCommand" lang="C#" 
    /// title="Example ConsoleCommand Class" /></example>
    public abstract class ConsoleCommand
	{
        /// <summary>
        /// ConsoleController Instance
        /// </summary>
        public static ConsoleController cc;

		/// <summary>
		/// The name of the ConsoleCommand (What the user will have to type in console to trigger the command). Cannot contain spaces!
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// The help message that will be displayed for the command when the user types "help"
		/// </summary>
		public abstract string Help { get; }

        /// <summary>
        /// The function that will get called when the command is ran.
        /// </summary>
        /// <param name="args">The arguments the user passed after the command.</param>
        /// <example><code source="Examples.cs" region="ConsoleCommandRun" lang="C#" 
        /// title="ConsoleCommand Add" /></example>
        public abstract void Run(string[] args);


        /// <summary>
        /// Adds a console command.
        /// </summary>
        /// <param name="cmd">The instance of the command to add.</param>
        /// <example><code source="Examples.cs" region="ConsoleCommandAdd" lang="C#" 
        /// title="ConsoleCommand Add" /></example>
        public static void Add(ConsoleCommand cmd)
		{
            cc.registerCommand(cmd.Name, cmd.Run, cmd.Help);
        }
	}
}
