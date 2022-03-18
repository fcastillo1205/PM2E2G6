using Newtonsoft.Json;
using Plugin.Media;
using PM2E2GRUPO6.Controlador;
using PM2E2GRUPO6.Modelo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PM2E2GRUPO6.Clases;
using System.Text.RegularExpressions;

namespace PM2E2GRUPO6.Vista
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Modificar : ContentPage
    {
        CancellationTokenSource cts;
        string lati = "", longi = "", base64Val = "";

       

        public Modificar(string pId, string pDescripcion, string pLatitud, string pLongitud, string pImagen, string pAudio)
        {
            InitializeComponent();
            txtDescripcion.Text = pDescripcion;
            txtId.Text = pId;

            string base64Image = pImagen;

            Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
            base64Image = regex.Replace(base64Image, string.Empty);

            var imageBytes = System.Convert.FromBase64String(base64Image);

            // Return a new ImageSource
            imagen.Source = ImageSource.FromStream(() => { return new MemoryStream(imageBytes); });

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
                    //file.GetStream().CopyTo(stream);
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
                if (String.IsNullOrWhiteSpace(txtDescripcion.Text) == true)
                {
                    await DisplayAlert("Mensaje", "La Descripcion esta vacia", "Ok");
                }
                else
                {
                    _ = GetCurrentLocation(true);
                }
            }
        }

        public async void EvaluarInternet()
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                _ = GetCurrentLocation(false);
            }
            else
            {
                await DisplayAlert("error", "Sin Internet", "Ok");

            }
        }


        async Task GetCurrentLocation(bool guardar)
        {


            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location == null)
                {
                    await DisplayAlert("error", "GPS Inactivo", "Ok");
                }

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");

                    lati = location.Latitude.ToString();
                    longi = location.Longitude.ToString();
                    txtLatitud.Text = lati;
                    txtLongitud.Text = longi;


                    if (guardar == true)
                    {
                         ModificarSitios();
                    }

                }
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("error", "no es compatible con la excepción del dispositivo GPS", "Ok");

            }
            catch (FeatureNotEnabledException)
            {
                await DisplayAlert("error", "la ubicacion no habilitado en la excepción del dispositivo", "Ok");
            }
            catch (PermissionException)
            {
                await DisplayAlert("error", "No tiene Permisos de ubicacion", "Ok");
            }
            catch (Exception)
            {
                await DisplayAlert("error", "No se puede tener la ubicacion", "Ok");
            }
        }

        protected override void OnDisappearing()
        {
            if (cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
            base.OnDisappearing();
        }


        protected override void OnAppearing()
        {

            base.OnAppearing();
            EvaluarInternet();
        }

        public async void ModificarSitios()
        {


            Sitios_ID.SitiosID siti = new Sitios_ID.SitiosID();
            {
                siti.id = txtId.Text.ToString();
                siti.descripcion = txtDescripcion.Text.ToString();
                siti.latitud = txtLatitud.Text.ToString();
                siti.longitud = txtLongitud.Text.ToString();
                siti.fotografia = "data:image/png;base64," + base64Val;

            };


            try
            {
                using (HttpClient cliente = new HttpClient())
                {
                    Uri RequestUri = new Uri(DireccionesServidor.UpdateSitios);
                    var json = JsonConvert.SerializeObject(siti);
                    var contentJSON = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await cliente.PostAsync(RequestUri, contentJSON);

                    if (response.IsSuccessStatusCode)
                    {

                        await DisplayAlert("Datos", "Sitio Modificado Exitosamente", "OK");
                        await Navigation.PushAsync(new Listado_Sitios());
                    }



                }
            }
            catch (Exception e)
            {
                await DisplayAlert("Datos", "No se Puedo registrar " + e.Message, "OK");
            }


        }

    }
}