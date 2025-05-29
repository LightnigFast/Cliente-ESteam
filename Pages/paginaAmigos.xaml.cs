using Cliente_TFG.Classes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static Cliente_TFG.Pages.paginaRecargaSaldo;

namespace Cliente_TFG.Pages
{
    /// <summary>
    /// Lógica de interacción para paginaAmigos.xaml
    /// </summary>
    public partial class paginaAmigos : Page
    {
        private MainWindow ventanaPrincipal;
        private string amigoActual = null;
        private Dictionary<string, List<MensajeChat>> historialChat = new Dictionary<string, List<MensajeChat>>();
        private List<SolicitudAmistad> solicitudesPendientes = new List<SolicitudAmistad>();
        private List<Amigo> listaDeAmigos = new List<Amigo>();
        private static readonly HttpClient client = new HttpClient();

        public paginaAmigos(MainWindow mainWindow)
        {
            InitializeComponent();
            ventanaPrincipal = mainWindow;

            InicializarDatosReales();

            // Inicializar datos de ejemplo
            InicializarDatosEjemplo();

            // Aplicar colores del tema actual
            AplicarTemaActual();
        }

        private void InicializarDatosReales()
        {
            InicializarCodigoDeAmigoDeUsuarioActual();

        }

        private void InicializarCodigoDeAmigoDeUsuarioActual()
        {
            if (ventanaPrincipal.Usuario.codigo_amigo != null)
            {
                txtIdAmigoUsuarioActual.Text = ventanaPrincipal.Usuario.codigo_amigo;
            }
            else
            {
                txtIdAmigoUsuarioActual.Text = "No tienes codigo de amigo";
            }
        }

        private void AplicarTemaActual()
        {
            // Aplicar colores a los elementos principales
            btnAceptar.Background = AppTheme.Actual.FondoPanel;
            btnAceptar.BorderBrush = AppTheme.Actual.BordePanel;
            btnAceptar.Foreground = AppTheme.Actual.TextoPrincipal;

            btnRechazar.Background = AppTheme.Actual.FondoPanel;
            btnRechazar.BorderBrush = AppTheme.Actual.BordePanel;
            btnRechazar.Foreground = AppTheme.Actual.TextoPrincipal;

            btnEnviar.Background = AppTheme.Actual.FondoPanel;
            btnEnviar.Foreground = AppTheme.Actual.BordePanel;
        }

        private void InicializarDatosEjemplo()
        {
            // Inicializar solicitudes de amistad
            solicitudesPendientes.Add(new SolicitudAmistad(123456, "Juan"));

            // Inicializar amigos
            listaDeAmigos.Add(new Amigo
            {
                NombreUsuario = "Pepe",
                IdUsuario = "100001",
                Estado = EstadoAmigo.Conectado
            });
            listaDeAmigos.Add(new Amigo
            {
                NombreUsuario = "Manolo",
                IdUsuario = "100002",
                Estado = EstadoAmigo.Conectado
            });
            listaDeAmigos.Add(new Amigo
            {
                NombreUsuario = "Javi",
                IdUsuario = "100003",
                Estado = EstadoAmigo.Conectado
            });

            // Inicializar historial de chat
            historialChat["Pepe"] = new List<MensajeChat>
            {
                new MensajeChat { Remitente = "Pepe", Contenido = "Hola, ¿cómo estás?", Fecha = DateTime.Now.AddHours(-2) },
                new MensajeChat { Remitente = "Yo", Contenido = "¡Muy bien! ¿Y tú?", Fecha = DateTime.Now.AddHours(-1) }
            };

            historialChat["Manolo"] = new List<MensajeChat>
            {
                new MensajeChat { Remitente = "Manolo", Contenido = "¿Jugamos una partida?", Fecha = DateTime.Now.AddDays(-1) }
            };

            historialChat["Javi"] = new List<MensajeChat>
            {
                new MensajeChat { Remitente = "Javi", Contenido = "¿Has visto las nuevas ofertas?", Fecha = DateTime.Now.AddDays(-2) },
                new MensajeChat { Remitente = "Yo", Contenido = "Sí, están geniales", Fecha = DateTime.Now.AddDays(-2).AddMinutes(5) }
            };
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para aceptar solicitud de amistad
            if (solicitudesPendientes.Count > 0)
            {
                var solicitud = solicitudesPendientes[0];

                // Agregar a la lista de amigos
                listaDeAmigos.Add(new Amigo
                {
                    NombreUsuario = solicitud.NombreUsuario,
                    IdUsuario = solicitud.IdUsuario,
                    Estado = EstadoAmigo.Conectado
                });

                // Crear un nuevo elemento en la lista de amigos
                AgregarAmigoALista(solicitud.NombreUsuario);

                // Eliminar la solicitud
                solicitudesPendientes.RemoveAt(0);

                // Actualizar la UI para mostrar que no hay solicitudes pendientes
                ActualizarSolicitudesPendientes();
            }
        }

