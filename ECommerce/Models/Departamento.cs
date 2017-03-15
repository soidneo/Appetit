using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Models
{
    public  class Departamento
    {
        [Key]
        public int DepartamentoID { get; set; }

        [Display(Name ="Departamento")]
        [Required(ErrorMessage ="El campo {0} es obligatorio")]
        [StringLength(256, ErrorMessage = "El campo {0} debe tener entre {2} y {1}", MinimumLength = 3)]
        [Index("Departamento_Nombre_Index", IsUnique = true)]
        public string Nombre { get; set; }

        public virtual ICollection <Ciudad> Ciudades { get; set; }
        public virtual ICollection<Empresa> Empresas { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
        public virtual ICollection<Bodega> Bodegas { get; set; }
        public virtual ICollection<Cliente> Clientes { get; set; }
    }
}
