using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{


    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }


        [HttpPost("register")]

        // When I pass something to API it has to be in form of OBJECT not multiple parameters.
        // So UserDto userDto, not string username, string password
        // ActionResult gives me possibility to return different HTTP status codes as a response.
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // check if username alrady exist
            bool userExist = await UserExists(registerDto.Username);

            if (userExist)
            {
                // return bad request 400 with the message
                return BadRequest("Username is taken");
            }

            using var hmac = new HMACSHA512();

            // This user will be stored in db
            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                // password needs to convert in byte array
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            // I'm basically saying to EF to track this.
            _context.Add(user);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Create new UserDto that will be returned to the client.
            var userDto = new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };

            return userDto;
        }


        // Login
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // get user from the db
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            // check if user is null, if so return 401 Unauthorized with propper message.
            if (user == null)
            {
                return Unauthorized("Invalid username");
            }

            // compute hash from provided user password. If I create new instance of HMACSHA512 with user.PasswordSalt
            // I'm going to be able to encript the same password hash as when user did registration.
            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // Itterate over byte array and check if element at the same location match.
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid password");
                }
            }

            // Create new UserDto that will be returned to the client
            var userDto = new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };

            return Ok(userDto);
        }


        // helper method
        private async Task<bool> UserExists(string username)
        {
            // this will check if any username in the database match provided username.
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }



    }
}
