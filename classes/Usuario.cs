using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Net;

namespace Cliente_TFG.Classes
{
    public class Usuario
    {
        private string codigo_amigo;
        private string contraseña;
        private string correo;
        private string descripcion;
        private double dinero;
        private string estado;
        private string foto_perfil;
        private int id_usuario;
        private string nombre_cuenta;
        private string nombre_usuario;
        private string token;
        private List<int> bibliotecaJuegos = new List<int>();



        public void CargarDatos(int idUser)
        {
            var url = $"http://127.0.0.1:5000/users/{idUser}";
            using (var webClient = new WebClient())
            {
                string jsonString = webClient.DownloadString(url);

                var jsonObj = JObject.Parse(jsonString);
                var usuarioJson = jsonObj["usuario"];

                codigo_amigo = (string)usuarioJson["codigo_amigo"];
                contraseña = (string)usuarioJson["contrasena"];
                correo = (string)usuarioJson["correo"];
                descripcion = (string)usuarioJson["descripcion"];
                dinero = (double)usuarioJson["dinero"];
                estado = (string)usuarioJson["estado"];
                foto_perfil = (string)usuarioJson["foto_perfil"];
                id_usuario = (int)usuarioJson["id_usuario"];
                nombre_cuenta = (string)usuarioJson["nombre_cuenta"];
                nombre_usuario = (string)usuarioJson["nombre_usuario"];
                token = ((string)usuarioJson["token"]).Trim();
            }
            CargarBiblioteca(idUser);
        }

        public void CargarBiblioteca(int idUser)
        {
            var url = $"http://127.0.0.1:5000/library/{idUser}";
            using (var webClient = new WebClient())
            {
                string jsonString = webClient.DownloadString(url);

                var jsonObj = JObject.Parse(jsonString);
                var juegosArray = jsonObj["juegos"] as JArray;

                bibliotecaJuegos.Clear();

                foreach (var juego in juegosArray)
                {
                    int appId = (int)juego["app_id"];
                    bibliotecaJuegos.Add(appId);
                }
            }
        }




        //GETTERS
        public string CodigoAmigo => codigo_amigo;
        public string Contraseña => contraseña;
        public string Correo => correo;
        public string Descripcion => descripcion;
        public double Dinero => dinero;
        public string Estado => estado;
        public string FotoPerfil => foto_perfil;
        public int IdUsuario => id_usuario;
        public string NombreCuenta => nombre_cuenta;
        public string NombreUsuario => nombre_usuario;
        public string Token => token;
        public List<int> BibliotecaJuegos => bibliotecaJuegos;


    }
}
