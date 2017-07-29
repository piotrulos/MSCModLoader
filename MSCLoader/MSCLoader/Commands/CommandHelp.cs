namespace MSCLoader.Commands
{
    public class CommandHelp : ConsoleCommand
	{
        public override string Name => "help";
        public override string Help => "This screen";

        public override void Run(string[] args)
		{
			ModConsole.Print("<color=green><b>Available commands:</b></color>");
            cc.help(args);
		}
	}
}
