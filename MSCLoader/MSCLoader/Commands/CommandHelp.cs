namespace MSCLoader.Commands
{
    public class CommandHelp : ConsoleCommand
	{
		public override string Name { get { return "help"; } }
		public override string Help { get { return "This screen"; } }

		public override void Run(string[] args)
		{
			ModConsole.Print("<color=green><b>Available commands:</b></color>");
            cc.help(args);
		}
	}
}
