using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Models
{
    public class Inventario
    {
        [Key]
        public int InventarioID { get; set; }

        [Required]
        public int BodegaID { get; set; }

        [Required]
        public int ProductoID { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public double stock { get; set; }

        public virtual Bodega Bodega { get; set; }
        public virtual Producto Producto { get; set; }
    }
}
