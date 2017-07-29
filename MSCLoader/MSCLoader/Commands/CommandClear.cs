namespace MSCLoader.Commands
{
    public class CommandClear : ConsoleCommand
	{
        public override string Name => "clear";
        public override string Help => "Clears console screen";

        public override void Run(string[] args)
		{
			ModConsole.Clear();
		}
	}
}
