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
    /// Lógica de interacción para paginaTienda.xaml
    /// </summary>
    public partial class paginaTienda : Page
    {

        private string[] imagenesCarrusel;
        private int indiceActual = 0;


        public paginaTienda()
        {
            InitializeComponent();

            imagenesCarrusel = new string[]
            {
                "https://cdn.akamai.steamstatic.com/steam/apps/730/capsule_616x353.jpg",
                "https://cdn.akamai.steamstatic.com/steam/apps/570/capsule_616x353.jpg",
                "https://cdn.akamai.steamstatic.com/steam/apps/440/capsule_616x353.jpg"
            };

            MostrarImagenActual();
        }

        private void MostrarImagenActual()
        {
            imagenTiendaGrande.Source = new BitmapImage(new Uri(imagenesCarrusel[indiceActual]));
        }

        private void BtnAnterior_Click(object sender, RoutedEventArgs e)
        {
            if (indiceActual > 0)
            {
                indiceActual--;
                MostrarImagenActual();
            }
        }

        private void BtnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            if (indiceActual < imagenesCarrusel.Length - 1)
            {
                indiceActual++;
                MostrarImagenActual();
            }
        }

    }
}
