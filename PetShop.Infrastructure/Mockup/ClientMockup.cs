using PetShop.Application.Interfaces.Repository;
using PetShop.Domain.Entities;

namespace PetShop.Infrastructure.Mockup
{
    public class ClientMockup : IClientRepository
    {
        internal static List<Client> _clientMockups = new List<Client>() {
            new Client(){
             taxnumber="123456",
             taxnumberend = "6",
             guid = "09796a3c-6670-4918-b38e-4d8152acc4f3",
             fullname = "John Wick",
            },
            new Client(){
             taxnumber="987452",
             taxnumberend = "2",
             guid = "fd66622f-80ce-4ebb-99dc-85037d9aaf2f",
             fullname = "Via li",
            },
        };

        public async Task<Client?> Create(Client client)
        {
            return await Task.Run(() =>
            {
                if (!_clientMockups.Where(x => x.taxnumber == client.taxnumber).Any())
                {
                    _clientMockups.Add(client);
                    return client;
                }
                else
                {
                    throw new InvalidDataException($"Duplicated client: {client.taxnumber}");
                }
            });
        }

        public async Task<bool> Delete(Client client)
        {
            return await Task.Run(() =>
            {
                var list = _clientMockups.Where(x => x.taxnumber == client.taxnumber);
                if (list.Any())
                {
                    _clientMockups.Remove(list.First());
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        public async Task<Client?> Retrieve(string taxNumberEnd, string taxNumber)
        {
            return await Task.Run(() =>
            {
                return _clientMockups.Where(x => x.taxnumber == taxNumber).FirstOrDefault();
            });
        }

        public async Task<bool> Update(Client client)
        {
            return await Task.Run(() =>
            {
                var list = _clientMockups.Where(x => x.taxnumber == client.taxnumber);
                if (list.Any())
                {
                    var item = list.First();
                    item.fullname = client.fullname;
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }
    }
}
