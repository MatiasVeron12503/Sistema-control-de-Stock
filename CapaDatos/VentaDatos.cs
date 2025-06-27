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
    public class VentaDatos
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MayoristaDB"].ConnectionString;

        public void Crear(Venta venta)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Insertar Venta
                        string queryVenta = "INSERT INTO Ventas (IdCliente, FechaVenta, Total) " +
                                           "OUTPUT INSERTED.IdVenta " +
                                           "VALUES (@IdCliente, @FechaVenta, @Total)";
                        SqlCommand cmdVenta = new SqlCommand(queryVenta, conn, transaction);
                        cmdVenta.Parameters.AddWithValue("@IdCliente", venta.IdCliente);
                        cmdVenta.Parameters.AddWithValue("@FechaVenta", venta.FechaVenta);
                        cmdVenta.Parameters.AddWithValue("@Total", venta.Total);
                        int idVenta = (int)cmdVenta.ExecuteScalar();

                        // Insertar DetalleVentas
                        foreach (var detalle in venta.Detalles)
                        {
                            string queryDetalle = "INSERT INTO DetalleVentas (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) " +
                                                 "VALUES (@IdVenta, @IdArticulo, @Cantidad, @PrecioUnitario, @Subtotal)";
                            SqlCommand cmdDetalle = new SqlCommand(queryDetalle, conn, transaction);
                            cmdDetalle.Parameters.AddWithValue("@IdVenta", idVenta);
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

        public List<Venta> ConsultarVentasPorArticuloYCliente(int idArticulo, int idCliente)
        {
            if (string.IsNullOrEmpty(connectionString))
                return new List<Venta>();

            List<Venta> ventas = new List<Venta>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT v.*, dv.*
                    FROM Ventas v
                    INNER JOIN DetalleVentas dv ON v.IdVenta = dv.IdVenta
                    WHERE v.IdCliente = @IdCliente AND dv.IdArticulo = @IdArticulo";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                cmd.Parameters.AddWithValue("@IdArticulo", idArticulo);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                Venta ventaActual = null;
                while (reader.Read())
                {
                    int idVenta = (int)reader["IdVenta"];
                    if (ventaActual == null || ventaActual.IdVenta != idVenta)
                    {
                        ventaActual = new Venta
                        {
                            IdVenta = idVenta,
                            IdCliente = (int)reader["IdCliente"],
                            FechaVenta = (DateTime)reader["FechaVenta"],
                            Total = (decimal)reader["Total"]
                        };
                        ventas.Add(ventaActual);
                    }
                    ventaActual.Detalles.Add(new DetalleVenta
                    {
                        IdDetalleVenta = (int)reader["IdDetalleVenta"],
                        IdVenta = idVenta,
                        IdArticulo = (int)reader["IdArticulo"],
                        Cantidad = (int)reader["Cantidad"],
                        PrecioUnitario = (decimal)reader["PrecioUnitario"],
                        Subtotal = (decimal)reader["Subtotal"]
                    });
                }
            }
            return ventas;
        }
    }
}
