using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using POS_API.DTO;
using POS_API.Interface;
using POS_API.Models;
using System.Data;
using System;
using static System.Collections.Specialized.BitVector32;

namespace POS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly POSDBContext db;
        private readonly IGlobalService global;
        public UsersController(POSDBContext _db, IGlobalService _global)
        {
            db = _db;
            global = _global;
        }

        // GET
        [HttpGet]
        [Route("/GetUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            if (db.Users == null)
            {
                return NotFound();
            }
            return await db.Users.ToListAsync();
        }

        // GET by Id
        [HttpGet]
        [Route("/GetUserById/{Id}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUserById(int Id)
        {
            if (db.Users == null)
            {
                return NotFound();
            }
            var user = await db.Users.Where(x => x.Id == Id).ToListAsync();
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        // POST
        [HttpPost]
        [Route("/PostUser")]
        public async Task<ActionResult<IEnumerable<User>>> PostUser(User user, int userId)
        {
            string dataFrom = string.Empty;
            string dataTo = string.Empty;
            try
            {
                if (user.Id == 0)
                {
                    user.FullName = user.FirstName + " " + user.MiddleName?.ToCharArray()[0] + ". " + user.LastName;
                    db.Users.Add(user);

                    dataTo = "Added new User = " + user.FullName +
                            ", Username = " + user.Username +
                            ", UserTypeId = " + user.UserTypeId +
                            ".";

                    TransactionLogs Tlogs = new TransactionLogs();
                    Tlogs.UserId = userId;
                    Tlogs.Module = "Manage/Users";
                    Tlogs.DataFrom = dataFrom;
                    Tlogs.DataTo = dataTo;
                    Tlogs.Action = "Add";
                    Tlogs.TransactionDate = DateTime.Now;

                    global.SaveTransactionLogs(Tlogs);
                }
                else
                {
                    var _user = await db.Users.FindAsync(user.Id);
                    dataFrom = "From User = " + _user?.FullName +
                        ", Username = " + _user?.Username +
                        ", UserTypeId = " + _user?.UserTypeId +
                        ".";
                    dataTo = "Updated User into = " + user.FirstName + " " + user.MiddleName?.ToCharArray()[0] + ". " + user.LastName +
                        ", Username = " + user.Username +
                        ", UserTypeId = " + user.UserTypeId +
                        ".";

                    TransactionLogs Tlogs = new TransactionLogs();
                    Tlogs.UserId = userId;
                    Tlogs.Module = "Manage/Users";
                    Tlogs.DataFrom = dataFrom;
                    Tlogs.DataTo = dataTo;
                    Tlogs.Action = "Update";
                    Tlogs.TransactionDate = DateTime.Now;

                    global.SaveTransactionLogs(Tlogs);

                    if (_user != null)
                    {
                        _user.FirstName = user.FirstName;
                        _user.MiddleName = user.MiddleName;
                        _user.LastName = user.LastName;
                        _user.FullName = user.FirstName + " " + user.MiddleName?.ToCharArray()[0] + ". " + user.LastName;
                        _user.Username = user.Username;
                        _user.Password = user.Password;
                    }
                    else
                    {
                        return NotFound();
                    }
                    db.Entry(_user).State = EntityState.Modified;
                }
                await db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
         
        }

        // DELETE by Id
        [HttpDelete]
        [Route("/DeleteUserById")]
        public async Task<ActionResult> DeleteUserById(int Id, int userId)
        {
            if (db.Users == null)
            {
                return NotFound();
            }
            var user = await db.Users.FindAsync(Id);
            if (user != null)
            {
                db.Entry(user).State = EntityState.Deleted;

                var dataFrom = "From User = " + user?.FullName +
                        ", Username = " + user?.Username +
                        ", UserTypeId = " + user?.UserTypeId +
                        ".";

                TransactionLogs Tlogs = new TransactionLogs();
                Tlogs.UserId = userId;
                Tlogs.Module = "Manage/Products";
                Tlogs.DataFrom = dataFrom;
                Tlogs.DataTo = "Removed";
                Tlogs.Action = "Delete";
                Tlogs.TransactionDate = DateTime.Now;

                global.SaveTransactionLogs(Tlogs);

                await db.SaveChangesAsync();

                return Ok();
            }
            else
            {
                return NotFound();
            }
         
        }

        // GET
        [HttpPost]
        [Route("/Login")]
        public async Task<ActionResult<User>> Login(ValidateUserDTO validate)
        {
            try
            {
                var UserTypeId = db.UserTypes.Where(x => x.Description.Equals(validate.UserType)).FirstOrDefault()?.Id;

                var user = await db.Users.Where(x => x.Username.Equals(validate.Username) && x.Password.Equals(validate.Password) && x.UserTypeId == UserTypeId).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    global.SaveUserLogs(user, true);
                    return Ok(user);
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
  

        }

        // GET
        [HttpPost]
        [Route("/Logout/{userId}")]
        public async Task<ActionResult<User>> Logout(int userId)
        {
            var user = await db.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                global.SaveUserLogs(user, false);
                return Ok(user);
            }

        }
    }
}
