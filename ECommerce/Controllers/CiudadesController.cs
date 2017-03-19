using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ECommerce.Models;
using ECommerce.Clases;

namespace ECommerce.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class CiudadesController : Controller
    {
        private ECommerceContext db = new ECommerceContext();

        // GET: Ciudades
        
        public ActionResult Index()
        {
            var Ciudades = db.Ciudades.Include(c => c.Departamento);
            return View(Ciudades.ToList());
        }

        // GET: Ciudades/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ciudad ciudad = db.Ciudades.Find(id);
            if (ciudad == null)
            {
                return HttpNotFound();
            }
            return View(ciudad);
        }

        public ActionResult Create()
        {
            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(),
                "DepartamentoID", "Nombre");
            return View();
        }

        // POST: Ciudades/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Ciudad ciudad)
        {
            if (ModelState.IsValid)
            {
                db.Ciudades.Add(ciudad);
                var respuesta = DbHelper.Guardar(db);
                if (respuesta.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, respuesta.Message);
            }

            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(),
                "DepartamentoID", "Nombre");
            return View(ciudad);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ciudad ciudad = db.Ciudades.Find(id);
            if (ciudad == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(),
                "DepartamentoID", "Nombre");
            return View(ciudad);
        }

        // POST: Ciudades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ciudad ciudad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ciudad).State = EntityState.Modified;
                var respuesta = DbHelper.Guardar(db);
                if (respuesta.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, respuesta.Message);
            }
            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(),
                "DepartamentoID", "Nombre");
            return View(ciudad);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ciudad ciudad = db.Ciudades.Find(id);
            if (ciudad == null)
            {
                return HttpNotFound();
            }
            return View(ciudad);
        }

        // POST: Ciudades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ciudad ciudad = db.Ciudades.Find(id);
            db.Ciudades.Remove(ciudad);
            var respuesta = DbHelper.Guardar(db);
            if (respuesta.Succeeded)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, respuesta.Message);
            return View(ciudad);
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
