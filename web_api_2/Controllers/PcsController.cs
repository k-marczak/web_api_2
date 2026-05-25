using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_api_2.Data;
using web_api_2.DTOs;
using web_api_2.Models;

namespace web_api_2.Controllers;

[ApiController]
[Route("api/pcs")]
public class PcsController : ControllerBase
{
    private readonly PCsDbContext _context;

    public PcsController(PCsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PcResponseDto>>> GetAllPcs()
    {
        var pcs = await _context.PCs
            .Select(pc => new PcResponseDto
            {
                Id = pc.Id,
                Name = pc.Name,
                Weight = pc.Weight,
                Warranty = pc.Warranty,
                CreatedAt = pc.CreatedAt,
                Stock = pc.Stock
            })
            .ToListAsync();

        return Ok(pcs);
    }

    [HttpGet("{id:int}/components")]
    public async Task<ActionResult<PcWithComponentsDto>> GetPcComponents(int id)
    {
        var pc = await _context.PCs
            .Include(pc => pc.PcComponents)
                .ThenInclude(pcComponent => pcComponent.Component)
                    .ThenInclude(component => component.Manufacturer)
            .Include(pc => pc.PcComponents)
                .ThenInclude(pcComponent => pcComponent.Component)
                    .ThenInclude(component => component.Type)
            .FirstOrDefaultAsync(pc => pc.Id == id);

        if (pc is null)
        {
            return NotFound();
        }

        var response = new PcWithComponentsDto
        {
            Id = pc.Id,
            Name = pc.Name,
            Weight = pc.Weight,
            Warranty = pc.Warranty,
            CreatedAt = pc.CreatedAt,
            Stock = pc.Stock,
            Components = pc.PcComponents.Select(pcComponent => new PcComponentDto
            {
                Amount = pcComponent.Amount,
                Component = new ComponentDto
                {
                    Code = pcComponent.Component.Code,
                    Name = pcComponent.Component.Name,
                    Description = pcComponent.Component.Description,
                    Manufacturer = new ComponentManufacturerDto
                    {
                        Id = pcComponent.Component.Manufacturer.Id,
                        Abbreviation = pcComponent.Component.Manufacturer.Abbreviation,
                        FullName = pcComponent.Component.Manufacturer.FullName,
                        FoundationDate = pcComponent.Component.Manufacturer.FoundationDate
                    },
                    Type = new ComponentTypeDto
                    {
                        Id = pcComponent.Component.Type.Id,
                        Abbreviation = pcComponent.Component.Type.Abbreviation,
                        Name = pcComponent.Component.Type.Name
                    }
                }
            }).ToList()
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<PcResponseDto>> CreatePc(CreatePcDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Field Name is required...");
        }

        var pc = new Pc
        {
            Name = request.Name,
            Weight = request.Weight,
            Warranty = request.Warranty,
            CreatedAt = request.CreatedAt,
            Stock = request.Stock
        };

        _context.PCs.Add(pc);
        await _context.SaveChangesAsync();

        var response = new PcResponseDto
        {
            Id = pc.Id,
            Name = pc.Name,
            Weight = pc.Weight,
            Warranty = pc.Warranty,
            CreatedAt = pc.CreatedAt,
            Stock = pc.Stock
        };

        return CreatedAtAction(nameof(GetPcComponents), new { id = pc.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PcResponseDto>> UpdatePc(int id, UpdatePcDto request)
    {
        var pc = await _context.PCs.FirstOrDefaultAsync(pc => pc.Id == id);

        if (pc is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Field Name is required...");
        }

        pc.Name = request.Name;
        pc.Weight = request.Weight;
        pc.Warranty = request.Warranty;
        pc.CreatedAt = request.CreatedAt;
        pc.Stock = request.Stock;

        await _context.SaveChangesAsync();

        var response = new PcResponseDto
        {
            Id = pc.Id,
            Name = pc.Name,
            Weight = pc.Weight,
            Warranty = pc.Warranty,
            CreatedAt = pc.CreatedAt,
            Stock = pc.Stock
        };

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePc(int id)
    {
        var pc = await _context.PCs
            .Include(pc => pc.PcComponents)
            .FirstOrDefaultAsync(pc => pc.Id == id);

        if (pc is null)
        {
            return NotFound();
        }

        _context.PcComponents.RemoveRange(pc.PcComponents);
        _context.PCs.Remove(pc);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}