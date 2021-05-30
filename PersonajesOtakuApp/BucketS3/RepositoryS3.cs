using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonajesOtakuApp.BukcetS3
{
    public class RepositoryS3
    {

        private static string NombreBucket = "pruebabucket07";
        private static string RegionBucket = "us-east-2";


        private readonly IAmazonS3 amazonS3;

        public RepositoryS3(IAmazonS3 amazonS3)
        {
            this.amazonS3 = amazonS3;
        }


        public async Task<bool> UploadImageToS3(IFormFile file)
        {
            var putRequest = new PutObjectRequest()
            {
                BucketName = NombreBucket,
                Key = file.FileName,
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType
            };
            var response = await amazonS3.PutObjectAsync(putRequest);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;

        }


        public async void DeleteImageFromS3(string nameImage)
        {
            var deleteRequest = new DeleteObjectRequest()
            {
                BucketName = NombreBucket,
                Key = nameImage,
            };

            
            try
            {
                //no retorna una respuesta para saber si el objeto ha sido borrado
                var responseObject = await amazonS3.DeleteObjectAsync(deleteRequest);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }



        public string GetRutaImagen(string nombreImagen)
        {
            string rutaDefinitiva = "https://" + NombreBucket + ".s3." + RegionBucket + ".amazonaws.com/" + nombreImagen;
            return rutaDefinitiva;
        }


    }
}
