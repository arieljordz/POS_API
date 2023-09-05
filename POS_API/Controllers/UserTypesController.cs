using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using POS_API.Models;

namespace POS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTypesController : ControllerBase
    {
        private readonly POSDBContext db;
        public UserTypesController(POSDBContext _db)
        {
            db = _db;
        }

        // GET
        [HttpGet]
        [Route("/GetUserTypes")]
        public async Task<ActionResult<IEnumerable<UserType>>> GetUserTypes()
        {
            if (db.UserTypes == null)
            {
                return NotFound();
            }
            return await db.UserTypes.ToListAsync();
        }

        // GET by Id
        [HttpGet]
        [Route("/GetUserTypeById/{Id}")]
        public async Task<ActionResult<UserType>> GetUserTypeById(int Id)
        {
            if (db.UserTypes == null)
            {
                return NotFound();
            }
            var userType = await db.UserTypes.FindAsync(Id);
            if (userType == null)
            {
                return NotFound();
            }
            return userType;
        }

        // POST
        [HttpPost]
        [Route("/PostUserType")]
        public async Task<ActionResult<IEnumerable<UserType>>> PostUserType(UserType userType)
        {
            db.UserTypes.Add(userType);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserTypes), new { Id = userType.Id }, userType);
        }

        // POST by ID
        [HttpPatch]
        [Route("/PutUserTypeById/{Id}")]
        public async Task<ActionResult> PutUserTypeById(int Id, UserType userType)
        {
            if (userType.Id != Id)
            {
                return BadRequest();
            }
            db.Entry(userType).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return Ok();
        }

        // DELETE by Id
        [HttpDelete]
        [Route("/DeleteUserTypeById/{Id}")]
        public async Task<ActionResult> DeleteUserTypeById(int Id)
        {
            if (db.UserTypes == null)
            {
                return NotFound();
            }
            var userType = await db.UserTypes.FindAsync(Id);
            if (userType == null)
            {
                return NotFound();
            }
            db.Entry(userType).State = EntityState.Deleted;
            await db.SaveChangesAsync();
            return Ok();
        }
    }
}
