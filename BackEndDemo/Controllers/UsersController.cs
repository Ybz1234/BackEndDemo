using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BackEndDemo.Models;
using System;
using System.Collections.Generic;

namespace BackEndDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<User[]> Get()
        {
            try
            {
                List<User> users = DatabaseServicesUsers.GetAllUsers();
                return Ok(users.ToArray());
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(int id)
        {
            try
            {
                User user = DatabaseServicesUsers.GetUserById(id);
                if (user == null)
                    return NotFound($"User with id: {id} wasn't found.");
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] User value)
        {
            try
            {
                if (value == null)
                    return BadRequest("User is null.");
                if (value.Id != 0)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Cannot specify Id for new User.");

                value.Id = DatabaseServicesUsers.InsertUser(value);

                return CreatedAtAction(nameof(Get), new { id = value.Id }, value);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put(int id, [FromBody] User value)
        {
            try
            {
                if (value == null || value.Id != id)
                    return BadRequest();

                int rowsAffected = DatabaseServicesUsers.UpdateUser(value);
                if (rowsAffected == 0)
                    return NotFound($"User with id: {id} wasn't found, can't update.");

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Delete(int id)
        {
            try
            {
                if (id == 0)
                    return BadRequest();

                bool isDeleted = DatabaseServicesUsers.DeleteUser(id);
                if (!isDeleted)
                    return NotFound($"User with id: {id} wasn't found, can't delete.");

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
