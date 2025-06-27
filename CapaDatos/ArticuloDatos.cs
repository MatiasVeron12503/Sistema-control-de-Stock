using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using VentaMayorista.CapaEntidades;
using System.Configuration;

namespace VentaMayorista.CapaDatos
{
    public class ArticuloDatos
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MayoristaDB"].ConnectionString;


        public void Crear(Articulo articulo)
        {
            // Validaciones
            if (articulo == null || string.IsNullOrWhiteSpace(articulo.Nombre) || string.IsNullOrWhiteSpace(articulo.Categoria) || articulo.PrecioUnitario < 0 || articulo.Stock < 0)
                throw new ArgumentException("Datos del artículo inválidos.");

            // Continua aqui
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Articulos (Nombre, Categoria, PrecioUnitario, Stock, FechaCreacion) VALUES (@Nombre, @Categoria, @PrecioUnitario, @Stock, @FechaCreacion)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Nombre", articulo.Nombre);
                    cmd.Parameters.AddWithValue("@Categoria", articulo.Categoria);
                    cmd.Parameters.AddWithValue("@PrecioUnitario", articulo.PrecioUnitario);
                    cmd.Parameters.AddWithValue("@Stock", articulo.Stock);
                    cmd.Parameters.AddWithValue("@FechaCreacion", DateTime.Now);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al crear el artículo: " + ex.Message);
            }
        }

        public List<Articulo> ObtenerTodos()
        {
            List<Articulo> articulos = new List<Articulo>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Articulos";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        articulos.Add(new Articulo
                        {
                            IdArticulo = (int)reader["IdArticulo"],
                            Nombre = reader["Nombre"].ToString(),
                            Categoria = reader["Categoria"].ToString(),
                            PrecioUnitario = (decimal)reader["PrecioUnitario"],
                            Stock = (int)reader["Stock"],
                            FechaCreacion = (DateTime)reader["FechaCreacion"]
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al obtener los artículos: " + ex.Message);
            }
            return articulos;
        }

        public List<Articulo> ObtenerPorCategoria(string categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria))
                throw new ArgumentException("La categoría no puede estar vacía.");

            List<Articulo> articulos = new List<Articulo>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Articulos WHERE Categoria = @Categoria";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Categoria", categoria);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        articulos.Add(new Articulo
                        {
                            IdArticulo = (int)reader["IdArticulo"],
                            Nombre = reader["Nombre"].ToString(),
                            Categoria = reader["Categoria"].ToString(),
                            PrecioUnitario = (decimal)reader["PrecioUnitario"],
                            Stock = (int)reader["Stock"],
                            FechaCreacion = (DateTime)reader["FechaCreacion"]
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al obtener artículos de la categoría {categoria}: " + ex.Message);
            }
            return articulos;
        }

        public Articulo ObtenerPorId(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Articulos WHERE IdArticulo = @IdArticulo";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@IdArticulo", id);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return new Articulo
                        {
                            IdArticulo = (int)reader["IdArticulo"],
                            Nombre = reader["Nombre"].ToString(),
                            Categoria = reader["Categoria"].ToString(),
                            PrecioUnitario = (decimal)reader["PrecioUnitario"],
                            Stock = (int)reader["Stock"],
                            FechaCreacion = (DateTime)reader["FechaCreacion"]
                        };
                    }
                    return null;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al obtener el artículo: " + ex.Message);
            }
        }

        public void Actualizar(Articulo articulo)
        {
            if (articulo == null || articulo.IdArticulo <= 0 || string.IsNullOrWhiteSpace(articulo.Nombre) || string.IsNullOrWhiteSpace(articulo.Categoria))
                throw new ArgumentException("Datos del artículo inválidos.");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Articulos SET Nombre = @Nombre, Categoria = @Categoria, " +
                                   "PrecioUnitario = @PrecioUnitario, Stock = @Stock WHERE IdArticulo = @IdArticulo";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@IdArticulo", articulo.IdArticulo);
                    cmd.Parameters.AddWithValue("@Nombre", articulo.Nombre);
                    cmd.Parameters.AddWithValue("@Categoria", articulo.Categoria);
                    cmd.Parameters.AddWithValue("@PrecioUnitario", articulo.PrecioUnitario);
                    cmd.Parameters.AddWithValue("@Stock", articulo.Stock);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                        throw new Exception("No se encontró el artículo para actualizar.");
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al actualizar el artículo: " + ex.Message);
            }
        }

        public void Eliminar(int idArticulo)
        {
            if (idArticulo <= 0)
                throw new ArgumentException("ID del artículo inválido.");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Articulos WHERE IdArticulo = @IdArticulo";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@IdArticulo", idArticulo);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                        throw new Exception("No se encontró el artículo para eliminar.");
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al eliminar el artículo: " + ex.Message);
            }
        }

    }
}
