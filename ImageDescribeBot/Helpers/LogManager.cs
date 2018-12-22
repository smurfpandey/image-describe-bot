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
        void Info(object message);
        void InfoFormat(string format, params object[] args);
        void Warn(object message);
        void Error(object message, Exception ex = null);

        void Shutdown();
        
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
        public void Error(object message, Exception ex = null)
        {
            _logger.Error(message, ex);
        }

        public void Warn(object message)
        {
            _logger.Warn(message);
        }

        public void Info(object message)
        {
            _logger.Info(message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _logger.InfoFormat(format, args);
        }

        public void Shutdown()
        {
            _logger.Logger.Repository.Shutdown();
        }
    }

    public static class LogManager
    {
        public static ILogger GetLogger(Type type) =>            
            new Log4NetWrapper(type);
    }
}
