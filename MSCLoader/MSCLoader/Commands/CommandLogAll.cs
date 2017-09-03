//Class for adding console commands
namespace MSCLoader.Commands
{
#pragma warning disable CS1591
    public class CommandLogAll : ConsoleCommand
    {
        // What the player has to type into the console to execute your commnad
        public override string Name => "log-all";

        // The help that's displayed for your command when typing help
        public override string Help => "Log <b>ALL</b> mod errors (Warning! May spam console)";

        // The function that's called when executing command
        public override void Run(string[] args)
        {
            ModConsole.Print(string.Format("<color=orange>Log All errors is set to <b>{0}</b></color>", !ModLoader.LogAllErrors));
            ModLoader.LogAllErrors = !ModLoader.LogAllErrors;
        }
    }
#pragma warning restore CS1591
}
