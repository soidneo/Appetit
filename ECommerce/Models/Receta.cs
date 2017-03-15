using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Models
{
    public class Receta
    {
        [Key]
        public int RecetaID { get; set; }

        [Display(Name = "Receta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(256, ErrorMessage = "El campo {0} debe tener entre {2} y {1}", MinimumLength = 3)]
        [Index("Receta_EmpresaID_Descripcion_Index", 2, IsUnique = true)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar una empresa")]
        [Index("Receta_EmpresaID_Descripcion_Index", 1, IsUnique = true)]
        public int EmpresaID { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comentarios { get; set; }
        
        public virtual Empresa Empresa { get; set; }
        public virtual ICollection<RecetaDetalle> RecetaDetalles { get; set; }
        public virtual ICollection<Producto> Productos { get; set; }
    }
}