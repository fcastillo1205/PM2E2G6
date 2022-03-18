﻿using Newtonsoft.Json;
using PM2E2GRUPO6.Modelo;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PM2E2GRUPO6.Controlador
{
    public class ControladorSitios
    {
        public async static Task<Sitios_ID.RestSitios> GetSitios()
        {

            Sitios_ID.RestSitios listaSitios = new Sitios_ID.RestSitios();
            string url = DireccionesServidor.GetSitios;

            using (HttpClient cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var contenido = response.Content.ReadAsStringAsync().Result;

                    listaSitios = JsonConvert.DeserializeObject<Sitios_ID.RestSitios>(contenido);
                }

            }
            return listaSitios;
        }


        public async static Task<List<Modelo.FourSquare.Venue>> GetListSites(double latitud, double longitud)
        {

            List<Modelo.FourSquare.Venue> sitioscercas = new List<Modelo.FourSquare.Venue>();

 
            using (HttpClient cliente = new HttpClient())
            {
                var respuesta = await cliente.GetAsync(DireccionesServidor.getUrl(latitud, longitud));

                if (respuesta.IsSuccessStatusCode)
                {
                    var json = respuesta.Content.ReadAsStringAsync().Result;

                    var lugares = JsonConvert.DeserializeObject<Modelo.FourSquare.VenuesRest>(json);

                    sitioscercas = lugares.response.venues as List<Modelo.FourSquare.Venue>;
                }

            }



            return sitioscercas;
        }




    }
}
