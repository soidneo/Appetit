using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECommerce.Clases
{
    public class DbHelper
    {
        public static Respuesta Guardar(ECommerceContext db)
        {
            try
            {
                db.SaveChanges();
                return new Respuesta { Succeeded = true, };
            }
            catch (Exception ex)
            {
                var respuesta = new Respuesta { Succeeded = false, };
                if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null)
                {
                    if (ex.InnerException.InnerException.Message.Contains("_Index"))
                    {
                        respuesta.Message = "¡Error! el registro ya existe";
                        if (ex.InnerException.InnerException.Message.Contains("Referencia_Index"))
                        {
                            respuesta.Message = "¡Error! el Código de barras ya existe";
                        }
                        if (ex.InnerException.InnerException.Message.Contains("Descripcion_Index"))
                        {
                            respuesta.Message = "¡Error! el Nombre del producto ya existe"; 
                        }
                        if (ex.InnerException.InnerException.Message.Contains("Nombre_Index"))
                        {
                            respuesta.Message = "¡Error! el Nombre ya existe";
                        }
                    }
                    if (ex.InnerException.InnerException.Message.Contains("REFERENCE"))
                    {
                        respuesta.Message = "¡Error! no se puede eliminar. Tiene registros relacionados";
                    }

                }
                else
                {
                    respuesta.Message = ex.Message;
                }
                return respuesta;
            }
        }

        public static int GetEstado(string descripcion, ECommerceContext db)
        {
            var estado = db.Estados.Where(e => e.Descripcion == descripcion).FirstOrDefault();
            if (estado == null)
            {
                estado = new Estado { Descripcion = descripcion, };
                db.Estados.Add(estado);
                db.SaveChanges();
            }
            return estado.EstadoID;
        }
    }
}