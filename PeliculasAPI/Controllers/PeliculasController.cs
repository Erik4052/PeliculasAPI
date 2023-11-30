using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Servicios;

namespace PeliculasAPI.Controllers
{
        [ApiController]
        [Route("api/peliculas")]
        public class PeliculasController: ControllerBase
        {
            private readonly ApplicationDbContext context;
            private readonly IMapper mapper;
            private readonly IAlmacenadorArchivos almacenadorArchivos;
            private readonly string contenedor = "peliculas";

            public PeliculasController(
                ApplicationDbContext context, 
                IMapper mapper,
                IAlmacenadorArchivos almacenadorArchivos )
            {
                this.context = context;
                this.mapper = mapper;
                this.almacenadorArchivos = almacenadorArchivos;
            }

        [HttpGet]
        public async Task<ActionResult<List<PeliculaDTO>>> Get()
        {
            var peliculas = await context.Pelicuas.ToListAsync();
            return mapper.Map<List<PeliculaDTO>>(peliculas);
        }

        [HttpGet("{id}", Name = "obtenerPelicula")]
        public async Task<ActionResult<PeliculaDTO>> Get(int id)
        {
            var pelicula = await context.Pelicuas.FirstOrDefaultAsync(x => x.Id == id);
            
            if(pelicula == null)
            {
                return NotFound();
            }

            return mapper.Map<PeliculaDTO>(pelicula);

        }


        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var pelicula = mapper.Map<Pelicula>(peliculaCreacionDTO);
            if (peliculaCreacionDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await peliculaCreacionDTO.Poster.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();//Arreglo de bytes a subir a Azure Storage
                    var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);//Metodo que nos ayudara a tener la extension del archivo
                    pelicula.Poster = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor, peliculaCreacionDTO.Poster.ContentType); ;
                }
            }
            context.Add(pelicula);
            await context.SaveChangesAsync();
            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return new CreatedAtRouteResult("obtenerPelicula", new {id = pelicula.Id }, peliculaDTO);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var peliculaDB = await context.Pelicuas.FirstOrDefaultAsync(x => x.Id == id);
            if (peliculaDB == null)
            {
                return NotFound();
            }
            /*
             pasa los datos de peliculasCreacionDTO a actorDB, 
             de esta forma se mapea a la clase, utilizando el 
             metodo Map del mapper. Esto lo hacemos para actualizar 
             bien los datos
            */
            peliculaDB = mapper.Map(peliculaCreacionDTO, peliculaDB);

            if (peliculaCreacionDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await peliculaCreacionDTO.Poster.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();//Arreglo de bytes a subir a Azure Storage
                    var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);//Metodo que nos ayudara a tener la extension del archivo
                    peliculaDB.Poster = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, peliculaCreacionDTO.Poster.ContentType, peliculaDB.Poster); ;
                }
            }

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PeliculaPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entidadDB = await context.Pelicuas.FirstOrDefaultAsync(x => x.Id == id);
            if (entidadDB == null)
            {
                return NotFound();
            }

            var entidadDTO = mapper.Map<PeliculaPatchDTO>(entidadDB);
            patchDocument.ApplyTo(entidadDTO, ModelState);

            var esValido = TryValidateModel(entidadDTO);

            if (!esValido)
            {
                return BadRequest();
            }

            mapper.Map(entidadDTO, entidadDB);
            //Manda los cambios a la base de datos
            await context.SaveChangesAsync();
            //NoContent es un 204
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Pelicuas.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Pelicula() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
