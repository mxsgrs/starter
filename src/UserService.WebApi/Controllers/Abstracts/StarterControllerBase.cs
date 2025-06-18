namespace UserService.WebApi.Controllers.Abstracts;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StarterControllerBase() : ControllerBase { }
