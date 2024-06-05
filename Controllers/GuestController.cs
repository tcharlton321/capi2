using Microsoft.AspNetCore.Mvc;
using CaesarsAPI.Services;
using CaesarsAPI.Models;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CaesarsAPI.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        public GuestController(IMemoryCache cache)
        {
            //provides us access to the cache
            _cache = cache;
        }

        // GET: api/<GuestController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<GuestController>/getGuest/5
        [HttpGet("getGuest/{id}")]
        public ActionResult Get(int id)
        {
            Guest? guest = _cache.GetOrCreate(id, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(80));
                DB_manager dbMan = new DB_manager();
                return dbMan.GetGuestbyId(id);
            });

            if (guest == null)
            {
                return StatusCode(500);
            }
            string jsonString = JsonSerializer.Serialize<Guest>(guest);

            return Content(jsonString, "application/json");
        }

        // GET api/<GuestController>/search
        [HttpGet("search")]
        public ActionResult Search([FromQuery] string wildcard, [FromQuery] int max, [FromQuery] int offset)
        {
            DB_manager dbMan = new DB_manager();
            List<Guest> searchResults = dbMan.WildSearchGuests(wildcard, max, offset);

            string jsonString = JsonSerializer.Serialize<List<Guest>>(searchResults);

            return Content(jsonString, "application/json");
        }

        // POST api/<GuestController>
        [HttpPost("newGuest")]
        public void Post([FromBody] JsonObject value)
        {
            string error = null;
            try
            {
                Guest guest = JsonSerializer.Deserialize<Guest>(value);
                DB_manager dbMan = new DB_manager();
                dbMan.InsertNewGuest(guest);
            }
            catch (Exception ex)
            {
                error = ex.Message.ToString(); Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                if(error != null)
                {
                    Response.StatusCode = 500;
                } 
                else 
                {
                    Response.StatusCode = 200;
                }
            }
            
        }

        // Patch api/<GuestController>/5
        [HttpPatch("updateGuest/{id}")]
        public void Patch(int id, [FromBody] JsonObject value)
        {
            int affectedRows = 0; // a check variable to verify that a row was changed, if a row isnt changed then we should possibly present that info to the user or invalidate an optimistic update.
            string error = null;
            try
            {
                DB_manager dbMan = new DB_manager();

                Guest guest_orig = JsonSerializer.Deserialize<Guest>(value);
                Guest guest_new = dbMan.GetGuestbyId(id);

                if (guest_orig.Equals(guest_new))
                {
                    Response.StatusCode = 500;
                } else
                {
                    affectedRows = dbMan.UpdateGuest(guest_orig);
                }
            }
            catch (Exception ex)
            {
                error = ex.Message.ToString(); Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                if (error != null)
                {
                    Response.StatusCode = 500;
                }
                else if(affectedRows == 0)
                {
                    //not sure what to do here for a response, but it shoudl be handled in some way.
                    Response.StatusCode = 400;
                } else
                {
                    Response.StatusCode = 200;
                    _cache.Remove(id);
                }
            }
        }

        // DELETE api/<GuestController>/5
        [HttpDelete("deleteGuest/{id}")]
        public void Delete(int id)
        {
            DB_manager dbMan = new DB_manager();
            _cache.Remove(id);
            dbMan.DeleteGuest(id);
        }
    }
}
