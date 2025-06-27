using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaMayorista.CapaEntidades
{
    public class Compra
    {
        public int IdCompra { get; set; }
        public DateTime FechaCompra { get; set; }
        public string Proveedor { get; set; }
        public decimal Total { get; set; }
        public List<DetalleCompra> Detalles { get; set; } = new List<DetalleCompra>();
    }
}
