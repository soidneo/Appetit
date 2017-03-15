using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ECommerce.Models
{
    public class CompraDetalle
    {
        [Key]
        public int CompraDetalleID { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int CompraID { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int ProductoID { get; set; }

        [Display(Name = "Producto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(30, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres", MinimumLength = 3)]
        public string descripcion { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "Debe seleccionar una {0}")]
        [Display(Name = "Impuesto")]
        public double Tasa { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "Debe seleccionar una {0}")]
        public decimal Precio { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "Debe seleccionar una {0}")]
        public double Cantidad { get; set; }

        public virtual Compra Compra { get; set; }
        public virtual Producto Producto { get; set; }
    }
}