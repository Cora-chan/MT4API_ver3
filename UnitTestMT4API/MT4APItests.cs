using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestMT4API;
using P23.MetaTrader4.Manager.Contracts;



namespace UnitTestMT4API
{
    [TestClass]
    public class MT4API
    {
        [TestMethod]
        public void Getid_test()
        {
            var controller = new UsersController();
            var result = controller.Get(99999980);
            Assert.AreEqual(result.Name, "kangkang985");
        }


        [TestMethod]
        public void Postid_test()
        {
            var controller = new UsersController();
            controller.Post();
            var user = new UserRecord
            {
                Group = "demoforexaugs",
                Leverage = 100,
                Name = "YueKANG000",
                Password = "qwe123",
                PasswordInvestor = "qwe123",

            };
            Assert.AreEqual(user.Name, "YueKANG000");
        }


        [TestMethod]
        public void Putid_test()
        {
            var controller = new UsersController();
            controller.Put(99999980);
            var user = new UserRecord
            {

                Leverage = 400,
                Name = "tongji985",
                Password = "qwe123",
                PasswordInvestor = "qwe123",

            };
                Assert.AreEqual(user.Name, "tongji985");
        }

    }
}
