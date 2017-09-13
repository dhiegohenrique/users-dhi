using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UsersDhi.Models;
using UsersDhi.Services;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace UsersDhi.Controllers
{
    [Produces("application/json")]
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// Retorna todos os usu�rios.
        /// </summary>
        /// <response code="200">Usu�rios retornados com sucesso</response>
        [HttpGet]
        [ProducesResponseType(typeof(User), 200)]
        public List<User> GetUser()
        {
            return this.userService.GetAllUsers();
        }

        /// <summary>
        /// Retorna um usu�rio pelo ID.
        /// </summary>
        /// <response code="200">Usu�rio retornado com sucesso</response>
        /// <response code="404">Usu�rio n�o encontrado</response>
        [HttpGet("{Id}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await this.userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// Atualiza um usu�rio pelo ID.
        /// </summary>
        /// <param Name="id">ID do usu�rio</param>
        /// <param Name="user">Dados do usu�rio</param>
        /// <response code="204">Usu�rio atualizado com sucesso</response>
        /// <response code="400">Campos obrigat�rios n�o preenchidos</response>
        [HttpPut("{Id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> PutUser([FromRoute] int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest("O ID informado n�o pertence ao usu�rio");
            }

            if (!this.userService.UserExists(user.Id))
            {
                return NotFound("Usu�rio n�o encontrado");
            }

            try
            {
                await this.userService.InsertUpdate(user);
                return NoContent();
            } catch (Exception)
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Cadastra um usu�rio.
        /// </summary>
        /// <param Name="user">Dados do usu�rio</param>
        /// <response code="201">Usu�rio cadastrado com sucesso</response>
        /// <response code="400">Campos obrigat�rios n�o preenchidos</response>
        /// <response code="500">Username j� cadastrado</response>
        [HttpPost]
        [ProducesResponseType(typeof(User), 201)]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (this.userService.UsernameExists(user.Username))
            {
                JsonResult jsonResult = new JsonResult(new { username = $"O username '{user.Username}' j� est� cadastrado" });
                jsonResult.StatusCode = StatusCodes.Status500InternalServerError;
                return jsonResult;
            }

            try
            {
                user.Id = 0;
                await this.userService.InsertUpdate(user);
                return CreatedAtAction("GetUser", new { id = user.Id }, user);
            } catch (Exception)
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Deleta um usu�rio pelo ID.
        /// </summary>
        /// <param Name="id">ID do usu�rio</param>
        /// <response code="204">Usu�rio deletado com sucesso</response>
        /// <response code="500">Erro ao deletar usu�rio</response>
        [HttpDelete("{Id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            if (!this.userService.UserExists(id))
            {
                return NotFound("Usu�rio n�o encontrado");
            }

            try
            {
                await this.userService.Remove(id);
                return NoContent();
            } catch (Exception)
            {
                return BadRequest(ModelState);
            }
        }
    }
}