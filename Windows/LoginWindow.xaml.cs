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
using System.Windows.Shapes;

namespace Cliente_TFG.Windows
{
    /// <summary>
    /// Lógica de interacción para VentanaLogin.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        //PARA LA VENTANA DE REGISTRO
        private void AbrirRegistro_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Aquí abrirías la ventana de registro.");
        }

        //METODO PARA PODER CERRAR LA VENTANA
        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //METODO PARA PODER MOVER LA VENTANA CON EL RATON
        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
