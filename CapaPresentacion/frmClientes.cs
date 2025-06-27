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
    public partial class frmClientes : Form
    {
        private readonly ClienteLogica clienteLogica = new ClienteLogica();
        private Cliente clienteSeleccionado;

        public frmClientes()
        {
            InitializeComponent();
            CargarClientes();
        }
        private void CargarClientes()
        {
            try
            {
                dataGridViewClientes.DataSource = clienteLogica.ObtenerTodos();
                // Opcional: Ocultar la columna FechaCreacion si no es relevante
                dataGridViewClientes.Columns["FechaCreacion"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar campos obligatorios
                if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtApellido.Text) ||
                    string.IsNullOrWhiteSpace(txtDNI.Text))
                {
                    MessageBox.Show("Los campos Nombre, Apellido y DNI son obligatorios.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Cliente cliente = new Cliente
                {
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    DNI = txtDNI.Text.Trim(),
                    Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                    Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim()
                };

                clienteLogica.Crear(cliente);
                MessageBox.Show("Cliente agregado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarClientes();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar el cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewClientes.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Seleccione un cliente para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                clienteSeleccionado = (Cliente)dataGridViewClientes.SelectedRows[0].DataBoundItem;
                txtNombre.Text = clienteSeleccionado.Nombre;
                txtApellido.Text = clienteSeleccionado.Apellido;
                txtDNI.Text = clienteSeleccionado.DNI;
                txtTelefono.Text = clienteSeleccionado.Telefono;
                txtEmail.Text = clienteSeleccionado.Email;

                btnAgregar.Enabled = false; // Deshabilitar Agregar durante edición
                btnGuardar.Enabled = true; // Habilitar Guardar
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos del cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewClientes.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Seleccione un cliente para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var cliente = (Cliente)dataGridViewClientes.SelectedRows[0].DataBoundItem;
                var result = MessageBox.Show($"¿Está seguro de que desea eliminar al cliente {cliente.Nombre} {cliente.Apellido}?",
                                             "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    clienteLogica.Eliminar(cliente.IdCliente);
                    MessageBox.Show("Cliente eliminado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarClientes();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (clienteSeleccionado == null)
                {
                    MessageBox.Show("No hay un cliente seleccionado para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validar campos obligatorios
                if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtApellido.Text) ||
                    string.IsNullOrWhiteSpace(txtDNI.Text))
                {
                    MessageBox.Show("Los campos Nombre, Apellido y DNI son obligatorios.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                clienteSeleccionado.Nombre = txtNombre.Text.Trim();
                clienteSeleccionado.Apellido = txtApellido.Text.Trim();
                clienteSeleccionado.DNI = txtDNI.Text.Trim();
                clienteSeleccionado.Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim();
                clienteSeleccionado.Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim();

                clienteLogica.Actualizar(clienteSeleccionado);
                MessageBox.Show("Cliente actualizado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarClientes();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar el cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtDNI.Clear();
            txtTelefono.Clear();
            txtEmail.Clear();
            clienteSeleccionado = null;
            btnAgregar.Enabled = true;
            btnGuardar.Enabled = false;
        }

    }
}
