using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoctorConnect.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RelativeApiController : ControllerBase
    {
        private readonly IRelativeService _relativeService;

        public RelativeApiController(IRelativeService relativeService)
        {
            _relativeService = relativeService;
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatientId(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Patient id is required.",
                    Data = null
                });
            }

            var relatives = await _relativeService.GetByPatientIdAsync(patientId);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Relatives fetched successfully.",
                Data = relatives
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Relative relative)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)),
                    Data = null
                });
            }

            try
            {
                var createdRelative = await _relativeService.CreateAsync(relative);
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Relative created successfully.",
                    Data = new Relative
                    {
                        Id = createdRelative.Id,
                        FullName = createdRelative.FullName,
                        PhoneNumber = createdRelative.PhoneNumber,
                        Relationship = createdRelative.Relationship,
                        DateOfBirth = createdRelative.DateOfBirth,
                        Gender = createdRelative.Gender,
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Relative relative)
        {
            if (string.IsNullOrWhiteSpace(id) || id != relative.Id)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid relative id.",
                    Data = null
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)),
                    Data = null
                });
            }

            try
            {
                var updatedRelative = await _relativeService.UpdateAsync(relative);
                if (updatedRelative == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Relative not found.",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Relative updated successfully.",
                    Data = new Relative
                    {
                        Id = updatedRelative.Id,
                        FullName = updatedRelative.FullName,
                        PhoneNumber = updatedRelative.PhoneNumber,
                        Relationship = updatedRelative.Relationship,
                        DateOfBirth = updatedRelative.DateOfBirth,
                        Gender = updatedRelative.Gender,
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
