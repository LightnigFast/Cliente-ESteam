using Cliente_TFG.Classes;
using Cliente_TFG.Windows;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cliente_TFG
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        private string ip = Config.IP;

        //METODO PARA COMPROBAR SI TENEMOS UN TOKEN VALIDO
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            string rutaToken = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ClienteTFG", "token.txt"
            );

            //VERIFICAMOS SI EXISTE EL TOKEN EN LA CARPETA
            if (File.Exists(rutaToken))
            {
                string tokenGuardado = File.ReadAllText(rutaToken);

                using (HttpClient client = new HttpClient())
                {
                    var datos = new { token = tokenGuardado };
                    var contenido = new StringContent(JsonConvert.SerializeObject(datos), Encoding.UTF8, "application/json");

                    try
                    {
                        HttpResponseMessage resp = await client.PostAsync($"http://{ip}:50000/login/validar_token", contenido);

                        if (resp.IsSuccessStatusCode)
                        {
                            string json = await resp.Content.ReadAsStringAsync();
                            dynamic data = JsonConvert.DeserializeObject(json);
                            int idUsuario = data.id_usuario;

                            MainWindow mainWindow = new MainWindow(idUsuario);
                            mainWindow.Show();
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        //SI ALGO FALLA, ABRIMOS EL LOGINWINDOW
                    }
                }
            }

            //SI NO HAY TOKEN, ABRIMOS LOGIN WINDOW
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}
