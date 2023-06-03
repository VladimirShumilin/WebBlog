namespace WebBlog.Extensions
{
    /// <summary>
    /// Класс содержит заранее созданные делегаты для Logger
    /// такой подход обеспечивает лучшую производительность при логировании
     /// </summary>
    internal static class LoggerExtensions
    {
        private static readonly Action<ILogger, string, Exception?> 
            commonError = LoggerMessage.Define<string>(
                logLevel: LogLevel.Error,
                eventId: 1,
                formatString: "Executing action at '{StartTime}'");

        private static readonly Action<ILogger, string, Exception?>
            commonInfo = LoggerMessage.Define<string>(
                logLevel: LogLevel.Information,
                eventId: 2,
                formatString: "Executing action at '{StartTime}'");

        /// <summary>
        /// Записывает строку msg и объект ex в лог уровня LogLevel.Error и ID равным 1.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public static void CommonError(
            this ILogger logger,Exception? ex ,string msg)
        {
            commonError(logger, msg, ex);
        }
        /// <summary>
        /// Записывает строку msg  в лог уровня LogLevel.Information и ID равным 2.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public static void CommonInfo(
            this ILogger logger, string msg)
        {
            commonInfo(logger, msg, null);
        }
    }
}
