using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
    /// Lógica de interacción para Cabezera.xaml
    /// </summary>
    public partial class Cabezera : UserControl, INotifyPropertyChanged
    {
        //PARA LAS OPCIONES
        public event RoutedEventHandler AtrasPresionado;
        public event RoutedEventHandler AvanzarPresionado;

        public event RoutedEventHandler BibliotecaPresionado;
        public event RoutedEventHandler TiendaPresionado;
        public event RoutedEventHandler AmigosPresionado;


        //PARA EL MENU
        public event PropertyChangedEventHandler PropertyChanged;
        private string _nombreUsuario = "NombreUsuario";
        private string _estadoTexto = "En línea";
        private Color _estadoColor = (Color)ColorConverter.ConvertFromString("#FF5EBD3E");
        public event RoutedEventHandler VerPerfilPresionado;

        public Cabezera()
        {
            InitializeComponent();
            this.DataContext = this;

            // PRUEBA: CAMBIAR ESTADO MANUALMENTE
            EstadoActual = "Conectado";
        }

        //ATRAS CLICK
        private void BtnAtras_Click(object sender, RoutedEventArgs e)
        {
            //CAMBIAR A LA PÁGINA ANTERIOR
            AtrasPresionado?.Invoke(this, e);
        }

        //ADELANT CLICK
        private void BtnAdelante_Click(object sender, RoutedEventArgs e)
        {
            //CAMBIAR A LA PÁGINA SIGUIENTE
            AvanzarPresionado?.Invoke(this, e);
        }


        //BIBLIOTECA CLICK
        public void biblioteca_click(object sender, RoutedEventArgs e)
        {
            BibliotecaPresionado?.Invoke(this, e);
        }


        //TIENDA CLICK
        public void tienda_click(object sender, RoutedEventArgs e)
        {
            TiendaPresionado?.Invoke(this, e);
        }

        //AMIGOS CLICK
        public void amigos_click(object sender, RoutedEventArgs e)
        {
            AmigosPresionado?.Invoke(this, e);
        }




        //PARTE PARA EL USUARIO
        public string NombreUsuario
        {
            get => _nombreUsuario;
            set { _nombreUsuario = value; OnPropertyChanged(nameof(NombreUsuario)); }
        }

        public string EstadoTexto
        {
            get => _estadoTexto;
            set { _estadoTexto = value; OnPropertyChanged(nameof(EstadoTexto)); }
        }

        public Color EstadoColor
        {
            get => _estadoColor;
            set { _estadoColor = value; OnPropertyChanged(nameof(EstadoColor)); }
        }


        private void GridPerfilUsuario_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed
                && !StatusMenu.IsOpen)
            {
                StatusMenu.PlacementTarget = gridPerfilUsuario;
                StatusMenu.IsOpen = true;
                e.Handled = true;
            }
        }

        //CLICK EN EL MENU DE LOS ESTADOS
        private void StatusMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                string nuevoEstado = menuItem.Tag.ToString();
                
                EstadoActual = nuevoEstado;
            }
        }

        //VER PERFIL CLICK
        public void VerPerfil_Click(object sender, RoutedEventArgs e)
        {
            VerPerfilPresionado?.Invoke(this, e);
        }


        //PARA CAMBIAR EL ESTADO DE LA CUENTA
        private string _estadoActual;
        public string EstadoActual
        {
            get => _estadoActual;
            set
            {
                _estadoActual = value;
                switch (value)
                {
                    case "Conectado":
                        EstadoTexto = "En línea";
                        EstadoColor = (Color)ColorConverter.ConvertFromString("#FF5EBD3E");
                        break;
                    case "Ausente":
                        EstadoTexto = "Ausente";
                        EstadoColor = (Color)ColorConverter.ConvertFromString("#FFE6B905");
                        break;
                    case "Desconectado":
                        EstadoTexto = "Desconectado";
                        EstadoColor = (Color)ColorConverter.ConvertFromString("#FFBD3E3E");
                        break;
                    default:
                        EstadoTexto = "Invisible";
                        EstadoColor = (Color)ColorConverter.ConvertFromString("#656565");
                        break;
                }

                OnPropertyChanged(nameof(EstadoActual));
            }
        }
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }

}

