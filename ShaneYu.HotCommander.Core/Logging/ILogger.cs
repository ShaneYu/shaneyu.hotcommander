﻿using System;

using JetBrains.Annotations;

namespace ShaneYu.HotCommander.Logging
{
    public interface ILogger
    {
        [StringFormatMethod("format")]
        void Debug(string format, params object[] args);

        [StringFormatMethod("format")]
        void Info(string format, params object[] args);

        [StringFormatMethod("format")]
        void Warn(string format, params object[] args);

        [StringFormatMethod("format")]
        void Warn(Exception ex, string format, params object[] args);

        [StringFormatMethod("format")]
        void Error(string format, params object[] args);
        void Error(Exception ex);
        [StringFormatMethod("format")]
        void Error(Exception ex, string format, params object[] args);
        [StringFormatMethod("format")]
        void Fatal(Exception ex, string format, params object[] args);
    }

}
