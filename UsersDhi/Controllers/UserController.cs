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
        /// Retorna todos os usuários.
        /// </summary>
        /// <response code="200">Usuários retornados com sucesso</response>
        [HttpGet]
        [ProducesResponseType(typeof(User), 200)]
        public List<User> GetUser()
        {
            return this.userService.GetAllUsers();
        }

        /// <summary>
        /// Retorna um usuário pelo ID.
        /// </summary>
        /// <response code="200">Usuário retornado com sucesso</response>
        /// <response code="404">Usuário não encontrado</response>
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
        /// Atualiza um usuário pelo ID.
        /// </summary>
        /// <param Name="id">ID do usuário</param>
        /// <param Name="user">Dados do usuário</param>
        /// <response code="204">Usuário atualizado com sucesso</response>
        /// <response code="400">Campos obrigatórios não preenchidos</response>
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
                return BadRequest("O ID informado não pertence ao usuário");
            }

            if (!this.userService.UserExists(user.Id))
            {
                return NotFound("Usuário não encontrado");
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
        /// Cadastra um usuário.
        /// </summary>
        /// <param Name="user">Dados do usuário</param>
        /// <response code="201">Usuário cadastrado com sucesso</response>
        /// <response code="400">Campos obrigatórios não preenchidos</response>
        /// <response code="500">Username já cadastrado</response>
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
                JsonResult jsonResult = new JsonResult(new { username = $"O username '{user.Username}' já está cadastrado" });
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
        /// Deleta um usuário pelo ID.
        /// </summary>
        /// <param Name="id">ID do usuário</param>
        /// <response code="204">Usuário deletado com sucesso</response>
        /// <response code="500">Erro ao deletar usuário</response>
        [HttpDelete("{Id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            if (!this.userService.UserExists(id))
            {
                return NotFound("Usuário não encontrado");
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