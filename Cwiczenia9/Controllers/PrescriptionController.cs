using Cwiczenia9.DTOs;
using Cwiczenia9.Models;
using Cwiczenia9.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cwiczenia9.Controllers;

[ApiController]
[Route("[controller]")]
public class PrescriptionController(IDbService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePrescription([FromBody] PrescriptionRequestDto request)
    {
        return await service.AddPrescriptionAsync(request);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientDetais(int id)
    {
        return await service.GetPatientDetailsAsync(id);
    }
}