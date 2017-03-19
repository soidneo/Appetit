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
using ECommerce.Clases;

namespace ECommerce.Controllers
{
    [Authorize(Roles = "VirtualWaiter  ")]
    public class PedidoMesasController : Controller
    {
        
        private ECommerceContext db = new ECommerceContext();

        public ActionResult AddProducto()
        {
            var mesa = db.Mesas.Where(u => u.Nombre == User.Identity.Name).FirstOrDefault();
            ViewBag.ProductoID = new SelectList(db.Productos.Where(p => p.EmpresaID == mesa.EmpresaID &&
            p.RecetaID != null || p.RecetaID == 1), "ProductoID", "Descripcion");
            return PartialView();
        }

        [HttpPost]
        public ActionResult AddProducto(AddProductoVista vista)
        {
            var mesa = db.Mesas.Where(u => u.Nombre == User.Identity.Name).FirstOrDefault();
            if (ModelState.IsValid)
            {
                var producto = db.Productos.Find(vista.ProductoID);
                var pedidoMesaDetallesTmp = db.PedidoMesaDetalleTmps.Where(
                    u => u.UserName == User.Identity.Name && u.ProductoID == vista.ProductoID).FirstOrDefault();
                if (pedidoMesaDetallesTmp == null)
                {
                    pedidoMesaDetallesTmp = new PedidoMesaDetalleTmp
                    {
                        Descripcion = producto.Descripcion,
                        Precio = producto.Precio,
                        ProductoID = producto.ProductoID,
                        Cantidad = vista.Cantidad,
                        Tasa = producto.Impuesto.Tasa,
                        UserName = User.Identity.Name,
                    };
                    db.PedidoMesaDetalleTmps.Add(pedidoMesaDetallesTmp);
                }
                else
                {
                    pedidoMesaDetallesTmp.Cantidad += vista.Cantidad;
                    db.Entry(pedidoMesaDetallesTmp).State = EntityState.Modified;
                }
                var respuesta = DbHelper.Guardar(db);
                if (respuesta.Succeeded == false)
                {
                    ModelState.AddModelError(string.Empty, respuesta.Message);
                    return RedirectToAction("Create");
                }
                return RedirectToAction("Create");
            }
            ViewBag.ProductoID = new SelectList(db.Productos.Where(p => p.EmpresaID == mesa.EmpresaID &&
            p.RecetaID == null), "ProductoID", "Descripcion");
            return PartialView();
        }

        public ActionResult DelProducto(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pedidoMesaDetallesTmp = db.PedidoMesaDetalleTmps.Where(
                    u => u.UserName == User.Identity.Name && u.ProductoID == id).FirstOrDefault();
            if (pedidoMesaDetallesTmp == null)
            {
                return HttpNotFound();
            }
            db.PedidoMesaDetalleTmps.Remove(pedidoMesaDetallesTmp);
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
            var mesa = db.Mesas.Where(u => u.Nombre == User.Identity.Name).FirstOrDefault();
            var pedidosMesa = db.PedidoMesas.Where(v => v.EmpresaID == mesa.EmpresaID)
                .Include(v => v.Mesa)
                .Include(v => v.Estado)
                .OrderBy(v => v.Fecha)
                .ThenBy(v => v.Estado);
            return View(pedidosMesa.ToPagedList((int)page, 5));
        }

        // GET: Ventas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PedidoMesa pedidoMesa = db.PedidoMesas.Find(id);
            if (pedidoMesa == null)
            {
                return HttpNotFound();
            }
            return View(pedidoMesa);
        }

        // GET: Ventas/Create
        public ActionResult Create()
        {
            var mesa = db.Mesas.Where(u => u.Nombre == User.Identity.Name).FirstOrDefault();

            var vista = new NuevoPedidoMesaVista
            {
                Fecha = DateTime.Now,
                Detalles = db.PedidoMesaDetalleTmps.Where(v => v.UserName == User.Identity.Name).ToList(),
                MesaID = mesa.MesaID,
            };
            return View(vista);
        }

        // POST: Ventas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NuevoPedidoMesaVista vista)
        {
            if (ModelState.IsValid)
            {
                var response = MovimientosHelper.NuevoPedidoMesa(vista, User.Identity.Name);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            vista.Detalles = db.PedidoMesaDetalleTmps.Where(v => v.UserName == User.Identity.Name).ToList();
            return View(vista);
        }

        // GET: PedidoMesas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PedidoMesa pedidoMesa = db.PedidoMesas.Find(id);
            if (pedidoMesa == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmpresaID = new SelectList(db.Empresas, "EmpresaID", "Nombre", pedidoMesa.EmpresaID);
            ViewBag.EstadoID = new SelectList(db.Estados, "EstadoID", "Descripcion", pedidoMesa.EstadoID);
            ViewBag.MesaID = new SelectList(db.Mesas, "MesaID", "Nombre", pedidoMesa.MesaID);
            return View(pedidoMesa);
        }

        // POST: PedidoMesas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PedidoMesaID,EmpresaID,MesaID,EstadoID,Fecha,Comentarios")] PedidoMesa pedidoMesa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pedidoMesa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmpresaID = new SelectList(db.Empresas, "EmpresaID", "Nombre", pedidoMesa.EmpresaID);
            ViewBag.EstadoID = new SelectList(db.Estados, "EstadoID", "Descripcion", pedidoMesa.EstadoID);
            ViewBag.MesaID = new SelectList(db.Mesas, "MesaID", "Nombre", pedidoMesa.MesaID);
            return View(pedidoMesa);
        }

        // GET: PedidoMesas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PedidoMesa pedidoMesa = db.PedidoMesas.Find(id);
            if (pedidoMesa == null)
            {
                return HttpNotFound();
            }
            return View(pedidoMesa);
        }

        // POST: PedidoMesas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PedidoMesa pedidoMesa = db.PedidoMesas.Find(id);
            db.PedidoMesas.Remove(pedidoMesa);
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
