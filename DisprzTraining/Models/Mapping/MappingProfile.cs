using AutoMapper;
using DisprzTraining.Models;
using DisprzTraining.Models.DTOs;

namespace DisprzTraining.Utils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Appointment, AppointmentDTO>();
            CreateMap<CreateAppointmentDTO, Appointment>();
            CreateMap<UpdateAppointmentDTO, Appointment>();
        }
    }
}
