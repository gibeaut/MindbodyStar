using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MindbodyStar.Models;
using MindbodyStar.Services;

namespace MindbodyStar.Controllers
{
    public class StarController : Controller
    {
        public ActionResult Index()
        {
            var starService = new StarService();
            var model = new StarViewModel() {StudioIDs = starService.GetStudios()} ;

            return View(model);
        }

        public ActionResult Load()
        {
            return View(new StarLoadViewModel());
        }

        public ActionResult StudioNotSetup(string message)
        {
            return View();
        }

        public ActionResult StarBoard(int studioid)
        {
            var starService = new StarService();
            try
            {
                var stars = starService.GetClientVisits("", "", studioid);
                var model = new StarBoardViewModel() { Stars = stars };

                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("StudioNotSetup", new {message = e.Message});
            }
        }

        public ActionResult NewStarBoard(StarLoadViewModel starLoad)
        {
            var starService = new StarService();
            try
            {
                var stars = starService.GetClientVisits(starLoad.Username, starLoad.Password, starLoad.StudioID);

                var model = new StarBoardViewModel() {Stars = stars};

              return View("StarBoard", model);
            }
            catch (Exception e)
            {
                return RedirectToAction("StudioNotSetup");
            }
        }

    }
}