        private void btnRechazar_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para rechazar solicitud de amistad
            if (solicitudesPendientes.Count > 0)
            {
                solicitudesPendientes.RemoveAt(0);
                ActualizarSolicitudesPendientes();
            }
        }

        private void ActualizarSolicitudesPendientes()
        {
            // Actualizar la UI para mostrar el número correcto de solicitudes pendientes
            // En una implementación real, esto se conectaría a una base de datos o servicio
        }

        private void AgregarAmigoALista(string nombreAmigo)
        {
            // Crear un nuevo elemento en la lista de amigos
            Border bordeAmigo = new Border
            {
                Background = AppTheme.Actual.FondoPanel,
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(5, 2, 5, 2)
            };

            bordeAmigo.MouseEnter += amigo_MouseEnter;
            bordeAmigo.MouseLeave += amigo_MouseLeave;
            bordeAmigo.MouseLeftButtonDown += amigo_MouseLeftButtonDown;
            bordeAmigo.Tag = nombreAmigo;

            Grid gridAmigo = new Grid { Height = 40 };
            gridAmigo.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            gridAmigo.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Indicador de estado
            Border indicadorEstado = new Border
            {
                Width = 10,
                Height = 10,
                Background = new SolidColorBrush(Color.FromRgb(76, 175, 80)), // Verde para conectado
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(5, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(indicadorEstado, 0);
            gridAmigo.Children.Add(indicadorEstado);

            // Nombre del amigo
            TextBlock textoNombre = new TextBlock
            {
                Text = nombreAmigo,
                Foreground = AppTheme.Actual.TextoPrincipal,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };
            Grid.SetColumn(textoNombre, 1);
            gridAmigo.Children.Add(textoNombre);

            bordeAmigo.Child = gridAmigo;
            listaAmigos.Children.Add(bordeAmigo);

            // Inicializar historial de chat vacío para el nuevo amigo
            if (!historialChat.ContainsKey(nombreAmigo))
            {
                historialChat[nombreAmigo] = new List<MensajeChat>();
            }
        }

        private void amigo_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = AppTheme.Actual.RatonEncima;
                border.Cursor = Cursors.Hand;
            }
        }

