using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using System.Windows.Threading;

namespace Cliente_TFG.Pages
{
    /// <summary>
    /// Lógica de interacción para paginaJuegoTienda.xaml
    /// </summary>
    public partial class paginaJuegoTienda : Page
    {
        private int indiceActual = 0;
        private DispatcherTimer timerCarrusel;
        private List<Image> imagenesCarrusel = new List<Image>();

        private int imagenesCargadas = 0; //CONTADOR PARA LAS IMAGENES CARGADAS
        private bool animacionCompletada = false;


        public paginaJuegoTienda()
        {
            InitializeComponent();
            CargarCarrusel();
        }

        //PARTE DEL CARRUSEL
        private async void CargarCarrusel()
        {
            string[] capturas = new string[]
            {
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_796601d9d67faf53486eeb26d0724347cea67ddc.1920x1080.jpg?t=1729703045",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_d830cfd0550fbb64d80e803e93c929c3abb02056.1920x1080.jpg?t=1729703045",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_13bb35638c0267759276f511ee97064773b37a51.1920x1080.jpg?t=1729703045",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_0f8cf82d019c614760fd20801f2bb4001da7ea77.1920x1080.jpg?t=1729703045",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_ef82850f036dac5772cb07dbc2d1116ea13eb163.1920x1080.jpg?t=1729703045",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_76f6730dbb911650ba1f41c8e5b4bac638b5beea.1920x1080.jpg?t=1729703045",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_808cdd373d78c3cf3a78e7026ebb1a15895e0670.1920x1080.jpg?t=1729703045",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_ef98db5d5a4d877531a5567df082b0fb62d75c80.1920x1080.jpg?t=1729703045",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_2254a50f27951fb9028bc00b93a7f2ed7aac1e13.1920x1080.jpg?t=1729703045",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_54b9c26b028c84d5f8a5316f31ae6203953ed84d.1920x1080.jpg?t=1729703045",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_1b3b5fd437939a7ed00a2155269e78994cb998d3.1920x1080.jpg?t=1729703045",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_352666c1949ce3966bd966d6ea5a1afd532257bc.1920x1080.jpg?t=1729703045",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_63d2733b9b4ace01a41d5ba8afd653245d05d54a.1920x1080.jpg?t=1729703045",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_fe70d46859593aef623a0614f4686e2814405035.1920x1080.jpg?t=1729703045",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_bb2af3e83ac0385ff2055f2ab9697cdd83e351b7.1920x1080.jpg?t=1729703045",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_fb8e5e2ae29ce64e2898315c66b5db08989e8f91.1920x1080.jpg?t=1729703045",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_0db84c628a798e38ca57d69abda119bee1358008.1920x1080.jpg?t=1729703045",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_18e9ea2715f0407ee05e206073927a648db60d73.1920x1080.jpg?t=1729703045",
                "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/730/ss_2514675f364079b754b820cbc8b2e7c331d56a26.1920x1080.jpg?t=1729703045"
            };

            //LIMPIO PRIMERO EL CONTENIDO POR SI ACASO
            panelCarrusel.Children.Clear();
            stackMiniaturas.Children.Clear();
            imagenesCarrusel.Clear();

            for (int i = 0; i < capturas.Length; i++)
            {
                // IMAGEN PRINCIPAL GRANDE DEL CARRUSEL
                Image imagen = new Image
                {
                    Stretch = Stretch.UniformToFill,
                    Visibility = i == 0 ? Visibility.Visible : Visibility.Hidden
                };

                Grid.SetRow(imagen, 0);
                Grid.SetColumn(imagen, 0);
                panelCarrusel.Children.Add(imagen);
                imagenesCarrusel.Add(imagen);

                // Cargar imagen de forma asíncrona
                var bitmap = await CargarImagenAsync(capturas[i]);
                imagen.Source = bitmap;

                // IMAGENES DE LAS MINIATURAS
                Image miniatura = new Image
                {
                    Width = 100,
                    Height = 56,
                    Margin = new Thickness(5),
                    Cursor = Cursors.Hand,
                    VerticalAlignment = VerticalAlignment.Center
                };

                miniatura.Source = bitmap;

                // Añadir evento de click a las miniaturas
                int index = i; // Copia del index para que no cambie en medio del código
                miniatura.MouseLeftButtonDown += (sender, e) => CambiarImagenCarruselManual(index);

                stackMiniaturas.Children.Add(miniatura);

                imagenesCargadas++;

                //SI TODAS LAS IAMGENES SE HAN CARGADO, MUESTRO EL STACKPANEL DE LAS MINIATURAS
                if (imagenesCargadas == capturas.Length || imagenesCargadas >= 6)
                {
                    if (!animacionCompletada)
                    {
                        FadeIn(stackMiniaturas);
                        FadeIn(ScrollMiniaturas);
                    }
                    

                }
                else
                {
                    stackMiniaturas.Visibility = Visibility.Hidden;
                    ScrollMiniaturas.Visibility = Visibility.Hidden;
                }


            }


            IniciarCarrusel();
        }


        private void IniciarCarrusel()
        {
            timerCarrusel = new DispatcherTimer();
            timerCarrusel.Interval = TimeSpan.FromSeconds(7); //TIEMPO PARA CAMBIAR ENTRE IMAGENES
            timerCarrusel.Tick += CambiarImagenCarrusel;
            timerCarrusel.Start();
        }

        private void CambiarImagenCarrusel(object sender, EventArgs e)
        {
            if (imagenesCarrusel.Count == 0) return;

            imagenesCarrusel[indiceActual].Visibility = Visibility.Hidden;
            indiceActual = (indiceActual + 1) % imagenesCarrusel.Count;
            imagenesCarrusel[indiceActual].Visibility = Visibility.Visible;
        }

        private void CambiarImagenCarruselManual(int indice)
        {
            //COMPRUEBO QUE EL INDICE ES VALIDO
            if (indice < 0 || indice >= imagenesCarrusel.Count)
            {
                return;
            }

            //CAMBIO LA VISIBILIDAD EN TODAS LAS IMAGENES
            foreach (var img in imagenesCarrusel)
            {
                img.Visibility = Visibility.Hidden;
            }

            imagenesCarrusel[indice].Visibility = Visibility.Visible;
            indiceActual = indice;

            ReiniciarTemporizador();
        }

        //METODO PARA REINICIAR EL TEMPORIZADOR UNA VEZ HACES CLICK PARA QUE NO CAMBIE INSTANTANEAMENTE
        private void ReiniciarTemporizador()
        {
            if (timerCarrusel != null)
            {
                timerCarrusel.Stop();
                timerCarrusel.Start();
            }
        }

        private async Task<BitmapImage> CargarImagenAsync(string url)
        {
            var bitmap = new BitmapImage();
            try
            {
                var stream = await DescargarImagenAsync(url); //METODO ASINCRONO PARA DESCARGAR LA IMAGEN
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; //CACHEAR IMAGEN POR SI SE USA MAS ADELANTE
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la imagen: " + ex.Message);
            }
            return bitmap;
        }

        private async Task<Stream> DescargarImagenAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                return await response.Content.ReadAsStreamAsync();
            }
        }

        //METODO PARA LA ANIMACION DE LAS MINIATURAS
        private void FadeIn(UIElement element)
        {
            animacionCompletada = true;
            element.Opacity = 0;
            element.Visibility = Visibility.Visible;

            //CREO LA ANIMACION DE LA OPACIDAD
            var fadeInAnimation = new System.Windows.Media.Animation.DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(1) //DURACION EN SEGUNDOS DE LA ANIMACION
            };
            //APLICO LA ANIMACION
            element.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }


    }
}
