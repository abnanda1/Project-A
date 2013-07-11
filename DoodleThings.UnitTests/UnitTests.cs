using DoodleThings.Controllers;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoodleThings.UnitTests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void Basic_noop_test()
        {
        }

        [TestMethod]
        public void TestCreateNewUser()
        {
            var userController = new UserInfoController();
            var id = DateTime.Now.ToString("yyyyMMddHHmmss");
            userController.CreateNewLoggedOutUser(id, "TestUser" + id);
        }
    }
}
