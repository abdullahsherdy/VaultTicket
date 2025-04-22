﻿using API.DTOs;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        /// <summary>
        ///  Currently, the ticket creation is done through the booking process.
        ///  but, i keep it for future usage..maybe.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] TicketCreateDto dto)
        {
            var result = await _ticketService.CreateTicketAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _ticketService.GetAllTicketsAsync());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }

        [HttpGet("{id}/qr")]
        [Authorize]
        public async Task<IActionResult> GetQrImage(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null) return NotFound();

            return Ok(new { qrImage = ticket.QrCode });
        }

        [HttpPost("validate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ValidateQr([FromBody] string qrCode)
        {
            var valid = await _ticketService.ValidateQrCodeAsync(qrCode);
            return Ok(new { isValid = valid });
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var success = await _ticketService.UpdateTicketStatusAsync(id, status);
            return success ? Ok() : NotFound();
        }
    }
}
