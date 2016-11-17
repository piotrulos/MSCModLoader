using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSCLoader.Commands
{
	public class CommandHelp : ConsoleCommand
	{
		public override string Name { get { return "help"; } }
		public override string Help { get { return "Offers help about commands"; } }

		public override void Run(string[] args)
		{
			ModConsole.Print("Available commands");

			foreach (ConsoleCommand cmd in ConsoleCommand.Commands)
			{
				ModConsole.Print(cmd.Name + "    -    " + cmd.Help);
			}
		}
	}
}
