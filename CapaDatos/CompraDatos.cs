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
    public class CompraDatos
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MayoristaDB"].ConnectionString;

        public void Crear(Compra compra)
        {
            if (string.IsNullOrEmpty(compra.Proveedor)) throw new ArgumentException("El proveedor es obligatorio");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Insertar Compra
                        string queryCompra = "INSERT INTO Compras (FechaCompra, Proveedor, Total) " +
                                            "OUTPUT INSERTED.IdCompra " +
                                            "VALUES (@FechaCompra, @Proveedor, @Total)";
                        SqlCommand cmdCompra = new SqlCommand(queryCompra, conn, transaction);
                        cmdCompra.Parameters.AddWithValue("@FechaCompra", compra.FechaCompra);
                        cmdCompra.Parameters.AddWithValue("@Proveedor", compra.Proveedor);
                        cmdCompra.Parameters.AddWithValue("@Total", compra.Total);
                        int idCompra = (int)cmdCompra.ExecuteScalar();

                        // Insertar DetalleCompras
                        foreach (var detalle in compra.Detalles)
                        {
                            string queryDetalle = "INSERT INTO DetalleCompras (IdCompra, IdArticulo, Cantidad, PrecioUnitario, Subtotal) " +
                                                 "VALUES (@IdCompra, @IdArticulo, @Cantidad, @PrecioUnitario, @Subtotal)";
                            SqlCommand cmdDetalle = new SqlCommand(queryDetalle, conn, transaction);
                            cmdDetalle.Parameters.AddWithValue("@IdCompra", idCompra);
                            cmdDetalle.Parameters.AddWithValue("@IdArticulo", detalle.IdArticulo);
                            cmdDetalle.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                            cmdDetalle.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);
                            cmdDetalle.Parameters.AddWithValue("@Subtotal", detalle.Subtotal);
                            cmdDetalle.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<Compra> ObtenerTodos()
        {
            List<Compra> compras = new List<Compra>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT c.*, dc.*
                    FROM Compras c
                    LEFT JOIN DetalleCompras dc ON c.IdCompra = dc.IdCompra";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                Compra compraActual = null;
                while (reader.Read())
                {
                    int idCompra = (int)reader["IdCompra"];
                    if (compraActual == null || compraActual.IdCompra != idCompra)
                    {
                        compraActual = new Compra
                        {
                            IdCompra = idCompra,
                            FechaCompra = (DateTime)reader["FechaCompra"],
                            Proveedor = reader["Proveedor"].ToString(),
                            Total = (decimal)reader["Total"]
                        };
                        compras.Add(compraActual);
                    }
                    if (!reader.IsDBNull(reader.GetOrdinal("IdDetalleCompra")))
                    {
                        compraActual.Detalles.Add(new DetalleCompra
                        {
                            IdDetalleCompra = (int)reader["IdDetalleCompra"],
                            IdCompra = idCompra,
                            IdArticulo = (int)reader["IdArticulo"],
                            Cantidad = (int)reader["Cantidad"],
                            PrecioUnitario = (decimal)reader["PrecioUnitario"],
                            Subtotal = (decimal)reader["Subtotal"]
                        });
                    }
                }
            }
            return compras;
        }

        public Compra ObtenerPorId(int idCompra)
        {
            Compra compra = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT c.*, dc.*
                    FROM Compras c
                    LEFT JOIN DetalleCompras dc ON c.IdCompra = dc.IdCompra
                    WHERE c.IdCompra = @IdCompra";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdCompra", idCompra);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (compra == null)
                    {
                        compra = new Compra
                        {
                            IdCompra = (int)reader["IdCompra"],
                            FechaCompra = (DateTime)reader["FechaCompra"],
                            Proveedor = reader["Proveedor"].ToString(),
                            Total = (decimal)reader["Total"]
                        };
                    }
                    if (!reader.IsDBNull(reader.GetOrdinal("IdDetalleCompra")))
                    {
                        compra.Detalles.Add(new DetalleCompra
                        {
                            IdDetalleCompra = (int)reader["IdDetalleCompra"],
                            IdCompra = (int)reader["IdCompra"],
                            IdArticulo = (int)reader["IdArticulo"],
                            Cantidad = (int)reader["Cantidad"],
                            PrecioUnitario = (decimal)reader["PrecioUnitario"],
                            Subtotal = (decimal)reader["Subtotal"]
                        });
                    }
                }
            }
            return compra;
        }

        public void Actualizar(Compra compra)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Actualizar Compra
                        string queryCompra = "UPDATE Compras SET FechaCompra = @FechaCompra, Proveedor = @Proveedor, Total = @Total " +
                                            "WHERE IdCompra = @IdCompra";
                        SqlCommand cmdCompra = new SqlCommand(queryCompra, conn, transaction);
                        cmdCompra.Parameters.AddWithValue("@IdCompra", compra.IdCompra);
                        cmdCompra.Parameters.AddWithValue("@FechaCompra", compra.FechaCompra);
                        cmdCompra.Parameters.AddWithValue("@Proveedor", compra.Proveedor);
                        cmdCompra.Parameters.AddWithValue("@Total", compra.Total);
                        cmdCompra.ExecuteNonQuery();

                        // Eliminar detalles existentes
                        string queryDeleteDetalles = "DELETE FROM DetalleCompras WHERE IdCompra = @IdCompra";
                        SqlCommand cmdDeleteDetalles = new SqlCommand(queryDeleteDetalles, conn, transaction);
                        cmdDeleteDetalles.Parameters.AddWithValue("@IdCompra", compra.IdCompra);
                        cmdDeleteDetalles.ExecuteNonQuery();

                        // Insertar nuevos detalles
                        foreach (var detalle in compra.Detalles)
                        {
                            string queryDetalle = "INSERT INTO DetalleCompras (IdCompra, IdArticulo, Cantidad, PrecioUnitario, Subtotal) " +
                                                 "VALUES (@IdCompra, @IdArticulo, @Cantidad, @PrecioUnitario, @Subtotal)";
                            SqlCommand cmdDetalle = new SqlCommand(queryDetalle, conn, transaction);
                            cmdDetalle.Parameters.AddWithValue("@IdCompra", compra.IdCompra);
                            cmdDetalle.Parameters.AddWithValue("@IdArticulo", detalle.IdArticulo);
                            cmdDetalle.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                            cmdDetalle.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);
                            cmdDetalle.Parameters.AddWithValue("@Subtotal", detalle.Subtotal);
                            cmdDetalle.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Eliminar(int idCompra)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Eliminar detalles
                        string queryDeleteDetalles = "DELETE FROM DetalleCompras WHERE IdCompra = @IdCompra";
                        SqlCommand cmdDeleteDetalles = new SqlCommand(queryDeleteDetalles, conn, transaction);
                        cmdDeleteDetalles.Parameters.AddWithValue("@IdCompra", idCompra);
                        cmdDeleteDetalles.ExecuteNonQuery();

                        // Eliminar compra
                        string queryDeleteCompra = "DELETE FROM Compras WHERE IdCompra = @IdCompra";
                        SqlCommand cmdDeleteCompra = new SqlCommand(queryDeleteCompra, conn, transaction);
                        cmdDeleteCompra.Parameters.AddWithValue("@IdCompra", idCompra);
                        cmdDeleteCompra.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<Compra> ConsultarComprasPorArticulo(int idArticulo)
        {
            List<Compra> compras = new List<Compra>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT c.*, dc.*
                    FROM Compras c
                    INNER JOIN DetalleCompras dc ON c.IdCompra = dc.IdCompra
                    WHERE dc.IdArticulo = @IdArticulo";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdArticulo", idArticulo);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                Compra compraActual = null;
                while (reader.Read())
                {
                    int idCompra = (int)reader["IdCompra"];
                    if (compraActual == null || compraActual.IdCompra != idCompra)
                    {
                        compraActual = new Compra
                        {
                            IdCompra = idCompra,
                            FechaCompra = (DateTime)reader["FechaCompra"],
                            Proveedor = reader["Proveedor"].ToString(),
                            Total = (decimal)reader["Total"]
                        };
                        compras.Add(compraActual);
                    }
                    compraActual.Detalles.Add(new DetalleCompra
                    {
                        IdDetalleCompra = (int)reader["IdDetalleCompra"],
                        IdCompra = idCompra,
                        IdArticulo = (int)reader["IdArticulo"],
                        Cantidad = (int)reader["Cantidad"],
                        PrecioUnitario = (decimal)reader["PrecioUnitario"],
                        Subtotal = (decimal)reader["Subtotal"]
                    });
                }
            }
            return compras;
        }
    }
}
