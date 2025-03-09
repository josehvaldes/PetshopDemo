using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //throw new NotImplementedException();
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
