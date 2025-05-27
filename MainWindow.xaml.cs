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
using Cliente_TFG.Classes;
using Cliente_TFG.Pages;
using Cliente_TFG.UserControls;

namespace Cliente_TFG
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Usuario user;
        private bool online = true;
        public Usuario Usuario
        {
            get => user;
            set => user = value;
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = AppTheme.Actual;
            AppTheme.SetDark();
            cargarTema();
            user = new Usuario();

            framePrincipal.Navigated += FramePrincipal_Navigated;

            Cabecera_top.AtrasPresionado += boton_atras_presionado;
            Cabecera_top.AvanzarPresionado += boton_avanzar_presionado;
            Cabecera_top.BibliotecaPresionado += boton_biblioteca_presionado;
            Cabecera_top.TiendaPresionado += boton_tienda_presionado;
            Cabecera_top.AmigosPresionado += boton_amigos_presionado;

            Cabecera_top.VerPerfilPresionado += boton_verPerfil_presionado;


            if (online)
            {
                CargarDatosUsuario(5);
                Cabecera_top.NombreUsuario = user.NombreUsuario;
                Cabecera_top.Dinero = user.Dinero;
            }
            

            CargarPrimeraVentana();


        }

        private void cargarTema()
        {
            mainGrid.Background = AppTheme.Actual.GridPrincipal;
        }

        public void CargarDatosUsuario(int id)
        {
            user.CargarDatos(id);
            
        }

        private void CargarPrimeraVentana()
        {
            //var paginaTienda = new paginaTienda(this);
            //framePrincipal.Navigate(paginaTienda);

            //var paginaTienda = new paginaJuegoTienda(this, 3017860);
            //framePrincipal.Navigate(paginaTienda);

            var paginaBiblioteca = new paginaBiblioteca(this);
            framePrincipal.Navigate(paginaBiblioteca);
        }

        private void FramePrincipal_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is paginaTienda pagina)
            {
                pagina.RestaurarOpacidad();
            }
        }

        private void boton_atras_presionado(object sender, RoutedEventArgs e)
        {
            if (framePrincipal.CanGoBack) 
                framePrincipal.GoBack();
        }

        private void boton_avanzar_presionado(object sender, RoutedEventArgs e)
        {
            if (framePrincipal.CanGoForward)
                framePrincipal.GoForward();
        }

        private void boton_biblioteca_presionado(object sender, RoutedEventArgs e)
        {
            framePrincipal.Navigate(new paginaBiblioteca(this));
        }

        private void boton_tienda_presionado(object sender, RoutedEventArgs e)
        {
            framePrincipal.Navigate(new paginaTienda(this));
        }

        private void boton_amigos_presionado(object sender, RoutedEventArgs e)
        {
            framePrincipal.Navigate(new paginaAmigos());
        }

        private void boton_verPerfil_presionado(object sender, RoutedEventArgs e)
        {
            framePrincipal.Navigate(new paginaPerfil(this));
        }



    }
}
