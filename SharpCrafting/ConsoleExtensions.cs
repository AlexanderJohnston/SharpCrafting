using System;
using System.Collections.Generic;
using System.Text;

using Serilog.Sinks.SystemConsole.Themes ;

namespace SharpCrafting
{
    public static class ConsoleExtensions
    {
        public static SystemConsoleTheme BlueConsole { get; } = new SystemConsoleTheme(
            new Dictionary<ConsoleThemeStyle, SystemConsoleThemeStyle>
        {
            [ConsoleThemeStyle.Text] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Blue },
            [ConsoleThemeStyle.SecondaryText] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.DarkGray },
            [ConsoleThemeStyle.TertiaryText] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.DarkGray },
            [ConsoleThemeStyle.Invalid] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.DarkYellow },
            [ConsoleThemeStyle.Null] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.DarkRed },
            [ConsoleThemeStyle.Name] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Green },
            [ConsoleThemeStyle.String] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.White },
            [ConsoleThemeStyle.Number] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.DarkMagenta },
            [ConsoleThemeStyle.Boolean] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Yellow },
            [ConsoleThemeStyle.Scalar] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.DarkBlue },
            [ConsoleThemeStyle.LevelVerbose] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.DarkGray, Background = ConsoleColor.Yellow },
            [ConsoleThemeStyle.LevelDebug] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.DarkBlue, Background = ConsoleColor.Green },
            [ConsoleThemeStyle.LevelInformation] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.DarkGreen, Background = ConsoleColor.White },
            [ConsoleThemeStyle.LevelWarning] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.DarkCyan, Background = ConsoleColor.Yellow },
            [ConsoleThemeStyle.LevelError] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.DarkMagenta, Background = ConsoleColor.White },
            [ConsoleThemeStyle.LevelFatal] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.DarkRed, Background = ConsoleColor.Gray }
        });
    }
}
