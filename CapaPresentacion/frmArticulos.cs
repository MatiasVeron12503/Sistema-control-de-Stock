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
    public partial class frmArticulos : Form
    {
        private readonly ArticuloLogica articuloLogica = new ArticuloLogica();
        private readonly string categoria;
        private Articulo articuloEditando = null;

        // Constructor sin parámetros para el diseñador
        public frmArticulos() : this(null)
        {
        }

        public frmArticulos(string categoria)
        {
            InitializeComponent();
            this.categoria = categoria;
            this.Text = $"Gestión de {categoria ?? "Artículos"}";
            if (!DesignMode)
            {
                Titulo = categoria ?? "Artículos";
            }
        }

        public string Titulo
        {
            get => lblTitulo.Text;
            set => lblTitulo.Text = value;
        }

        private void frmArticulos_Load(object sender, EventArgs e)
        {
            if (!DesignMode) // Evita ejecutar esto en el diseñador
            {
                CargarArticulos();
                ConfigurarControles();
            }
        }

        private void CargarArticulos()
        {
            try
            {
                var articulos = string.IsNullOrEmpty(categoria)
                    ? articuloLogica.ObtenerTodos()
                    : articuloLogica.ObtenerPorCategoria(categoria);
                dgvArticulos.DataSource = articulos;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarControles()
        {
            cmbCategoria.Items.AddRange(new[] { "Comestibles", "Librería", "Electrodomésticos" });
            if (!string.IsNullOrEmpty(categoria))
            {
                cmbCategoria.SelectedItem = categoria;
                cmbCategoria.Enabled = false; // Bloquear categoría en formularios específicos
            }
            LimpiarFormulario();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                Articulo articulo = new Articulo
                {
                    Nombre = txtNombre.Text.Trim(),
                    Categoria = cmbCategoria.SelectedItem?.ToString(),
                    PrecioUnitario = decimal.Parse(txtPrecio.Text),
                    Stock = int.Parse(txtStock.Text)
                };
                articuloLogica.Crear(articulo);
                MessageBox.Show("Artículo creado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarArticulos();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvArticulos.SelectedRows.Count > 0)
            {
                try
                {
                    var id = (int)dgvArticulos.SelectedRows[0].Cells["IdArticulo"].Value;
                    articuloEditando = articuloLogica.ObtenerPorId(id);
                    txtNombre.Text = articuloEditando.Nombre;
                    cmbCategoria.SelectedItem = articuloEditando.Categoria;
                    txtPrecio.Text = articuloEditando.PrecioUnitario.ToString();
                    txtStock.Text = articuloEditando.Stock.ToString();
                    btnGuardar.Enabled = true;
                    btnCancelar.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (articuloEditando == null) return;

            try
            {
                articuloEditando.Nombre = txtNombre.Text.Trim();
                articuloEditando.Categoria = cmbCategoria.SelectedItem?.ToString();
                articuloEditando.PrecioUnitario = decimal.Parse(txtPrecio.Text);
                articuloEditando.Stock = int.Parse(txtStock.Text);
                articuloLogica.Actualizar(articuloEditando);
                MessageBox.Show("Artículo actualizado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarArticulos();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvArticulos.SelectedRows.Count > 0)
            {
                var id = (int)dgvArticulos.SelectedRows[0].Cells["IdArticulo"].Value;
                if (MessageBox.Show("¿Confirma que desea eliminar este artículo?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        articuloLogica.Eliminar(id);
                        MessageBox.Show("Artículo eliminado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarArticulos();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void LimpiarFormulario()
        {
            txtNombre.Clear();
            txtPrecio.Clear();
            txtStock.Clear();
            if (string.IsNullOrEmpty(categoria))
                cmbCategoria.SelectedIndex = -1;
            articuloEditando = null;
            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;
        }

        private void comestiblesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmComestibles().ShowDialog();
        }

        private void libreriaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmLibreria().ShowDialog();
        }

        private void electrodomesticosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmElectrodomesticos().ShowDialog();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }
    }
}
