using Microsoft.Data.Tools.Schema.Sql.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass()]
    public class SqlServerUnitTestLastYearBalanceByMonthByUser : SqlDatabaseTestClass
    {

        public SqlServerUnitTestLastYearBalanceByMonthByUser()
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
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction expenses_LastYearBalanceByMonthByUserTest_TestAction;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SqlServerUnitTestLastYearBalanceByMonthByUser));
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.RowCountCondition rowCountCondition1;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction expenses_LastYearBalanceByMonthByUserTest1_TestAction;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.InconclusiveCondition inconclusiveCondition2;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction expenses_LastYearBalanceByMonthByUserTest_PosttestAction;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.RowCountCondition rowCountCondition2;
            this.expenses_LastYearBalanceByMonthByUserTestData = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestActions();
            expenses_LastYearBalanceByMonthByUserTest_TestAction = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction();
            rowCountCondition1 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.RowCountCondition();
            expenses_LastYearBalanceByMonthByUserTest1_TestAction = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction();
            inconclusiveCondition2 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.InconclusiveCondition();
            expenses_LastYearBalanceByMonthByUserTest_PosttestAction = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction();
            rowCountCondition2 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.RowCountCondition();
            // 
            // expenses_LastYearBalanceByMonthByUserTest_TestAction
            // 
            expenses_LastYearBalanceByMonthByUserTest_TestAction.Conditions.Add(rowCountCondition1);
            expenses_LastYearBalanceByMonthByUserTest_TestAction.Conditions.Add(rowCountCondition2);
            resources.ApplyResources(expenses_LastYearBalanceByMonthByUserTest_TestAction, "expenses_LastYearBalanceByMonthByUserTest_TestAction");
            // 
            // rowCountCondition1
            // 
            rowCountCondition1.Enabled = true;
            rowCountCondition1.Name = "rowCountCondition1";
            rowCountCondition1.ResultSet = 1;
            rowCountCondition1.RowCount = 60;
            // 
            // expenses_LastYearBalanceByMonthByUserTest1_TestAction
            // 
            expenses_LastYearBalanceByMonthByUserTest1_TestAction.Conditions.Add(inconclusiveCondition2);
            resources.ApplyResources(expenses_LastYearBalanceByMonthByUserTest1_TestAction, "expenses_LastYearBalanceByMonthByUserTest1_TestAction");
            // 
            // inconclusiveCondition2
            // 
            inconclusiveCondition2.Enabled = true;
            inconclusiveCondition2.Name = "inconclusiveCondition2";
            // 
            // expenses_LastYearBalanceByMonthByUserTest_PosttestAction
            // 
            resources.ApplyResources(expenses_LastYearBalanceByMonthByUserTest_PosttestAction, "expenses_LastYearBalanceByMonthByUserTest_PosttestAction");
            // 
            // expenses_LastYearBalanceByMonthByUserTestData
            // 
            this.expenses_LastYearBalanceByMonthByUserTestData.PosttestAction = expenses_LastYearBalanceByMonthByUserTest_PosttestAction;
            this.expenses_LastYearBalanceByMonthByUserTestData.PretestAction = null;
            this.expenses_LastYearBalanceByMonthByUserTestData.TestAction = expenses_LastYearBalanceByMonthByUserTest_TestAction;
            // 
            // rowCountCondition2
            // 
            rowCountCondition2.Enabled = true;
            rowCountCondition2.Name = "rowCountCondition2";
            rowCountCondition2.ResultSet = 2;
            rowCountCondition2.RowCount = 0;
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
        public void expenses_LastYearBalanceByMonthByUserTest()
        {
            SqlDatabaseTestActions testActions = this.expenses_LastYearBalanceByMonthByUserTestData;
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

        private SqlDatabaseTestActions expenses_LastYearBalanceByMonthByUserTestData;
    }
}
