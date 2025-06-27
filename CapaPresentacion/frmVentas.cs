using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VentaMayorista.CapaEntidades;
using VentaMayorista.CapaLogica;

namespace VentaMayorista.CapaPresentacion
{
    public partial class frmVentas : Form
    {
        private readonly VentaLogica ventaLogica = new VentaLogica();
        private readonly ClienteLogica clienteLogica = new ClienteLogica();
        private readonly ArticuloLogica articuloLogica = new ArticuloLogica();
        private Venta venta = new Venta();

        public frmVentas()
        {
            InitializeComponent();
            CargarClientes();
            CargarArticulos();
        }
        private void CargarClientes()
        {
            comboBoxClientes.DataSource = clienteLogica.ObtenerTodos();
            comboBoxClientes.DisplayMember = "Nombre";
            comboBoxClientes.ValueMember = "IdCliente";
        }

        private void CargarArticulos()
        {
            dataGridViewArticulos.DataSource = articuloLogica.ObtenerTodos();
        }

        private void btnAgregarDetalle_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewArticulos.SelectedRows.Count == 0)
                    throw new Exception("Seleccione un artículo.");

                var articulo = (Articulo)dataGridViewArticulos.SelectedRows[0].DataBoundItem;
                if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
                    throw new Exception("La cantidad debe ser un número mayor a 0.");

                // Verificar stock disponible
                if (cantidad > articulo.Stock)
                    throw new Exception($"Stock insuficiente. Disponible: {articulo.Stock}, Solicitado: {cantidad}");

                var detalle = new DetalleVenta
                {
                    IdArticulo = articulo.IdArticulo,
                    Cantidad = cantidad,
                    PrecioUnitario = articulo.PrecioUnitario,
                    Subtotal = cantidad * articulo.PrecioUnitario
                };

                venta.Detalles.Add(detalle);
                dataGridViewDetalles.DataSource = null;
                dataGridViewDetalles.DataSource = venta.Detalles;
                txtCantidad.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnConfirmarVenta_Click(object sender, EventArgs e)
        {
            try
            {
                venta.IdCliente = (int)comboBoxClientes.SelectedValue;
                ventaLogica.RegistrarVenta(venta);
                MessageBox.Show("Venta registrada con éxito", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                venta = new Venta();
                dataGridViewDetalles.DataSource = null;
                txtCantidad.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
