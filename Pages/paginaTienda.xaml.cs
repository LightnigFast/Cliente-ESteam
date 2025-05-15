using Cliente_TFG.Classes;
using Newtonsoft.Json;
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
using System.Windows.Threading;
using static Cliente_TFG.Pages.paginaTienda;

namespace Cliente_TFG.Pages
{
    /// <summary>
    /// Lógica de interacción para paginaTienda.xaml
    /// </summary>
    public partial class paginaTienda : Page
    {
        private MainWindow ventanaPrincipal;

        //CARRUSEL
        private List<int> appidCarrusel = new List<int>();
        private List<string> imagenesCarrusel = new List<string>();
        private List<List<string>> miniaturasCarrusel = new List<List<string>>();
        private List<string> nombresCarrusel = new List<string>();
        private List<string> precioCarrusel = new List<string>();
        private DispatcherTimer carruselTimer;
        private int indiceActual = 0;

        //OFERTAS
        private List<string> imagenesOfertas = new List<string>();
        private List<string> precioOfertas = new List<string>();
        private List<string> descuentoOfertas = new List<string>();
        private List<int> appidOfertas = new List<int>();

        //OFERTAS ESPECIALES
        private List<string> imagenesOfertasEspeciales = new List<string>();
        private List<string> precioOfertasEspeciales = new List<string>();
        private List<string> descuentoOfertasEspeciales = new List<string>();
        private List<int> appidOfertasEspeciales = new List<int>();


        public paginaTienda(MainWindow mainWindow)
        {
            InitializeComponent();
            //CARGO LOS COLORES LO PRIMERO DE TODO
            carruselTituloJuego.Foreground = AppTheme.Actual.TextoPrincipal;
            carruselPrecioJuego.Foreground = AppTheme.Actual.TextoPrincipal;

            ventanaPrincipal = mainWindow;

            //DESHABILITAR ESTO PARA EVITAR ERRORES EN LA CARGA DEL PRORGAMA
            carruselMiniaturasImagenes.IsEnabled = false;

            CargarDatosJsonCarrusel();
            CargarDatosJsonOfertas();
            CargarDatosJsonOfertasDeterminadoPrecio();

            //CargarCarrusel();
            //CargarOfertas();
            //CargarOfertasDeterminadoPrecio();
            CargarNuevosLanzamientos();
        }

        //METODO PARA CARGAR LOS DATOS DEL JSON
        private async void CargarDatosJsonCarrusel()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string json = await client.GetStringAsync("http://127.0.0.1:5000/store/carrusel");
                    var response = JsonConvert.DeserializeObject<CarruselResponse>(json);

                    bool primeraImagenCargada = false;

