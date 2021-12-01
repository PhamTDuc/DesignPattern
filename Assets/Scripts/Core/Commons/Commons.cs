using UnityEngine;
using System.Diagnostics;

namespace Commons
{
    public class Logger
    {
        #region LogWarning
        //-----------------------------------
        //--------------------- Log , warning, 

        [Conditional("DEVELOPMENT")]
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }
        [Conditional("DEVELOPMENT")]
        public static void LogIf(bool condition, object message)
        {
            if (condition)
            {
                UnityEngine.Debug.Log(message);
            }
        }
        // [Conditional("DEVELOPMENT")]
        // public static void Log(string format, params object[] args)
        // {
        //     UnityEngine.Debug.Log(string.Format(format, args));
        // }
        [Conditional("DEVELOPMENT")]
        public static void LogIf(bool condition, string format, params object[] args)
        {
            if (condition)
            {
                UnityEngine.Debug.Log(string.Format(format, args));
            }
        }
        [Conditional("DEVELOPMENT")]
        public static void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        [Conditional("DEVELOPMENT")]
        public static void LogWarning(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogWarning(message, context);
        }

        [Conditional("DEVELOPMENT")]
        public static void LogWarning(UnityEngine.Object context, string format, params object[] args)
        {
            UnityEngine.Debug.LogWarning(string.Format(format, args), context);
        }
        #endregion


        [Conditional("DEVELOPMENT")]
        public static void WarningUnless(bool condition, object message)
        {
            if (!condition) UnityEngine.Debug.LogWarning(message);
        }

        [Conditional("DEVELOPMENT")]
        public static void WarningUnless(bool condition, object message, UnityEngine.Object context)
        {
            if (!condition) UnityEngine.Debug.LogWarning(message, context);
        }

        [Conditional("DEVELOPMENT")]
        public static void WarningUnless(bool condition, UnityEngine.Object context, string format, params object[] args)
        {
            if (!condition) UnityEngine.Debug.LogWarning(string.Format(format, args), context);
        }

        #region Assert
        //---------------------------------------------
        //------------- Assert ------------------------

        /// Throw an exception if condition = false
        [Conditional("DEVELOPMENT")]
        public static void Assert(bool condition)
        {
            if (!condition) throw new UnityException();
        }

        /// Throw an exception if condition = false, show message on console's log
        [Conditional("DEVELOPMENT")]
        public static void Assert(bool condition, string message)
        {
            UnityEngine.Debug.Assert(condition, message);
        }

        /// Throw an exception if condition = false, show message on console's log
        [Conditional("DEVELOPMENT")]
        public static void Assert(bool condition, string format, params object[] args)
        {
            UnityEngine.Debug.Assert(condition, string.Format(format, args));
        }
        #endregion
    }
}