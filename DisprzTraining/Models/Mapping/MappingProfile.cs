using AutoMapper;
using DisprzTraining.Models;
using DisprzTraining.Models.DTOs;
using System;

namespace DisprzTraining.Utils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Appointment -> AppointmentDTO
            CreateMap<Appointment, AppointmentDTO>();
            
            // CreateAppointmentDTO -> Appointment
            CreateMap<CreateAppointmentDTO, Appointment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore()); // Ignore User navigation property
            
            // UpdateAppointmentDTO -> Appointment
            CreateMap<UpdateAppointmentDTO, Appointment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Ignore UserId
                .ForMember(dest => dest.User, opt => opt.Ignore()); // Ignore User navigation property
            
            // User -> UserDTO
            CreateMap<User, UserDTO>();
            
            // RegisterDTO -> User
            CreateMap<RegisterDTO, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow)) // Set CreatedAt
                .ForMember(dest => dest.Appointments, opt => opt.Ignore()); // Ignore Appointments collection
        }
    }
}
