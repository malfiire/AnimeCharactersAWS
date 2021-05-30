using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonajesOtakuApp.Models
{
    [DynamoDBTable("personajes_otaku")]
    public class Personaje
    {
        [DynamoDBProperty("id")]
        [DynamoDBHashKey]
        public Guid Id{ get; set; }

        [DynamoDBProperty("nombre")]
        public string Nombre { get; set; }

        [DynamoDBProperty("anime")]
        public string Anime { get; set; }

        [DynamoDBProperty("habilidad")]
        public string Habilidad { get; set; }

        [DynamoDBProperty("rutaImagen")]
        public string RutaImagen { get; set; }


    }
}
