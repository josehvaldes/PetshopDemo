using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Application.Requests
{
    public class CallResponse
    {
        public string SaleId { get; set; } = string.Empty;
        public List<string> Messages { get; }
        
        public CallResponse() 
        {
            Messages = new List<string>();
        }

        public void AddMessage(string message) 
        { 
            Messages.Add(message);
        }

        public bool IsCompleted() 
        {
            return !Messages.Any();
        }
    }
}
