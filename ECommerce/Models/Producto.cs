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
    public class Producto
    {
        [Key]
        public int ProductoID { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar una empresa")]
        [Index("Producto_EmpresaID_Descripcion_Index", 1, IsUnique = true)]
        [Index("Producto_EmpresaID_Referencia_Index", 1, IsUnique = true)]
        public int EmpresaID { get; set; }

        [Display(Name = "Producto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(256, ErrorMessage = "El campo {0} debe tener entre {2} y {1}", MinimumLength = 3)]
        [Index("Producto_EmpresaID_Descripcion_Index", 2, IsUnique = true)]
        public string Descripcion { get; set; }

        [Display(Name = "Código de barras")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(13, ErrorMessage = "El campo {0} debe tener entre {2} y {1}", MinimumLength = 3)]
        [Index("Producto_EmpresaID_Referencia_Index", 2, IsUnique = true)]
        public string Referencia { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar una {0}")]
        [Display(Name = "Categoria", Prompt = "[Seleccione una categoria...]")]
        public int CategoriaID { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar una {0}")]
        [Display(Name = "Impuesto", Prompt = "[Seleccione un impuesto...]")]
        public int ImpuestoID { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar una {0}")]
        [Display(Name = "Unidad", Prompt = "[Seleccione una unidad...]")]
        public int UnidadID { get; set; }

        [Display(Name = "Categoria", Prompt = "[Seleccione una receta...]")]
        public int? RecetaID { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        [Range(0, double.MaxValue, ErrorMessage = "Debe seleccionar una {0} entre {1} y {2}")]
        public decimal Precio { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Image { get; set; }

        [NotMapped]
        [Display(Name ="Imagen")]
        public HttpPostedFileBase ImageFile { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comentarios { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public double stock { get { return Inventarios == null ? 0 : Inventarios.Sum(i => i.stock); } }

        public virtual Empresa Empresa { get; set; }
        public virtual Categoria Categoria { get; set; }
        public virtual Unidad Unidad { get; set; }
        public virtual Impuesto Impuesto { get; set; }
        public virtual Receta Receta { get; set; }
        public virtual ICollection<Inventario> Inventarios { get; set; }
        public virtual ICollection<RecetaDetalle> RecetaDetalles { get; set; }
        public virtual ICollection<RecetaDetalleTmp> RecetaDetalleTmps { get; set; }
        public virtual ICollection<VentaDetalle> VentaDetalles { get; set; }
        public virtual ICollection<VentaDetalleTmp> VentaDetalleTmps { get; set; }
        public virtual ICollection<CompraDetalle> CompraDetalles { get; set; }
        public virtual ICollection<CompraDetalleTmp> CompraDetalleTmps { get; set; }
        public virtual ICollection<FacturaDetalle> FacturaDetalles { get; set; }
        public virtual ICollection<FacturaDetalleTmp> FacturaDetalleTmps { get; set; }
        public virtual ICollection<PedidoMesaDetalle> PedidoMesaDetalles { get; set; }
        public virtual ICollection<PedidoMesaDetalleTmp> PedidoMesaDetalleTmps { get; set; }
    }
}
