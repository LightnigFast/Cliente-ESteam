using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cliente_TFG.Pages
{
    /// <summary>
    /// Lógica de interacción para paginaBiblioteca.xaml
    /// </summary>
    public partial class paginaBiblioteca : Page
    {
        //DATOS DE PRUEBA
        private string[] appids;
        private string[] Nombres;

        //PARA EL FONDO
        private List<string> imagenesFondo = new List<string>();
        private List<string> imagenesLogos = new List<string>();

        //PARA LOS DEMAS JUEGOS DE LA BIBLIOTECA
        private List<string> imagenVerticalJuegos = new List<string>();

        public paginaBiblioteca()
        {
            InitializeComponent();

            //CARGAR DATOS DE PRUEBA
            CargarDatosDePrueba();

            //PARA EL FONDO
            CargarImagenesFondo();

        }


        private void CargarDatosDePrueba()
        {

            appids = new string[]
            {
                "730",
                "570",
                "578080",
                "252490",
                "2507950",
                "1903340",
                "1086940",
                "3017860",
            };

            Nombres = new string[]
            {
                "Counter Strike 2",
                "Dota 2",
                "PUBG: BATTLEGROUNDS",
                "Rust",
                "2507950",
                "Delta Force",
                "Baldur's Gate 3",
                "DOOM: The Dark Ages",
            };

        }


        //METODO PARA CARGAR LAS IMAGENES DE FONDO DE LA INTERFAZ
        private void CargarImagenesFondo()
        {
            foreach (var appidJuego in appids)
            {
                //IMAGEN FONDO
                string imagenFondo = $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{appidJuego}/library_hero.jpg";
                imagenesFondo.Add(imagenFondo);

                //IMAGEN LOGO
                string iamgenLogo = $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{appidJuego}/logo.png";
                imagenesLogos.Add(iamgenLogo);

                //IMAGEN VERTICAL PARA LA BIBLIOTECA
                string iamgenVerticalJuego = $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{appidJuego}/library_600x900.jpg";
                imagenVerticalJuegos.Add(iamgenVerticalJuego);
            }

            if (imagenesFondo.Count == 0)
            {
                MessageBox.Show("No se encontraron imágenes para el fondo.");
            }
            else
            {
                CargarFondo();
                CargarJuegosBibioteca();
            }
        }


        //METODO PARA CARGAR EL FONDO
        private void CargarFondo()
        {
            imgFondo.Source = new BitmapImage(new Uri(imagenesFondo.First(), UriKind.Absolute));

            ImagenLogo.Source = new BitmapImage(new Uri(imagenesLogos.First(), UriKind.Absolute));

        }

        //METODO PARA CARGAR TODOS LOS DEMAS JUEGOS DE LA BIBLIOTECA
        private void CargarJuegosBibioteca()
        {
            for (int i = 0; i < imagenVerticalJuegos.Count; i++)
            {
                var juegosBiblioteca = imagenVerticalJuegos[i];

                Image img = new Image
                {
                    Source = new BitmapImage(new Uri(juegosBiblioteca)),
                    Stretch = Stretch.Uniform,
                    Height = 170,
                    Margin = new Thickness(5),
                    Tag = i //ALMACENAMOS EL ÍNDICE PARA IDENTIFICAR EL JUEGO
                };

                //EVENTO DE CLIC
                img.MouseLeftButtonUp += ImagenJuego_Click;

                panelJuegosBiblioteca.Children.Add(img);
            }
        }


        private void ImagenJuego_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image img && img.Tag is int index)
            {
                //ANIMACIÓN DE FADE OUT
                var fadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(300)));
                fadeOut.Completed += (s, _) =>
                {
                    //CAMBIAR IMAGEN DESPUÉS DEL FADE OUT
                    imgFondo.Source = new BitmapImage(new Uri(imagenesFondo[index], UriKind.Absolute));
                    ImagenLogo.Source = new BitmapImage(new Uri(imagenesLogos[index], UriKind.Absolute));

                    //ANIMACIÓN DE FADE IN
                    var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(300)));
                    imgFondo.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                    ImagenLogo.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                };

                //INICIAR FADE OUT
                imgFondo.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                ImagenLogo.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }
        }






    }
}
