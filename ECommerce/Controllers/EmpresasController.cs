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
    public class EmpresasController : Controller
    {
        private ECommerceContext db = new ECommerceContext();

        // GET: Empresas
        public ActionResult Index()
        {
            var empresas = db.Empresas.Include(e => e.Ciudad).Include(e => e.Departamento);
            return View(empresas.ToList());
        }

        // GET: Empresas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresas.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            return View(empresa);
        }

        // GET: Empresas/Create
        public ActionResult Create()
        {
            ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(0), "CiudadID", "Nombre");
            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(),
                "DepartamentoID", "Nombre");
            return View();
        }

        // POST: Empresas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                db.Empresas.Add(empresa);
                var respuesta = DbHelper.Guardar(db);
                if (respuesta.Succeeded == false)
                {
                    ModelState.AddModelError(string.Empty, respuesta.Message);
                    ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(0), "CiudadID", "Nombre");
                    ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(),
                        "DepartamentoID", "Nombre");
                    return View(empresa);
                }
                if (empresa.LogoFile != null)
                {
                    var folder = "~/Content/Logos";
                    var fileName = string.Format("{0}.jpg", empresa.EmpresaID);

                    if (FilesHelper.SubirImagen(empresa.LogoFile, folder,
                        fileName))
                    {
                        
                        var pic = string.Format("{0}/{1}", folder, fileName);
                        empresa.Logo = pic;
                        db.Entry(empresa).State = EntityState.Modified;
                        respuesta = DbHelper.Guardar(db);
                        if (respuesta.Succeeded == false)
                        {
                            ModelState.AddModelError(string.Empty, respuesta.Message);
                            ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(0), "CiudadID", "Nombre");
                            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(),
                                "DepartamentoID", "Nombre");
                            return View(empresa);
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(0), "CiudadID", "Nombre");
            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(),
                "DepartamentoID", "Nombre");
            return View(empresa);
        }

        // GET: Empresas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresas.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(empresa.DepartamentoID), "CiudadID", "Nombre",empresa.CiudadID);
            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(),
                "DepartamentoID", "Nombre",empresa.DepartamentoID);
            return View(empresa);
        }

        // POST: Empresas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                if (empresa.LogoFile != null)
                {
                    var folder = "~/Content/Logos";
                    var fileName = string.Format("{0}.jpg", empresa.EmpresaID);
                    if (FilesHelper.SubirImagen(empresa.LogoFile, folder, fileName))
                    {
                        empresa.Logo = string.Format("{0}/{1}", folder, fileName);
                    }
                    
                }
                db.Entry(empresa).State = EntityState.Modified;
                var respuesta = DbHelper.Guardar(db);
                if (respuesta.Succeeded == false)
                {
                    ModelState.AddModelError(string.Empty, respuesta.Message);
                    ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(0), "CiudadID", "Nombre");
                    ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(),
                        "DepartamentoID", "Nombre");
                    return View(empresa);
                }
                return RedirectToAction("Index");
            }
            ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(empresa.DepartamentoID), "CiudadID", "Nombre");
            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(),
                "DepartamentoID", "Nombre");
            return View(empresa);
        }

        // GET: Empresas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresas.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            return View(empresa);
        }

        // POST: Empresas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Empresa empresa = db.Empresas.Find(id);
            db.Empresas.Remove(empresa);
            var respuesta = DbHelper.Guardar(db);
            if (respuesta.Succeeded)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, respuesta.Message);
            return View(empresa);
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
