using System.Collections.Generic;

namespace MSCLoader
{
    /// <summary>
    /// Allows for Mods to easily add rebindable console commands.
    /// </summary>
    public abstract class ConsoleCommand
	{
        public static ConsoleController cc = ModConsole.console.console;
        public static List<ConsoleCommand> Commands = new List<ConsoleCommand>();

		/// <summary>
		/// The name of the ConsoleCommand (What the user will have to type in console to trigger the command) [Cannot contain spaces].
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// The help message that will be displayed for the command when the user types "help"
		/// </summary>
		public abstract string Help { get; }

        /// <summary>
        /// The function that will get called when the command is ran.
        /// </summary>
        /// <param name="args">The arguments the user passed to the command.</param>
        public abstract void Run(string[] args);


		/// <summary>
		/// Adds a console command.
		/// </summary>
		/// <param name="cmd">The instance of the command to add.</param>
		public static void Add(ConsoleCommand cmd)
		{
            cc.registerCommand(cmd.Name, cmd.Run, cmd.Help);
        }
	}
}
