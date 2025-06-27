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
    public partial class frmComestibles : frmArticulos
    {
        public frmComestibles() : base("Comestibles")
        {
            InitializeComponent();
            this.Text = "Gestión de Comestibles";
            this.Titulo = "Comestibles";
        }
    }
}
