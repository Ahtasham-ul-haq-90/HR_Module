using Application.DTO.AUTH;
using Application.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
namespace HR_System.Controllers
{
    public class AccountController : APIBaseController
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO register)
        {

            var data = await _authService.RegisterAsync(register);
            return Ok(new { data });


        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var data = await _authService.LoginAsync(login);
            return Ok(new { data });
        }
    }
}
