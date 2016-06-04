
    public static class Logger
    {
        private static LogBase logger = null;

        public static void Log(LogTarget target, string logMessage)
        {
            switch (target)
            {
                case LogTarget.File:
                    logger = new FileLogger();
                    logger.Log(logMessage);
                    break;
                default: return;
            }
        }

        public static void Log(string logMessage)
        {
            logger = new FileLogger();
            logger.Log(logMessage);
        }
    }

    public enum LogTarget
    {
        File, Database, EventLog
    }
