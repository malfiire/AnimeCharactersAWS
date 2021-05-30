using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using PersonajesOtakuApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonajesOtakuApp.DynamoDb
{
    public class PersonajesRepository : IPersonajes
    {
        //para acceder a dynamo db en aws
        private readonly AmazonDynamoDBClient client;
        //para realizar operaciones
        private readonly DynamoDBContext context;

        public PersonajesRepository()
        {
            client = new AmazonDynamoDBClient();
            context = new DynamoDBContext(client);
        }


        public async Task AddPersonaje(Personaje personaje)
        {
            personaje.Id = Guid.NewGuid();
            await context.SaveAsync(personaje);
        }


        public async Task<List<Personaje>> GetListaPersonajes()
        {
            //podemos poner condiciones a la hora de recuperar una lista
            var scanConditions = new List<ScanCondition>() { new ScanCondition("Id", Amazon.DynamoDBv2.DocumentModel.ScanOperator.IsNotNull) };
            var searchResults = context.ScanAsync<Personaje>(scanConditions, null);
            
            // devuelve la lista de personajes
            var resultado = await searchResults.GetNextSetAsync();
            return resultado;
        }

        public async Task<Personaje> GetPersonaje(Guid id)
        {
            return await context.LoadAsync<Personaje>(id);
        }

        public async Task UpdatePersonaje(Personaje personaje)
        {
            await context.SaveAsync(personaje);
        }
        public async Task DeletePersonaje(Guid id)
        {
            await context.DeleteAsync<Personaje>(id);
        }

        
    }
}
