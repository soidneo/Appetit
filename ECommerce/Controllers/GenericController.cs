using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerce.Controllers
{
    public class GenericController : Controller
    {
        private ECommerceContext db = new ECommerceContext();
        // GET: Generic
        public JsonResult GetCities(int departamentoId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var ciudadesDe = db.Ciudades.Where(c => c.DepartamentoID == departamentoId);
            return Json(ciudadesDe);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}