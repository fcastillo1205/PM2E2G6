using Newtonsoft.Json;
using PM2E2GRUPO6.Controlador;
using PM2E2GRUPO6.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PM2E2GRUPO6.Vista
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Listado_Sitios : ContentPage
    {
        public Listado_Sitios()
        {
            InitializeComponent();
            Mostrar();
        }

        public async void Mostrar()
        {
            Sitios_ID.RestSitios listaSitios = new Sitios_ID.RestSitios();
            listaSitios = await ControladorSitios.GetSitios();
            listas.ItemsSource = listaSitios.Sitios_ID;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void listas_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
           
            var obj = (Sitios_ID.SitiosID)e.SelectedItem;

            string accion = await DisplayActionSheet("Acciones", "Cancelar", null, "Modificar", "Eliminar", "Mapa");

            if (accion == "Modificar")
            {
                await Navigation.PushAsync(new NavigationPage(new Modificar(obj.id,obj.descripcion,obj.latitud,obj.longitud,obj.fotografia,obj.audio)));
            }else if(accion == "Eliminar")
            {
                Sitios_ID.SitiosID delete = new Sitios_ID.SitiosID
                {
                    id = obj.id
                };


                try
                {
                    using (HttpClient cliente = new HttpClient())
                    {
                        Uri RequestUri = new Uri(DireccionesServidor.DeleteSitios);
                        var json = JsonConvert.SerializeObject(delete);
                        var contentJSON = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await cliente.PostAsync(RequestUri, contentJSON);

                        if (response.IsSuccessStatusCode)
                        {
                            Mostrar();
                            Console.WriteLine(response.Content.ReadAsStringAsync().Result);

                        }
                    }
                }
                catch (Exception)
                {

                }
            }else if(accion == "Mapa")
            {
                await Navigation.PushAsync(new NavigationPage(new PageMap( Convert.ToDouble(obj.latitud), Convert.ToDouble(obj.longitud), obj.descripcion)));
            }
           


        }
    }
}