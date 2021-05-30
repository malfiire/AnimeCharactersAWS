using PersonajesOtakuApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonajesOtakuApp.DynamoDb
{
    public interface IPersonajes
    {
        Task AddPersonaje(Personaje personaje);
        Task<Personaje> GetPersonaje(Guid id);
        Task<List<Personaje>> GetListaPersonajes();
        Task UpdatePersonaje(Personaje personaje);
        Task DeletePersonaje(Guid id);


    }
}
