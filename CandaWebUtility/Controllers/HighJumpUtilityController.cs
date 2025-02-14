using CandaWebUtility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CandaWebUtility.Controllers
{
    public class HighJumpUtilityController : Controller
    {
        [AllowAnonymous]
        public async Task<ActionResult> ChangeAllExpiryDates(HighJumpUtilitySearch model)
        {
            if (model.ProductSearchText == null)
            {
                return View("Index");
            }

            if (Session[HighJumpUser.ID] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string userID = Session[HighJumpUser.ID].ToString();

            await model.ChangeAllExpiryDates(userID);

            

            return View("Index", model);
        }

        [AllowAnonymous]
        public ActionResult FilterByLot(string ProductSearchText, string Lot)
        {
            HighJumpUtilitySearch model = new HighJumpUtilitySearch();
            model.Init();
            model.ProductSearchText = ProductSearchText;
            model.LotSearchText = Lot;
            model.Search();

            return View("Index", model);
        }

        // GET: HighJumpUtility
        [AllowAnonymous]
        public ActionResult Index()
        {

            if (Session[HighJumpUser.ID] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            HighJumpUtilitySearch model = new HighJumpUtilitySearch();
            model.Init();

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Move(Guid RowID)
        {
            if (Session[HighJumpUser.ID] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            HighJumpMoveBinLabel model = new HighJumpMoveBinLabel();
            model.Init(RowID);

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Move(HighJumpMoveBinLabel model)
        {

            if (Session[HighJumpUser.ID] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string userID = Session[HighJumpUser.ID].ToString();

            model.Save(userID);

            if (!String.IsNullOrEmpty(model.Warning))
            {
                string warning = model.Warning;
                decimal quantity = Convert.ToDecimal(model.NewQuantity);

                model.Init(model.RowID);

                model.Warning = warning;
                model.NewQuantity = quantity;

                return View(model);
            }

            HighJumpUtilitySearch searchModel = new HighJumpUtilitySearch();

            searchModel.ProductSearchText = model.Product;
            searchModel.LotSearchText = model.NewLot;
            searchModel.Search();

            return View("Index", searchModel);
        }

        [AllowAnonymous]
        public ActionResult Search(HighJumpUtilitySearch model)
        {
            model.Search();

            return View("Index", model);
        }
    }
}