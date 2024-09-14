using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using THY.Testing.Common;
using THY.Testing.Entities.Models;
using THY.Testing.Entities.Responses;

namespace THY.Testing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private static List<Customer> _customers = [];
        private static List<CustomersByRankCache> _customersByRankCache = [];
        private static List<CustomerAndNeighborsCache> _customerAndNeighborsCache = [];

        public CustomerController()
        {
            CreateCustomer();
        }

        /// <summary>
        /// Initializes the data for the customer controller.
        /// </summary>
        /// <returns></returns>
        [HttpPost("/customer/initdata")]
        public int CreateCustomer()
        {
            for (int i = 0; i < 1000000; i++)
            {
                var randomScore = RandomNumber.GenerateRandomScore();
                _customers.Add(new Customer(i, randomScore));
            }

            _customers = [.. _customers.OrderByDescending(c => c.Score).ThenBy(c => c.CustomerId)];

            return _customers.Count;
        }

        /// <summary>
        /// Updates the score of a customer.
        /// </summary>
        /// <param name="customerid">Customer Id</param>
        /// <param name="score">Score to be updated</param>
        /// <returns></returns>
        [HttpPost("/customer/{customerid}/score/{score}")]
        public async Task<decimal> UpdateScoreAsync(long customerid, decimal score)
        {
            if (customerid < 0)
                throw new ArgumentException("Invalid customerid");

            if (score < -1000 || score > 1000)
                throw new ArgumentException("Invalid score");

            var isExistCustomer = _customers.Any(r => r.CustomerId == customerid);
            if (isExistCustomer)
            {
                var updateCustomer = _customers.FirstOrDefault(r => r.CustomerId == customerid)!;
                _customers.Remove(updateCustomer);
                score = updateCustomer.Score + score;
            }

            _customersByRankCache = [];
            _customerAndNeighborsCache = [];
            return await InsertScoreAsync(customerid, score);
        }

        /// <summary>
        /// Inserts a score into the customer list.
        /// </summary>
        /// <param name="customerid"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        private static async Task<decimal> InsertScoreAsync(long customerid, decimal score)
        {
            int left = 0;
            int right = _customers.Count;

            while (left < right)
            {
                int mid = left + (right - left) / 2;
                if (_customers[mid].Score < score)
                {
                    right = mid;
                }
                else if (_customers[mid].Score > score)
                {
                    left = mid + 1;
                }
                else
                {
                    // If there are multiple customers with the same score, insert at the correct position
                    mid = _customers.IndexOf(_customers.Where(r => r.Score == score).FirstOrDefault()!);
                    while (mid < _customers.Count && _customers[mid].Score == score)
                    {
                        if (_customers[mid].CustomerId < customerid)
                        {
                            mid++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    left = mid;
                    break;
                }
            }

            await Task.Run(() => _customers.Insert(left, new Customer(customerid, score)));
            return score;
        }

        /// <summary>
        /// Gets the customers by rank.
        /// </summary>
        /// <param name="start">Start index</param>
        /// <param name="end">End index</param>
        /// <returns></returns>
        [HttpGet("/leaderboard")]
        public async Task<IEnumerable<LeaderBoardResponse>> GetCustomersByRankAsync(int start, int end)
        {
            if (start < 1 || end > _customers.Count || start > end)
                throw new ArgumentException("Invalid start or end");

            if (_customersByRankCache.Any(c => c.Start == start && c.End == end))
                return await Task.Run(() =>
                    _customersByRankCache.FirstOrDefault(c => c.Start == start && c.End == end)!.Customers);

            var customersByRank = _customers.Skip(start - 1).Take(end - start + 1);
            var result = await Task.Run(() => customersByRank.Select(c => new LeaderBoardResponse
            {
                CustomerId = c.CustomerId,
                Score = c.Score,
                Rank = _customers.IndexOf(c) + 1
            }));
            _customersByRankCache.Add(new CustomersByRankCache(start, end, result));
            return result;
        }

        /// <summary>
        /// Gets the customer and its neighbors.
        /// </summary>
        /// <param name="customerid">Customer Id</param>
        /// <param name="high">High rank</param>
        /// <param name="low">Low rank</param>
        /// <returns></returns>
        [HttpGet("/leaderboard/{customerid}")]
        public async Task<IEnumerable<LeaderBoardResponse>> GetCustomerAndNeighborsAsync(long customerid, int high = 0, int low = 0)
        {
            if (customerid < 0)
                throw new ArgumentException("Invalid customerid");

            if (high < 0 || low < 0)
                throw new ArgumentException("Invalid high or low");

            if (_customerAndNeighborsCache.Any(c => c.CustomerId == customerid && c.High == high && c.Low == low))
            {
                return await Task.Run(() =>
                    _customerAndNeighborsCache
                    .FirstOrDefault(c => c.CustomerId == customerid && c.High == high && c.Low == low)!.Customers);
            }

            int rank = _customers.FindIndex(c => c.CustomerId == customerid) + 1;

            int start = rank - low - 1;
            int end = high + low + 1;
            if (start < 0)
            {
                end += start;
                start = 0;
            }
            if (end > _customers.Count - 1)
            {
                end = _customers.Count - 1;
            }

            var neighbors = _customers.Skip(start).Take(end).ToList();

            var result = await Task.Run(() => neighbors.Select(c => new LeaderBoardResponse
            {
                CustomerId = c.CustomerId,
                Score = c.Score,
                Rank = _customers.IndexOf(c) + 1
            }));

            _customerAndNeighborsCache.Add(new CustomerAndNeighborsCache(customerid, result, high, low));
            return result;
        }
    }
}
