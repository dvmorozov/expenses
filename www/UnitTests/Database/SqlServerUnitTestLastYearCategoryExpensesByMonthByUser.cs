using Microsoft.Data.Tools.Schema.Sql.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SqlUnitTests
{
    [TestClass()]
    public class SqlServerUnitTestLastYearCategoryExpensesByMonthByUser : SqlDatabaseTestClass
    {

        public SqlServerUnitTestLastYearCategoryExpensesByMonthByUser()
        {
            InitializeComponent();
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            base.InitializeTest();
        }
        [TestCleanup()]
        public void TestCleanup()
        {
            base.CleanupTest();
        }

        #region Designer support code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction expenses_LastYearCategoryExpensesByMonthByUserTest_TestAction;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SqlServerUnitTestLastYearCategoryExpensesByMonthByUser));
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.EmptyResultSetCondition emptyResultSetCondition1;
            this.expenses_LastYearCategoryExpensesByMonthByUserTestData = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestActions();
            expenses_LastYearCategoryExpensesByMonthByUserTest_TestAction = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction();
            emptyResultSetCondition1 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.EmptyResultSetCondition();
            // 
            // expenses_LastYearCategoryExpensesByMonthByUserTest_TestAction
            // 
            expenses_LastYearCategoryExpensesByMonthByUserTest_TestAction.Conditions.Add(emptyResultSetCondition1);
            resources.ApplyResources(expenses_LastYearCategoryExpensesByMonthByUserTest_TestAction, "expenses_LastYearCategoryExpensesByMonthByUserTest_TestAction");
            // 
            // expenses_LastYearCategoryExpensesByMonthByUserTestData
            // 
            this.expenses_LastYearCategoryExpensesByMonthByUserTestData.PosttestAction = null;
            this.expenses_LastYearCategoryExpensesByMonthByUserTestData.PretestAction = null;
            this.expenses_LastYearCategoryExpensesByMonthByUserTestData.TestAction = expenses_LastYearCategoryExpensesByMonthByUserTest_TestAction;
            // 
            // emptyResultSetCondition1
            // 
            emptyResultSetCondition1.Enabled = true;
            emptyResultSetCondition1.Name = "emptyResultSetCondition1";
            emptyResultSetCondition1.ResultSet = 1;
        }

        #endregion


        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        #endregion

        [TestMethod()]
        public void expenses_LastYearCategoryExpensesByMonthByUserTest()
        {
            SqlDatabaseTestActions testActions = this.expenses_LastYearCategoryExpensesByMonthByUserTestData;
            // Execute the pre-test script
            // 
            System.Diagnostics.Trace.WriteLineIf((testActions.PretestAction != null), "Executing pre-test script...");
            SqlExecutionResult[] pretestResults = TestService.Execute(this.PrivilegedContext, this.PrivilegedContext, testActions.PretestAction);
            try
            {
                // Execute the test script
                // 
                System.Diagnostics.Trace.WriteLineIf((testActions.TestAction != null), "Executing test script...");
                SqlExecutionResult[] testResults = TestService.Execute(this.ExecutionContext, this.PrivilegedContext, testActions.TestAction);
            }
            finally
            {
                // Execute the post-test script
                // 
                System.Diagnostics.Trace.WriteLineIf((testActions.PosttestAction != null), "Executing post-test script...");
                SqlExecutionResult[] posttestResults = TestService.Execute(this.PrivilegedContext, this.PrivilegedContext, testActions.PosttestAction);
            }
        }
        private SqlDatabaseTestActions expenses_LastYearCategoryExpensesByMonthByUserTestData;
    }
}
