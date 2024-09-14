using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THY.Testing.Entities.Responses;

namespace THY.Testing.Entities.Models
{
    /// <summary>
    /// This class is used to cache the customers by rank for a given range of ranks.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="customers"></param>
    public class CustomersByRankCache(int start, int end, IEnumerable<LeaderBoardResponse> customers)
    {
        public int Start { get; set; } = start;
        public int End { get; set; } = end;
        public IEnumerable<LeaderBoardResponse> Customers { get; set; } = customers;
    }
}
