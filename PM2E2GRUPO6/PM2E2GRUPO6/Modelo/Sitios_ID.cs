using System;
using System.Collections.Generic;
using System.Text;

namespace PM2E2GRUPO6.Modelo
{
   public class Sitios_ID
    {
        public class SitiosID
        {
            public string id { get; set; }
            public string descripcion { get; set; }
            public string latitud { get; set; }
            public string longitud { get; set; }
            public string fotografia { get; set; }
            public string audio { get; set; }
        }

        public class RestSitios
        {
            public IList<SitiosID> Sitios_ID { get; set; }
        }

    }
}
