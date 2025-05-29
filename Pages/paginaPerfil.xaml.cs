using Cliente_TFG.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
using System.IO;
using Newtonsoft.Json.Linq;

namespace Cliente_TFG.Pages
{
    /// <summary>
    /// Lógica de interacción para paginaPerfil.xaml
    /// </summary>
    /// 

    public partial class paginaPerfil : Page
    {

        //private const string URL_API = "http://26.84.183.227:50000/";
        private MainWindow ventanaPrincipal;
        private List<string> imagenVerticalJuegos = new List<string>();

        public paginaPerfil(MainWindow ventanaPrincipal)
        {
            InitializeComponent();
            this.ventanaPrincipal = ventanaPrincipal;
            this.Loaded += Page_Loaded;
            CargarUltimosJuegos();
            cargarTemas();

            cargarDatosUser();
        }

        private void cargarTemas()
        {
            nombreUser.Foreground = AppTheme.Actual.TextoPrincipal;
            nombreCuenta.Foreground = AppTheme.Actual.TextoPrincipal;
            descripccionCuenta.Foreground = AppTheme.Actual.TextoPrincipal;
            txtBiografia.Foreground = AppTheme.Actual.TextoPrincipal;

            gridCentral.Background = AppTheme.Actual.FondoPanel;

        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)

        {

            await RecargarUsuarioDesdeServidor();

        }

