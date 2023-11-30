namespace PeliculasAPI.Servicios
{
    public class AlmacenadorArchivosLocal : IAlmacenadorArchivos
    {
        private readonly IWebHostEnvironment env; //Con esta variable obtenemos la ruta de donde se encuentra el wwwroot
        private readonly IHttpContextAccessor httpContextAccessor;//Determinamos el dominio donde tenemos publicado el webapi

        public AlmacenadorArchivosLocal(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task BorrarArchivo(string ruta, string contenedor)
        {
            if (ruta != null)
            {
                var nombreArchivo = Path.GetFileName(ruta);
                string directorio = Path.Combine(env.WebRootPath, contenedor, nombreArchivo);
                if (File.Exists(directorio))
                {
                    File.Delete(directorio);
                }

            }
            return Task.FromResult(0);
        }

        public async Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string ruta, string contentType)
        {
            await BorrarArchivo(ruta, contenedor);
            return await GuardarArchivo(contenido, extension, contenedor, contentType);
        
        }

        public async Task<string> GuardarArchivo(byte[] contenido, string extennsion, string contenedor, string contentType)
        {
            var nombreArchivo = $"{Guid.NewGuid()}{extennsion}";
            //Combinamos la direccion del wwwroot con el contenedor, que es el nombre de la carpeta
            string folder = Path.Combine(env.WebRootPath, contenedor);
            if(!Directory.Exists(folder)) 
            {
                Directory.CreateDirectory(folder);
            }

            string ruta = Path.Combine(folder, nombreArchivo);
            //Escribimos en el disco duro el contenido del archivo
            await File.WriteAllBytesAsync(ruta, contenido);
            var urlActual = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var urlParaBD = Path.Combine(urlActual, contenedor, nombreArchivo).Replace("\\", "/");
            return urlParaBD;
        }
    }
}
