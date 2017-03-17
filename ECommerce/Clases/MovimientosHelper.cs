using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECommerce.Clases
{
    public class MovimientosHelper : IDisposable
    {
        private static ECommerceContext db = new ECommerceContext();



        public void Dispose()
        {
            db.Dispose();
        }

        public static Respuesta NuevaVenta(NuevaOrdenVista vista, string userName)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var user = db.Usuarios.Where(u => u.UserName == userName).FirstOrDefault();
                    var venta = new Venta
                    {
                        EmpresaID = user.EmpresaID,
                        ClienteID = vista.ClienteID,
                        Fecha = DateTime.Now,
                        Comentarios = vista.Comentarios,
                        EstadoID = DbHelper.GetEstado("Creada", db),
                    };
                    db.Ventas.Add(venta);
                    db.SaveChanges();

                    var detalles = db.VentaDetalleTmps.Where(v => v.UserName == userName).ToList();
                    foreach (var detalle in detalles)
                    {
                        var ventaDetalles = new VentaDetalle
                        {
                            VentaID = venta.VentaID,
                            descripcion = detalle.Descripcion,
                            Precio = detalle.Precio,
                            ProductoID = detalle.ProductoID,
                            Cantidad = detalle.Cantidad,
                            Tasa = detalle.Tasa,
                        };
                        db.VentaDetalles.Add(ventaDetalles);
                        db.VentaDetalleTmps.Remove(detalle);
                    }
                    db.SaveChanges();
                    transaction.Commit();
                    return new Respuesta { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Respuesta { Succeeded = false, Message = ex.Message, };
                }
            }
        }

        public static Respuesta NuevaCompra(NuevaCompraVista vista, string userName)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var user = db.Usuarios.Where(u => u.UserName == userName).FirstOrDefault();
                    var compra = new Compra
                    {
                        EmpresaID = user.EmpresaID,
                        ProveedorID = vista.ProveedorID,
                        BodegaID = vista.BodegaID,
                        FormaPagoID = vista.FormaPagoID,
                        Fecha = vista.Fecha,
                        Comentarios = vista.Comentarios,
                        EstadoID = DbHelper.GetEstado("Creada", db),
                    };
                    db.Compras.Add(compra);
                    db.SaveChanges();

                    var detalles = db.CompraDetalleTmps.Where(v => v.UserName == userName).ToList();
                    foreach (var detalle in detalles)
                    {
                        var compraDetalles = new CompraDetalle
                        {
                            CompraID = compra.CompraID,
                            descripcion = detalle.Descripcion,
                            Precio = detalle.Precio,
                            ProductoID = detalle.ProductoID,
                            Cantidad = detalle.Cantidad,
                            Tasa = detalle.Tasa,
                        };
                        db.CompraDetalles.Add(compraDetalles);
                        db.CompraDetalleTmps.Remove(detalle);
                    }
                    db.SaveChanges();
                    foreach (var detalle in detalles)
                    {
                        var invProducto = db.Inventarios
                            .Where(p => p.ProductoID == detalle.ProductoID
                            && p.BodegaID == compra.BodegaID).FirstOrDefault();
                        double stockActual = 0;
                        if (invProducto != null)
                        {
                            stockActual = invProducto.stock;
                        }
                        var inventario = new Inventario
                        {
                            BodegaID = compra.BodegaID,
                            ProductoID = detalle.ProductoID,
                            stock = stockActual + detalle.Cantidad,
                        };
                        db.Inventarios.Add(inventario);
                    }
                    db.SaveChanges();
                    transaction.Commit();
                    return new Respuesta { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Respuesta { Succeeded = false, Message = ex.Message, };
                }
            }
        }


        public static Respuesta NuevaFactura(NuevaFacturaVista vista, string userName)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var user = db.Usuarios.Where(u => u.UserName == userName).FirstOrDefault();
                    var factura = new Factura
                    {
                        EmpresaID = user.EmpresaID,
                        ClienteID = vista.ClienteID,
                        BodegaID = vista.BodegaID,
                        FormaPagoID = vista.FormaPagoID,
                        Fecha = vista.Fecha,
                        Comentarios = vista.Comentarios,
                        EstadoID = DbHelper.GetEstado("Creada", db),
                        VentaID = null,

                    };
                    db.Facturas.Add(factura);
                    db.SaveChanges();

                    var detalles = db.FacturaDetalleTmps.Where(v => v.UserName == userName).ToList();
                    foreach (var detalle in detalles)
                    {
                        var facturaDetalles = new FacturaDetalle
                        {
                            FacturaID = factura.FacturaID,
                            descripcion = detalle.Descripcion,
                            Precio = detalle.Precio,
                            ProductoID = detalle.ProductoID,
                            Cantidad = detalle.Cantidad,
                            Descuento = detalle.Descuento,
                            Tasa = detalle.Tasa,
                        };
                        db.FacturaDetalles.Add(facturaDetalles);
                        db.FacturaDetalleTmps.Remove(detalle);
                    }
                    db.SaveChanges();
                    foreach (var detalle in detalles)
                    {
                        var producto = db.Productos
                            .Where(p => p.ProductoID == detalle.ProductoID).FirstOrDefault();
                        var productos = db.RecetaDetalles
                            .Where(rd => rd.RecetaID == producto.RecetaID).ToList();
                        foreach (var item in productos)
                        {
                            var invProducto = db.Inventarios
                            .Where(p => p.ProductoID == item.ProductoID
                            && p.BodegaID == factura.BodegaID).FirstOrDefault();
                            if (invProducto == null)
                            {
                                return new Respuesta
                                {
                                    Succeeded = false,
                                    Message = "No hay productos en inventario",
                                };
                            }
                            var count = invProducto.stock;
                            if (count >= (detalle.Cantidad * item.Cantidad))
                            {
                                var inventario = new Inventario
                                {
                                    BodegaID = factura.BodegaID,
                                    ProductoID = item.ProductoID,
                                    stock = count - (detalle.Cantidad * item.Cantidad),
                                };
                                db.Inventarios.Add(inventario);
                                db.Inventarios.Remove(invProducto);
                            }
                            else
                            {
                                return new Respuesta
                                {
                                    Succeeded = false,
                                    Message = "No hay suficientes productos en inventario",
                                };
                            }
                        }
                    }
                    db.SaveChanges();
                    transaction.Commit();
                    return new Respuesta { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Respuesta { Succeeded = false, Message = ex.Message, };
                }
            }
        }


        public static Respuesta NuevaReceta(NuevaRecetaVista vista, string userName)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var user = db.Usuarios.Where(u => u.UserName == userName).FirstOrDefault();
                    var receta = new Receta
                    {
                        EmpresaID = user.EmpresaID,
                        Descripcion = vista.Descripcion,
                        Comentarios = vista.Comentarios,
                    };
                    db.Recetas.Add(receta);
                    db.SaveChanges();

                    var detalles = db.RecetaDetalleTmps.Where(v => v.UserName == userName).ToList();
                    foreach (var detalle in detalles)
                    {
                        var recetaDetalles = new RecetaDetalle
                        {
                            RecetaID = receta.RecetaID,
                            ProductoID = detalle.ProductoID,
                            Cantidad = detalle.Cantidad,
                            Precio = detalle.Precio,
                        };
                        db.RecetaDetalles.Add(recetaDetalles);
                        db.RecetaDetalleTmps.Remove(detalle);
                    }
                    db.SaveChanges();
                    transaction.Commit();
                    return new Respuesta { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Respuesta { Succeeded = false, Message = ex.Message, };
                }
            }
        }

        public static Respuesta NuevoPedidoMesa(NuevoPedidoMesaVista vista, string userName)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var mesa = db.Mesas.Where(u => u.Nombre == userName).FirstOrDefault();
                    var pedidoMesa = new PedidoMesa
                    {
                        EmpresaID = mesa.EmpresaID,
                        MesaID = vista.MesaID,
                        Fecha = DateTime.Now,
                        Comentarios = vista.Comentarios,
                        EstadoID = DbHelper.GetEstado("Creada", db),
                    };
                    db.PedidoMesas.Add(pedidoMesa);
                    db.SaveChanges();

                    var detalles = db.PedidoMesaDetalleTmps.Where(v => v.UserName == userName).ToList();
                    foreach (var detalle in detalles)
                    {
                        var pedidoMesaDetalles = new PedidoMesaDetalle
                        {
                            PedidoMesaID = pedidoMesa.PedidoMesaID,
                            descripcion = detalle.Descripcion,
                            Precio = detalle.Precio,
                            ProductoID = detalle.ProductoID,
                            Cantidad = detalle.Cantidad,
                            Tasa = detalle.Tasa,
                        };
                        db.PedidoMesaDetalles.Add(pedidoMesaDetalles);
                        db.PedidoMesaDetalleTmps.Remove(detalle);
                    }
                    db.SaveChanges();
                    transaction.Commit();
                    return new Respuesta { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Respuesta { Succeeded = false, Message = ex.Message, };
                }
            }
        }

    }
}