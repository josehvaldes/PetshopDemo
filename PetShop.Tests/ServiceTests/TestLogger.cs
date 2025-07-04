﻿using Microsoft.Extensions.Logging;

namespace PetShop.Tests.ServiceTests
{
    public class TestLogger<T>: ILogger<T>
    {
        public List<string> Messages;
        public TestLogger() 
        {
            Messages = new List<string>();
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Messages.Add(state?.ToString()??string.Empty);
        }
    }
}
