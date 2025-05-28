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
using Cliente_TFG.Classes;
using Newtonsoft.Json;

namespace Cliente_TFG.UserControls
{
    /// <summary>
    /// Lógica de interacción para RegistroControl.xaml
    /// </summary>
    public partial class RegistroControl : UserControl
    {
        public delegate void LoginRequested();
        public event LoginRequested VolverAlLogin;

        public RegistroControl()
        {
            InitializeComponent();
        }

        private void VolverLogin_Click(object sender, MouseButtonEventArgs e)
        {
            VolverAlLogin?.Invoke();
        }

        private async void Registrarse_Click(object sender, RoutedEventArgs e)
        {
            txtErrores.Text = "";
            string nombreCuenta = txtNombreCuenta.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string contrasena = txtPassword.Password;

            //VALIDACIONES BASICAS
            if (string.IsNullOrEmpty(nombreCuenta) || string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
            {
                txtErrores.Text = "Por favor, rellena todos los campos.";
                //MessageBox.Show("Por favor, rellena todos los campos.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool exito = await RegistrarUsuarioAsync(nombreCuenta, correo, contrasena);
            if (exito)
            {
                MessageBox.Show("Registro exitoso. Ahora puedes iniciar sesión.", "Registro", MessageBoxButton.OK, MessageBoxImage.Information);
                VolverAlLogin?.Invoke();
            }
        }


        public async Task<bool> RegistrarUsuarioAsync(string nombreCuenta, string correo, string contrasena)
        {
            using (HttpClient client = new HttpClient())
            {
                var datos = new
                {
                    nombre_cuenta = nombreCuenta,
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
                    string url = "http://" + Config.IP + ":50000/users/";

                    HttpResponseMessage response = await client.PostAsync(url, contenido);

                    if (response.IsSuccessStatusCode)
                    {
                        //REGISTRO CORRECTO
                        return true;
                    }
                    else
                    {
                        string errorJson = await response.Content.ReadAsStringAsync();
                        errorJson = errorJson.Replace('•', '\"'); // CORRIGE LAS COMILLAS MALFORMADAS
                        dynamic errorObj = JsonConvert.DeserializeObject(errorJson);
                        string mensajeError = errorObj.error;
                        MessageBox.Show("Error en registro: " + mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR DE CONEXIÓN: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

    }
}
