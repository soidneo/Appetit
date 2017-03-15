using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ECommerce.Models
{
    public class NuevaRecetaVista
    {
        [Display(Name = "Receta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(30, ErrorMessage = "El campo {0} debe tener entre {2} y {1}", MinimumLength = 3)]
        public string Descripcion { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comentarios { get; set; }

        public List<RecetaDetalleTmp> Detalles { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public decimal TotalValor { get { return Detalles == null ? 0 : Detalles.Sum(d => d.Producto.Precio * (decimal)d.Cantidad); } }
    }
}