                    foreach (var juego in response.juegos)
                    {
                        if (juego.capturas_miniatura != null && juego.capturas_miniatura.Count > 0)
                        {
                            string imagenGrande = "https://cdn.akamai.steamstatic.com/steam/apps/" + juego.app_id + "/capsule_616x353.jpg";
                            string fallback = juego.header_image;
                            if (await ImagenExiste(imagenGrande))
                            {
                                imagenesCarrusel.Add(imagenGrande);
                            }
                            else
                            {
                                imagenesCarrusel.Add(fallback);
                            }

                            appidCarrusel.Add(juego.app_id);

                            miniaturasCarrusel.Add(juego.capturas_miniatura);
                            nombresCarrusel.Add(juego.nombre);
                            if (string.IsNullOrEmpty(juego.precio?.precio_inicial) || juego.precio.precio_inicial == "0")
                            {
                                precioCarrusel.Add("Free To Play");
                            }
                            else
                            {
                                double precioEuros = double.Parse(juego.precio.precio_inicial) / 100.0;
                                precioCarrusel.Add(precioEuros.ToString("0.00") + " €");
                            }



                            //MUESTRO LA PRIMERA IMAGEN INMEDIATAMENTE NADA MAS TENENRLA
                            if (!primeraImagenCargada)
                            {
                                //SIGNO LA PRIMERA IAMGEN Y EL TITULO
                                imagenTiendaGrande.Source = new BitmapImage(new Uri(imagenesCarrusel[0]));
                                carruselTituloJuego.Text = nombresCarrusel[0];
                                carruselPrecioJuego.Text = precioCarrusel[0];
                                CargarMiniaturas(indiceActual);

                                primeraImagenCargada = true;
                                
                                AplicarFadeIn(imagenTiendaGrande);
                                AplicarFadeIn(carruselTituloJuego);
                                AplicarFadeIn(carruselPrecioJuego);
                                panelJuegosDestacados.Background = AppTheme.Actual.FondoPanel;
                            }
                        }

                        //CREO LOS BOTONES PARA NAVEGAR POR EL CARRUSEL
                        int index = imagenesCarrusel.Count - 1; //ÍNDICE ACTUAL DEL JUEGO QUE ACABO DE AÑADIR
                        Button btn = new Button
                        {
                            //Content = (index + 1).ToString(),
                            Margin = new Thickness(2),
                            Padding = new Thickness(5),
                            Width = 30,
                            Height = 20,
                            FontSize = 18,
                            Background = Brushes.Transparent,
                            Foreground = AppTheme.Actual.TextoPrincipal,
                            BorderBrush = AppTheme.Actual.TextoPrincipal,
                            Cursor = Cursors.Hand
                        };

                        //EVENTO CLICK PARA NAVEGAR A ESE JUEGO
                        btn.Click += (s, e) =>
                        {
                            indiceActual = index;
                            imagenTiendaGrande.Source = new BitmapImage(new Uri(imagenesCarrusel[index]));
                            carruselTituloJuego.Text = nombresCarrusel[index];
                            carruselPrecioJuego.Text = precioCarrusel[index];
                            CargarMiniaturas(index);
                            AplicarFadeIn(imagenTiendaGrande);
                            AplicarFadeIn(carruselTituloJuego);
                            AplicarFadeIn(carruselPrecioJuego);
                            ReiniciarTimer();
                        };

                        botonesCarrusel.Children.Add(btn);

                    }

