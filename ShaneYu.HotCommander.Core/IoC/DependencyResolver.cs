using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Autofac;

using ShaneYu.HotCommander.Modules;

namespace ShaneYu.HotCommander.IoC
{
    public static class DependencyResolver
    {
        public static bool IsInitialized => Current != null;

        public static IContainer Current { get; private set; }

        public static void Initialize(Action<ContainerBuilder> registerTypes = null, Action<IHotCommandManager> registerCommands = null)
        {
            if (Current != null)
                throw new InvalidOperationException("You cannot initialize the dependency resolve more that once, please check the 'IsInitialized' before calling 'Initialize'.");

            var builder = new ContainerBuilder();
            
            registerTypes?.Invoke(builder);
            LoadCommandModules(builder);

            Current = builder.Build();

            var commandManager = Current.Resolve<IHotCommandManager>();
            registerCommands?.Invoke(commandManager);
            RegisterCommadModuleCommands(commandManager);
        }

        private static void LoadCommandModules(ContainerBuilder builder)
        {
            var binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!string.IsNullOrWhiteSpace(binPath))
            {
                foreach (
                    var assemblyFilePath in
                        Directory.GetFiles(binPath, @"HotCommander.Modules.*.dll", SearchOption.TopDirectoryOnly))
                {
                    var assembly = Assembly.LoadFile(assemblyFilePath);
                    RegisterModuleTypes(builder, assembly);
                }
            }
        }

        private static void RegisterModuleTypes(ContainerBuilder builder, Assembly assembly)
        {
            var moduleRegistras = from t in assembly.GetTypes()
                where !t.IsInterface && typeof(IHotCommandModuleIoc).IsAssignableFrom(t)
                let i = (IHotCommandModuleIoc) Activator.CreateInstance(t)
                select i;

            foreach (var moduleRegistra in moduleRegistras)
            {
                moduleRegistra?.RegisterTypes(builder);
            }
        }

        private static void RegisterCommadModuleCommands(IHotCommandManager commandManager)
        {
            var moduleTypes = from a in AppDomain.CurrentDomain.GetAssemblies()
                              from t in a.GetTypes()
                              where !t.IsInterface && typeof(IHotCommandModule).IsAssignableFrom(t)
                              let i = (IHotCommandModule)Activator.CreateInstance(t)
                              where i != null
                              select i;

            foreach (var moduleType in moduleTypes)
            {
                moduleType.RegisterInternalCommands(commandManager);
            }
        }
    }
}
