using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ECommerce.Models
{
    public class FormaPago
    {
        [Key]
        public int FormaPagoID { get; set; }

        [Display(Name = "Forma de Pago")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(256, ErrorMessage = "El campo {0} debe tener entre {2} y {1}", MinimumLength = 3)]
        [Index("FormaPago_EmpresaID_Descripcion_Index", 2, IsUnique = true)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar una empresa")]
        [Index("FormaPago_EmpresaID_Descripcion_Index", 1, IsUnique = true)]
        public int EmpresaID { get; set; }

        public virtual Empresa Empresa { get; set; }
        public virtual ICollection<Compra> Compras { get; set; }
        public virtual ICollection<Factura> Factura { get; set; }
    }
}