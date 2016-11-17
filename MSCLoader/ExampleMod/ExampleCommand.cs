using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSCLoader;

namespace ExampleMod
{
	public class ExampleCommand : ConsoleCommand
	{
		// What the player has to type into the console to execute your commnad
		public override string Name { get { return "example_command"; } }

		// The help that's displayed for your command when typing help
		public override string Help { get { return "Just type the command"; } }

		// The function that's called when the command is ran
		public override void Run(string[] args)
		{
			ModConsole.Print(args);
		}
	}
}
