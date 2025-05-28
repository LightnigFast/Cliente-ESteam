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

namespace Cliente_TFG.UserControls
{
    /// <summary>
    /// Lógica de interacción para LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        public delegate void RegistroRequested();
        public event RegistroRequested AbrirRegistro;
        //public string ip = "26.84.183.227";
        public string ip = "127.0.0.1";


        public LoginControl()
        {
            InitializeComponent();
        }

        private void AbrirRegistro_Click(object sender, MouseButtonEventArgs e)
        {
            AbrirRegistro?.Invoke();
        }

        private async void BotonLogin_Click(object sender, RoutedEventArgs e)
        {
            string correo = txtCorreo.Text;
            string contrasena = txtPassword.Password;

            bool exito = await LoginUsuarioAsync(correo, contrasena);
            if (exito)
            {
                //NAVEGAR A LA APP O MOSTRAR LA VENTANA PRINCIPAL
            }
        }

        public async Task<bool> LoginUsuarioAsync(string correo, string contrasena)
        {
            using (HttpClient client = new HttpClient())
            {
                var datos = new
                {
                    correo = correo,
                    contrasena = contrasena
                };

                var contenido = new StringContent(
                    JsonConvert.SerializeObject(datos),
                    Encoding.UTF8,
                    "application/json"
                );

                try
                {
                    HttpResponseMessage response = await client.PostAsync("http://" + ip + ":50000/login/", contenido);

                    if (response.IsSuccessStatusCode)
                    {
                        string respuestaJson = await response.Content.ReadAsStringAsync();
                        dynamic datosRespuesta = JsonConvert.DeserializeObject(respuestaJson);
                        string idUsuario = datosRespuesta.id_usuario;

                        MessageBox.Show("Login correcto. ID: " + idUsuario);
                        return true;
                    }
                    else
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show("Error en login: " + error);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR DE CONEXIÓN: " + ex.Message);
                    return false;
                }
            }
        }



    }
}
