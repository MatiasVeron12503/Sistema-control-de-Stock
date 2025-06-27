using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaMayorista.CapaDatos;
using VentaMayorista.CapaEntidades;

namespace VentaMayorista.CapaLogica
{
    public class VentaLogica
    {
        private readonly ArticuloDatos articuloDatos = new ArticuloDatos();
        private readonly VentaDatos ventaDatos = new VentaDatos();

        public void RegistrarVenta(Venta venta)
        {
            if (venta.Detalles.Count == 0) throw new Exception("La venta debe incluir al menos un artículo");

            foreach (var detalle in venta.Detalles)
            {
                var articulo = articuloDatos.ObtenerPorId(detalle.IdArticulo);
                if (articulo == null) throw new Exception($"El artículo {detalle.IdArticulo} no existe");
                if (articulo.Stock < detalle.Cantidad)
                    throw new Exception($"Stock insuficiente para {articulo.Nombre}");
            }

            venta.Total = venta.Detalles.Sum(d => d.Subtotal);
            venta.FechaVenta = DateTime.Now;

            ventaDatos.Crear(venta);

            foreach (var detalle in venta.Detalles)
            {
                var articulo = articuloDatos.ObtenerPorId(detalle.IdArticulo);
                articulo.Stock -= detalle.Cantidad;
                articuloDatos.Actualizar(articulo);
            }
        }

        public List<Venta> ConsultarVentasPorArticuloYCliente(int idArticulo, int idCliente)
        {
            if (ventaDatos == null)
                return new List<Venta>();
            try
            {
                return ventaDatos.ConsultarVentasPorArticuloYCliente(idArticulo, idCliente);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al consultar ventas: {ex.Message}", ex);
            }
        }
    }
}
