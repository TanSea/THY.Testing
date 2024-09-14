using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using System.Net.Http.Json;
using THY.Testing.Api.Controllers;
using THY.Testing.Entities.Responses;

namespace ApiTest
{
    [TestClass]
    public class CustomerControllerTests
    {
        private readonly CustomerController _controller;

        public CustomerControllerTests()
        {
            _controller = new CustomerController();
        }

        //[TestMethod]
        //public async Task InitializeDatabase_ReturnsCountOfCustomers()
        //{
        //    var result = await Task.Run(_controller.CreateCustomer);
        //    Assert.AreEqual(1000000, result);
        //}

        [TestMethod]
        public async Task UpdateScore_ReturnsNewScore()
        {
            var result = await _controller.UpdateScoreAsync(1001, 100);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetCustomersByRank_ReturnsSuccess()
        {
            var result = await _controller.GetCustomersByRankAsync(1001, 2000);
            Assert.AreEqual(1000, result.Count());
        }

        [TestMethod]
        public async Task GetCustomerAndAndNeighbors_ReturnsSuccess()
        {
            var result = await _controller.GetCustomerAndNeighborsAsync(1001, 3, 3);
            Assert.AreEqual(7, result.Count());
        }
    }
}