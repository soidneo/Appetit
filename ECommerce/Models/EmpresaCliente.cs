using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ECommerce.Models
{
    public class EmpresaCliente
    {
        [Key]
        public int EmpresaClienteID { get; set; }

        [Display(Name = "Empresa")]
        public int EmpresaID { get; set; }

        [Display(Name = "Cliente")]
        public int ClienteID { get; set; }

        public virtual Cliente Cliente { get; set; }
        public virtual Empresa Empresa { get; set; }

    }
}