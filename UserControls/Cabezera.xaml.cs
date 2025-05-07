using System;
using System.Collections.Generic;
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
    public partial class Cabezera : UserControl
    {
        public event RoutedEventHandler BibliotecaPresionado;
        public event RoutedEventHandler TiendaPresionado;
        public event RoutedEventHandler AmigosPresionado;

        public Cabezera()
        {
            InitializeComponent();
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


    }
}
