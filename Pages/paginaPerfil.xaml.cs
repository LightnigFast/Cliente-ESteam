using Cliente_TFG.Classes;
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

namespace Cliente_TFG.Pages
{
    /// <summary>
    /// Lógica de interacción para paginaPerfil.xaml
    /// </summary>
    /// 

    public partial class paginaPerfil : Page
    {

        private MainWindow ventanaPrincipal;

        public paginaPerfil(MainWindow ventanaPrincipal)
        {
            InitializeComponent();
            this.ventanaPrincipal = ventanaPrincipal;

            cargarTemas();

            cargarDatosUser();
        }

        private void cargarTemas()
        {
            nombreUser.Foreground = AppTheme.Actual.TextoPrincipal;
            nombreCuenta.Foreground = AppTheme.Actual.TextoPrincipal;
            descripccionCuenta.Foreground = AppTheme.Actual.TextoPrincipal;
            txtBiografia.Foreground = AppTheme.Actual.TextoPrincipal;

            gridCnetral.Background = AppTheme.Actual.FondoPanel;

        }

        private void cargarDatosUser()
        {
            Usuario usuario = ventanaPrincipal.Usuario;

            nombreUser.Text = usuario.NombreUsuario;
            nombreCuenta.Text = usuario.NombreCuenta;
            descripccionCuenta.Text = usuario.Descripcion;

            string urlImagen = usuario.FotoPerfil;

            if (!string.IsNullOrEmpty(urlImagen) && Uri.IsWellFormedUriString(urlImagen, UriKind.Absolute))
            {
                imagenUser.Source = new BitmapImage(new Uri(urlImagen, UriKind.Absolute));
            }
            else
            {
                string rutaImagenLocal = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "imagenes", "default.png"); // Esta foto es la default. Por si no se ponen nada. 
                imagenUser.Source = new BitmapImage(new Uri(rutaImagenLocal, UriKind.Absolute));
                MessageBox.Show("URL de la foto inválida o vacía: " + urlImagen);
            }
        }
    }
}
