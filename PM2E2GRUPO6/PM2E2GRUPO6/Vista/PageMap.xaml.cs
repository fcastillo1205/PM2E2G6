using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace PM2E2GRUPO6.Vista
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageMap : ContentPage
    {
        double latitud, longitud;
        public PageMap(double pLatitud, double pLongitud, string pDescripcion)
        {
            InitializeComponent();

            latitud = pLatitud;
            longitud = pLongitud;

            Pin pin = new Pin
            {
                Label = pDescripcion,
                Address = "Ubicacion",
                Type = PinType.Place,
                Position = new Position(pLatitud, pLongitud)
            };
            MapaPais.Pins.Add(pin);
            MapaPais.MoveToRegion(mapSpan: MapSpan.FromCenterAndRadius(new Position(pLatitud, pLongitud), Distance.FromKilometers(0.10)));

        }

        private void btnInDrive_Clicked(object sender, EventArgs e)
        {
            Xamarin.Essentials.Map.OpenAsync(latitud, longitud, new MapLaunchOptions { NavigationMode = NavigationMode.Driving });
        }
    }
}