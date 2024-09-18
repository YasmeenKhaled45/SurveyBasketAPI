using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Constants;
using SurveyBasket.Api.Contracts.Filters;
using SurveyBasket.Api.Contracts.Roles;
using SurveyBasket.Api.Services;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(IRoleService roleService) : ControllerBase
    {
        private readonly IRoleService roleService = roleService;
        [HttpGet("GetRoles")]
        [HasPermission(Permissions.GetRoles)]
        public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken)
        {
            var result = await roleService.GetAllRoles(cancellationToken);
            return Ok(result);
        }
        [HttpPost("AddRole")]
        [HasPermission(Permissions.AddRoles)]
        public async Task<IActionResult> Add([FromBody] RoleRequest request)
        {
            var result = await roleService.AddRoleAsync(request);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
        }
        [HttpPost("{id}")]
        [HasPermission(Permissions.UpdateRoles)]
        public async Task<IActionResult> UpdateRole([FromRoute]string id,[FromBody] RoleRequest request)
        {
            var result = await roleService.UpdateRole(id, request);

            return result.IsSuccess ? NoContent() : BadRequest(result);
        }
    }
}
