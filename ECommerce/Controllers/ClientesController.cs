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
    public class ClientesController : Controller
    {
        private ECommerceContext db = new ECommerceContext();

        // GET: Clientes
        public ActionResult Index()
        {
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var clientes = new List<Cliente>();
            if (user != null)
            {
                var qry = (from cl in db.Clientes
                           join ec in db.EmpresaClientes on cl.ClienteID equals ec.ClienteID
                           join em in db.Empresas on ec.EmpresaID equals em.EmpresaID
                           where em.EmpresaID == user.EmpresaID
                           select new { cl }).ToList();
                
                foreach (var item in qry)
                {
                    clientes.Add(item.cl);
                }
            }
            return View(clientes);
        }

        // GET: Clientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // GET: Clientes/Create
        public ActionResult Create()
        {
            ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(0), "CiudadID", "Nombre");
            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(), "DepartamentoID", "Nombre");
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Clientes.Add(cliente);
                        var respuesta = DbHelper.Guardar(db);
                        if (!respuesta.Succeeded)
                        {
                            ModelState.AddModelError(string.Empty, respuesta.Message);
                            ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(0), "CiudadID", "Nombre", cliente.CiudadID);
                            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(), "DepartamentoID", "Nombre", cliente.DepartamentoID);
                            return View(cliente);

                        }
                        UsuariosHelper.CreateUserAsp(cliente.UserName, "Cliente");
                        var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                        var empresaCliente = new EmpresaCliente
                        {
                            EmpresaID = user.EmpresaID,
                            ClienteID = cliente.ClienteID,
                        };
                        db.EmpresaClientes.Add(empresaCliente);
                        db.SaveChanges();
                        transaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
            }

            ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(0), "CiudadID", "Nombre", cliente.CiudadID);
            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(), "DepartamentoID", "Nombre", cliente.DepartamentoID);
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(cliente.DepartamentoID), "CiudadID", "Nombre", cliente.CiudadID);
            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(), "DepartamentoID", "Nombre", cliente.DepartamentoID);
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cliente).State = EntityState.Modified;
                var respuesta = DbHelper.Guardar(db);
                if (respuesta.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, respuesta.Message);
            }
            ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(cliente.DepartamentoID), "CiudadID", "Nombre", cliente.CiudadID);
            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(), "DepartamentoID", "Nombre", cliente.DepartamentoID);
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var cliente = db.Clientes.Find(id);
            var user = db.Usuarios.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var empresaCliente = db.EmpresaClientes.Where(ec => ec.EmpresaID == user.EmpresaID &&
            ec.ClienteID == cliente.ClienteID).FirstOrDefault();
            db.EmpresaClientes.Remove(empresaCliente);
            using (var transaction = db.Database.BeginTransaction())
            {
                var respuesta = DbHelper.Guardar(db);
                if (respuesta.Succeeded)
                {
                    transaction.Commit();
                    return RedirectToAction("Index");
                }
                transaction.Rollback();
                ModelState.AddModelError(string.Empty, respuesta.Message);
                return View(cliente);
            }
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
