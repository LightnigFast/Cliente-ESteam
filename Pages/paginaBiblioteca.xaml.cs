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
using Newtonsoft.Json;
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
        private List<string> imagenesFondo = new List<string>();
        private List<string> imagenesLogos = new List<string>();

        //PARA LOS DEMAS JUEGOS DE LA BIBLIOTECA
        private List<string> imagenVerticalJuegos = new List<string>();

        public paginaBiblioteca(MainWindow ventanaPrincipal)
        {
            InitializeComponent();
            this.ventanaPrincipal = ventanaPrincipal;

            //CARGAR DATOS DE PRUEBA
            //CargarDatosDePrueba();

            //CARGAR DATOS DEL JSON
            this.Loaded += async (s, e) =>
            {
                borrarDatos();
                await CargarDatosJson();

                //PARA EL FONDO
                CargarImagenesFondo();
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

        private async Task CargarDatosJson()
        {
            using (HttpClient client = new HttpClient())
            {
               

                try
                {
                    string json = await client.GetStringAsync("http://127.0.0.1:5000/library/" + ventanaPrincipal.Usuario.IdUsuario);
                    var response = JsonConvert.DeserializeObject<BibliotecaResponse>(json);
                        
                    List<string> listaAppids = new List<string>();
                    List<string> listaNombres = new List<string>();

                    foreach (var juego in response.juegos)
                    {
                        string id = juego.app_id.ToString();

                        if (!listaAppids.Contains(id))
                        {
                            listaAppids.Add(id);
                            listaNombres.Add(juego.nombre);
                        }
                    }

                    appids = listaAppids.ToArray();
                    Nombres = listaNombres.ToArray();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR AL CARGAR LA BIBLIOTECA: " + ex.Message);
                }
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
            imgFondo.ImageFailed += (s, e) =>
            {
                Uri fallbackUri = new Uri($"https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/{appids[0]}/header.jpg");
                imgFondo.Source = new BitmapImage((fallbackUri));
            };


            if (imagenesLogos.Count != 0)
            {
                ImagenLogo.Source = new BitmapImage(new Uri(imagenesLogos.First(), UriKind.Absolute));

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
                    Source = new BitmapImage(new Uri(juegosBiblioteca)),
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

                    //CAMBIAMOS IMÁGENES
                    imgFondo.Source = new BitmapImage(new Uri(imagenesFondo[index.Value], UriKind.Absolute));
                    ImagenLogo.Source = new BitmapImage(new Uri(imagenesLogos[index.Value], UriKind.Absolute));

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
            Uri fallbackUri = new Uri($"https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/{appids[0]}/header.jpg");
            imgFondo.Source = new BitmapImage(fallbackUri);
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
