using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ConcurrencyProject.Models
{
    public class BankAccount
    {
        public int BankAccountId { get; set; }
        public string AccountName { get; set; }
        [ConcurrencyCheck]
        public int Balance { get; set; }

        public void UpdateBalance(DbContext context, int newBalance, int oldBalance)
        {
            Balance = newBalance;
            context.Entry(this).Property(p => p.Balance).OriginalValue = oldBalance;
        }
    }
}
