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
    /// <summary>
    /// API endpoints for managing calendar appointments
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [SwaggerTag("Create, read, update and delete appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the AppointmentsController
        /// </summary>
        /// <param name="appointmentService">The appointment service</param>
        /// <param name="mapper">The AutoMapper instance</param>
        public AppointmentsController(IAppointmentService appointmentService, IMapper mapper)
        {
            _appointmentService = appointmentService;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all appointments
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/appointments
        ///
        /// </remarks>
        /// <returns>A list of all appointments</returns>
        /// <response code="200">Returns the list of appointments</response>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all appointments", 
            Description = "Retrieves a list of all appointments",
            OperationId = "GetAppointments",
            Tags = new[] { "Appointments" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AppointmentDTO>))]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
            return Ok(appointmentDtos);
        }

        /// <summary>
        /// Retrieves a specific appointment by ID
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/appointments/5
        ///
        /// </remarks>
        /// <param name="id">The ID of the appointment to retrieve</param>
        /// <returns>The requested appointment</returns>
        /// <response code="200">Returns the requested appointment</response>
        /// <response code="404">If the appointment is not found</response>
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get appointment by ID", 
            Description = "Retrieves a specific appointment by its ID",
            OperationId = "GetAppointmentById",
            Tags = new[] { "Appointments" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AppointmentDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
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

        /// <summary>
        /// Retrieves all appointments for a specific user
        /// </summary>
        [HttpGet("user/{userId}")]
        [SwaggerOperation(
            Summary = "Get appointments by user ID",
            Description = "Retrieves all appointments for a specific user",
            OperationId = "GetAppointmentsByUserId",
            Tags = new[] { "Appointments" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AppointmentDTO>))]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var appointments = await _appointmentService.GetAppointmentsByUserIdAsync(userId);
            var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
            return Ok(appointmentDtos);
        }

        /// <summary>
        /// Creates a new appointment
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/appointments
        ///     {
        ///        "title": "Team Meeting",
        ///        "startTime": "2023-06-01T09:00:00Z",
        ///        "endTime": "2023-06-01T10:00:00Z",
        ///        "description": "Weekly team sync-up",
        ///        "isAllDay": false,
        ///        "location": "Conference Room A"
        ///     }
        ///
        /// </remarks>
        /// <param name="appointmentDto">The appointment data</param>
        /// <returns>The created appointment</returns>
        /// <response code="201">Returns the newly created appointment</response>
        /// <response code="400">If the appointment data is invalid (e.g., end time before start time)</response>
        /// <response code="409">If the appointment conflicts with an existing appointment</response>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new appointment", 
            Description = "Creates a new appointment in the calendar",
            OperationId = "CreateAppointment",
            Tags = new[] { "Appointments" }
        )]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AppointmentDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(string))]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDTO appointmentDto)
        {
            // Add model validation check
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var appointment = await _appointmentService.CreateAppointmentAsync(appointmentDto);
                var createdAppointmentDto = _mapper.Map<AppointmentDTO>(appointment);
                return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, createdAppointmentDto);
            }
            catch (ArgumentException ex)
            {
                // For validation errors like end time before start time
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // For scheduling conflicts with existing appointments
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing appointment
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/appointments/5
        ///     {
        ///        "title": "Updated Team Meeting",
        ///        "startTime": "2023-06-01T09:30:00Z",
        ///        "endTime": "2023-06-01T10:30:00Z",
        ///        "description": "Weekly team sync-up with project updates",
        ///        "isAllDay": false,
        ///        "location": "Conference Room B"
        ///     }
        ///
        /// </remarks>
        /// <param name="id">The ID of the appointment to update</param>
        /// <param name="appointmentDto">The updated appointment data</param>
        /// <returns>The updated appointment</returns>
        /// <response code="200">Returns the updated appointment</response>
        /// <response code="400">If the appointment data is invalid (e.g., end time before start time)</response>
        /// <response code="404">If the appointment is not found</response>
        /// <response code="409">If the appointment conflicts with an existing appointment</response>
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Update an appointment", 
            Description = "Updates an existing appointment by ID",
            OperationId = "UpdateAppointment",
            Tags = new[] { "Appointments" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AppointmentDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(string))]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentDTO appointmentDto)
        {
            // Add model validation check
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var appointment = await _appointmentService.UpdateAppointmentAsync(id, appointmentDto);
                var updatedAppointmentDto = _mapper.Map<AppointmentDTO>(appointment);
                return Ok(updatedAppointmentDto);
            }
            catch (KeyNotFoundException ex)
            {
                // For appointment not found
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                // For validation errors like end time before start time
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // For scheduling conflicts with existing appointments
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Deletes an appointment
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/appointments/5
        ///
        /// </remarks>
        /// <param name="id">The ID of the appointment to delete</param>
        /// <returns>No content</returns>
        /// <response code="204">If the appointment was successfully deleted</response>
        /// <response code="404">If the appointment is not found</response>
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete an appointment", 
            Description = "Deletes an existing appointment by ID",
            OperationId = "DeleteAppointment",
            Tags = new[] { "Appointments" }
        )]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
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