        private void cargarDatosUser()
        {
            Usuario usuario = ventanaPrincipal.Usuario;

            nombreUser.Text = usuario.nombre_usuario;
            nombreCuenta.Text = usuario.nombre_cuenta;
            descripccionCuenta.Text = usuario.descripcion;

            string urlImagen = usuario.foto_perfil;

            if (!string.IsNullOrEmpty(urlImagen) && Uri.IsWellFormedUriString(urlImagen, UriKind.Absolute))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;  // Forzar no usar caché
                bitmap.UriSource = new Uri(urlImagen, UriKind.Absolute);
                bitmap.EndInit();

                imagenUser.Source = bitmap;
            }
            else
            {
                string rutaImagenLocal = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "imagenes", "default.png");
                imagenUser.Source = new BitmapImage(new Uri(rutaImagenLocal, UriKind.Absolute));
                MessageBox.Show("URL de la foto inválida o vacía: " + urlImagen);
            }
        }





        private async Task<string> SubirImagenAImgurAsync(string ruta)
        {
            string clientId = "9a3f4409fb9efc2";

            using (var httpClient = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {
                    byte[] imagenBytes = File.ReadAllBytes(ruta);
                    var imageContent = new ByteArrayContent(imagenBytes);
                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                    content.Add(imageContent, "image");

                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Client-ID", clientId);

                    var response = await httpClient.PostAsync("https://api.imgur.com/3/image", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        dynamic resultado = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                        return resultado.data.link;
                    }

                    MessageBox.Show("Error al subir la imagen a Imgur");
                    return null;
                }
            };



        }

        private async Task ActualizarFotoUsuarioConUrl(string urlImagen)
        {
            var httpClient = new HttpClient();

            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("id_usuario", ventanaPrincipal.Usuario.id_usuario.ToString()),
            new KeyValuePair<string, string>("foto_perfil", urlImagen)
        });

            var response = await httpClient.PostAsync($"http://" + ventanaPrincipal.IP + ":50000/user_profile/actualizar_foto", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                MessageBox.Show("Error al actualizar la foto: " + error);
            }
            else
            {
                MessageBox.Show("Foto actualizada correctamente");
            }
        }



        // Método para recargar usuario desde servidor
        private async Task RecargarUsuarioDesdeServidor()
        {
            int idUsuario = ventanaPrincipal.Usuario.IdUsuario;
            Usuario usuarioActualizado = await ObtenerUsuarioActualizadoAsync(idUsuario);

            if (usuarioActualizado != null)
            {
                // Actualiza el objeto usuario local con los nuevos datos
                ventanaPrincipal.user = usuarioActualizado;

                // También actualiza la interfaz principal si tienes controles que muestran info del usuario
                ventanaPrincipal.Cabecera_top.NombreUsuario = usuarioActualizado.NombreUsuario;
                ventanaPrincipal.Cabecera_top.Dinero = usuarioActualizado.Dinero;
                //PARA LA FOTO
                var uri = new Uri(usuarioActualizado.FotoPerfil, UriKind.Absolute);
                ventanaPrincipal.Cabecera_top.FotoPerfil = new BitmapImage(uri);

                // Refresca el UI en esta página
                cargarDatosUser();
            }
        }

        private async Task<Usuario> ObtenerUsuarioActualizadoAsync(int idUsuario)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = $"http://" + ventanaPrincipal.IP + $":50000/users/{idUsuario}";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        // Parseamos el JSON para obtener solo el objeto 'usuario'
                        var jsonObj = Newtonsoft.Json.Linq.JObject.Parse(json);
                        var usuarioJson = jsonObj["usuario"].ToString();

                        // Deserializamos solo el objeto usuario
                        Usuario usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJson);

                        return usuario;
                    }
                    else
                    {
                        MessageBox.Show("Error al obtener el usuario desde el servidor.");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Excepción al obtener usuario: " + ex.Message);
                    return null;
                }
            }
        }

        private async void btnSubirImagen_Click_1(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Imágenes (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (dialog.ShowDialog() == true)
            {
                string rutaImagen = dialog.FileName;
                string urlImgur = await SubirImagenAImgurAsync(rutaImagen);

                if (!string.IsNullOrEmpty(urlImgur))
                {
                    await ActualizarFotoUsuarioConUrl(urlImgur);
                    await RecargarUsuarioDesdeServidor(); // Para refrescar la UI con la nueva foto
                }
            }
        }

        private async void CargarUltimosJuegos()
        {
            try
            {
                int idUsuario = ventanaPrincipal.user.IdUsuario;
                string url = $"http://{ventanaPrincipal.IP}:50000/library/{idUsuario}/ultimos_juegos";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    JObject json = JObject.Parse(response);
                    var juegosJson = json["ultimas_compras"];

                    if (juegosJson != null)
                    {
                        // Controles en la interfaz gráfica
                        var juegosUI = new List<(Image caratula, TextBlock nombre, TextBlock fecha)>
                {
                    (caratulaJuego1, nombreJuego1, fechaJuego1),
                    (caratulaJuego2, nombreJuego2, fechaJuego2),
                    (caratulaJuego3, nombreJuego3, fechaJuego3)
                };

                        // Crear lista de objetos Juego
                        List<Juego> juegos = new List<Juego>();
                        foreach (var item in juegosJson)
                        {
                            juegos.Add(new Juego
                            {
                                AppId = item["app_id"].Value<int>(),
                                Nombre = item["nombre"].Value<string>(),
                                FechaCompra = DateTime.Parse(item["fecha_compra"].Value<string>())
                            });
                        }

                        // Cargar datos en UI
                        for (int i = 0; i < Math.Min(juegos.Count, juegosUI.Count); i++)
                        {
                            Juego juego = juegos[i];

                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri(juego.UrlCaratula, UriKind.Absolute);
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();

                            juegosUI[i].caratula.Source = bitmap;
                            juegosUI[i].caratula.Tag = juego;

                            // ⚠️ Añadir los eventos aquí:
                            juegosUI[i].caratula.MouseEnter += CaratulaJuego_MouseEnter;
                            //juegosUI[i].caratula.MouseLeave += CaratulaJuego_MouseLeave;

                            juegosUI[i].nombre.Text = juego.Nombre;
                            juegosUI[i].fecha.Text = $"Comprado: {juego.FechaCompra:dd/MM/yyyy}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar juegos: {ex.Message}");
            }
        }

        private void CaratulaJuego_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Image imagen && imagen.Tag is Juego juego)
            {
                string ruta = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "ClienteTFG", "imagenes", $"{juego.AppId}_fondo.jpg");

                CambiarFondo(ruta);
            }
        }

        //private void CaratulaJuego_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    // Fondo original
        //    RootGrid.Background = new SolidColorBrush(Color.FromRgb(45, 45, 48));
        //}

        private void CambiarFondo(string ruta)
        {
            if (File.Exists(ruta))
            {
                ImageBrush brush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(ruta, UriKind.Absolute)),
                    Stretch = Stretch.UniformToFill,
                    Opacity = 0.4 // Ajusta si quieres efecto sutil
                };
                RootGrid.Background = brush;
            }
        }

        public class Juego
        {
            public string Nombre { get; set; }
            public int AppId { get; set; }
            public DateTime FechaCompra { get; set; }

            public string UrlCaratula => $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{AppId}/library_600x900.jpg";
        }
    }
}

