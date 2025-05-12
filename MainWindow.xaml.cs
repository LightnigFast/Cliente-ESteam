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
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = AppTheme.Actual;

            Cabecera_top.BibliotecaPresionado += boton_biblioteca_presionado;
            Cabecera_top.TiendaPresionado += boton_tienda_presionado;
            Cabecera_top.AmigosPresionado += boton_amigos_presionado;


        }

        private void boton_biblioteca_presionado(object sender, RoutedEventArgs e)
        {
            framePrincipal.Navigate(new paginaBiblioteca());
        }

        private void boton_tienda_presionado(object sender, RoutedEventArgs e)
        {
            framePrincipal.Navigate(new paginaTienda());
        }

        private void boton_amigos_presionado(object sender, RoutedEventArgs e)
        {
            framePrincipal.Navigate(new paginaAmigos());
        }
    }
}
