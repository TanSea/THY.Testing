using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THY.Testing.Entities.Models
{
    /// <summary>
    /// This class represents a customer in the system.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="score"></param>
    public class Customer(long customerId, decimal score = 0) // The default score of a customer is zero
    {
        public long CustomerId { get; set; } = customerId;
        public decimal Score { get; set; } = score;
    }
}
