using Cortex.Mediator.Notifications;
using Cortex.Streams;
using PetShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Application.EventHandlers
{
    public class SalesEventHandler : INotificationHandler<Sale>
    {
        //private readonly IStream<Sale, object> _stream;
        public SalesEventHandler()
        {
            // Constructor logic if needed
        }

        public async Task Handle(Sale notification, CancellationToken cancellationToken)
        {
            //TODO create the pipeline to send sales notifications to IStream

        }
    }
}
