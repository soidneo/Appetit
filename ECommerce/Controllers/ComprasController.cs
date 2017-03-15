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
    public class ComprasController : Controller
    {
        private ECommerceContext db = new ECommerceContext();

        // GET: Compras
        public ActionResult Index()
        {
            var compras = db.Compras.Include(c => c.Bodega).Include(c => c.Empresa).Include(c => c.Estado).Include(c => c.FormaPago).Include(c => c.Proveedor);
            return View(compras.ToList());
        }

        // GET: Compras/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Compra compra = db.Compras.Find(id);
            if (compra == null)
            {
                return HttpNotFound();
            }
            return View(compra);
        }

        // GET: Compras/Create
        public ActionResult Create()
        {
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.ClienteID = new SelectList(CombosHelper.GetClientes(user.EmpresaID), "ClienteID", "FullName");
            var compra = new NuevaCompraVista
            {
                Fecha = DateTime.Now,
                Detalles = db.CompraDetalleTmps.Where(v => v.UserName == User.Identity.Name).ToList(),
            };
            
            
            ViewBag.BodegaID = new SelectList(CombosHelper.GetBodegas(user.EmpresaID), "BodegaID", "Nombre");
            ViewBag.FormaPagoID = new SelectList(CombosHelper.GetFormaPagos(user.EmpresaID), "FormaPagoID", "Descripcion");
            ViewBag.ProveedorID = new SelectList(db.Proveedors, "ProveedorID", "UserName");
            return View(compra);
        }

        // POST: Compras/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NuevaCompraVista vista)
        {
            if (ModelState.IsValid)
            {
                var response = MovimientosHelper.NuevaCompra(vista, User.Identity.Name);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            vista.Detalles = db.CompraDetalleTmps.Where(v => v.UserName == User.Identity.Name).ToList();

            ViewBag.BodegaID = new SelectList(CombosHelper.GetBodegas(user.EmpresaID), "BodegaID", "Nombre", vista.BodegaID);
            ViewBag.FormaPagoID = new SelectList(CombosHelper.GetFormaPagos(user.EmpresaID), "FormaPagoID", "Descripcion", vista.FormaPagoID);
            ViewBag.ProveedorID = new SelectList(db.Proveedors, "ProveedorID", "UserName", vista.ProveedorID);
            return View(vista);
        }

        // GET: Compras/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Compra compra = db.Compras.Find(id);
            if (compra == null)
            {
                return HttpNotFound();
            }
            ViewBag.BodegaID = new SelectList(db.Bodegas, "BodegaID", "Nombre", compra.BodegaID);
            ViewBag.EmpresaID = new SelectList(db.Empresas, "EmpresaID", "Nombre", compra.EmpresaID);
            ViewBag.EstadoID = new SelectList(db.Estados, "EstadoID", "Descripcion", compra.EstadoID);
            ViewBag.FormaPagoID = new SelectList(db.FormaPagos, "FormaPagoID", "Descripcion", compra.FormaPagoID);
            ViewBag.ProveedorID = new SelectList(db.Proveedors, "ProveedorID", "UserName", compra.ProveedorID);
            return View(compra);
        }

        // POST: Compras/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CompraID,EmpresaID,ProveedorID,BodegaID,EstadoID,FormaPagoID,Fecha,Comentarios")] Compra compra)
        {
            if (ModelState.IsValid)
            {
                db.Entry(compra).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BodegaID = new SelectList(db.Bodegas, "BodegaID", "Nombre", compra.BodegaID);
            ViewBag.EmpresaID = new SelectList(db.Empresas, "EmpresaID", "Nombre", compra.EmpresaID);
            ViewBag.EstadoID = new SelectList(db.Estados, "EstadoID", "Descripcion", compra.EstadoID);
            ViewBag.FormaPagoID = new SelectList(db.FormaPagos, "FormaPagoID", "Descripcion", compra.FormaPagoID);
            ViewBag.ProveedorID = new SelectList(db.Proveedors, "ProveedorID", "UserName", compra.ProveedorID);
            return View(compra);
        }

        // GET: Compras/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Compra compra = db.Compras.Find(id);
            if (compra == null)
            {
                return HttpNotFound();
            }
            return View(compra);
        }

        // POST: Compras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Compra compra = db.Compras.Find(id);
            db.Compras.Remove(compra);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
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
                var compraDetallesTmp = db.CompraDetalleTmps.Where(
                    u => u.UserName == User.Identity.Name && u.ProductoID == vista.ProductoID).FirstOrDefault();
                if (compraDetallesTmp == null)
                {
                    compraDetallesTmp = new CompraDetalleTmp
                    {
                        Descripcion = producto.Descripcion,
                        Precio = producto.Precio,
                        ProductoID = producto.ProductoID,
                        Cantidad = vista.Cantidad,
                        Tasa = producto.Impuesto.Tasa,
                        UserName = User.Identity.Name,
                    };
                    db.CompraDetalleTmps.Add(compraDetallesTmp);
                }
                else
                {
                    compraDetallesTmp.Cantidad += vista.Cantidad;
                    db.Entry(compraDetallesTmp).State = EntityState.Modified;
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
