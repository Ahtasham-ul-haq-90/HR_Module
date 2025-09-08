using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.AUTH;
using Application.DTO.ServiceResponse;

namespace Application.Interface
{
    public interface IAuthService
    {
        Task<UserRegisterDTO> RegisterAsync(RegisterDTO model);
        Task<string> LoginAsync(LoginDto model);
    }
}
