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
    [Authorize(Roles = "Admin")]
    public class BodegasController : Controller
    {
        private ECommerceContext db = new ECommerceContext();

        
        public ActionResult Index()
        {
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var bodegas = db.Bodegas.Include(b => b.Ciudad).Include(b => b.Departamento);
            return View(bodegas.ToList());
        }

        // GET: Bodegas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bodega bodega = db.Bodegas.Find(id);
            if (bodega == null)
            {
                return HttpNotFound();
            }
            return View(bodega);
        }

        // GET: Bodegas/Create
        public ActionResult Create()
        {
            ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(0), "CiudadID", "Nombre");
            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(), "DepartamentoID", "Nombre");
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var bodega = new Bodega { EmpresaID = user.EmpresaID, };
            return View(bodega);
        }

        // POST: Bodegas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Bodega bodega)
        {
            if (ModelState.IsValid)
            {
                db.Bodegas.Add(bodega);
                var respuesta = DbHelper.Guardar(db);
                if (respuesta.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty,respuesta.Message);
            }
            ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(0), "CiudadID", "Nombre", bodega.CiudadID);
            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(), "DepartamentoID", "Nombre", bodega.DepartamentoID);
            return View(bodega);
        }

        // GET: Bodegas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bodega bodega = db.Bodegas.Find(id);
            if (bodega == null)
            {
                return HttpNotFound();
            }
            ViewBag.CiudadID = new SelectList(db.Ciudades, "CiudadID", "Nombre", bodega.CiudadID);
            ViewBag.DepartamentoID = new SelectList(db.Departamentos, "DepartamentoID", "Nombre", bodega.DepartamentoID);
            return View(bodega);
        }

        // POST: Bodegas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Bodega bodega)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bodega).State = EntityState.Modified;
                var respuesta = DbHelper.Guardar(db);
                if (respuesta.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, respuesta.Message);
            }
            ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(bodega.DepartamentoID), "CiudadID", "Nombre", bodega.CiudadID);
            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(), "DepartamentoID", "Nombre", bodega.DepartamentoID);
            return View(bodega);
        }

        // GET: Bodegas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bodega bodega = db.Bodegas.Find(id);
            if (bodega == null)
            {
                return HttpNotFound();
            }
            return View(bodega);
        }

        // POST: Bodegas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bodega bodega = db.Bodegas.Find(id);
            db.Bodegas.Remove(bodega);
            var respuesta = DbHelper.Guardar(db);
            if (respuesta.Succeeded)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, respuesta.Message);
            return View(bodega);
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
