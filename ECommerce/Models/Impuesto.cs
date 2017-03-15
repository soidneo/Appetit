using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Models
{
    public class Impuesto
    {
        [Key]
        public int ImpuestoID { get; set; }

        [Display(Name = "Impuesto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(100, ErrorMessage = "El campo {0} debe tener entre {2} y {1}", MinimumLength = 3)]
        [Index("Impuesto_EmpresaID_Descripcion_Index", 2, IsUnique = true)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DisplayFormat(DataFormatString ="{0:P2}",ApplyFormatInEditMode = false)]
        [Range(0.00, 1.00, ErrorMessage = "Debe seleccionar una {0} entre {1} y {2}")]
        public double Tasa { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar una empresa")]
        [Index("Impuesto_EmpresaID_Descripcion_Index", 1, IsUnique = true)]
        public int EmpresaID { get; set; }

        public virtual Empresa Empresa { get; set; }
        public virtual ICollection<Producto> Productos { get; set; }
    }
}
