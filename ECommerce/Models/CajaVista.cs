using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECommerce.Models
{
    public class CajaVista
    {
        //E.Descripcion as Estado, M.Nombre as Nombre, Fecha,Comentarios
        public int VentaID { get; set; }
        public string Estado { get; set; }
        public string Nombre { get; set; }
        public DateTime Fecha { get; set; }
        public string Comentarios { get; set; }
        public string Tipo { get; set; }
    }
}