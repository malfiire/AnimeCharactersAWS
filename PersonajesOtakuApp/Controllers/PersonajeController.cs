using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonajesOtakuApp.BukcetS3;
using PersonajesOtakuApp.DynamoDb;
using PersonajesOtakuApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PersonajesOtakuApp.Controllers
{
    [Route("[controller]")]
    public class PersonajeController : Controller
    {

        private readonly IPersonajes repository;
        private readonly RepositoryS3 bucketS3;
        public PersonajeController(IPersonajes repository, RepositoryS3 bucketS3)
        {
            this.repository = repository;
            this.bucketS3 = bucketS3;
        }

        
        public async Task<IActionResult> Index()
        {
            var personajes = await repository.GetListaPersonajes();
            return View(personajes);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View("CreateAndEdit", new Personaje());
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult>Create(Personaje personaje, [FromForm] IFormFile file)
        {
            try
            {
                //insertar la imagen en el bucket
                bool insertada = await bucketS3.UploadImageToS3(file);
                if (insertada)
                {
                    //llamar  a la funcion que te crea la url con esa foto
                    string ruta = bucketS3.GetRutaImagen(file.FileName);

                    //actualizamos el objeto ya con la ruta de la imagen
                    personaje.RutaImagen = ruta;

                    //el objeto tiene que tener imagen para poder subirlo
                    //al dynamodb
                    await repository.AddPersonaje(personaje);
                }

                
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View();
            }

        }

        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var personaje = await repository.GetPersonaje(id);

            return View("CreateAndEdit", personaje);
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> Edit(Personaje  personaje, [FromForm] IFormFile file)
        {
            //comprobar si ha modificado la foto
            //si el archivo file => es != nulo => ha modificado la foto
            if (file!=null)
            {
                //obtener el nombre de la foto
                var nameImage = Path.GetFileName(personaje.RutaImagen);
                //borrar la foto antigua en el s3 
                bucketS3.DeleteImageFromS3(nameImage);

                //subir la foto nueva en el s3
                bool responseUploadImage = await bucketS3.UploadImageToS3(file);
                    if (responseUploadImage)
                    {
                        //la imagen se ha subido
                        //obtenemos la ruta de la foto
                        var rutaImagen = bucketS3.GetRutaImagen(file.FileName);
                        //modificar el objeto personaje con la nueva ruta de la foto
                        personaje.RutaImagen = rutaImagen;
                        //actualizar en dynamodb
                        await repository.UpdatePersonaje(personaje);

                        return RedirectToAction("Index");
                    }
                

            }
            else
            {
                //el usuario no ha modificado nada o ha modificado algún campo diferente a la foto
                await repository.UpdatePersonaje(personaje);
                return RedirectToAction("Index");
            }

            return View();


        }



        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult>Delete(Guid id)
        {
            //obtener el objeto con ese id
            var personaje = await repository.GetPersonaje(id);

            //obtener el nombre de la foto
            var nombreImagen = Path.GetFileName(personaje.RutaImagen);

            //eliminar la foto del bucket
            bucketS3.DeleteImageFromS3(nombreImagen);

            //eliminar el objeto de dynamodb
            await repository.DeletePersonaje(id);

            return RedirectToAction("Index");

        }




    }



}

