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
            var operations = new Operations();
            var date = DateTime.Now;
            //var newExpense = new NewExpense(1, operations, date);
            var newExpense = new NewExpense(1, null, date);
            Assert.AreEqual(newExpense.Name, null);
        }
    }
}