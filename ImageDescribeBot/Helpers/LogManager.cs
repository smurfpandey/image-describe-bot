using System;
using System.Collections.Generic;
using System.Text;

namespace ImageDescribeBot
{
    public interface ILogger
    {
        void Debug(object message);
        bool IsDebugEnabled { get; }

        // continue for all methods like Error, Fatal ...
    }

    public class Log4NetWrapper : ILogger
    {
        private readonly log4net.ILog _logger;

        public Log4NetWrapper(Type type)
        {
            _logger = log4net.LogManager.GetLogger(type);
        }

        public void Debug(object message)
        {
            _logger.Debug(message);
        }

        public bool IsDebugEnabled
        {
            get { return _logger.IsDebugEnabled; }
        }

        // complete ILogger interface implementation
    }

    public static class LogManager
    {
        public static ILogger GetLogger(Type type) =>            
            new Log4NetWrapper(type);
    }
}
