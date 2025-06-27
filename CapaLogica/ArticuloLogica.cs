using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaMayorista.CapaDatos;
using VentaMayorista.CapaEntidades;


namespace VentaMayorista.CapaLogica
{
    public class ArticuloLogica
    {
        private readonly ArticuloDatos articuloDatos = new ArticuloDatos();
        private readonly List<string> categoriasPermitidas = new List<string> { "Comestibles", "Librería", "Electrodomésticos" };


        public void Crear(Articulo articulo)
        {
            ValidarArticulo(articulo, esCreacion: true);

            // Verificar si ya existe un artículo con el mismo nombre en la misma categoría
            var articulos = articuloDatos.ObtenerPorCategoria(articulo.Categoria);
            if (articulos.Any(a => a.Nombre.Equals(articulo.Nombre, StringComparison.OrdinalIgnoreCase)))
                throw new Exception($"Ya existe un artículo con el nombre '{articulo.Nombre}' en la categoría '{articulo.Categoria}'.");

            try
            {
                articuloDatos.Crear(articulo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el artículo: " + ex.Message);
            }
        }

        public List<Articulo> ObtenerTodos()
        {
            try
            {
                return articuloDatos.ObtenerTodos();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los artículos: " + ex.Message);
            }
        }

        public List<Articulo> ObtenerPorCategoria(string categoria)
        {
            if (!categoriasPermitidas.Contains(categoria))
                throw new ArgumentException($"La categoría '{categoria}' no es válida.");

            try
            {
                return articuloDatos.ObtenerPorCategoria(categoria);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener artículos de la categoría '{categoria}': " + ex.Message);
            }
        }

        public Articulo ObtenerPorId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del artículo debe ser mayor que cero.");

            try
            {
                var articulo = articuloDatos.ObtenerPorId(id);
                if (articulo == null)
                    throw new Exception($"No se encontró un artículo con ID {id}.");
                return articulo;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el artículo: " + ex.Message);
            }
        }

        public void Actualizar(Articulo articulo)
        {
            ValidarArticulo(articulo, esCreacion: false);

            // Verificar si otro artículo tiene el mismo nombre en la misma categoría
            var articulos = articuloDatos.ObtenerPorCategoria(articulo.Categoria);
            if (articulos.Any(a => a.Nombre.Equals(articulo.Nombre, StringComparison.OrdinalIgnoreCase) && a.IdArticulo != articulo.IdArticulo))
                throw new Exception($"Ya existe otro artículo con el nombre '{articulo.Nombre}' en la categoría '{articulo.Categoria}'.");

            try
            {
                articuloDatos.Actualizar(articulo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el artículo: " + ex.Message);
            }
        }

        public void Eliminar(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del artículo debe ser mayor que cero.");

            try
            {
                articuloDatos.Eliminar(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el artículo: " + ex.Message);
            }
        }

        private void ValidarArticulo(Articulo articulo, bool esCreacion)
        {
            if (articulo == null)
                throw new ArgumentException("El artículo no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(articulo.Nombre) || articulo.Nombre.Length < 3)
                throw new ArgumentException("El nombre del artículo debe tener al menos 3 caracteres.");

            if (!categoriasPermitidas.Contains(articulo.Categoria))
                throw new ArgumentException($"La categoría '{articulo.Categoria}' no es válida. Categorías permitidas: {string.Join(", ", categoriasPermitidas)}.");

            if (articulo.PrecioUnitario <= 0)
                throw new ArgumentException("El precio unitario debe ser mayor que cero.");

            if (articulo.Stock < 0)
                throw new ArgumentException("El stock no puede ser negativo.");

            if (!esCreacion && articulo.IdArticulo <= 0)
                throw new ArgumentException("El ID del artículo es inválido para la actualización.");
        }
    }
}
