using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MindbodyStar;
using MindbodyStar.Controllers;

namespace MindbodyStar.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            StarController controller = new StarController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            StarController controller = new StarController();

            // Act
            ViewResult result = controller.StarBoard(-99) as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            StarController controller = new StarController();

            // Act
            ViewResult result = controller.Load() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
