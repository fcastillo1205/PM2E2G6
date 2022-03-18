using Newtonsoft.Json;
using Plugin.AudioRecorder;
using Plugin.Media;
using PM2E2GRUPO6.Controlador;
using PM2E2GRUPO6.Modelo;
using PM2E2GRUPO6.Vista;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
//using Plugin.AudioRe corder;

namespace PM2E2GRUPO6
{
    public partial class MainPage : ContentPage
    {
        CancellationTokenSource cts;
        string lati = "", longi = "", base64Val="";
        private readonly AudioRecorderService audioRecorderService = new AudioRecorderService();
        private readonly AudioPlayer audioPlayer = new AudioPlayer();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void btnfotocap_Clicked(object sender, EventArgs e)
        {
            var tomarfoto = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "miApp",
                Name = "Image.jpg"

            });




            if (tomarfoto != null)
            {
                imagen.Source = ImageSource.FromStream(() => { return tomarfoto.GetStream(); });
            }

            Byte[] imagenByte = null;

            using (var stream = new MemoryStream())
            {
                tomarfoto.GetStream().CopyTo(stream);
                tomarfoto.Dispose();
                imagenByte = stream.ToArray();

                base64Val = Convert.ToBase64String(imagenByte);
            }


        }

        private async void btnsalnvar_Clicked(object sender, EventArgs e)
        {

            
            if (String.IsNullOrWhiteSpace(base64Val) == true)
            {
                await DisplayAlert("Mensaje", "Foto vacia", "Ok");
            }
            else
            {
                if(String.IsNullOrWhiteSpace(txtdescripLarga.Text) == true)
                {
                    await DisplayAlert("Mensaje", "La Descripcion esta vacia", "Ok");
                }
                else
                {
                    _ = GetCurrentLocation(true);
                }
            }

        }
        /* FUNCION AL PRESIONAR EL ICONO DE AUDIO PARA EMPEZAR A GRABAR/DETENER AUDIO */
        async void recordAudio_Clicked(object sender, EventArgs e)
        {
            var status = await Permissions.RequestAsync<Permissions.Microphone>();
            var status2 = await Permissions.RequestAsync<Permissions.StorageRead>();
            var status3 = await Permissions.RequestAsync<Permissions.StorageWrite>();

            if (status != PermissionStatus.Granted || status2 != PermissionStatus.Granted || status3 != PermissionStatus.Granted)
                return;

            if (audioRecorderService.IsRecording)
            {
                await audioRecorderService.StopRecording(); /* LINEA DE CODIGO PARA DETENER GRABACIÓN */
                audioPlayer.Play(audioRecorderService.GetAudioFilePath()); /* REPRODUCTOR DEL AUDIO audioPlayer.Play(); en los parentesis colocar la variable que trae el audio */

                var audioPath = audioRecorderService.GetAudioFilePath(); /* audioPath es la ubicacion donde se guarda localmente, esa misma variable usar para mandar directamente el audio */

                await DisplayAlert(null, "Grabación Guardada " + audioPath, "OK");
            }
            else
            {
                await audioRecorderService.StartRecording(); /* LINEA DE CODIGO PARA GRABAR GRABACIÓN */
                await DisplayAlert(null, "Grabando", "OK");
            }
        }

        private async void btnlis_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Listado_Sitios());
        }


        public async void EvaluarInternet()
        {
            var current = Connectivity.NetworkAccess;

            // en caso de tener internet obtener la ubicacion
            if (current == NetworkAccess.Internet)
            {
                // Connection to internet is available
                //es falso para no guardar informacion solo mostrar, si el parametro es true guarda
                _ = GetCurrentLocation(false);
            }
            else
            {
                await DisplayAlert("error", "Sin Internet", "Ok");

            }
        }


        async Task GetCurrentLocation( bool guardar )
        {

            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if(location == null)
                {
                    await DisplayAlert("error", "GPS Inactivo", "Ok");
                }

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");

                    lati = location.Latitude.ToString();
                    longi = location.Longitude.ToString();
                    lbllatitud.Text = lati;
                    lbllongitud.Text = longi;


    
                    if(guardar == true)
                    {
                        GuardarSitios();
                    }

                }
            }
            catch (FeatureNotSupportedException)
            {
                // Handle not supported on device exception
                await DisplayAlert("error", "no es compatible con la excepción del dispositivo GPS", "Ok");

            }
            catch (FeatureNotEnabledException)
            {
                // Handle not enabled on device exception
                await DisplayAlert("error", "la ubicacion no habilitado en la excepción del dispositivo", "Ok");
            }
            catch (PermissionException)
            {
                // Handle permission exception
                await DisplayAlert("error", "No tiene Permisos de ubicacion", "Ok");
            }
            catch (Exception)
            {
                // Unable to get location
                await DisplayAlert("error", "No se puede tener la ubicacion", "Ok");
            }
        }

       
        protected override void OnDisappearing()
        {
            if (cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
            base.OnDisappearing();
        }

        private async void btnCapturarFoto_Clicked(object sender, EventArgs e)
        {

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                return;
            }
            try
            {
                Stream streama = null;
                var file = await CrossMedia.Current.PickPhotoAsync().ConfigureAwait(true);


                if (file == null)
                    return;

                streama = file.GetStream();


                imagen.Source = ImageSource.FromStream(() => streama);

                Byte[] imagenByte = null;

                using (var stream = new MemoryStream())
                {
                    
                    file.GetStream().CopyTo(stream);
                    file.Dispose();
                    imagenByte = stream.ToArray();

                    base64Val = Convert.ToBase64String(imagenByte);
                    
                }




            }
            catch 
            {
                
            }
        }

        protected override void OnAppearing()
        {
             
            base.OnAppearing();
            EvaluarInternet();
        }

        public async void GuardarSitios()
        {


            Sitios siti = new Sitios
            {
                descripcion = txtdescripLarga.Text,
                latitud = lati,
                longitud = longi,
                fotografia = "data:image/png;base64," + base64Val

            };


            try
            {
                using (HttpClient cliente = new HttpClient())
                {
                    Uri RequestUri = new Uri(DireccionesServidor.saveSitios);
                    var json = JsonConvert.SerializeObject(siti);
                    var contentJSON = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await cliente.PostAsync(RequestUri, contentJSON);

                    if (response.IsSuccessStatusCode)
                    {

                        await DisplayAlert("Datos", "Regitrado Exitosamente", "OK");
                        txtdescripLarga.Text =  base64Val = String.Empty;
                        imagen.Source = "perfil.jpg";
                    }
                    


                }
            }
            catch(Exception e)
            {
                await DisplayAlert("Datos", "No se Puedo registrar "+e.Message, "OK");
            }


        }
    }
}
