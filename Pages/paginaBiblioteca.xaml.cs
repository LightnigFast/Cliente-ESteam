using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Windows.Media.Effects;
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
        private List<int> listaAppids;
        private List<string> nombresJuegos;
        private BibliotecaResponse bibliotecaTotal = new BibliotecaResponse
        {
            juegos = new List<Juego>()
        };

        public paginaBiblioteca(MainWindow ventanaPrincipal)
        {
            InitializeComponent();
            this.ventanaPrincipal = ventanaPrincipal;

            this.Loaded += async (s, e) =>
            {
                borrarDatos();

                CargarDatosJson();

                //OBTENEMOS LA BIBLIOTECA PRIMERO
                if (ventanaPrincipal.Online)
                    await ObtenerBibliotecaDesdeApiAsync();

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
                //Cargar la biblioteca local (lista de appids)
                listaAppids = LocalStorage.CargarBiblioteca();
                nombresJuegos = LocalStorage.CargarBibliotecaNombreJuegos();

                if (listaAppids == null || listaAppids.Count == 0)
                {
                    Console.WriteLine("No hay juegos en la biblioteca local.");
                    appids = new string[0];
                    Nombres = new string[0]; 
                    return;
                }

                //Convertir a string[]
                appids = listaAppids.Select(id => id.ToString()).ToArray();

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



        public async Task<BibliotecaResponse> ObtenerBibliotecaDesdeApiAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = "http://" + Config.IP + $":50000/library/" + ventanaPrincipal.Usuario.IdUsuario;
                    //MessageBox.Show($"Llamando a API: {url}");

                    string json = await client.GetStringAsync(url);
                    //MessageBox.Show($"Respuesta API: {json.Substring(0, Math.Min(200, json.Length))}");

                    BibliotecaResponse response = JsonConvert.DeserializeObject<BibliotecaResponse>(json);

                    if (response?.juegos != null)
                    {
                        bibliotecaTotal.juegos = response.juegos;
                    }
                    else
                    {
                        bibliotecaTotal.juegos = new List<Juego>();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error obteniendo biblioteca: " + ex.Message);
                    bibliotecaTotal.juegos = new List<Juego>();
                }
            }

            return bibliotecaTotal;
        }







        public class Juego
        {
            public int app_id { get; set; }
            public string captura { get; set; }
            public string nombre { get; set; }
            public string header { get; set; }
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
                if (imagenFondo == null)
                {
                    //IMAGEN INVÁLIDA PARA FORZAR IMAGEFAILED
                    imagenFondo = GenerarImagenFondoFallback(appidJuego, nombreFondo);
                }
                else
                {
                    
                    imagenesFondo.Add(imagenFondo);
                }

                string nombreLogo = $"{appidJuego}_logo.png";
                string urlLogo = $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{appidJuego}/logo.png";
                var imagenLogo = await ObtenerImagenAsync(urlLogo, nombreLogo);
                if (imagenLogo != null)
                {
                    imagenesLogos.Add(imagenLogo);
                }
                else
                {
                    //IMAGEN INVÁLIDA PARA FORZAR IMAGEFAILED
                    imagenesLogos.Add(null);
                }

                string nombreVertical = $"{appidJuego}_vertical.jpg";
                string urlVertical = $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{appidJuego}/library_600x900.jpg";
                var imagenVertical = await ObtenerImagenAsync(urlVertical, nombreVertical);

                if (imagenVertical == null)
                {
                    imagenVertical = GenerarImagenFallback(appidJuego, nombreVertical);
                }

                if (imagenVertical != null)
                {
                    
                    imagenVerticalJuegos.Add(imagenVertical);
                }
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
            //PRIMERO INTENTA CARGAR LOCAL
            if (LocalStorage.ExisteImagen(nombreArchivo))
            {
                return LocalStorage.CargarImagenLocal(nombreArchivo);
            }

            // I NO EXISTE LOCAL, INTENTA DESCARGAR Y GUARDAR
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
                //SI FALLA DESCARGAR, DEVUELVE NULL
                return null;
            }
        }

        private BitmapImage GenerarImagenFondoFallback(string appid, string nombreVertical)
        {
            // INTENTA CARGAR LA IMAGEN LOCAL
            if (LocalStorage.ExisteImagen(nombreVertical))
            {
                return LocalStorage.CargarImagenLocal(nombreVertical);
            }

            int id = int.Parse(appid);
            var juego = bibliotecaTotal?.juegos?.FirstOrDefault(j => j.app_id == id);

            if (juego == null)
                return null;

            string fallbackUrl = juego.header ?? juego.captura;

            if (string.IsNullOrEmpty(fallbackUrl))
                return null;

            try
            {
                using (WebClient client = new WebClient())
                {
                    byte[] datosImagen = client.DownloadData(fallbackUrl);

                    // GUARDAR LA IMAGEN EN DISCO
                    string nombreArchivo = $"{appid}_fondo.jpg";
                    LocalStorage.GuardarImagen(nombreArchivo, datosImagen);

                    // CREAR BitmapImage DESDE LOS DATOS EN MEMORIA
                    BitmapImage bmp = new BitmapImage();
                    using (MemoryStream ms = new MemoryStream(datosImagen))
                    {
                        bmp.BeginInit();
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.StreamSource = ms;
                        bmp.EndInit();
                        bmp.Freeze(); // NECESARIO SI USARÁS ESTA IMAGEN EN OTRO HILO
                    }

                    return bmp;
                }
            }
            catch (Exception)
            {
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
                if(imagenesLogos.First() == null)
                {
                    txtFalloLogo.Background = (Brush)(new BrushConverter().ConvertFrom("#80000000"));
                    txtFalloLogo.Text = nombresJuegos[0];
                }
                
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


        private async Task<BitmapImage> CargarImagenFallbackFondo(string appid)
        {
            string nombreHeader = $"{appid}_header.jpg";
            string urlHeader = $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{appid}/header.jpg";
            return await ObtenerImagenAsync(urlHeader, nombreHeader);
        }




        //METODO PARA GENERAR LA IMAGEN SI NO SE CARGA CORRECTAMENTE POR QUE NO EXISTE
        private BitmapImage GenerarImagenFallback(string appid, string nombreVertical)
        {
            //INTENTO CARGAR EL LOCAL PRIMERO
            if (LocalStorage.ExisteImagen(nombreVertical))
            {
                return LocalStorage.CargarImagenLocal(nombreVertical);
            }

            int id = int.Parse(appid);
            var juego = bibliotecaTotal?.juegos?.FirstOrDefault(j => j.app_id == id);

            if (juego == null)
                return null;

            string fallbackUrl = juego.header ?? juego.captura;

            if (string.IsNullOrEmpty(fallbackUrl))
                return null;

            try
            {
                //DECARGAR LA IMAGEN EN MEMORIA
                byte[] imageBytes;
                using (var httpClient = new HttpClient())
                {
                    imageBytes = httpClient.GetByteArrayAsync(fallbackUrl).GetAwaiter().GetResult();
                }


                BitmapImage imagenCargada = new BitmapImage();
                using (var ms = new MemoryStream(imageBytes))
                {
                    imagenCargada.BeginInit();
                    imagenCargada.CacheOption = BitmapCacheOption.OnLoad;
                    imagenCargada.StreamSource = ms;
                    imagenCargada.EndInit();
                    imagenCargada.Freeze();
                }

                if (imagenCargada.PixelWidth == 0 || imagenCargada.PixelHeight == 0)
                {
                    MessageBox.Show("La imagen no se cargó correctamente o está vacía.");
                    return null;
                }

                int anchoFinal = 300;
                int altoFinal = 450;

                double ratioOriginal = (double)imagenCargada.PixelWidth / imagenCargada.PixelHeight;
                double ratioDestino = (double)anchoFinal / altoFinal;

                //CALCULAMOS EL TAMAÑO DE LA IMAGEN PEQUEÑA PARA QUE CUMPLA LA PROPORCION
                int anchoRender;
                int altoRender;
                if (ratioOriginal > ratioDestino)
                {
                    anchoRender = anchoFinal;
                    altoRender = (int)(anchoFinal / ratioOriginal);
                }
                else
                {
                    altoRender = altoFinal;
                    anchoRender = (int)(altoFinal * ratioOriginal);
                }
                int desplazamientoY = (altoFinal - altoRender) / 2;

                //IMAGEN DE FONDO CON BLUR
                double ratioFondo = (double)imagenCargada.PixelWidth / imagenCargada.PixelHeight;

                int anchoFondo, altoFondo;
                if (ratioFondo > (double)anchoFinal / altoFinal)
                {
                    altoFondo = altoFinal;
                    anchoFondo = (int)(altoFinal * ratioFondo);
                }
                else
                {
                    anchoFondo = anchoFinal;
                    altoFondo = (int)(anchoFinal / ratioFondo);
                }

                int offsetX = (anchoFinal - anchoFondo) / 2;
                int offsetY = (altoFinal - altoFondo) / 2;

                DrawingVisual fondoVisual = new DrawingVisual();
                using (DrawingContext dc = fondoVisual.RenderOpen())
                {
                    dc.DrawImage(imagenCargada, new Rect(offsetX, offsetY, anchoFondo, altoFondo));
                }
                RenderTargetBitmap fondoBitmap = new RenderTargetBitmap(anchoFinal, altoFinal, 96, 96, PixelFormats.Pbgra32);
                fondoBitmap.Render(fondoVisual);

                //APLICAR BLUR A LA IMAGEN
                DrawingVisual blurVisual = new DrawingVisual();
                blurVisual.Effect = new BlurEffect { Radius = 15 };
                using (DrawingContext dc = blurVisual.RenderOpen())
                {
                    dc.DrawImage(fondoBitmap, new Rect(0, 0, anchoFinal, altoFinal));
                }
                RenderTargetBitmap fondoBlurred = new RenderTargetBitmap(anchoFinal, altoFinal, 96, 96, PixelFormats.Pbgra32);
                fondoBlurred.Render(blurVisual);

                //CREAMOS LA IAMGEN CENTRADA Y DETRAS LA DE FONDO CON BLUR
                DrawingVisual finalVisual = new DrawingVisual();
                using (DrawingContext dc = finalVisual.RenderOpen())
                {
                    //FONDO BORROSO
                    dc.DrawImage(fondoBlurred, new Rect(0, 0, anchoFinal, altoFinal));

                    //IMAGEN PEQUEÑA
                    dc.DrawImage(imagenCargada, new Rect((anchoFinal - anchoRender) / 2, desplazamientoY, anchoRender, altoRender));
                }

                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(anchoFinal, altoFinal, 96, 96, PixelFormats.Pbgra32);
                renderBitmap.Render(finalVisual);

                //GUARDAR IMAGEN RENDERIZADA
                string nombreArchivoRender = $"{appid}_vertical.jpg";
                LocalStorage.GuardarImagenRenderizada(nombreArchivoRender, renderBitmap);

                var bmp = new BitmapImage();
                using (var stream = new MemoryStream())
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                    encoder.Save(stream);
                    stream.Position = 0;

                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.StreamSource = stream;
                    bmp.EndInit();
                    bmp.Freeze();
                }

                return bmp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
                return null;
            }
        }

        public static BitmapSource RenderVisualToBitmap(UIElement element, int width, int height, double dpi = 96)
        {
            var renderTarget = new RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.Pbgra32);
            element.Measure(new Size(width, height));
            element.Arrange(new Rect(new Size(width, height)));
            renderTarget.Render(element);
            return renderTarget;
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
                    txtFalloLogo.Text = "";

                    //ASIGNAMOS TAG PARA SABER QUÉ ÍNDICE ES EN EL EVENTO
                    ImagenLogo.Tag = index;
                    imgFondo.Tag = index;


                    //ASIGNAMOS MANEJADORES DE NUEVO PRIMERO
                    imgFondo.ImageFailed += ImgFondo_ImageFailed;
                    ImagenLogo.ImageFailed += ImagenLogo_ImageFailed;

                    //CAMBIAMOS IMÁGENES
                    imgFondo.Source = imagenesFondo[index.Value];

                    if (imagenesLogos[index.Value] != null)
                        ImagenLogo.Source = imagenesLogos[index.Value];
                    else
                        ImagenLogo.Source = new BitmapImage(new Uri("https://noexiste.este.dominio/logo.png"));



                    //ANIMACIÓN ENTRADA
                    var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(300)));
                    imgFondo.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                    ImagenLogo.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                    txtFalloLogo.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                    BotonJugar.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                };

                //APLICAMOS LA ANIMACIÓN PARA QUE SE DISPARA EL .Completed
                imgFondo.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                ImagenLogo.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                txtFalloLogo.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                BotonJugar.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }
        }


        private async void ImgFondo_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (sender is Image img && img.Tag is int index)
            {
                string nombreFondo = $"{appids[index]}_fondo.jpg";
                string fallbackString = $"https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/{appids[index]}/header.jpg";
                Uri fallbackUri = new Uri($"https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/{appids[index]}/header.jpg");
                img.Source = new BitmapImage(fallbackUri);
                var imagenFondo = await ObtenerImagenAsync(fallbackString, nombreFondo);
                if (imagenFondo != null)
                    imagenesFondo.Add(imagenFondo);
            }
        }

        private void ImagenLogo_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (sender is Image img && img.Tag is int idx)
            {
                
                txtFalloLogo.Background = (Brush)(new BrushConverter().ConvertFrom("#80000000"));
                txtFalloLogo.Text = nombresJuegos[idx];
                txtFalloLogo.Visibility = Visibility.Visible;
                
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
                if (i < nombresJuegos.Count && nombresJuegos[i].ToLower().Contains(filtro))
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
