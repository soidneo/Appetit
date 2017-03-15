using System;
using System.Collections;
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
    public class UsuariosController : Controller
    {
        private ECommerceContext db = new ECommerceContext();

        // GET: Usuarios
        public ActionResult Index()
        {
            var usuarios = db.Usuarios.Include(u => u.Ciudad).Include(u => u.Departamento).Include(u => u.Empresa);
            return View(usuarios.ToList());
        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(0), "CiudadID", "Nombre");
            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(), "DepartamentoID", "Nombre");
            ViewBag.EmpresaID = new SelectList(CombosHelper.GetEmpresas(), "EmpresaID", "Nombre");
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Usuarios.Add(usuario);
                var respuesta = DbHelper.Guardar(db);
                if (respuesta.Succeeded)
                {
                    UsuariosHelper.CreateUserAsp(usuario.UserName, "Admin");
                    if (usuario.PhotoFile != null)
                    {
                        var folder = "~/Content/Photos";
                        var fileName = string.Format("{0}.jpg", usuario.UsuarioID);

                        if (FilesHelper.SubirImagen(usuario.PhotoFile, folder,
                            fileName))
                        {
                            var pic = string.Format("{0}/{1}", folder, fileName);
                            usuario.Photo = pic;
                            db.Entry(usuario).State = EntityState.Modified;
                            respuesta = DbHelper.Guardar(db);
                            if (respuesta.Succeeded == false)
                            {
                                ModelState.AddModelError(string.Empty, respuesta.Message);
                                ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(0), "CiudadID", "Nombre", usuario.CiudadID);
                                ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(), "DepartamentoID", "Nombre", usuario.DepartamentoID);
                                ViewBag.EmpresaID = new SelectList(CombosHelper.GetEmpresas(), "EmpresaID", "Nombre", usuario.EmpresaID);
                                return View(usuario);
                            }
                        }
                    }
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, respuesta.Message);
            }
            ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(0), "CiudadID", "Nombre", usuario.CiudadID);
            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(), "DepartamentoID", "Nombre", usuario.DepartamentoID);
            ViewBag.EmpresaID = new SelectList(CombosHelper.GetEmpresas(), "EmpresaID", "Nombre", usuario.EmpresaID);
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(usuario.DepartamentoID), "CiudadID", "Nombre", usuario.CiudadID);
            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(), "DepartamentoID", "Nombre", usuario.DepartamentoID);
            ViewBag.EmpresaID = new SelectList(CombosHelper.GetEmpresas(), "EmpresaID", "Nombre", usuario.EmpresaID);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (usuario.PhotoFile != null)
                {
                    var folder = "~/Content/Photos";
                    var fileName = string.Format("{0}.jpg", usuario.UsuarioID);
                    if (FilesHelper.SubirImagen(usuario.PhotoFile, folder, fileName))
                    {
                        usuario.Photo = string.Format("{0}/{1}", folder, fileName);
                    }
                }
                var db2 = new ECommerceContext();
                var currentUser = db2.Usuarios.Find(usuario.UsuarioID);
                if (currentUser.UserName != usuario.UserName)
                {
                    UsuariosHelper.UpdateUserName(currentUser.UserName, usuario.UserName);
                }
                db.Entry(usuario).State = EntityState.Modified;
                var respuesta = DbHelper.Guardar(db);
                if (respuesta.Succeeded)
                {
                    db2.Dispose();
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, respuesta.Message);
            }
            ViewBag.CiudadID = new SelectList(CombosHelper.GetCiudades(usuario.DepartamentoID), "CiudadID", "Nombre", usuario.CiudadID);
            ViewBag.DepartamentoID = new SelectList(CombosHelper.GetDepartamentos(), "DepartamentoID", "Nombre", usuario.DepartamentoID);
            ViewBag.EmpresaID = new SelectList(CombosHelper.GetEmpresas(), "EmpresaID", "Nombre", usuario.EmpresaID);
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var usuario = db.Usuarios.Find(id);
            db.Usuarios.Remove(usuario);
            var respuesta = DbHelper.Guardar(db);
            if (respuesta.Succeeded)
            {
                UsuariosHelper.DeleteUser(usuario.UserName,"User");
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, respuesta.Message);
            return View(usuario);
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
