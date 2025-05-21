using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                /*"730",
                "570",
                "578080",
                "252490",
                "2507950",
                "1903340",
                "1086940",
                "3017860",
                "2456740",
                "805550",
                "2878980",
                "3159330",
                "2909400",
                "2531310",
                "3058630",
                "2669320",
                "1687950",
                "244210",
                "1551360",
                "3164500",
                "2842040",
                "2488620",*/
            };

            Nombres = new string[]
            {
                /*"Counter Strike 2",
                "Dota 2",
                "PUBG: BATTLEGROUNDS",
                "Rust",
                "Delta Force",
                "Clair Obscur: Expedition 33",
                "Baldur's Gate 3",
                "DOOM: The Dark Ages",
                "inZOI",
                "Assetto Corsa Competizione",
                "NBA 2K25",
                "Assassin's Creed Shadows",
                "FINAL FANTASY VII REBIRTH",
                "The Last of Us™ Part II Remastered",
                "Assetto Corsa EVO",
                "EA SPORTS FC 25",
                "Persona 5 Royal",
                "Assetto Corsa",
                "Forza Horizon 5",
                "Schedule I",
                "Star Wars Outlaws",
                "F1® 24",*/
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
                //CARGO UNA IMAGEN ESTATICA
                string imagenFondov2 = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "library", "Banner_no_games_library.png");
                imagenesFondo.Add(imagenFondov2);
                CargarFondo();
                //QUITO EL BOTON
                BotonJugar.IsEnabled = false;
                BotonJugar.Opacity = 0;
                //PONGO EL MENSAJE DE ABAJO
                cargarMensajeBiblioteca("Actualmente no dispones de juegos en tu biblioteca. Accede a la tienda para para comprar tu primer juego");
                //MessageBox.Show("No se encontraron imágenes para el fondo.");
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

            if (imagenesLogos.Count != 0)
            {
                ImagenLogo.Source = new BitmapImage(new Uri(imagenesLogos.First(), UriKind.Absolute));
            }
            

        }

        //METODO PARA CARGAR TODOS LOS DEMAS JUEGOS DE LA BIBLIOTECA
        private void CargarJuegosBibioteca()
        {
            for (int i = 0; i < imagenVerticalJuegos.Count; i++)
            {
                var juegosBiblioteca = imagenVerticalJuegos[i];

                // CREAMOS EL SCALE TRANSFORM PARA APLICAR LA ANIMACIÓN
                var scale = new ScaleTransform(1.0, 1.0);

                Image img = new Image
                {
                    Source = new BitmapImage(new Uri(juegosBiblioteca)),
                    Stretch = Stretch.Uniform,
                    Height = 170,
                    Margin = new Thickness(11),
                    Tag = i,
                    RenderTransform = scale,
                    RenderTransformOrigin = new Point(0.5, 0.5)
                };

                // EVENTO DE CLIC
                img.MouseLeftButtonUp += ImagenJuego_Click;

                // EVENTOS DE ANIMACIÓN AL PASAR EL RATÓN
                img.MouseEnter += (s, e) =>
                {
                    var anim = new DoubleAnimation(1.0, 1.1, TimeSpan.FromMilliseconds(150));
                    scale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
                    scale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
                };

                img.MouseLeave += (s, e) =>
                {
                    var anim = new DoubleAnimation(1.1, 1.0, TimeSpan.FromMilliseconds(150));
                    scale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
                    scale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
                };

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
                    BotonJugar.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                };

                //INICIAR FADE OUT
                imgFondo.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                ImagenLogo.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                BotonJugar.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }
        }


        //PARTE PARA EL BUSCADOR
        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filtro = txtBuscar.Text.ToLower();
            panelJuegosBiblioteca.Children.Clear();

            bool hayCoincidencias = false;

            for (int i = 0; i < imagenVerticalJuegos.Count; i++)
            {
                if (i < Nombres.Length && Nombres[i].ToLower().Contains(filtro))
                {
                    hayCoincidencias = true;

                    string juegosBiblioteca = imagenVerticalJuegos[i];
                    var scale = new ScaleTransform(1.0, 1.0);

                    Image img = new Image
                    {
                        Source = new BitmapImage(new Uri(juegosBiblioteca)),
                        Stretch = Stretch.Uniform,
                        Height = 170,
                        Margin = new Thickness(11),
                        Tag = i,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    img.MouseLeftButtonUp += ImagenJuego_Click;

                    img.MouseEnter += (s2, e2) =>
                    {
                        var anim = new DoubleAnimation(1.0, 1.1, TimeSpan.FromMilliseconds(150));
                        scale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
                        scale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
                    };

                    img.MouseLeave += (s2, e2) =>
                    {
                        var anim = new DoubleAnimation(1.1, 1.0, TimeSpan.FromMilliseconds(150));
                        scale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
                        scale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
                    };

                    panelJuegosBiblioteca.Children.Add(img);
                }
            }

            if (!hayCoincidencias)
            {

                cargarMensajeBiblioteca("Ningún juego coincide con tu búsqueda. Quizás lo encuentres en la tienda.");
                

            }
        }

        private void cargarMensajeBiblioteca(string mensaje)
        {
            Grid gridContenedor = new Grid()
            {
                Width = panelJuegosBiblioteca.ActualWidth,
                Height = panelJuegosBiblioteca.ActualHeight,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            TextBlock textoError = new TextBlock()
            {
                Text = mensaje,
                Foreground = Brushes.White,
                FontSize = 16,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                MaxWidth = 500
            };

            gridContenedor.Children.Add(textoError);
            panelJuegosBiblioteca.Children.Add(gridContenedor);

            panelJuegosBiblioteca.SizeChanged += (s, ev) =>
            {
                gridContenedor.Width = panelJuegosBiblioteca.ActualWidth;
                gridContenedor.Height = panelJuegosBiblioteca.ActualHeight;
            };
        }

    }
}
