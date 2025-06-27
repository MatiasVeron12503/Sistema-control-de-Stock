using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VentaMayorista.CapaPresentacion
{
    public partial class frmElectrodomesticos : frmArticulos
    {
        public frmElectrodomesticos() : base("Electrodomésticos")
        {
            InitializeComponent();
            this.Text = "Gestión de Electrodomésticos";
            this.Titulo = "Electrodomésticos";
        }
    }
}
