using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Models
{
    public class Unidad
    {
        [Key]
        public int UnidadID { get; set; }

        [Display(Name = "Unidad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(100, ErrorMessage = "El campo {0} debe tener entre {2} y {1}", MinimumLength = 1)]
        [Index("Unidad_EmpresaID_Descripcion_Index", 2, IsUnique = true)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar una empresa")]
        [Index("Unidad_EmpresaID_Descripcion_Index", 1, IsUnique = true)]
        public int EmpresaID { get; set; }
        
        public virtual Empresa Empresa { get; set; }
        public virtual ICollection<Producto> Productos { get; set; }
    }
}
