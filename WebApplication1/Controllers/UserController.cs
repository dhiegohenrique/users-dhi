using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication23.Models;
using UnitTestProject1.Services;

namespace WebApplication1.Controllers
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
        [HttpGet("{id}")]
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
        /// Atualiza um usuário.
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="user">Dados do usuário</param>
        /// <response code="204">Usuário deletado com sucesso</response>
        /// <response code="500">Erro ao deletar usuário</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> PutUser([FromRoute] int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.id)
            {
                return BadRequest();
            }

            try
            {
                user.registerdate = DateTime.Now;
                await this.userService.InsertUpdate(user);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return NotFound(ex);
            }
            catch (Exception)
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Cadastra um usuário.
        /// </summary>
        /// <param name="user">Dados do usuário</param>
        /// <response code="201">Usuário cadastrado com sucesso</response>
        /// <response code="500">Erro ao cadastrar usuário</response>            
        [HttpPost]
        [ProducesResponseType(typeof(User), 201)]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                user.registerdate = DateTime.Now;

                await this.userService.InsertUpdate(user);
                return CreatedAtAction("GetUser", new { id = user.id }, user);
            }
            catch (DbUpdateException ex)
            {
                return NotFound(ex);
            }
            catch (Exception)
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Deleta um usuário.
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <response code="204">Usuário deletado com sucesso</response>
        /// <response code="500">Erro ao deletar usuário</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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