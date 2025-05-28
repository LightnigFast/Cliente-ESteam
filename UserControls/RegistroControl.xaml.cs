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
    /// Lógica de interacción para RegistroControl.xaml
    /// </summary>
    public partial class RegistroControl : UserControl
    {
        public delegate void LoginRequested();
        public event LoginRequested VolverAlLogin;

        public RegistroControl()
        {
            InitializeComponent();
        }

        private void VolverLogin_Click(object sender, MouseButtonEventArgs e)
        {
            VolverAlLogin?.Invoke();
        }

        private void Registrarse_Click(object sender, RoutedEventArgs e)
        {
            //lógica de registro
        }
    }
}
