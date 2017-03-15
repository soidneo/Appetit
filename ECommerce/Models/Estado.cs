using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ECommerce.Models
{
    public class Estado
    {
        [Key]
        public int EstadoID { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo {0} debe tener entre {2} y {1}", MinimumLength = 3)]
        [Index("Estado_Descripcion_Index", IsUnique = true)]
        [Display(Name = "Estado")]
        public string Descripcion { get; set; }

        public virtual ICollection<Venta> Ventas { get; set; }
        public virtual ICollection<Factura> Factura { get; set; }
        public virtual List<Compra> Compras { get; set; }
    }
}