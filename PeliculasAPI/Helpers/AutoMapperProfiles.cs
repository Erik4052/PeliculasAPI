﻿using AutoMapper;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles() 
        {
            CreateMap<Genero, GeneroDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Genero>();

            CreateMap<Actor, ActorDTO>().ReverseMap();
            //ForMember ayuda a ignorar la propiedad que estamos seleccionando en la arrow function, que es la foto en este caso
            CreateMap<ActorCreacionDTO, Actor>().ForMember(x => x.Foto, options => options.Ignore());
            CreateMap<ActorPatchDTO, Actor>().ReverseMap();

            CreateMap<Pelicula, PeliculaDTO>().ReverseMap();
            //ForMember ayuda a ignorar la propiedad que estamos seleccionando en la arrow function, que es la foto en este caso
            CreateMap<PeliculaCreacionDTO, Pelicula>().ForMember(x => x.Poster, options => options.Ignore());
            CreateMap<PeliculaPatchDTO, Pelicula>().ReverseMap();
        }
    }
}
