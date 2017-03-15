using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ECommerce.Clases
{
    public class CombosHelper : IDisposable
    {
        private static ECommerceContext db = new ECommerceContext();

        public static List<Departamento> GetDepartamentos()
        {
            var departamentos = db.Departamentos.ToList();
            departamentos.Add(new Departamento
            {
                DepartamentoID = 0,
                Nombre = "[Seleccione un departamento...]"
            });
            return departamentos.OrderBy(d => d.Nombre).ToList();
        }
        public static List<Ciudad> GetCiudades(int id)
        {
            var ciudades = db.Ciudades.Where(c => c.DepartamentoID == id).ToList();
            ciudades.Add(new Ciudad
            {
                CiudadID = 0,
                Nombre = "[Seleccione una ciudad...]"
            });
            return ciudades.OrderBy(c => c.Nombre).ToList();
        }

        public static List<Cliente> GetClientes(int EmpresaID)
        {
            var qry = (from cl in db.Clientes
                       join ec in db.EmpresaClientes on cl.ClienteID equals ec.ClienteID
                       join em in db.Empresas on ec.EmpresaID equals em.EmpresaID
                       where em.EmpresaID == EmpresaID
                       select new { cl }).ToList();
            var clientes = new List<Cliente>();
            foreach (var item in qry)
            {
                clientes.Add(item.cl);
            }

            clientes.Add(new Cliente
            {
                ClienteID = 0,
                Nombre = "[Seleccione un cliente...]"
            });
            return clientes.OrderBy(c => c.Nombre).ThenBy(d => d.Apellido).ToList();
        }

        public static List<FormaPago> GetFormaPagos(int EmpresaID)
        {
            var formaPagos = db.FormaPagos.Where(fp => fp.EmpresaID == EmpresaID).ToList();
            formaPagos.Add(new FormaPago 
            {
                FormaPagoID = 0,
                Descripcion = "[Seleccione una Forma de Pago...]"
            });
            return formaPagos.OrderBy(i => i.Descripcion).ToList();
        }

        public static List<Bodega> GetBodegas(int EmpresaID)
        {
            var bodegas = db.Bodegas.Where(fp => fp.EmpresaID == EmpresaID).ToList();
            bodegas.Add(new Bodega
            {
                BodegaID = 0,
                Nombre = "[Seleccione una Forma de Pago...]"
            });
            return bodegas.OrderBy(i => i.Nombre).ToList();
        }

        

        public static List<Empresa> GetEmpresas()
        {
            var empresas = db.Empresas.ToList();
            empresas.Add(new Empresa
            {
                EmpresaID = 0,
                Nombre = "[Seleccione una empresa...]"
            });
            return empresas.OrderBy(e => e.Nombre).ToList();
        }
        public static List<Categoria> GetCategorias(int EmpresaID)
        {
            var categorias = db.Categorias.Where(c => c.EmpresaID == EmpresaID).ToList();
            categorias.Add(new Categoria
            {
                CategoriaID = 0,
                Descripcion = "[Seleccione una categoria...]"
            });
            return categorias.OrderBy(c => c.Descripcion).ToList();
        }

        public static List<Impuesto> GetImpuestos(int EmpresaID)
        {
            var impuestos = db.Impuestos.Where(i => i.EmpresaID == EmpresaID).ToList();
            impuestos.Add(new Impuesto
            {
                ImpuestoID = 0,
                Descripcion = "[Seleccione un impuesto...]"
            });
            return impuestos.OrderBy(i => i.Descripcion).ToList();
        }
        public static List<Unidad> GetUnidades(int EmpresaID)
        {
            var unidades = db.Unidades.Where(u => u.EmpresaID == EmpresaID).ToList();
            unidades.Add(new Unidad
            {
                UnidadID = 0,
                Descripcion = "[Seleccione una unidad...]"
            });
            return unidades.OrderBy(u => u.Descripcion).ToList();
        }

        public static List<Receta> GetRecetas(int EmpresaID)
        {
            var recetas = db.Recetas.Where(r => r.EmpresaID == EmpresaID).ToList();
            recetas.Add(new Receta
            {
                RecetaID = 0,
                Descripcion = "[Seleccione una receta...]"
            });
            return recetas.OrderBy(r => r.Descripcion).ToList();
        }

        public static List<Producto> getProductos(int empresaID)
        {
            var productos = db.Productos.Where(i => i.EmpresaID == empresaID).ToList();
            productos.Add(new Producto
            {
                ProductoID = 0,
                Descripcion = "[Seleccione un producto...]"
            });
            return productos.OrderBy(i => i.Descripcion).ToList();
        }
        public static List<Producto> getProductos(int empresaID, bool sw)
        {
            var productos = db.Productos.Where(p => p.EmpresaID == empresaID).ToList();
            return productos.OrderBy(p => p.Descripcion).ToList();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
