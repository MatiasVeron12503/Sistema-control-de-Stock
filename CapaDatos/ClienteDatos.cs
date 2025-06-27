using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaMayorista.CapaEntidades;

namespace VentaMayorista.CapaDatos
{
    public class ClienteDatos
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MayoristaDB"].ConnectionString;

        public void Crear(Cliente cliente)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Verificar si el DNI ya existe
                string queryCheck = "SELECT COUNT(*) FROM Clientes WHERE DNI = @DNI";
                SqlCommand cmdCheck = new SqlCommand(queryCheck, conn);
                cmdCheck.Parameters.AddWithValue("@DNI", cliente.DNI);
                conn.Open();
                int count = (int)cmdCheck.ExecuteScalar();
                if (count > 0)
                    throw new Exception("El DNI ya está registrado");

                // Insertar cliente
                string query = "INSERT INTO Clientes (Nombre, Apellido, DNI, Telefono, Email) " +
                               "VALUES (@Nombre, @Apellido, @DNI, @Telefono, @Email)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", cliente.Apellido);
                cmd.Parameters.AddWithValue("@DNI", cliente.DNI);
                cmd.Parameters.AddWithValue("@Telefono", (object)cliente.Telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)cliente.Email ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        public List<Cliente> ObtenerTodos()
        {
            List<Cliente> clientes = new List<Cliente>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Clientes";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    clientes.Add(new Cliente
                    {
                        IdCliente = (int)reader["IdCliente"],
                        Nombre = reader["Nombre"].ToString(),
                        Apellido = reader["Apellido"].ToString(),
                        DNI = reader["DNI"].ToString(),
                        Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader["Telefono"].ToString(),
                        Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader["Email"].ToString(),
                        FechaCreacion = (DateTime)reader["FechaCreacion"]
                    });
                }
            }
            return clientes;
        }

        public Cliente ObtenerPorId(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Clientes WHERE IdCliente = @IdCliente";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdCliente", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Cliente
                    {
                        IdCliente = (int)reader["IdCliente"],
                        Nombre = reader["Nombre"].ToString(),
                        Apellido = reader["Apellido"].ToString(),
                        DNI = reader["DNI"].ToString(),
                        Telefono = reader["Telefono"].ToString(),
                        Email = reader["Email"].ToString(),
                        FechaCreacion = (DateTime)reader["FechaCreacion"]
                    };
                }
                return null;
            }
        }

        public void Actualizar(Cliente cliente)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Clientes SET Nombre = @Nombre, Apellido = @Apellido, " +
                               "DNI = @DNI, Telefono = @Telefono, Email = @Email WHERE IdCliente = @IdCliente";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);
                cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", cliente.Apellido);
                cmd.Parameters.AddWithValue("@DNI", cliente.DNI);
                cmd.Parameters.AddWithValue("@Telefono", (object)cliente.Telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)cliente.Email ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Eliminar(int idCliente)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Clientes WHERE IdCliente = @IdCliente";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
