using System.Linq;
using System.Reflection;

using Autofac;
using Autofac.Core;

using ShaneYu.HotCommander.Logging;

using Module = Autofac.Module;

namespace ShaneYu.HotCommander.UI.WPF.Logging
{
    /// <summary>
    /// Logger Injection Module
    /// </summary>
    public class LoggerInjectionModule : Module
    {
        #region Overrides

        /// <summary>
        /// Override to attach module-specific functionality to a component registration.
        /// </summary>
        /// <remarks>
        /// This method will be called for all existing <i>and future</i> component registrations - ordering is not important.
        /// </remarks>
        /// <param name="registry">The component registry.</param>
        /// <param name="registration">The registration to attach functionality to.</param>
        protected override void AttachToComponentRegistration(IComponentRegistry registry, IComponentRegistration registration)
        {
            registration.Preparing += OnComponentPreparing;
            registration.Activated += OnComponentActivated;
        }

        #endregion

        #region Event Handlers

        private static void OnComponentPreparing(object sender, PreparingEventArgs e)
        {
            var typePreparing = e.Component.Activator.LimitType;

            // By default, the name supplied to the logging instance is the name of the type in which it is being injected into.
            var loggerName = typePreparing.FullName;

            //If there is a class-level logger attribute, then promote its supplied name value instead as the logger name to use.
            var loggerAttribute = (LoggerAttribute)typePreparing.GetCustomAttributes(typeof(LoggerAttribute), true).FirstOrDefault();
            if (loggerAttribute != null)
            {
                loggerName = loggerAttribute.Name;
            }

            e.Parameters = e.Parameters.Union(new Parameter[]
            {
                new ResolvedParameter(
                    (p, i) => p.ParameterType == typeof (ILogger),
                    (p, i) =>
                    {
                        // If the parameter being injected has its own logger attribute, then promote its name value instead as the logger name to use.
                        loggerAttribute = (LoggerAttribute) p.GetCustomAttributes(typeof (LoggerAttribute), true).FirstOrDefault();
                        if (loggerAttribute != null)
                        {
                            loggerName = loggerAttribute.Name;
                        }

                        // Return a new Logger instance for injection, parameterised with the most appropriate name which we have determined above.
                        return new NLogLogger(loggerName);
                    }),

                // Always make an unamed instance of Logger available for use in delegate-based registration e.g.: Register((c,p) => new Foo(p.TypedAs<Logger>())
                new TypedParameter(typeof (ILogger), new NLogLogger(loggerName))
            });
        }

        private static void OnComponentActivated(object sender, ActivatedEventArgs<object> e)
        {
            var instanceType = e.Instance.GetType();
            var loggerName = instanceType.FullName;

            //If there is a class-level logger attribute, then promote its supplied name value instead as the logger name to use.
            var loggerAttribute = (LoggerAttribute)instanceType.GetCustomAttributes(typeof(LoggerAttribute), true).FirstOrDefault();
            if (loggerAttribute != null)
            {
                loggerName = loggerAttribute.Name;
            }

            var properties = instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(ILogger) && p.CanWrite && p.GetIndexParameters().Length == 0);

            // Set the properties located.
            foreach (var propToSet in properties)
            {
                propToSet.SetValue(e.Instance, new NLogLogger(loggerName), null);
            }
        }

        #endregion
    }
}
