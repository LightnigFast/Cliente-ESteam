using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cliente_TFG.UserControls
{
    /// <summary>
    /// Lógica de interacción para LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        public delegate void RegistroRequested();
        public event RegistroRequested AbrirRegistro;



        public LoginControl()
        {
            InitializeComponent();
        }

        private void AbrirRegistro_Click(object sender, MouseButtonEventArgs e)
        {
            AbrirRegistro?.Invoke();
        }

    }
}
