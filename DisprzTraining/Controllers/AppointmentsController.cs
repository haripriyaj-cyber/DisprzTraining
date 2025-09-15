using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DisprzTraining.Business.Services;
using DisprzTraining.Models;
using DisprzTraining.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DisprzTraining.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IMapper _mapper;

        public AppointmentsController(IAppointmentService appointmentService, IMapper mapper)
        {
            _appointmentService = appointmentService;
            _mapper = mapper;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all appointments", Description = "Retrieves a list of all appointments")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AppointmentDTO>))]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
            return Ok(appointmentDtos);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get appointment by ID", Description = "Retrieves a specific appointment by its ID")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AppointmentDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                var appointmentDto = _mapper.Map<AppointmentDTO>(appointment);
                return Ok(appointmentDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new appointment", Description = "Creates a new appointment")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AppointmentDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDTO appointmentDto)
        {
            try
            {
                var appointment = await _appointmentService.CreateAppointmentAsync(appointmentDto);
                var createdAppointmentDto = _mapper.Map<AppointmentDTO>(appointment);
                return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, createdAppointmentDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update an appointment", Description = "Updates an existing appointment")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AppointmentDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentDTO appointmentDto)
        {
            try
            {
                var appointment = await _appointmentService.UpdateAppointmentAsync(id, appointmentDto);
                var updatedAppointmentDto = _mapper.Map<AppointmentDTO>(appointment);
                return Ok(updatedAppointmentDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete an appointment", Description = "Deletes an existing appointment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _appointmentService.DeleteAppointmentAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
