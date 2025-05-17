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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cliente_TFG.Pages
{
    /// <summary>
    /// Lógica de interacción para paginaBiblioteca.xaml
    /// </summary>
    public partial class paginaBiblioteca : Page
    {

        private string[] appids;

        //PARA EL FONDO
        private List<string> imagenesFondo = new List<string>();

        public paginaBiblioteca()
        {
            InitializeComponent();

            //CARGAR DATOS DE PRUEBA
            CargarDatosDePrueba();

            //PARA EL FONDO
            CargarImagenesFondo();

        }


        private void CargarDatosDePrueba()
        {

            appids = new string[]
            {
                "3017860",
                "570",
                "578080",
                "252490",
                "2507950",
                "1903340",
                "1086940",
                "3017860",
            };

    }


        //METODO PARA CARGAR LAS IMAGENES DE FONDO DE LA INTERFAZ
        private void CargarImagenesFondo()
        {
            foreach (var appidJuego in appids)
            {
                // Construimos la URL directamente, sin descargar
                string url = $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{appidJuego}/library_hero.jpg";
                imagenesFondo.Add(url);
            }

            if (imagenesFondo.Count == 0)
            {
                MessageBox.Show("No se encontraron imágenes para el fondo.");
            }
            else
            {
                CargarFondo();
            }
        }



        private void CargarFondo()
        {
            imgFondo.Source = new BitmapImage(new Uri(imagenesFondo.First(), UriKind.Absolute));

        }









    }
}
