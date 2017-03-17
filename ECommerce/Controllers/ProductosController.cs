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
using PagedList;

namespace ECommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductosController : Controller
    {
        private ECommerceContext db = new ECommerceContext();

        // GET: Productos
        public ActionResult Index(int? page = null)
        {
            page = (page ?? 1);
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var productos = db.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Impuesto)
                .Where(p => p.EmpresaID == user.EmpresaID)
                .OrderBy(p => p.Categoria.Descripcion)
                .ThenBy(p => p.Descripcion);
            return View(productos.ToPagedList((int)page, 4));
        }

        // GET: Productos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productos.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // GET: Productos/Create
        public ActionResult Create()
        {
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.CategoriaID = new SelectList(CombosHelper.GetCategorias(user.EmpresaID), "CategoriaID", "Descripcion");
            ViewBag.ImpuestoID = new SelectList(CombosHelper.GetImpuestos(user.EmpresaID), "ImpuestoID", "Descripcion");
            ViewBag.UnidadID = new SelectList(CombosHelper.GetUnidades(user.EmpresaID), "UnidadID", "Descripcion");
            ViewBag.RecetaID = new SelectList(CombosHelper.GetRecetas(user.EmpresaID), "RecetaID", "Descripcion");
            var producto = new Producto
            { EmpresaID = user.EmpresaID, };
            return PartialView(producto);
        }

        // POST: Productos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                if(producto.RecetaID == 0) producto.RecetaID = null;
                db.Productos.Add(producto);
                var respuesta = DbHelper.Guardar(db);
                if (respuesta.Succeeded == false)
                {
                    ModelState.AddModelError(string.Empty, respuesta.Message);
                    var user2 = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                    ViewBag.UnidadID = new SelectList(CombosHelper.GetUnidades(user2.EmpresaID), "UnidadID", "Descripcion",producto.UnidadID);
                    ViewBag.RecetaID = new SelectList(CombosHelper.GetRecetas(user2.EmpresaID), "RecetaID", "Descripcion",producto.RecetaID);
                    ViewBag.CategoriaID = new SelectList(CombosHelper.GetCategorias(user2.EmpresaID), "CategoriaID", "Descripcion", producto.CategoriaID);
                    ViewBag.ImpuestoID = new SelectList(CombosHelper.GetImpuestos(user2.EmpresaID), "ImpuestoID", "Descripcion", producto.ImpuestoID);
                    return View(producto);
                }
                if (producto.ImageFile != null)
                {
                    var folder = "~/Content/Images";
                    var fileName = string.Format("{0}.jpg", producto.ProductoID);

                    if (FilesHelper.SubirImagen(producto.ImageFile, folder,
                        fileName))
                    {

                        var pic = string.Format("{0}/{1}", folder, fileName);
                        producto.Image = pic;
                        db.Entry(producto).State = EntityState.Modified;
                        respuesta = DbHelper.Guardar(db);
                        if (respuesta.Succeeded == false)
                        {
                            ModelState.AddModelError(string.Empty, respuesta.Message);
                            var user2 = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                            ViewBag.UnidadID = new SelectList(CombosHelper.GetUnidades(user2.EmpresaID), "UnidadID", "Descripcion",producto.UnidadID);
                            ViewBag.RecetaID = new SelectList(CombosHelper.GetRecetas(user2.EmpresaID), "RecetaID", "Descripcion",producto.RecetaID);
                            ViewBag.CategoriaID = new SelectList(CombosHelper.GetCategorias(user2.EmpresaID), "CategoriaID", "Descripcion", producto.CategoriaID);
                            ViewBag.ImpuestoID = new SelectList(CombosHelper.GetImpuestos(user2.EmpresaID), "ImpuestoID", "Descripcion", producto.ImpuestoID);
                            return View(producto);
                        }

                    }

                }
                return RedirectToAction("Index");
            }

            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.UnidadID = new SelectList(CombosHelper.GetUnidades(user.EmpresaID), "UnidadID", "Descripcion",producto.UnidadID);
            ViewBag.RecetaID = new SelectList(CombosHelper.GetRecetas(user.EmpresaID), "RecetaID", "Descripcion",producto.RecetaID);
            ViewBag.CategoriaID = new SelectList(CombosHelper.GetCategorias(user.EmpresaID), "CategoriaID", "Descripcion", producto.CategoriaID);
            ViewBag.ImpuestoID = new SelectList(CombosHelper.GetImpuestos(user.EmpresaID), "ImpuestoID", "Descripcion", producto.ImpuestoID);
            
            return PartialView(producto);
        }

        // GET: Productos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var producto = db.Productos.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }

            ViewBag.UnidadID = new SelectList(CombosHelper.GetUnidades(producto.EmpresaID), "UnidadID", "Descripcion", producto.UnidadID);
            ViewBag.RecetaID = new SelectList(CombosHelper.GetRecetas(producto.EmpresaID), "RecetaID", "Descripcion", producto.RecetaID);
            ViewBag.CategoriaID = new SelectList(CombosHelper.GetCategorias(producto.EmpresaID), "CategoriaID", "Descripcion", producto.CategoriaID);
            ViewBag.ImpuestoID = new SelectList(CombosHelper.GetImpuestos(producto.EmpresaID), "ImpuestoID", "Descripcion", producto.ImpuestoID);
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Producto producto)
        {
            var respuesta = new Respuesta();
            if (ModelState.IsValid)
            {
                if (producto.ImageFile != null)
                {
                    var folder = "~/Content/Images";
                    var fileName = string.Format("{0}.jpg", producto.ProductoID);
                    if (FilesHelper.SubirImagen(producto.ImageFile, folder, fileName))
                    {
                        producto.Image = string.Format("{0}/{1}", folder, fileName);
                    }

                }
                if (producto.RecetaID == 0) producto.RecetaID = null;
                db.Entry(producto).State = EntityState.Modified;
                respuesta = DbHelper.Guardar(db);
                if (respuesta.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            ModelState.AddModelError(string.Empty, respuesta.Message);

            ViewBag.UnidadID = new SelectList(CombosHelper.GetUnidades(producto.EmpresaID), "UnidadID", "Descripcion", producto.UnidadID);
            ViewBag.RecetaID = new SelectList(CombosHelper.GetRecetas(producto.EmpresaID), "RecetaID", "Descripcion", producto.RecetaID);
            ViewBag.CategoriaID = new SelectList(CombosHelper.GetCategorias(producto.EmpresaID), "CategoriaID", "Descripcion", producto.CategoriaID);
            ViewBag.ImpuestoID = new SelectList(CombosHelper.GetImpuestos(producto.EmpresaID), "ImpuestoID", "Descripcion", producto.ImpuestoID);
            return View(producto);
        }

        // GET: Productos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productos.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Producto producto = db.Productos.Find(id);
            db.Productos.Remove(producto);
            var respuesta = DbHelper.Guardar(db);
            if (respuesta.Succeeded == false)
            {
                ModelState.AddModelError(string.Empty, respuesta.Message);
                return View(producto);
            }

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