        private void amigo_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = AppTheme.Actual.FondoPanel;
                border.Cursor = Cursors.Arrow;
            }
        }

        private void amigo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                string nombreAmigo = "";

                // Obtener el nombre del amigo
                if (border.Tag != null)
                {
                    nombreAmigo = border.Tag.ToString();
                }
                else if (border.Name == "amigoPepe")
                {
                    nombreAmigo = "Pepe";
                }
                else if (border.Name == "amigoManolo")
                {
                    nombreAmigo = "Manolo";
                }
                else if (border.Name == "amigoJavi")
                {
                    nombreAmigo = "Javi";
                }

                // Cambiar el amigo actual y cargar su historial de chat
                amigoActual = nombreAmigo;
                CargarHistorialChat(nombreAmigo);
            }
        }

        private void CargarHistorialChat(string nombreAmigo)
        {
            // Limpiar el área de mensajes
            areaMensajes.Children.Clear();

            // Verificar si existe historial para este amigo
            if (historialChat.ContainsKey(nombreAmigo))
            {
                foreach (var mensaje in historialChat[nombreAmigo])
                {
                    AgregarMensajeAChat(mensaje);
                }
            }

            // Hacer scroll hasta el último mensaje
            scrollMensajes.ScrollToEnd();
        }

        private void AgregarMensajeAChat(MensajeChat mensaje)
        {
            // Crear el contenedor del mensaje
            Border bordeMensaje = new Border
            {
                Padding = new Thickness(10),
                Margin = new Thickness(5),
                CornerRadius = new CornerRadius(10),
                MaxWidth = 400
            };

            // Configurar el estilo según el remitente
            if (mensaje.Remitente == "Yo")
            {
                bordeMensaje.Background = AppTheme.Actual.BordePanel;
                bordeMensaje.HorizontalAlignment = HorizontalAlignment.Right;
            }
            else
            {
                bordeMensaje.Background = AppTheme.Actual.RatonEncima;
                bordeMensaje.HorizontalAlignment = HorizontalAlignment.Left;
            }

            // Crear el contenido del mensaje
            StackPanel contenidoMensaje = new StackPanel();

            // Texto del mensaje
            TextBlock textoMensaje = new TextBlock
            {
                Text = mensaje.Contenido,
                TextWrapping = TextWrapping.Wrap,
                Foreground = mensaje.Remitente == "Yo" ? AppTheme.Actual.FondoPanel : AppTheme.Actual.TextoPrincipal
            };
            contenidoMensaje.Children.Add(textoMensaje);

            // Hora del mensaje
            TextBlock textoHora = new TextBlock
            {
                Text = mensaje.Fecha.ToString("HH:mm"),
                FontSize = 10,
                HorizontalAlignment = HorizontalAlignment.Right,
                Foreground = mensaje.Remitente == "Yo" ? AppTheme.Actual.FondoPanel : AppTheme.Actual.TextoSecundario,
                Margin = new Thickness(0, 5, 0, 0)
            };
            contenidoMensaje.Children.Add(textoHora);

            bordeMensaje.Child = contenidoMensaje;
            areaMensajes.Children.Add(bordeMensaje);

            // Aplicar animación de fade in
            AplicarFadeIn(bordeMensaje);
        }

        private void AplicarFadeIn(UIElement elemento)
        {
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            elemento.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        }

        private void btnEnviar_Click(object sender, RoutedEventArgs e)
        {
            EnviarMensaje();
        }

        private void txtMensaje_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EnviarMensaje();
            }
        }

        private void EnviarMensaje()
        {
            string contenido = txtMensaje.Text.Trim();

            if (!string.IsNullOrEmpty(contenido) && !string.IsNullOrEmpty(amigoActual))
            {
                // Crear nuevo mensaje
                MensajeChat nuevoMensaje = new MensajeChat
                {
                    Remitente = "Yo",
                    Contenido = contenido,
                    Fecha = DateTime.Now
                };

                // Agregar al historial
                historialChat[amigoActual].Add(nuevoMensaje);

                // Mostrar en la UI
                AgregarMensajeAChat(nuevoMensaje);

                // Limpiar el campo de texto
                txtMensaje.Text = string.Empty;

                // Hacer scroll hasta el último mensaje
                scrollMensajes.ScrollToEnd();

                // Simular respuesta después de un tiempo aleatorio (solo para demostración)
                SimularRespuesta();
            }
        }

        private void SimularRespuesta()
        {
            // Solo para demostración: simular una respuesta después de un tiempo aleatorio
            if (!string.IsNullOrEmpty(amigoActual))
            {
                Random rnd = new Random();
                int tiempoEspera = rnd.Next(1000, 3000); // Entre 1 y 3 segundos

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(tiempoEspera);
                timer.Tick += (s, e) =>
                {
                    // Detener el timer
                    timer.Stop();

                    // Crear respuesta simulada
                    string[] respuestas = new string[] {
                        "¡Genial!",
                        "¿Qué tal va todo?",
                        "¿Jugamos una partida?",
                        "Vale, hablamos luego",
                        "Estoy ocupado ahora mismo",
                        "¿Has visto las nuevas ofertas de la tienda?"
                    };

                    string respuesta = respuestas[rnd.Next(respuestas.Length)];
                    MensajeChat mensajeRespuesta = new MensajeChat
                    {
                        Remitente = amigoActual,
                        Contenido = respuesta,
                        Fecha = DateTime.Now
                    };

                    // Agregar al historial
                    historialChat[amigoActual].Add(mensajeRespuesta);

                    // Mostrar en la UI
                    AgregarMensajeAChat(mensajeRespuesta);

                    // Hacer scroll hasta el último mensaje
                    scrollMensajes.ScrollToEnd();
                };
                timer.Start();
            }
        }

        private void btnBuscarAmigos_Click(object sender, RoutedEventArgs e)
        {
            // Implementar lógica para buscar amigos
            MessageBox.Show("Funcionalidad de búsqueda de amigos en desarrollo", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Clases auxiliares para manejar los datos
        public class MensajeChat
        {
            public string Remitente { get; set; }
            public string Contenido { get; set; }
            public DateTime Fecha { get; set; }
        }

        public class SolicitudAmistad
        {
            public SolicitudAmistad(int id_usuario, string nombre_usuario)
            {
                IdUsuario = id_usuario.ToString();
                NombreUsuario = nombre_usuario;
            }

            public string NombreUsuario { get; set; }
            public string IdUsuario { get; set; }
        }

        public class Amigo
        {
            public string NombreUsuario { get; set; }
            public string IdUsuario { get; set; }
            public EstadoAmigo Estado { get; set; }
        }

        public enum EstadoAmigo
        {
            Conectado,
            Ausente,
            Ocupado,
            Desconectado
        }

        private void btnCopiarIdAmigo_Click(object sender, RoutedEventArgs e)
        {
            String output = txtIdAmigoUsuarioActual.Text;
            System.Windows.Clipboard.SetText(output);
        }

        private void btnEnviarSolicitud_Click(object sender, RoutedEventArgs e)
        {
            String friendCodeABuscar = txtBuscarAmigos.Text;
            if (String.IsNullOrEmpty(friendCodeABuscar)) 
            {
                MessageBox.Show("Tienes que rellenar el campo con el código de amigo del usuario al que quieres agregar");
                return;
            }
            SolicitudAmistad user = GetUsuarioPorCodigoDeAmigo(friendCodeABuscar);
            if (user == null)
            {
                MessageBox.Show($"No existe un usuario con codigo de amigo: {friendCodeABuscar}");
                return;
            }
            MandarSolicitudDeAmistadAsync(user);
        }

        private async void MandarSolicitudDeAmistadAsync(SolicitudAmistad user)
        {
            string url = $"http://{ventanaPrincipal.IP}:50000/friend_list/solicitudes_amistad";
            try
            {
                // Validar inputs
                if (ventanaPrincipal.user == null || ventanaPrincipal.user.id_usuario <= 0)
                {
                    MessageBox.Show("Error: El usuario actual no está definido o tiene un ID inválido.");
                    return;
                }
                if (user == null)
                {
                    MessageBox.Show("Error: El usuario destino no está definido o tiene un ID inválido.");
                    return;
                }

                // Construir la cadena del body
                string jsonBody = $"{{\n  \"de_usuario_id\": {ventanaPrincipal.user.id_usuario},\n  \"para_usuario_id\": {user.IdUsuario}\n}}";
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Hacer la solicitud
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Solicitud de amistad enviada correctamente.");
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error del servidor: {errorResponse} (Código: {response.StatusCode})");
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Error de red: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}");
            }

        }

        // TODO - Comprobar errores
        private SolicitudAmistad GetUsuarioPorCodigoDeAmigo(String friendCode)
        {
            try
            {
                if (string.IsNullOrEmpty(friendCode))
                {
                    return null;
                }
                var url = $"http://" + ventanaPrincipal.IP + $":50000/users/friend_code/{friendCode}";
                using (var webClient = new WebClient())
                {
                    string jsonString;
                    try
                    {
                        jsonString = webClient.DownloadString(url);
                    }
                    catch (WebException ex)
                    {
                        // El mensaje de la excepcion ya salta en el siguiente metodo
                        return null;
                    }

                    var jsonObj = JObject.Parse(jsonString);
                    var usuarioJson = jsonObj["usuario"];

                    int id_usuario = (int)usuarioJson["id_usuario"];
                    string nombre_usuario = (string)usuarioJson["nombre_usuario"];
                    return new SolicitudAmistad(id_usuario, nombre_usuario);
                }
            }
            catch 
            {
                return null;
            }
        }
    }
}