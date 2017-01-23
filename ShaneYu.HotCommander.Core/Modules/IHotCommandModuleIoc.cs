using Autofac;

namespace ShaneYu.HotCommander.Modules
{
    public interface IHotCommandModuleIoc
    {
        void RegisterTypes(ContainerBuilder builder);
    }
}