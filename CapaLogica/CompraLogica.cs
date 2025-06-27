using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaMayorista.CapaDatos;
using VentaMayorista.CapaEntidades;

namespace VentaMayorista.CapaLogica
{
    public class CompraLogica
    {
        private readonly ArticuloDatos articuloDatos = new ArticuloDatos();
        private readonly CompraDatos compraDatos = new CompraDatos();

        public void RegistrarCompra(Compra compra)
        {
            if (compra == null) throw new ArgumentNullException(nameof(compra));
            if (compra.Detalles == null || compra.Detalles.Count == 0) throw new Exception("La compra debe incluir al menos un artículo");
            if (string.IsNullOrEmpty(compra.Proveedor)) throw new Exception("El proveedor es obligatorio");

            foreach (var detalle in compra.Detalles)
            {
                var articulo = articuloDatos.ObtenerPorId(detalle.IdArticulo);
                if (articulo == null) throw new Exception($"El artículo con ID {detalle.IdArticulo} no existe");
                if (detalle.Cantidad <= 0) throw new Exception($"La cantidad del artículo {articulo.Nombre} debe ser mayor a 0");
                if (detalle.PrecioUnitario <= 0) throw new Exception($"El precio unitario del artículo {articulo.Nombre} debe ser mayor a 0");
            }

            compra.Total = compra.Detalles.Sum(d => d.Subtotal);
            compra.FechaCompra = DateTime.Now;

            compraDatos.Crear(compra);

            foreach (var detalle in compra.Detalles)
            {
                var articulo = articuloDatos.ObtenerPorId(detalle.IdArticulo);
                articulo.Stock += detalle.Cantidad; // Sumar al stock
                articuloDatos.Actualizar(articulo);
            }
        }

        public List<Compra> ObtenerTodasCompras()
        {
            return compraDatos.ObtenerTodos();
        }

        public Compra ObtenerCompraPorId(int idCompra)
        {
            if (idCompra <= 0) throw new ArgumentException("El ID de la compra debe ser mayor a 0");
            var compra = compraDatos.ObtenerPorId(idCompra);
            if (compra == null) throw new Exception("La compra no existe");
            return compra;
        }

        public void ActualizarCompra(Compra compra)
        {
            if (compra == null) throw new ArgumentNullException(nameof(compra));
            if (compra.IdCompra <= 0) throw new Exception("El ID de la compra es inválido");
            if (compra.Detalles == null || compra.Detalles.Count == 0) throw new Exception("La compra debe incluir al menos un artículo");
            if (string.IsNullOrEmpty(compra.Proveedor)) throw new Exception("El proveedor es obligatorio");

            foreach (var detalle in compra.Detalles)
            {
                var articulo = articuloDatos.ObtenerPorId(detalle.IdArticulo);
                if (articulo == null) throw new Exception($"El artículo con ID {detalle.IdArticulo} no existe");
                if (detalle.Cantidad <= 0) throw new Exception($"La cantidad del artículo {articulo.Nombre} debe ser mayor a 0");
                if (detalle.PrecioUnitario <= 0) throw new Exception($"El precio unitario del artículo {articulo.Nombre} debe ser mayor a 0");
            }

            // Calcular el total
            compra.Total = compra.Detalles.Sum(d => d.Subtotal);

            // Obtener la compra existente para ajustar el stock
            var compraExistente = compraDatos.ObtenerPorId(compra.IdCompra);
            if (compraExistente == null) throw new Exception("La compra no existe");

            // Revertir el stock de los detalles existentes
            foreach (var detalleExistente in compraExistente.Detalles)
            {
                var articulo = articuloDatos.ObtenerPorId(detalleExistente.IdArticulo);
                articulo.Stock -= detalleExistente.Cantidad; // Restar el stock anterior
                articuloDatos.Actualizar(articulo);
            }

            // Actualizar la compra en la base de datos
            compraDatos.Actualizar(compra);

            // Aplicar el nuevo stock
            foreach (var detalle in compra.Detalles)
            {
                var articulo = articuloDatos.ObtenerPorId(detalle.IdArticulo);
                articulo.Stock += detalle.Cantidad; // Sumar el nuevo stock
                articuloDatos.Actualizar(articulo);
            }
        }

        public void EliminarCompra(int idCompra)
        {
            if (idCompra <= 0) throw new ArgumentException("El ID de la compra debe ser mayor a 0");

            // Obtener la compra para revertir el stock
            var compra = compraDatos.ObtenerPorId(idCompra);
            if (compra == null) throw new Exception("La compra no existe");

            // Revertir el stock
            foreach (var detalle in compra.Detalles)
            {
                var articulo = articuloDatos.ObtenerPorId(detalle.IdArticulo);
                articulo.Stock -= detalle.Cantidad; // Restar el stock
                articuloDatos.Actualizar(articulo);
            }

            // Eliminar la compra
            compraDatos.Eliminar(idCompra);
        }

        public List<Compra> ConsultarComprasPorArticulo(int idArticulo)
        {
            if (idArticulo <= 0) throw new ArgumentException("El ID del artículo debe ser mayor a 0");
            return compraDatos.ConsultarComprasPorArticulo(idArticulo);
        }
    }
}
