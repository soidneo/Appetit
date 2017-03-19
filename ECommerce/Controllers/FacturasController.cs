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
    public class FacturasController : Controller
    {
        private ECommerceContext db = new ECommerceContext();
        public ActionResult AddProducto()
        {
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.ProductoID = new SelectList(db.Productos.Where(p => p.EmpresaID == user.EmpresaID &&
            p.RecetaID != null || p.EmpresaID == user.EmpresaID && p.RecetaID == 1), "ProductoID", "Descripcion");
            return PartialView();
        }
        [HttpPost]
        public ActionResult AddProducto(AddProductoVista vista)
        {
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (ModelState.IsValid)
            {
                var producto = db.Productos.Find(vista.ProductoID);
                var facturaDetallesTmp = db.FacturaDetalleTmps.Where(
                    u => u.UserName == User.Identity.Name && u.ProductoID == vista.ProductoID).FirstOrDefault();
                if (facturaDetallesTmp == null)
                {
                    facturaDetallesTmp = new FacturaDetalleTmp
                    {
                        Descripcion = producto.Descripcion,
                        Precio = producto.Precio,
                        ProductoID = producto.ProductoID,
                        Cantidad = vista.Cantidad,
                        Tasa = producto.Impuesto.Tasa,
                        UserName = User.Identity.Name,
                    };
                    db.FacturaDetalleTmps.Add(facturaDetallesTmp);
                }
                else
                {
                    facturaDetallesTmp.Cantidad += vista.Cantidad;
                    db.Entry(facturaDetallesTmp).State = EntityState.Modified;
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
            p.RecetaID != null), "ProductoID", "Descripcion");
            return PartialView();
        }
        public ActionResult DelProducto(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var facturaDetallesTmp = db.FacturaDetalleTmps.Where(
                    u => u.UserName == User.Identity.Name && u.ProductoID == id).FirstOrDefault();
            if (facturaDetallesTmp == null)
            {
                return HttpNotFound();
            }
            db.FacturaDetalleTmps.Remove(facturaDetallesTmp);
            var respuesta = DbHelper.Guardar(db);
            if (respuesta.Succeeded == false)
            {
                ModelState.AddModelError(string.Empty, respuesta.Message);
                return RedirectToAction("Create");
            }

            return RedirectToAction("Create");
        }

        // GET: Facturas
        public ActionResult Index()
        {
            var facturas = db.Facturas.Include(f => f.Bodega).Include(f => f.Cliente).Include(f => f.Empresa).Include(f => f.Estado).Include(f => f.FormaPago).Include(f => f.Venta);
            return View(facturas.ToList());
        }

        // GET: Facturas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Factura factura = db.Facturas.Find(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            return View(factura);
        }

        // GET: Facturas/Create
        public ActionResult Create()
        {
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var factura = new NuevaFacturaVista
            {
                Fecha = DateTime.Now,
                Detalles = db.FacturaDetalleTmps.Where(v => v.UserName == User.Identity.Name).ToList(),
            };

            ViewBag.ClienteID = new SelectList(CombosHelper.GetClientes(user.EmpresaID), "ClienteID", "FullName");
            ViewBag.BodegaID = new SelectList(CombosHelper.GetBodegas(user.EmpresaID), "BodegaID", "Nombre");
            ViewBag.FormaPagoID = new SelectList(CombosHelper.GetFormaPagos(user.EmpresaID), "FormaPagoID", "Descripcion");
            return View(factura);
        }

        // POST: Facturas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NuevaFacturaVista vista)
        {
            if (ModelState.IsValid)
            {
                var response = MovimientosHelper.NuevaFactura(vista, User.Identity.Name);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            vista.Detalles = db.FacturaDetalleTmps.Where(v => v.UserName == User.Identity.Name).ToList();

            ViewBag.BodegaID = new SelectList(db.Bodegas, "BodegaID", "Nombre", vista.BodegaID);
            ViewBag.ClienteID = new SelectList(db.Clientes, "ClienteID", "UserName", vista.ClienteID);            
            ViewBag.FormaPagoID = new SelectList(db.FormaPagos, "FormaPagoID", "Descripcion", vista.FormaPagoID);
            return View(vista);
        }

        // GET: Facturas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Factura factura = db.Facturas.Find(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            ViewBag.BodegaID = new SelectList(db.Bodegas, "BodegaID", "Nombre", factura.BodegaID);
            ViewBag.ClienteID = new SelectList(db.Clientes, "ClienteID", "UserName", factura.ClienteID);
            ViewBag.EmpresaID = new SelectList(db.Empresas, "EmpresaID", "Nombre", factura.EmpresaID);
            ViewBag.EstadoID = new SelectList(db.Estados, "EstadoID", "Descripcion", factura.EstadoID);
            ViewBag.FormaPagoID = new SelectList(db.FormaPagos, "FormaPagoID", "Descripcion", factura.FormaPagoID);
            ViewBag.VentaID = new SelectList(db.Ventas, "VentaID", "Comentarios", factura.VentaID);
            return View(factura);
        }

        // POST: Facturas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FacturaID,EmpresaID,ClienteID,BodegaID,EstadoID,FormaPagoID,VentaID,Fecha,Comentarios")] Factura factura)
        {
            if (ModelState.IsValid)
            {
                db.Entry(factura).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BodegaID = new SelectList(db.Bodegas, "BodegaID", "Nombre", factura.BodegaID);
            ViewBag.ClienteID = new SelectList(db.Clientes, "ClienteID", "UserName", factura.ClienteID);
            ViewBag.EmpresaID = new SelectList(db.Empresas, "EmpresaID", "Nombre", factura.EmpresaID);
            ViewBag.EstadoID = new SelectList(db.Estados, "EstadoID", "Descripcion", factura.EstadoID);
            ViewBag.FormaPagoID = new SelectList(db.FormaPagos, "FormaPagoID", "Descripcion", factura.FormaPagoID);
            ViewBag.VentaID = new SelectList(db.Ventas, "VentaID", "Comentarios", factura.VentaID);
            return View(factura);
        }

        // GET: Facturas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Factura factura = db.Facturas.Find(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            return View(factura);
        }

        // POST: Facturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Factura factura = db.Facturas.Find(id);
            db.Facturas.Remove(factura);
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
