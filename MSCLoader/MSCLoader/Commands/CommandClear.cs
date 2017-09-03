namespace MSCLoader.Commands
{
#pragma warning disable CS1591
    public class CommandClear : ConsoleCommand
	{
        public override string Name => "clear";
        public override string Help => "Clears console screen";

        public override void Run(string[] args)
		{
			ModConsole.Clear();
		}
	}
#pragma warning restore CS1591

}
