﻿using DoodleThings.Controllers;
using DoodleThings.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        public void GetUserInfoFromId()
        {
            var userInfo = new UserInfoController().GetUserInfoFromId("TestUser1");
            Debug.Assert(userInfo != null);
        }
    }
}
