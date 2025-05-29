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

        public paginaPerfil(MainWindow ventanaPrincipal)
        {
            InitializeComponent();
            this.ventanaPrincipal = ventanaPrincipal;
            this.Loaded += Page_Loaded;
          
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
    }
}
