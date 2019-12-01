using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialApps.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialApps.Models;

namespace SocialApps.Controllers.Tests
{
    [TestClass()]
    public class MobileControllerTests
    {
        [TestMethod()]
        public void EditExpenseTestNewExpense()
        {
            //  Tests default values.
            var newExpense = new NewExpense(1, null, DateTime.Now);
            Assert.AreEqual(newExpense.Name, null);
            Assert.AreEqual(newExpense.Currency, null);
            Assert.AreEqual(newExpense.Note, null);
            //  Should have defined value by default
            //  (see https://github.com/dvmorozov/expenses/issues/148).
            Assert.AreEqual(newExpense.Monthly, false);
        }
    }
}