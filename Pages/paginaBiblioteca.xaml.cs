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
using Cliente_TFG.Classes;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
namespace Cliente_TFG.Pages
{
    /// <summary>
    /// Lógica de interacción para paginaBiblioteca.xaml
    /// </summary>
    public partial class paginaBiblioteca : Page
    {
        private MainWindow ventanaPrincipal;

        //DATOS DE PRUEBA
        private string[] appids;
        private string[] Nombres;

        //PARA EL FONDO
        private List<BitmapImage> imagenesFondo = new List<BitmapImage>();
        private List<BitmapImage> imagenesLogos = new List<BitmapImage>();
        private List<BitmapImage> imagenVerticalJuegos = new List<BitmapImage>();

        //PARA LOS DEMAS JUEGOS DE LA BIBLIOTECA

        public paginaBiblioteca(MainWindow ventanaPrincipal)
        {
            InitializeComponent();
            this.ventanaPrincipal = ventanaPrincipal;

            this.Loaded += async (s, e) =>
            {
                borrarDatos();


                CargarDatosJson();


                await CargarImagenesFondo();
            };
        }


        //METODO PARA BORRAR LOS DATOS CUANDO VUELVAS ATRAS (SI NO SE HACE, SE DUPLICAN LOS JUEGOS)
        private void borrarDatos()
        {
            appids = Array.Empty<string>();
            Nombres = Array.Empty<string>();
            imagenesFondo.Clear();
            imagenesLogos.Clear();
            imagenVerticalJuegos.Clear();
            panelJuegosBiblioteca.Children.Clear();
        }

        private void CargarDatosJson()
        {
            try
            {
                // Cargar la biblioteca local (lista de appids)
                List<int> listaAppids = LocalStorage.CargarBiblioteca();

                if (listaAppids == null || listaAppids.Count == 0)
                {
                    Console.WriteLine("No hay juegos en la biblioteca local.");
                    appids = new string[0];
                    Nombres = new string[0]; // si quieres nombres tendrás que obtenerlos de otro sitio
                    return;
                }

                // Convertir a string[]
                appids = listaAppids.Select(id => id.ToString()).ToArray();

                // Como no tienes nombres en el JSON, simplemente rellena Nombres con valores vacíos o iguales a appids
                Nombres = appids.Select(id => $"Juego {id}").ToArray();

                Console.WriteLine($"Se cargaron {appids.Length} appids desde biblioteca local.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR AL CARGAR LA BIBLIOTECA LOCAL: " + ex.Message);
                appids = new string[0];
                Nombres = new string[0];
            }
        }



        public class Juego
        {
            public int app_id { get; set; }
            public string captura { get; set; }
            public string nombre { get; set; }
        }

        public class BibliotecaResponse
        {
            public List<Juego> juegos { get; set; }
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
                "2488620",
            };

            Nombres = new string[]
            {
                "Counter Strike 2",
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
                "F1® 24",
            };

        }


        //METODO PARA CARGAR LAS IMAGENES DE FONDO DE LA INTERFAZ
        private async Task CargarImagenesFondo()
        {

            imagenesFondo.Clear();
            imagenesLogos.Clear();
            imagenVerticalJuegos.Clear();

            foreach (var appidJuego in appids)
            {
                string nombreFondo = $"{appidJuego}_fondo.jpg";
                string urlFondo = $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{appidJuego}/library_hero.jpg";
                var imagenFondo = await ObtenerImagenAsync(urlFondo, nombreFondo);
                if (imagenFondo != null)
                    imagenesFondo.Add(imagenFondo);

                string nombreLogo = $"{appidJuego}_logo.png";
                string urlLogo = $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{appidJuego}/logo.png";
                var imagenLogo = await ObtenerImagenAsync(urlLogo, nombreLogo);
                if (imagenLogo != null)
                    imagenesLogos.Add(imagenLogo);

                string nombreVertical = $"{appidJuego}_vertical.jpg";
                string urlVertical = $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{appidJuego}/library_600x900.jpg";
                var imagenVertical = await ObtenerImagenAsync(urlVertical, nombreVertical);
                if (imagenVertical != null)
                    imagenVerticalJuegos.Add(imagenVertical);
            }

            if (imagenesFondo.Count == 0)
            {
                // Cargar imagen por defecto si no hay fondos
                var fallbackPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "library", "Banner_no_games_library.png");
                var fallbackImage = new BitmapImage(new Uri(fallbackPath));
                imagenesFondo.Add(fallbackImage);
            }

