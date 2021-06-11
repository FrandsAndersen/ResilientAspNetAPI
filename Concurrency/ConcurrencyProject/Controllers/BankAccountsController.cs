﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConcurrencyProject.Data;
using ConcurrencyProject.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ConcurrencyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BankAccountsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/BankAccounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankAccount>>> GetBankAccounts()
        {
            return await _context.BankAccounts.ToListAsync();
        }

        // GET: api/BankAccounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BankAccount>> GetBankAccount(int id)
        {
            var bankAccount = await _context.BankAccounts.FindAsync(id);

            if (bankAccount == null)
            {
                return NotFound();
            }

            return bankAccount;
        }

        // POST: api/BankAccounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<String>> PostBankAccount(int newBalance, int oldBalance)
        {
            var bankAccount = _context.BankAccounts.First();

            bankAccount.UpdateBalance(_context,newBalance,oldBalance);
            var returnMessage = "";
            try
            {
                
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                var errorEntry = e.Entries.Single();
                returnMessage = DiagnoseBalanceConcurrencyConflict(errorEntry);
            }

            return returnMessage;
        }

        private string DiagnoseBalanceConcurrencyConflict(EntityEntry errorEntry)
        {
            var bankAccount = errorEntry.Entity as BankAccount;
            if (bankAccount == null)
            {
                throw new NotSupportedException("Unknown conflict revolving: " + errorEntry.Metadata.Name);
            }

            var dbEntity = _context.BankAccounts.AsNoTracking()
                .SingleOrDefault(b => b.BankAccountId == bankAccount.BankAccountId);
            if (dbEntity == null)
            {
                return "The bankaccount with name: " + bankAccount.AccountName + " was deleted by another user.... Here are your available actions: .......";
            }
            else
            {
                return "The bankaccount with name: " + bankAccount.AccountName + " has the following balance: " + dbEntity.Balance + 
                       ". You expected the balance to be: " + bankAccount.Balance + "! Here are your options for handling this disconnected concurrency: ......";
            }
        }

        [HttpPut("{triggerConcurrency}")]
        public async Task<IActionResult> PutBankAccountTriggerConcurrency(bool triggerConcurrency)
        {
            try
            {
                var account = _context.BankAccounts.First();
                
                // Updating the same Entity twice to trigger exception
                if (triggerConcurrency)
                {
                    System.Diagnostics.Debug.WriteLine("Making concurrent update of accountId: " + account.BankAccountId);

                    await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE dbo.BankAccounts SET Balance = @p0 " +
                        "WHERE BankAccountId = @p1",
                        account.Balance * 2, account.BankAccountId);
                }

                account.Balance = -2;
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException e)
            {
                    System.Diagnostics.Debug.WriteLine(e);
                    throw;
            }
            return NoContent();
        }


        // DELETE: api/BankAccounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBankAccount(int id)
        {
            var bankAccount = await _context.BankAccounts.FindAsync(id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            _context.BankAccounts.Remove(bankAccount);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BankAccountExists(int id)
        {
            return _context.BankAccounts.Any(e => e.BankAccountId == id);
        }
    }
}
