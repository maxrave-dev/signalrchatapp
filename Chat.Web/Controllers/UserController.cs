using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Chat.Web.Data;
using Chat.Web.Hubs;
using Chat.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Web.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserController(ApplicationDbContext context,
        IMapper mapper,
        IHubContext<ChatHub> hubContext,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _hubContext = hubContext;
        _httpContextAccessor = httpContextAccessor; 
    }
    
    [HttpGet]
    public async Task<IActionResult> GetApplicationUser()
    {
        var crUser = HttpContext.User;
        var userName = crUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        ApplicationUser currentUser = _context.Users.FirstOrDefault(u => u.UserName == userName);
        return Ok(currentUser);
    }
}
