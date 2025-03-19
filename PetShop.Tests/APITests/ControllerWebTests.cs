using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetShopAPI;
using FluentAssertions;
using System.Net;
using NUnit.Framework;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http.Headers;

namespace PetShop.Tests.APITests
{
    public class ControllerWebTests : IDisposable //: IClassFixture<CustomWebApplicationFactory>
    {

        private CustomWebApplicationFactory _factory;
        private HttpClient _client;

        [OneTimeSetUp]
        public void OntimeSetup() 
        {
            _factory = new CustomWebApplicationFactory();
        }

        [SetUp]
        public void Setup() 
        {
            //_client = _factory.CreateClient();
        }

        public void Dispose() 
        {
            //_factory?.Dispose();
        } 

        [Test]
        public async Task Get_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("api/setup/");
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
