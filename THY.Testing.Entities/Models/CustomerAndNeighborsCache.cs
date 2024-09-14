using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THY.Testing.Entities.Responses;

namespace THY.Testing.Entities.Models
{
    /// <summary>
    /// This class is used to cache the customers and their neighbors for a given customerId, high and low values.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="high"></param>
    /// <param name="low"></param>
    /// <param name="customers"></param>
    public class CustomerAndNeighborsCache(
        long customerId, IEnumerable<LeaderBoardResponse> customers, int high = 0, int low = 0)
    {
        public long CustomerId { get; set; } = customerId;
        public int High { get; set; } = high;
        public int Low { get; set; } = low;
        public IEnumerable<LeaderBoardResponse> Customers { get; set; } = customers;
    }
}
