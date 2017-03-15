using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ECommerce.Models
{
    public class Mesa
    {
        [Key]
        public int MesaID { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar una {0}")]
        [Display(Name = "Empresa")]
        [Index("Mesa_EmpresaID_Nombre_Index", 1, IsUnique = true)]
        public int EmpresaID { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(256, ErrorMessage = "El campo {0} debe tener entre {2} y {1}", MinimumLength = 1)]
        [Index("Mesa_EmpresaID_Nombre_Index", 2, IsUnique = true)]
        [Display(Name = "Mesa")]
        public string Nombre { get; set; }

        public virtual Empresa Empresa { get; set; }
    }
}