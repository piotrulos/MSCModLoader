namespace MSCLoader.Commands
{
    public class CommandClear : ConsoleCommand
	{
		public override string Name { get { return "clear"; } }
		public override string Help { get { return "Clears console screen"; } }

		public override void Run(string[] args)
		{
			ModConsole.Clear();
		}
	}
}
