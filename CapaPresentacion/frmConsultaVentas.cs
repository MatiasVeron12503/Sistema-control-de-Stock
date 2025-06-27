using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VentaMayorista.CapaLogica;

namespace VentaMayorista.CapaPresentacion
{
    public partial class frmConsultaVentas : Form
    {
        private readonly VentaLogica ventaLogica = new VentaLogica();
        private readonly ClienteLogica clienteLogica = new ClienteLogica();
        private readonly ArticuloLogica articuloLogica = new ArticuloLogica();

        public frmConsultaVentas()
        {
            InitializeComponent();
            CargarClientes();
            CargarArticulos();

        }

        private void CargarClientes()
        {
            try
            {
                var clientes = clienteLogica.ObtenerTodos();
                cmbCliente.DataSource = clientes;
                cmbCliente.DisplayMember = "Nombre"; // Mostrar el nombre del cliente
                cmbCliente.ValueMember = "IdCliente"; // Almacenar el IdCliente
                cmbCliente.SelectedIndex = -1; // No seleccionar nada por defecto
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarArticulos()
        {
            try
            {
                var articulos = articuloLogica.ObtenerTodos();
                cmbArticulo.DataSource = articulos;
                cmbArticulo.DisplayMember = "Nombre"; // Mostrar el nombre del artículo
                cmbArticulo.ValueMember = "IdArticulo"; // Almacenar el IdArticulo
                cmbArticulo.SelectedIndex = -1; // No seleccionar nada por defecto
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar artículos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbCliente.SelectedIndex == -1 || cmbArticulo.SelectedIndex == -1)
                {
                    MessageBox.Show("Por favor, seleccione un cliente y un artículo.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int idCliente = (int)cmbCliente.SelectedValue;
                int idArticulo = (int)cmbArticulo.SelectedValue;

                var ventas = ventaLogica.ConsultarVentasPorArticuloYCliente(idArticulo, idCliente);
                dgvVentas.DataSource = ventas;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar ventas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            cmbCliente.SelectedIndex = -1;
            cmbArticulo.SelectedIndex = -1;
            dgvVentas.DataSource = null;
        }
    }
}
