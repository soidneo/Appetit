using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Models
{
    public class Ciudad
    {
        [Key]
        public int CiudadID { get; set; }

        [Display(Name = "Ciudad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(256, ErrorMessage = "El campo {0} debe tener entre {2} y {1}", MinimumLength = 3)]
        [Index("Ciudad_DepartamentoID_Nombre_Index", 2,IsUnique = true)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(1,double.MaxValue,ErrorMessage ="Debe seleccionar un departamento")]
        [Index("Ciudad_DepartamentoID_Nombre_Index", 1, IsUnique = true)]
        public int DepartamentoID { get; set; }

        public virtual Departamento Departamento { get; set; }
        public virtual ICollection<Empresa> Empresas { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
        public virtual ICollection<Bodega> Bodegas { get; set; }
        public virtual ICollection<Cliente> Clientes { get; set; }
    }
}
