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
            CargarImagenesOfertas();
        }

        //PARTE PARA EL CARRUSEL
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

        //PARTE DE LAS OFERTES ESPECIALES
        private void CargarImagenesOfertas()
        {
            string[] urls = new string[]
            {
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/23400/header.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/620/header.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/570/header.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/440/header.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/header.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/550/header.jpg"
            };

            int columnas = 3;



            for (int i = 0; i < urls.Length; i++)
            {
                int row = i / columnas;
                int col = i % columnas;

                //CREO LA IMAGEN
                Image img = new Image
                {
                    Source = new BitmapImage(new Uri(urls[i])),
                    Stretch = Stretch.UniformToFill,
                    Height = 160,
                    Margin = new Thickness(0)
                };

                //TEXTO DEL PRECIO
                TextBlock textoPrecio = new TextBlock
                {
                    Text = "15,99" + "€ ",
                    Height = 25,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Right,
                    FontSize = 17,
                    Foreground = (Brush)new BrushConverter().ConvertFromString("#baee12"),
                    Background = (Brush)new BrushConverter().ConvertFromString("#533939"),

                };

                //STACKPANEL PARA METER LA IMAGEN Y EL PRECIO
                StackPanel contenedor = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(5)
                };

                contenedor.Children.Add(img);
                contenedor.Children.Add(textoPrecio);

                Grid.SetRow(contenedor, row);
                Grid.SetColumn(contenedor, col);
                panelOfertasEspaciales.Children.Add(contenedor);
            }

        }



    }
}