                    if (imagenesCarrusel.Count == 0)
                    {
                        MessageBox.Show("No se encontraron imágenes en el carrusel.");
                    }
                    else
                    {
                        //SI AL MENOS UNA IMAGEN ESTA DISPONIBLE, MUESTRO EL CARRUSEL
                        CargarCarrusel();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar datos del carrusel: {ex.Message}");
                }
            }
        }



        //PARTE PARA EL CARRUSEL
        private void CargarCarrusel()
        {
            /*
            imagenesCarrusel = new string[]
            {
                "https://cdn.akamai.steamstatic.com/steam/apps/2488620/capsule_616x353.jpg",
                "https://cdn.akamai.steamstatic.com/steam/apps/2669320/capsule_616x353.jpg",
                "https://cdn.akamai.steamstatic.com/steam/apps/1451190/capsule_616x353.jpg"
            };
            */


            //INICIALIZA TIMER SI NO EXISTE
            if (carruselTimer == null)
            {
                carruselTimer = new DispatcherTimer();
                carruselTimer.Interval = TimeSpan.FromSeconds(7);
                carruselTimer.Tick += (s, e) =>
                {
                    indiceActual = (indiceActual + 1) % imagenesCarrusel.Count;
                    imagenTiendaGrande.Source = new BitmapImage(new Uri(imagenesCarrusel[indiceActual]));
                    carruselTituloJuego.Text = nombresCarrusel[indiceActual];
                    carruselPrecioJuego.Text = precioCarrusel[indiceActual];
                    CargarMiniaturas(indiceActual);
                    AplicarFadeIn(imagenTiendaGrande);
                    AplicarFadeIn(carruselTituloJuego);
                    AplicarFadeIn(carruselPrecioJuego);

                };
            }
            //VUELVO A ACTIVARLO PARA QUE FUNCIONEN LOS EVENTOS
            carruselMiniaturasImagenes.IsEnabled = true;

            //ASIGNA EVENTOS
            panelJuegosDestacados.MouseDown += async (s, e) =>
            {
                var paginaJuegoTienda = new paginaJuegoTienda(ventanaPrincipal, appidCarrusel[indiceActual]);
                await AplicarFadeOutAsync(panelJuegosDestacados);
                ventanaPrincipal.framePrincipal.Navigate(paginaJuegoTienda);
            };


            panelJuegosDestacados.MouseEnter += (s, e) =>
            {
                panelJuegosDestacados.Background = AppTheme.Actual.RatonEncima;
            };

            panelJuegosDestacados.MouseLeave += (s, e) =>
            {
                panelJuegosDestacados.Background = AppTheme.Actual.FondoPanel;
            };

            BtnAnterior.Click += (s, e) =>
            {
                indiceActual = (indiceActual - 1 + imagenesCarrusel.Count) % imagenesCarrusel.Count;
                imagenTiendaGrande.Source = new BitmapImage(new Uri(imagenesCarrusel[indiceActual]));
                carruselTituloJuego.Text = nombresCarrusel[indiceActual];
                carruselPrecioJuego.Text = precioCarrusel[indiceActual];
                CargarMiniaturas(indiceActual);
                AplicarFadeIn(imagenTiendaGrande);
                AplicarFadeIn(carruselTituloJuego);
                AplicarFadeIn(carruselPrecioJuego);
                ReiniciarTimer();
            };

            BtnSiguiente.Click += (s, e) =>
            {
                indiceActual = (indiceActual + 1) % imagenesCarrusel.Count;
                imagenTiendaGrande.Source = new BitmapImage(new Uri(imagenesCarrusel[indiceActual]));
                carruselTituloJuego.Text = nombresCarrusel[indiceActual];
                carruselPrecioJuego.Text = precioCarrusel[indiceActual];
                CargarMiniaturas(indiceActual);
                AplicarFadeIn(imagenTiendaGrande);
                AplicarFadeIn(carruselTituloJuego);
                AplicarFadeIn(carruselPrecioJuego);
                ReiniciarTimer();
            };

            //IMAGEN INICIAL
            imagenTiendaGrande.Source = new BitmapImage(new Uri(imagenesCarrusel[indiceActual]));
            carruselTituloJuego.Text = nombresCarrusel[indiceActual];
            carruselPrecioJuego.Text = precioCarrusel[indiceActual];
            CargarMiniaturas(indiceActual);
            //AplicarFadeIn(imagenTiendaGrande);
            //AplicarFadeIn(carruselTituloJuego);
            //AplicarFadeIn(carruselPrecioJuego);

            //INICIA EL TIMER
            carruselTimer.Start();
        }

        //MÉTODO PARA REINICIAR EL TIMER
        private void ReiniciarTimer()
        {
            carruselTimer.Stop();
            carruselTimer.Start();
        }

        //METODO PARA QUE TODO SE VEA MAS NATURAL CON UNA TRASICION FADEIN
        private void AplicarFadeIn(UIElement elemento)
        {
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            elemento.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        }

        private async Task AplicarFadeOutAsync(UIElement elemento)
        {
            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            elemento.BeginAnimation(UIElement.OpacityProperty, fadeOut);

            await Task.Delay(500); //ESPERA A QUE TERMINE LA ANIMACIÓN
        }


        //CARGO LAS IMAGENES DE LAS MINIATURAS
        private void CargarMiniaturas(int indiceJuego)
        {
            carruselMiniaturasImagenes.Children.Clear();

            foreach (var miniatura in miniaturasCarrusel[indiceJuego])
            {
                Image img = new Image
                {
                    Source = new BitmapImage(new Uri(miniatura)),
                    Stretch = System.Windows.Media.Stretch.Uniform,
                    Height = 110,
                    Margin = new Thickness(5),
                    Cursor = Cursors.Hand
                };

                //EVENTO PARA CAMBIAR LA IMAGEN GRANDE AL PASAR EL RATÓN
                img.MouseEnter += (s, e) =>
                {
                    imagenTiendaGrande.Source = new BitmapImage(new Uri(miniatura));
                    AplicarFadeIn(imagenTiendaGrande);
                    carruselTimer.Stop();
                };

                //EVENTO PARA VOLVER A LA IMAGEN DEL CARRUSEL AL SALIR
                img.MouseLeave += (s, e) =>
                {
                    imagenTiendaGrande.Source = new BitmapImage(new Uri(imagenesCarrusel[indiceJuego]));
                    AplicarFadeIn(imagenTiendaGrande);
                    carruselTimer.Start();
                };
                
                carruselMiniaturasImagenes.Children.Add(img);
            }
            //AplicarFadeIn(carruselMiniaturasImagenes);
        }

        private async Task<bool> ImagenExiste(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            }
        }


        public class Precio
        {
            public string descuento { get; set; }
            public string precio_inicial { get; set; }
        }

        public class JuegoCarrusel
        {
            public int app_id { get; set; }
            public List<string> capturas_miniatura { get; set; }
            public bool f2p { get; set; }
            public string nombre { get; set; }
            public string header_image { get; set; }
            public Precio precio { get; set; }
        }

        //NUEVA CLASE CONTENEDORA
        public class CarruselResponse
        {
            public List<JuegoCarrusel> juegos { get; set; }
        }






        //PARTE DE LAS OFERTES ESPECIALES
        private void CargarOfertas()
        {
            
            /*
            string[] urls = new string[]
            {
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/23400/header.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/620/header.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/570/header.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/440/header.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/header.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/550/header.jpg"
            };
            */

            int columnas = 3;

            for (int i = 0; i < imagenesOfertas.Count; i++)
            {
                int row = i / columnas;
                int col = i % columnas;

                //CREO LA IMAGEN
                Image img = new Image
                {
                    Source = new BitmapImage(new Uri(imagenesOfertas[i])),
                    Stretch = Stretch.UniformToFill,
                    Height = 160,
                    Margin = new Thickness(0)
                };

                //CREA UN GRID CON DOS COLUMNAS
                Grid infoHorizontal = new Grid
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Background = AppTheme.Actual.FondoPanel,
                    Margin = new Thickness(0, 0, 0, 0),
                    Height = 30,
                };

                //DEFINICIÓN DE COLUMNAS
                infoHorizontal.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); //Nombre
                infoHorizontal.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }); //Precio

                //TEXTO DEL DESCUENTO (IZQUIERDA)
                TextBlock textoDescuentoPorcentaje = new TextBlock
                {
                    Text = "-" + descuentoOfertas[i] + "%",
                    FontSize = 15,
                    FontWeight = FontWeights.Bold,
                    Foreground = AppTheme.Actual.TextoPrecio,
                    Background = AppTheme.Actual.FondoDescuento,
                    Width = 40,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 0, 10, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    TextTrimming = TextTrimming.CharacterEllipsis
                };
                Grid.SetColumn(textoDescuentoPorcentaje, 0);

                //TEXTO DEL PRECIO (DERECHA)
                TextBlock textoPrecio = new TextBlock
                {
                    Text = precioOfertas[i],
                    FontSize = 15,
                    FontWeight = FontWeights.Bold,
                    Foreground = AppTheme.Actual.TextoPrincipal,
                    Padding = new Thickness(0, 0, 5, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                Grid.SetColumn(textoPrecio, 1);

                //AÑADE LOS TEXTBLOCK AL GRID
                infoHorizontal.Children.Add(textoDescuentoPorcentaje);
                infoHorizontal.Children.Add(textoPrecio);


                //CONTENEDOR VERTICAL GENERAL
                Thickness margen;
                if (col == 0)
                    margen = new Thickness(0, 0, 5, 0);
                else if (col == columnas - 1)
                    margen = new Thickness(5, 0, 0, 0);
                else
                    margen = new Thickness(5, 0, 5, 0);

                StackPanel contenedor = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = margen,
                    Tag = appidOfertas[i]
                };

                contenedor.Children.Add(img);
                contenedor.Children.Add(infoHorizontal);

                //EVENTOS
                contenedor.MouseLeftButtonDown += JuegoClick;
                contenedor.MouseEnter += (s, e) => JuegoEnterOferta(s, e, infoHorizontal);
                contenedor.MouseLeave += (s, e) => JuegoExitOferta(s, e, infoHorizontal);

                Grid.SetRow(contenedor, row);
                Grid.SetColumn(contenedor, col);
                panelOfertasEspaciales.Children.Add(contenedor);
                AplicarFadeIn(contenedor);
            }
        }

        private async void CargarDatosJsonOfertas()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string json = await client.GetStringAsync("http://127.0.0.1:5000/store/ofertas");
                    var response = JsonConvert.DeserializeObject<CarruselResponseOferta>(json);


                    foreach (var juegoOferta in response.juegos)
                    {

                        appidOfertas.Add(juegoOferta.app_id);
                        imagenesOfertas.Add(juegoOferta.header_image);
                        if (string.IsNullOrEmpty(juegoOferta.precio?.precio_inicial) || juegoOferta.precio.precio_inicial == "0")
                        {
                            precioOfertas.Add("Free To Play");
                        }
                        else
                        {

                            double precioEuros = double.Parse(juegoOferta.precio.precio_inicial) / 100.0;
                            double descuento = double.Parse(juegoOferta.precio.descuento);
                            double precioFinal = precioEuros * (1 - (descuento / 100.0));

                            precioOfertas.Add(precioFinal.ToString("0.00") + " € ");
                            descuentoOfertas.Add(descuento.ToString());
                        }
                        
                    }

                    if (imagenesOfertas.Count == 0)
                    {
                        MessageBox.Show("No se encontraron imágenes para las ofertas.");
                    }
                    else
                    {
                        //SI AL MENOS UNA IMAGEN ESTA DISPONIBLE, MUESTRO LAS OFERTAS
                        CargarOfertas();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar datos del carrusel: {ex.Message}");
                }
            }
        }

        public class CarruselResponseOferta
        {
            public List<JuegoOferta> juegos { get; set; }
        }

        public class JuegoOferta
        {
            public int app_id { get; set; }
            public string header_image { get; set; }
            public string nombre { get; set; }
            public PrecioOferta precio { get; set; }
        }

        public class PrecioOferta
        {
            public string descuento { get; set; }
            public string precio_inicial { get; set; }
        }





        //PARTE PARA LAS OFERTAS ESPECIFICAS
        private void CargarOfertasDeterminadoPrecio()
        {
            /*
            string[] urls = new string[]
            {
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1200/capsule_231x87.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/400/capsule_231x87.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/620/capsule_231x87.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1174180/capsule_231x87.jpg"
            };
            */

            int columnas = 4;

            for (int i = 0; i < imagenesOfertasEspeciales.Count; i++)
            {
                int col = i % columnas;

                //CREO LA IMAGEN
                Image img = new Image
                {
                    Source = new BitmapImage(new Uri(imagenesOfertasEspeciales[i])),
                    Stretch = Stretch.UniformToFill,
                    Margin = new Thickness(0)
                };

                //TEXTO DEL PRECIO
                TextBlock textoPrecio = new TextBlock
                {
                    Text = precioOfertasEspeciales[i],
                    Height = 25,
                    FontSize = 15,
                    FontWeight = FontWeights.Bold,
                    Foreground = AppTheme.Actual.TextoPrincipal,
                    Background = AppTheme.Actual.FondoPanel,
                    Padding = new Thickness(0, 0, 5, 0),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Right,

                };

                //STACKPANEL PARA METER LA IMAGEN Y EL PRECIO
                Thickness margen;

                if (col == 0) //primera columna
                    margen = new Thickness(0, 0, 5, 0);
                else if (col == columnas - 1) //última columna
                    margen = new Thickness(5, 0, 0, 0);
                else
                    margen = new Thickness(5, 0, 5, 0);

                StackPanel contenedor = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = margen,
                    Tag = appidOfertasEspeciales[i]
                };

                contenedor.Children.Add(img);
                contenedor.Children.Add(textoPrecio);

                //EVENTO DE CLICK
                contenedor.MouseLeftButtonDown += JuegoClick;

                //EVENTO MOUSE ENTER Y EXIT PARA ESTE STACKPANEL
                contenedor.MouseEnter += (s, e) => JuegoEnter(s, e, textoPrecio);
                contenedor.MouseLeave += (s, e) => JuegoExit(s, e, textoPrecio);

                Grid.SetColumn(contenedor, col);
                panelOfertaEspecifica.Children.Add(contenedor);

            }

        }

        private async void CargarDatosJsonOfertasDeterminadoPrecio()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string json = await client.GetStringAsync("http://127.0.0.1:5000/store/10.00");
                    var response = JsonConvert.DeserializeObject<OfertaEspeciallResponse>(json);


                    foreach (var juegoOferta in response.juegos)
                    {

                        appidOfertasEspeciales.Add(juegoOferta.app_id);
                        imagenesOfertasEspeciales.Add(juegoOferta.capsule_image);
                        if (string.IsNullOrEmpty(juegoOferta.precio?.precio_inicial) || juegoOferta.precio.precio_inicial == "0")
                        {
                            precioOfertasEspeciales.Add("Free To Play");
                        }
                        else
                        {

                            double precioEuros = double.Parse(juegoOferta.precio.precio_inicial) / 100.0;
                            double descuento = double.Parse(juegoOferta.precio.descuento);
                            double precioFinal = precioEuros * (1 - (descuento / 100.0));

                            precioOfertasEspeciales.Add(precioFinal.ToString("0.00") + " € ");
                            descuentoOfertasEspeciales.Add(descuento.ToString());
                        }

                    }

                    if (imagenesOfertas.Count == 0)
                    {
                        MessageBox.Show("No se encontraron imágenes para las ofertas.");
                    }
                    else
                    {
                        //SI AL MENOS UNA IMAGEN ESTA DISPONIBLE, MUESTRO LAS OFERTAS
                        CargarOfertasDeterminadoPrecio();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar datos de las ofertas espciales: {ex.Message}");
                }
            }
        }

        public class OfertaEspeciallResponse
        {
            public List<JuegoOfertaEspecial> juegos { get; set; }
        }
        public class JuegoOfertaEspecial
        {
            public int app_id { get; set; }
            public string capsule_image { get; set; }
            public string nombre { get; set; }
            public PrecioOferta precio { get; set; }
        }

        public class PrecioOfertaEspecial
        {
            public string descuento { get; set; }
            public string precio_inicial { get; set; }
        }




        //PARTE PARA LOS NUEVOS LANZAMIENTOS
        private void CargarNuevosLanzamientos()
        {
            string[] urls = new string[]
            {
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1850570/capsule_231x87.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/227300/capsule_231x87.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/2161700/capsule_231x87.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/582010/capsule_231x87.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/489830/capsule_231x87.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/105600/capsule_231x87.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/304930/capsule_231x87.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/271590/capsule_231x87.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1174180/capsule_231x87.jpg",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/381210/capsule_231x87.jpg"
            };




            for (int i = 0; i < urls.Length; i++)
            {
                Thickness margenFila = new Thickness(0, i == 0 ? 0 : 5, 0, i == urls.Length - 1 ? 0 : 5);

                //GRID CONTENEDOR DE LA FILA
                Grid filaGrid = new Grid
                {
                    Background = Brushes.Transparent, // Necesario para que el click funcione
                    Margin = margenFila,
                    Cursor = Cursors.Hand // Opcional: cambia el cursor
                };

                filaGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(231) }); // Imagen
                filaGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Texto
                filaGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) }); // Precio

                //IMAGEN
                Image img = new Image
                {
                    Source = new BitmapImage(new Uri(urls[i])),
                    Stretch = Stretch.UniformToFill,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                Grid.SetColumn(img, 0);
                filaGrid.Children.Add(img);

                //TEXTO DEL CONTENIDO
                Label textoContenido = new Label
                {
                    Content = "Half-Life 2\nAcción, Ciencia ficción",
                    FontSize = 16,
                    Foreground = AppTheme.Actual.TextoPrincipal,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Background = AppTheme.Actual.FondoPanel,
                    Padding = new Thickness(10, 0, 0, 0)
                };
                Grid.SetColumn(textoContenido, 1);
                filaGrid.Children.Add(textoContenido);

                //PRECIO
                Label textoPrecio = new Label
                {
                    Content = "15,99€",
                    FontSize = 16,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Background = AppTheme.Actual.FondoPanel,
                    Foreground = AppTheme.Actual.TextoPrecio,
                };
                Grid.SetColumn(textoPrecio, 2);
                filaGrid.Children.Add(textoPrecio);

                //EVENTO DE CLICK
                filaGrid.MouseLeftButtonDown += JuegoClick;

                //EVENTO MOUSE ENTER Y EXIT PARA ESTE STACKPANEL
                filaGrid.MouseEnter += (s, e) =>
                {
                    textoContenido.Background = AppTheme.Actual.RatonEncima;
                    textoPrecio.Background = AppTheme.Actual.RatonEncima;
                };
                filaGrid.MouseLeave += (s, e) =>
                {
                    textoContenido.Background = AppTheme.Actual.FondoPanel;
                    textoPrecio.Background = AppTheme.Actual.FondoPanel;
                };


                //AÑADIR LA FILA AL GRID PRINCIPAL
                Grid.SetRow(filaGrid, i);
                Grid.SetColumnSpan(filaGrid, 3);
                panelNuevosLanzamientos.Children.Add(filaGrid);
            }

        }




        //EVENTO DE CLICK
        private void JuegoClick(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("¡Has hecho clic en el Grid!");
            //AppTheme.Alternar();
            //RefrescarTemas(); // ACTUALIZA LOS COLORES AL CAMBIAR EL TEMA

            //var paginaJuegoTienda = new paginaJuegoTienda(ventanaPrincipal, appidCarrusel[indiceActual]);
            //await AplicarFadeOutAsync(panelJuegosDestacados);
            //ventanaPrincipal.framePrincipal.Navigate(paginaJuegoTienda);

            if (sender is FrameworkElement elemento && elemento.Tag is int appid)
            {
                var paginaJuegoTienda = new paginaJuegoTienda(ventanaPrincipal, appid);
                ventanaPrincipal.framePrincipal.Navigate(paginaJuegoTienda);
            }
        }

        //EVENTO ENTER
        private void JuegoEnter(object sender, MouseEventArgs e, TextBlock textoPrecio)
        {
            textoPrecio.Background = AppTheme.Actual.RatonEncima;
        }

        //EVENTO EXIT
        private void JuegoExit(object sender, MouseEventArgs e, TextBlock textoPrecio)
        {
            textoPrecio.Background = AppTheme.Actual.FondoPanel;
        }

        //EVENTO ENTER
        private void JuegoEnterOferta(object sender, MouseEventArgs e, Grid stackHorizontal)
        {
            stackHorizontal.Background = AppTheme.Actual.RatonEncima;
        }

        //EVENTO EXIT
        private void JuegoExitOferta(object sender, MouseEventArgs e, Grid stackHorizontal)
        {
            stackHorizontal.Background = AppTheme.Actual.FondoPanel;
        }


        private void RefrescarTemas()
        {
            // REFRESCA PANEL DE OFERTAS ESPECIALES
            foreach (var child in panelOfertasEspaciales.Children)
            {
                if (child is StackPanel stack)
                {
                    if (stack.Children.OfType<TextBlock>().FirstOrDefault() is TextBlock textoPrecio)
                    {
                        textoPrecio.Foreground = AppTheme.Actual.TextoPrecio;
                        textoPrecio.Background = AppTheme.Actual.FondoPanel;
                    }
                }
            }

            // REFRESCA PANEL DE OFERTAS ESPECÍFICAS
            foreach (var child in panelOfertaEspecifica.Children)
            {
                if (child is StackPanel stack)
                {
                    if (stack.Children.OfType<TextBlock>().FirstOrDefault() is TextBlock textoPrecio)
                    {
                        textoPrecio.Foreground = AppTheme.Actual.TextoPrecio;
                        textoPrecio.Background = AppTheme.Actual.FondoPanel;
                    }
                }
            }

            // REFRESCA PANEL DE NUEVOS LANZAMIENTOS
            foreach (var child in panelNuevosLanzamientos.Children)
            {
                if (child is Grid filaGrid)
                {
                    foreach (var elem in filaGrid.Children)
                    {
                        if (elem is Label lbl)
                        {
                            if (lbl.Content?.ToString()?.Contains("€") == true) // Es el precio
                            {
                                lbl.Background = AppTheme.Actual.FondoPanel;
                                lbl.Foreground = AppTheme.Actual.TextoPrecio;
                            }
                            else // Es el contenido
                            {
                                lbl.Background = AppTheme.Actual.FondoPanel;
                                lbl.Foreground = AppTheme.Actual.TextoPrincipal;
                            }
                        }
                    }
                }
            }
        }



    }
}
