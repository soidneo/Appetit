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
    public class RecetasController : Controller
    {
        private ECommerceContext db = new ECommerceContext();

        // GET: Recetas
        public ActionResult Index()
        {
            var recetas = db.Recetas.Include(r => r.Empresa);
            return View(recetas.ToList());
        }

        // GET: Recetas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Receta receta = db.Recetas.Find(id);
            if (receta == null)
            {
                return HttpNotFound();
            }
            return View(receta);
        }

        // GET: Recetas/Create
        public ActionResult Create()
        {
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.ClienteID = new SelectList(CombosHelper.GetClientes(user.EmpresaID), "ClienteID", "FullName");
            var vista = new NuevaRecetaVista
            {
                Detalles = db.RecetaDetalleTmps.Where(v => v.UserName == User.Identity.Name).ToList(),
            };
            return View(vista);
        }

        // POST: Recetas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NuevaRecetaVista vista)
        {
            if (ModelState.IsValid)
            {
                var response = MovimientosHelper.NuevaReceta(vista, User.Identity.Name);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            vista.Detalles = db.RecetaDetalleTmps.Where(v => v.UserName == User.Identity.Name).ToList();
            return View(vista);
        }

        // GET: Recetas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Receta receta = db.Recetas.Find(id);
            if (receta == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmpresaID = new SelectList(db.Empresas, "EmpresaID", "Nombre", receta.EmpresaID);
            return View(receta);
        }

        // POST: Recetas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RecetaID,Descripcion,EmpresaID,Comentarios")] Receta receta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(receta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmpresaID = new SelectList(db.Empresas, "EmpresaID", "Nombre", receta.EmpresaID);
            return View(receta);
        }

        // GET: Recetas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Receta receta = db.Recetas.Find(id);
            if (receta == null)
            {
                return HttpNotFound();
            }
            return View(receta);
        }

        // POST: Recetas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Receta receta = db.Recetas.Find(id);
            db.Recetas.Remove(receta);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //*********************************************
        public ActionResult AddProducto()
        {
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.ProductoID = new SelectList(db.Productos.Where(p => p.EmpresaID == user.EmpresaID &&
            p.RecetaID == null), "ProductoID", "Descripcion");
            return PartialView();
        }

        [HttpPost]
        public ActionResult AddProducto(AddProductoVista vista)
        {
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (ModelState.IsValid)
            {
                var producto = db.Productos.Find(vista.ProductoID);
                var recetaDetalleTmps = db.RecetaDetalleTmps.Where( 
                    rd => rd.UserName == User.Identity.Name && rd.ProductoID == vista.ProductoID).FirstOrDefault();
                if (recetaDetalleTmps == null)
                {
                    recetaDetalleTmps = new RecetaDetalleTmp
                    {
                        Descripcion = producto.Descripcion,
                        Precio = producto.Precio,                       
                        ProductoID = producto.ProductoID,
                        Cantidad = vista.Cantidad,
                        UserName = User.Identity.Name,
                    };
                    db.RecetaDetalleTmps.Add(recetaDetalleTmps);                }
                else
                {
                    recetaDetalleTmps.Cantidad += vista.Cantidad;
                    db.Entry(recetaDetalleTmps).State = EntityState.Modified;
                }
                var respuesta = DbHelper.Guardar(db);
                if (respuesta.Succeeded == false)
                {
                    ModelState.AddModelError(string.Empty, respuesta.Message);
                    return RedirectToAction("Create");
                }
                return RedirectToAction("Create");
            }
            ViewBag.ProductoID = new SelectList(CombosHelper.getProductos(user.EmpresaID), "ProductoID", "Descripcion");
            return PartialView();
        }

        public ActionResult DelProducto(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var recetaDetalleTmps = db.RecetaDetalleTmps.Where(
                    u => u.UserName == User.Identity.Name && u.ProductoID == id).FirstOrDefault();
            if (recetaDetalleTmps == null)
            {
                return HttpNotFound();
            }
            db.RecetaDetalleTmps.Remove(recetaDetalleTmps);
            var respuesta = DbHelper.Guardar(db);
            if (respuesta.Succeeded == false)
            {
                ModelState.AddModelError(string.Empty, respuesta.Message);
                return RedirectToAction("Create");
            }

            return RedirectToAction("Create");
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
