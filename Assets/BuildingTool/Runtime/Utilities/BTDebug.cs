using UnityEngine;

namespace BuildingTool.Runtime.Utilities
{
    /// <summary>
    /// Provides centralized logging for the BuildingTool editor system.
    /// Automatically formats logs with colored tags for improved readability and quick identification of log types.
    /// Supports success, warning, error, and informational log levels.
    /// </summary>
    public static class BTDebug
    {
        /// <summary>
        /// Contains hex color codes for each log level.
        /// </summary>
        private static class Colors
        {
            public const string Success = "#00FF00";  // Green
            public const string Warning = "#FFD700";  // Yellow
            public const string Error = "#FF0000";  // Red
            public const string Info = "#87CEFA";  // Light Blue
        }

        private const string Prefix = "BuildingTool";

        /// <summary>
        /// Logs a success message with a green "[BuildingTool]" prefix.
        /// Use this for confirmations of expected or completed actions.
        /// </summary>
        /// <param name="message">The content of the log message.</param>
        public static void LogSuccess(string message)
        {
            Debug.Log($"[<color={Colors.Success}>{Prefix}</color>] {message}");
        }

        /// <summary>
        /// Logs a warning message with a yellow "[BuildingTool]" prefix.
        /// Use this to indicate non-critical issues or fallbacks.
        /// </summary>
        /// <param name="message">The content of the log message.</param>
        public static void LogWarning(string message)
        {
            Debug.LogWarning($"[<color={Colors.Warning}>{Prefix}</color>] {message}");
        }

        /// <summary>
        /// Logs an error message with a red "[BuildingTool]" prefix.
        /// Use this to report critical failures or unexpected states.
        /// </summary>
        /// <param name="message">The content of the log message.</param>
        public static void LogError(string message)
        {
            Debug.LogError($"[<color={Colors.Error}>{Prefix}</color>] {message}");
        }

        /// <summary>
        /// Logs an informational message with a light blue "[BuildingTool]" prefix.
        /// Use this for neutral or descriptive messages that do not indicate a problem.
        /// </summary>
        /// <param name="message">The content of the log message.</param>
        public static void LogInfo(string message)
        {
            Debug.Log($"[<color={Colors.Info}>{Prefix}</color>] {message}");
        }

        /// <summary>
        /// Logs a message using the specified log level.
        /// Automatically delegates to the appropriate formatted method.
        /// </summary>
        /// <param name="level">The type of log to emit.</param>
        /// <param name="message">The content of the log message.</param>
        public static void Log(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Success:
                    LogSuccess(message);
                    break;
                case LogLevel.Warning:
                    LogWarning(message);
                    break;
                case LogLevel.Error:
                    LogError(message);
                    break;
                case LogLevel.Info:
                    LogInfo(message);
                    break;
            }
        }

        /// <summary>
        /// Enumerates all supported log levels for BuildingTool.
        /// </summary>
        public enum LogLevel
        {
            Success,
            Warning,
            Error,
            Info
        }
    }
}
