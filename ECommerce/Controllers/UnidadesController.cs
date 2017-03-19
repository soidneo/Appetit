using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ECommerce.Models;
using PagedList;
using System.Threading.Tasks;

namespace ECommerce.Controllers
{
    public class UnidadesController : Controller
    {
        private ECommerceContext db = new ECommerceContext();

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var unidades = from s in db.Unidades
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                unidades = unidades.Include(u => u.Empresa).Where(u => u.Descripcion.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    unidades = unidades.OrderByDescending(u => u.Descripcion);
                    break;
                default:  // Name ascending 
                    unidades = unidades.OrderBy(u => u.Descripcion);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(unidades.ToPagedList(pageNumber, pageSize));
        }
        /*
        // GET: Unidades
        public ActionResult Index(int? page = null)
        {
            page = (page ?? 1);
            var Unidades = db.Unidades.Include(u => u.Empresa).OrderBy(u => u.Descripcion);
            return View(Unidades.ToPagedList((int)page, 5));
        }

        // GET: Unidades/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unidad unidad = db.Unidades.Find(id);
            if (unidad == null)
            {
                return HttpNotFound();
            }
            return View(unidad);
        }
        */
        // GET: Unidades/Create
        public ActionResult Create()
        {
            ViewBag.EmpresaID = new SelectList(db.Empresas, "EmpresaID", "Nombre");
            return View();
        }

        // POST: Unidades/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UnidadID,Descripcion,EmpresaID")] Unidad unidad)
        {
            if (ModelState.IsValid)
            {
                db.Unidades.Add(unidad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmpresaID = new SelectList(db.Empresas, "EmpresaID", "Nombre", unidad.EmpresaID);
            return View(unidad);
        }

        // GET: Unidades/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unidad unidad = db.Unidades.Find(id);
            if (unidad == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmpresaID = new SelectList(db.Empresas, "EmpresaID", "Nombre", unidad.EmpresaID);
            return View(unidad);
        }

        // POST: Unidades/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UnidadID,Descripcion,EmpresaID")] Unidad unidad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(unidad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmpresaID = new SelectList(db.Empresas, "EmpresaID", "Nombre", unidad.EmpresaID);
            return View(unidad);
        }

        // GET: Unidades/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unidad unidad = db.Unidades.Find(id);
            if (unidad == null)
            {
                return HttpNotFound();
            }
            return View(unidad);
        }

        // POST: Unidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Unidad unidad = db.Unidades.Find(id);
            db.Unidades.Remove(unidad);
            db.SaveChanges();
            return RedirectToAction("Index");
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
