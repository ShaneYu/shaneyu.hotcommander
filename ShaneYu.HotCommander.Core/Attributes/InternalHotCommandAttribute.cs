using System;

namespace ShaneYu.HotCommander.Attributes
{
    /// <summary>
    /// Internal Command. 
    /// Makes a command an internal one, meaning that it cannot be customized. (Usually a "core" command class).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class InternalHotCommandAttribute : Attribute
    {
    }
}
