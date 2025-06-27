using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VentaMayorista.CapaPresentacion;

namespace VentaMayorista
{
    public partial class frmPrincipal : Form
    {
        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void btnArticulos_Click(object sender, EventArgs e)
        {
            frmArticulos frmarticulo = new frmArticulos(null);
            frmarticulo.ShowDialog();
        }

        private void btnClientes_Click(object sender, EventArgs e)
        {
            frmClientes frmclientes = new frmClientes();
            frmclientes.ShowDialog();
        }

        private void btnVentas_Click(object sender, EventArgs e)
        {
            frmVentas frmventas = new frmVentas();
            frmventas.ShowDialog();
        }

        private void btnConsultaVentas_Click(object sender, EventArgs e)
        {
            frmConsultaVentas frmconsultaVentas = new frmConsultaVentas();
            frmconsultaVentas.ShowDialog();
        }
    }
}
