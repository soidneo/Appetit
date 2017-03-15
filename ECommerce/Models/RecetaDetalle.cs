using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Models
{
    public class RecetaDetalle
    {
        [Key]
        public int RecetaDetalleID { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int RecetaID { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int ProductoID { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "Debe seleccionar una {0}")]
        public decimal Precio { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "Debe seleccionar una {0}")]
        public double Cantidad { get; set; }

        public virtual Receta Receta { get; set; }
        public virtual Producto Producto { get; set; }
    }
}
