using System;

using NLog;

using ILogger = ShaneYu.HotCommander.Logging.ILogger;

namespace ShaneYu.HotCommander.UI.WPF.Logging
{
    /// <summary>
    /// NLog implementation of <see cref="ILogger"/>
    /// </summary>
    public class NLogLogger : ILogger
    {
        #region Fields

        private readonly Logger _log;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Logger Name</param>
        public NLogLogger(string name)
        {
            _log = LogManager.GetLogger(name);
        }

        #endregion

        #region Public Methods

        public void Debug(string format, params object[] args)
        {
            Log(LogLevel.Debug, format, args);
        }

        public void Info(string format, params object[] args)
        {
            Log(LogLevel.Info, format, args);
        }

        public void Warn(string format, params object[] args)
        {
            Log(LogLevel.Warn, format, args);
        }

        public void Warn(Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Warn, format, args, ex);
        }

        public void Error(string format, params object[] args)
        {
            Log(LogLevel.Error, format, args);
        }

        public void Error(Exception ex)
        {
            Log(LogLevel.Error, null, null, ex);
        }

        public void Error(Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Error, format, args, ex);
        }

        public void Fatal(Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Fatal, format, args, ex);
        }

        private void Log(LogLevel level, string format, object[] args)
        {
            _log.Log(typeof(NLogLogger), new LogEventInfo(level, _log.Name, null, format, args));
        }

        private void Log(LogLevel level, string format, object[] args, Exception ex)
        {
            _log.Log(typeof(NLogLogger), new LogEventInfo(level, _log.Name, null, format, args, ex));
        }

        #endregion
    }
}
