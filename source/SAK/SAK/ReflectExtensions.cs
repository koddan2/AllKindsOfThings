using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable
namespace SAK
{
    /// <summary>
    /// Extensions for reflection.
    /// </summary>
    public static class ReflectExtensions
    {
        /// <summary>
        /// Use this extension to unwrap a nullable thing and fail with exception if it is null.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="maybeValue">The reference</param>
        /// <param name="message">the message</param>
        /// <param name="exceptionMaker">innerException => newException</param>
        /// <param name="context">leave empty (compiler assigns the value)</param>
        /// <param name="memberName">leave memberName empty (compiler assigns the value)</param>
        /// <param name="sourceFilePath">leave sourceFilePathempty (compiler assigns the value)</param>
        /// <param name="sourceLineNumber">leave sourceLineNumber empty (compiler assigns the value)</param>
        /// <returns>the non-null value.</returns>
        /// <exception cref="InvariantFailedException">If the value or reference is null.</exception>
        [DebuggerStepThrough]
        public static T OrFail<T>(
            this T? maybeValue,
            string? message = null,
            Func<Exception, Exception>? exceptionMaker = null,
            [CallerArgumentExpression("maybeValue")] string? context = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
            where T : struct
        {
            string GetMsg() => $"{(message is not null ? message + " - " : "")}expression: ({context}) (in member {memberName}) {sourceFilePath}:{sourceLineNumber}";
            return maybeValue
                ?? (exceptionMaker is null
                    ? throw new InvariantFailedException(GetMsg())
                    : throw exceptionMaker(new InvariantFailedException(GetMsg())));
        }

        /// <summary>
        /// Use this extension to unwrap a nullable thing and fail with exception if it is null.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="maybeValue">The reference</param>
        /// <param name="message">the message</param>
        /// <param name="exceptionMaker">innerException => newException</param>
        /// <param name="context">leave empty (compiler assigns the value)</param>
        /// <param name="memberName">leave memberName empty (compiler assigns the value)</param>
        /// <param name="sourceFilePath">leave sourceFilePathempty (compiler assigns the value)</param>
        /// <param name="sourceLineNumber">leave sourceLineNumber empty (compiler assigns the value)</param>
        /// <returns>the non-null value.</returns>
        /// <exception cref="InvariantFailedException">If the value or reference is null.</exception>
        [DebuggerStepThrough]
        public static T OrFail<T>(
            this T? maybeValue,
            string? message = null,
            Func<Exception, Exception>? exceptionMaker = null,
            [CallerArgumentExpression("maybeValue")] string? context = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
            where T : class
        {
            string GetMsg() => $"{(message is not null ? message + " - " : "")}expression: ({context}) (in member {memberName}) {sourceFilePath}:{sourceLineNumber}";
            return maybeValue
                ?? (exceptionMaker is null
                    ? throw new InvariantFailedException(GetMsg())
                    : throw exceptionMaker(new InvariantFailedException(GetMsg())));
        }

        /// <summary>
        /// Use this extension to unwrap an async nullable thing and fail with exception if it is null.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="maybeValue">The reference</param>
        /// <param name="message">the message</param>
        /// <param name="exceptionMaker">innerException => newException</param>
        /// <param name="context">leave empty (compiler assigns the value)</param>
        /// <param name="memberName">leave memberName empty (compiler assigns the value)</param>
        /// <param name="sourceFilePath">leave sourceFilePathempty (compiler assigns the value)</param>
        /// <param name="sourceLineNumber">leave sourceLineNumber empty (compiler assigns the value)</param>
        /// <returns>the non-null value.</returns>
        /// <exception cref="InvariantFailedException">If the value or reference is null.</exception>
        [DebuggerStepThrough]
        public static async Task<T> OrFailAsync<T>(
            this Task<T?> maybeValue,
            string? message = null,
            Func<Exception, Exception>? exceptionMaker = null,
            [CallerArgumentExpression("maybeValue")] string? context = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
            where T : class
        {
            string GetMsg() => $"{(message is not null ? message + " - " : "")}expression: ({context}) (in member {memberName}) {sourceFilePath}:{sourceLineNumber}";
            return (await maybeValue)
                ?? (exceptionMaker is null
                    ? throw new InvariantFailedException(GetMsg())
                    : throw exceptionMaker(new InvariantFailedException(GetMsg())));
        }
    }
}
