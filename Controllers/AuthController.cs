using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApiAutores.Models;
using WebApiAutores.Services;

namespace WebApiAutores.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
  private readonly UserManager<IdentityUser> _userManager;
  private readonly SignInManager<IdentityUser> _signInManager;
  private readonly IConfiguration _configuration;
  private readonly HashService _hashService;
  private readonly IDataProtector _dataProtector;

  public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
    IConfiguration configuration, IDataProtectionProvider dataProtectionProvider, HashService hashService)
  {
    _userManager = userManager;
    _signInManager = signInManager;
    _configuration = configuration;
    _hashService = hashService;
    _dataProtector = dataProtectionProvider.CreateProtector("valor_unico_y_quizas_secreto");
  }

  [HttpPost("Registrar", Name = "registrarUsuario")]
  public async Task<IActionResult> Registrar(CredencialesUsuarioDTO model)
  {
    var usuario = new IdentityUser { UserName = model.UserName, Email = model.Email };

    var resultado = await _userManager.CreateAsync(usuario, model.Password);

    if (resultado.Succeeded)
    {
      return Ok(await ConstruirToken(model));
    }

    return BadRequest(resultado.Errors);
  }

  [HttpPost("Login", Name = "loginUsuario")]
  public async Task<IActionResult> Login(CredencialesUsuarioDTO model)
  {
    var resultado =
      await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false,
        lockoutOnFailure: false);

    if(resultado.Succeeded)
    {
      return Ok(await ConstruirToken(model));
    }

    return BadRequest("Login incorrecto");
  }

  [HttpGet("RenovarToken", Name = "renovarTokenUsuario")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public async Task<IActionResult> RenovarToken()
  {
    var emailClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
    var email = emailClaim.Value;

    var credencialesUsuario = new CredencialesUsuarioDTO()
    {
      Email = email
    };

    return Ok(await ConstruirToken(credencialesUsuario));
  }

  [HttpPost("HacerAdmin", Name = "hacerAdmin")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
  public async Task<IActionResult> HacerAdmin(EditarAdminDTO model)
  {
    var usuario = await _userManager.FindByEmailAsync(model.Email);
    await _userManager.AddClaimAsync(usuario, new Claim(ClaimTypes.Role, "admin"));

    return NoContent();
  }

  [HttpPost("RemoveAdmin", Name = "removeAdmin")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
  public async Task<IActionResult> RemoveAdmin(EditarAdminDTO model)
  {
    var usuario = await _userManager.FindByEmailAsync(model.Email);
    await _userManager.RemoveClaimAsync(usuario, new Claim(ClaimTypes.Role, "admin"));

    return NoContent();
  }

  private async Task<RespuestaAuthDTO> ConstruirToken(CredencialesUsuarioDTO model)
  {
    var claims = new List<Claim>()
    {
      new(ClaimTypes.Email, model.Email),
      new(ClaimTypes.Name, model.UserName)
    };

    var usuario = await _userManager.FindByEmailAsync(model.Email);
    var claimsDb = await _userManager.GetClaimsAsync(usuario);

    claims.AddRange(claimsDb);

    var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("LlaveJWT")));
    var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

    var expiracion = DateTime.UtcNow.AddYears(1);

    var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion,
      signingCredentials: credenciales);

    return new RespuestaAuthDTO()
    {
      Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
      Expiracion = expiracion
    };
  }
}