            CargarFondo();
            CargarJuegosBibioteca();
        }


        private async Task<BitmapImage> ObtenerImagenAsync(string url, string nombreArchivo)
        {
            // PRIMERO INTENTA CARGAR LOCAL
            if (LocalStorage.ExisteImagen(nombreArchivo))
            {
                return LocalStorage.CargarImagenLocal(nombreArchivo);
            }

            // SI NO EXISTE LOCAL, INTENTA DESCARGAR Y GUARDAR
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var bytes = await client.GetByteArrayAsync(url);
                    LocalStorage.GuardarImagen(nombreArchivo, bytes);
                    return LocalStorage.CargarImagenLocal(nombreArchivo);
                }
            }
            catch
            {
                // SI FALLA DESCARGAR, DEVUELVE NULL
                return null;
            }
        }




        //METODO PARA CARGAR EL FONDO
        private void CargarFondo()
        {
            imgFondo.Source = imagenesFondo.First();
            imgFondo.ImageFailed += (s, e) =>
            {
                Uri fallbackUri = new Uri($"https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/{appids[0]}/header.jpg");
                imgFondo.Source = new BitmapImage(fallbackUri);
            };

            if (imagenesLogos.Count != 0)
            {
                ImagenLogo.Source = imagenesLogos.First();
                ImagenLogo.ImageFailed += (s, e) =>
                {
                    txtFalloLogo.Background = (Brush)(new BrushConverter().ConvertFrom("#80000000"));
                    txtFalloLogo.Text = Nombres[0];
                };
            }
        }


        //METODO PARA CARGAR TODOS LOS DEMAS JUEGOS DE LA BIBLIOTECA
        private void CargarJuegosBibioteca()
        {
            for (int i = 0; i < imagenVerticalJuegos.Count; i++)
            {
                int currentIndex = i;
                var juegosBiblioteca = imagenVerticalJuegos[currentIndex];

                var scale = new ScaleTransform(1.0, 1.0);
                double altura = 170;
                double proporcion = 600.0 / 900.0;
                double ancho = altura * proporcion;

                Image img = new Image
                {
                    Source = juegosBiblioteca,
                    Stretch = Stretch.Uniform,
                    Height = altura,
                    Margin = new Thickness(11),
                    Tag = i,
                    RenderTransform = scale,
                    RenderTransformOrigin = new Point(0.5, 0.5)
                };

                img.ImageFailed += (s, e) =>
                {
                    var appid = appids[currentIndex];
                    Uri fallbackUri = new Uri($"https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/{appid}/header.jpg");

                    //FONDO DIFUMINADO
                    Image fondoDifuminado = new Image
                    {
                        Source = new BitmapImage(fallbackUri),
                        Stretch = Stretch.UniformToFill,
                        Height = altura,
                        Width = ancho,
                        Effect = new System.Windows.Media.Effects.BlurEffect { Radius = 10 },
                        Opacity = 0.6
                    };

                    //IMAGEN ENCIMA
                    Image imagenPrincipal = new Image
                    {
                        Source = new BitmapImage(fallbackUri),
                        Stretch = Stretch.Uniform,
                        Height = altura,
                        Width = ancho,
                    };

                    Grid contenedor = new Grid
                    {
                        Height = altura,
                        Margin = new Thickness(11),
                        Tag = currentIndex,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    contenedor.Children.Add(fondoDifuminado);
                    contenedor.Children.Add(imagenPrincipal);

                    //EVENTOS
                    contenedor.MouseLeftButtonUp += ImagenJuego_Click;

                    contenedor.MouseEnter += (s2, e2) =>
                    {
                        var anim = new DoubleAnimation(1.0, 1.1, TimeSpan.FromMilliseconds(150));
                        scale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
                        scale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
                    };

                    contenedor.MouseLeave += (s2, e2) =>
                    {
                        var anim = new DoubleAnimation(1.1, 1.0, TimeSpan.FromMilliseconds(150));
                        scale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
                        scale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
                    };

                    //SUSTITUIS LA IMAGEN POR EL GRID
                    int pos = panelJuegosBiblioteca.Children.IndexOf((UIElement)s);
                    if (pos >= 0)
                    {
                        panelJuegosBiblioteca.Children.RemoveAt(pos);
                        panelJuegosBiblioteca.Children.Insert(pos, contenedor);
                    }
                };

                //ANIMACIONES SI CARGA BIEN
                img.MouseLeftButtonUp += ImagenJuego_Click;

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
            int? index = null;

            if (sender is Image img && img.Tag is int i1)
                index = i1;
            else if (sender is Grid grid && grid.Tag is int i2)
                index = i2;

            if (index.HasValue)
            {
                var fadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(300)));
                fadeOut.Completed += (s, _) =>
                {
                    //QUITAMOS MANEJADORES ANTERIORES
                    imgFondo.ImageFailed -= ImgFondo_ImageFailed;
                    ImagenLogo.ImageFailed -= ImagenLogo_ImageFailed;
                    txtFalloLogo.Background = Brushes.Transparent;

                    //ASIGNAMOS TAG PARA SABER QUÉ ÍNDICE ES EN EL EVENTO
                    ImagenLogo.Tag = index;
                    imgFondo.Tag = index;

                    //CAMBIAMOS IMÁGENES
                    imgFondo.Source = imagenesFondo[index.Value];
                    ImagenLogo.Source = imagenesLogos[index.Value];

                    //ASIGNAMOS MANEJADORES DE NUEVO
                    imgFondo.ImageFailed += ImgFondo_ImageFailed;
                    ImagenLogo.ImageFailed += ImagenLogo_ImageFailed;

                    txtFalloLogo.Text = "";

                    var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(300)));
                    imgFondo.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                    ImagenLogo.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                    BotonJugar.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                };

                imgFondo.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                ImagenLogo.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                BotonJugar.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }
        }

        private void ImgFondo_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (sender is Image img && img.Tag is int index)
            {
                Uri fallbackUri = new Uri($"https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/{appids[index]}/header.jpg");
                img.Source = new BitmapImage(fallbackUri);
            }
        }

        private void ImagenLogo_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (sender is Image img && img.Tag is int idx)
            {
                txtFalloLogo.Background = (Brush)(new BrushConverter().ConvertFrom("#80000000"));
                txtFalloLogo.Text = Nombres[idx];
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

                    BitmapImage juegosBiblioteca = imagenVerticalJuegos[i]; // ✅
                    var scale = new ScaleTransform(1.0, 1.0);

                    Image img = new Image
                    {
                        Source = juegosBiblioteca,
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
