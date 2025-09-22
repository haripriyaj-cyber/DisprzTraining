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
            // Appointment -> AppointmentDTO (this mapping is fine)
            CreateMap<Appointment, AppointmentDTO>();
            
            // CreateAppointmentDTO -> Appointment
            CreateMap<CreateAppointmentDTO, Appointment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            
            // UpdateAppointmentDTO -> Appointment
            CreateMap<UpdateAppointmentDTO, Appointment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            
            // Add these mappings
            CreateMap<User, UserDTO>();
            CreateMap<RegisterDTO, User>();
        }
    }
}
