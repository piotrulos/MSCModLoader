namespace MSCLoader.Commands
{
#pragma warning disable CS1591

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
#pragma warning restore CS1591

}
