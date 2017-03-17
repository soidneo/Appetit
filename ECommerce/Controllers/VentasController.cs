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
    [Authorize(Roles ="User")]
    public class VentasController : Controller
    {
        private ECommerceContext db = new ECommerceContext();

        public ActionResult AddProducto()
        {
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.ProductoID = new SelectList(db.Productos.Where(p => p.EmpresaID == user.EmpresaID &&
            p.RecetaID != null), "ProductoID", "Descripcion");
            return PartialView();
        }

        [HttpPost]
        public ActionResult AddProducto(AddProductoVista vista)
        {
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (ModelState.IsValid)
            {
                var producto = db.Productos.Find(vista.ProductoID);
                var ventaDetallesTmp = db.VentaDetalleTmps.Where(
                    u => u.UserName == User.Identity.Name && u.ProductoID == vista.ProductoID).FirstOrDefault();
                if (ventaDetallesTmp == null)
                {
                    ventaDetallesTmp = new VentaDetalleTmp
                    {
                        Descripcion = producto.Descripcion,
                        Precio = producto.Precio,
                        ProductoID = producto.ProductoID,
                        Cantidad = vista.Cantidad,
                        Tasa = producto.Impuesto.Tasa,
                        UserName = User.Identity.Name,
                    };
                    db.VentaDetalleTmps.Add(ventaDetallesTmp);
                }
                else
                {
                    ventaDetallesTmp.Cantidad += vista.Cantidad;
                    db.Entry(ventaDetallesTmp).State = EntityState.Modified;
                }
                var respuesta = DbHelper.Guardar(db);
                if (respuesta.Succeeded == false)
                {
                    ModelState.AddModelError(string.Empty, respuesta.Message);
                    return RedirectToAction("Create");
                }
                return RedirectToAction("Create");
            }
            ViewBag.ProductoID = new SelectList(db.Productos.Where(p => p.EmpresaID == user.EmpresaID &&
            p.RecetaID == null), "ProductoID", "Descripcion");
            return PartialView();
        }

        public ActionResult DelProducto(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ventaDetallesTmp = db.VentaDetalleTmps.Where(
                    u => u.UserName == User.Identity.Name && u.ProductoID == id).FirstOrDefault();
            if (ventaDetallesTmp == null)
            {
                return HttpNotFound();
            }
            db.VentaDetalleTmps.Remove(ventaDetallesTmp);
            var respuesta = DbHelper.Guardar(db);
            if (respuesta.Succeeded == false)
            {
                ModelState.AddModelError(string.Empty, respuesta.Message);
                return RedirectToAction("Create");
            }

            return RedirectToAction("Create");
        }
        // GET: Ventas
        public ActionResult Index(int? page = null)
        {
            page = (page ?? 1);
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var ventas = db.Ventas.Where(v => v.EmpresaID == user.EmpresaID)
                .Include(v => v.Cliente)
                .Include(v => v.Estado)
                .OrderBy(v => v.Fecha)
                .ThenBy(v => v.Estado);
            return View(ventas.ToPagedList((int)page,5));
        }

        // GET: Ventas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Venta venta = db.Ventas.Find(id);
            if (venta == null)
            {
                return HttpNotFound();
            }
            return View(venta);
        }

        // GET: Ventas/Create
        public ActionResult Create()
        {
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.ClienteID = new SelectList(CombosHelper.GetClientes(user.EmpresaID), "ClienteID", "FullName");
            var vista = new NuevaOrdenVista
            {
                Fecha = DateTime.Now,
                Detalles = db.VentaDetalleTmps.Where(v => v.UserName == User.Identity.Name).ToList(),
            };
            return View(vista);
        }

        // POST: Ventas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NuevaOrdenVista vista)
        {
            if (ModelState.IsValid)
            {
                var response = MovimientosHelper.NuevaVenta(vista, User.Identity.Name);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            vista.Detalles = db.VentaDetalleTmps.Where(v => v.UserName == User.Identity.Name).ToList();
            ViewBag.ClienteID = new SelectList(CombosHelper.GetClientes(user.EmpresaID), "ClienteID", "fullName");

            return View(vista);
        }

        // GET: Ventas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Venta venta = db.Ventas.Find(id);
            if (venta == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClienteID = new SelectList(db.Clientes, "ClienteID", "UserName", venta.ClienteID);
            ViewBag.EmpresaID = new SelectList(db.Empresas, "EmpresaID", "Nombre", venta.EmpresaID);
            ViewBag.EstadoID = new SelectList(db.Estados, "EstadoID", "Descripcion", venta.EstadoID);
            return View(venta);
        }

        // POST: Ventas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Venta venta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(venta).State = EntityState.Modified;
                var respuesta = DbHelper.Guardar(db);
                if (respuesta.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, respuesta.Message);
            }
            ViewBag.ClienteID = new SelectList(db.Clientes, "ClienteID", "UserName", venta.ClienteID);
            ViewBag.EmpresaID = new SelectList(db.Empresas, "EmpresaID", "Nombre", venta.EmpresaID);
            ViewBag.EstadoID = new SelectList(db.Estados, "EstadoID", "Descripcion", venta.EstadoID);
            return View(venta);
        }

        // GET: Ventas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Venta venta = db.Ventas.Find(id);
            if (venta == null)
            {
                return HttpNotFound();
            }
            return View(venta);
        }

        // POST: Ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var venta = db.Ventas.Find(id);
            db.Ventas.Remove(venta);
            var respuesta = DbHelper.Guardar(db);
            if (respuesta.Succeeded)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, respuesta.Message);
            return View(venta);
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
