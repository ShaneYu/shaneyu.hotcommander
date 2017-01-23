namespace ShaneYu.HotCommander.Modules
{
    public interface IHotCommandModule
    {
        void RegisterInternalCommands(IHotCommandManager commandManager);
    }
}