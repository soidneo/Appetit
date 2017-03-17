using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ECommerce.Models
{
    public class Empresa
    {
        [Key]
        public int EmpresaID { get; set; }

        [Display(Name = "Empresa")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(256, ErrorMessage = "El campo {0} debe tener entre {2} y {1}", MinimumLength = 3)]
        [Index("Empresa_Nombre_Index", IsUnique = true)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(30, ErrorMessage = "El campo {0} debe tener entre {2} y {1}", MinimumLength = 3)]
        [DataType(DataType.PhoneNumber)]
        public string Telefono { get; set; }

        [Display(Name = "Dirección")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(256, ErrorMessage = "El campo {0} debe tener entre {2} y {1}", MinimumLength = 3)]
        public string Direccion { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Logo { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(1,double.MaxValue,ErrorMessage = "Debe seleccionar un {0}")]
        public int DepartamentoID { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar una {0}")]
        public int CiudadID { get; set; }

        [NotMapped]
        public HttpPostedFileBase LogoFile { get; set; }

        public virtual Departamento Departamento { get; set; }
        public virtual Ciudad Ciudad { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
        public virtual ICollection<Categoria> Categorias { get; set; }
        public virtual ICollection<Unidad> Unidades { get; set; }
        public virtual ICollection<Receta> Recetas { get; set; }
        public virtual ICollection<Impuesto> Impuestos { get; set; }
        public virtual ICollection<Producto> Productos { get; set; }
        public virtual ICollection<Bodega> Bodegas { get; set; }
        public virtual ICollection<Mesa> Mesas { get; set; }
        public virtual ICollection<Cliente> Clientes { get; set; }
        public virtual ICollection<Venta> Ventas { get; set; }
        public virtual ICollection<Factura> Factura { get; set; }
        public virtual List<Compra> Compras { get; set; }
    }
}


