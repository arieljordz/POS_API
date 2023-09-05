using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using POS_API.Models;

namespace POS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDetailsController : ControllerBase
    {
        private readonly POSDBContext db;
        public ProductDetailsController(POSDBContext _db)
        {
            db = _db;
        }

        // GET
        [HttpGet]
        [Route("/GetProductDetails")]
        public async Task<ActionResult<IEnumerable<ProductDetails>>> GetProductDetails()
        {
            if (db.ProductDetails == null)
            {
                return NotFound();
            }
            return await db.ProductDetails.ToListAsync();
        }

        // GET by Id
        [HttpGet]
        [Route("/GetProductDetailById/{Id}")]
        public async Task<ActionResult<ProductDetails>> GetProductDetailById(int Id)
        {
            if (db.ProductDetails == null)
            {
                return NotFound();
            }
            var details = await db.ProductDetails.FindAsync(Id);
            if (details == null)
            {
                return NotFound();
            }
            return details;
        }

        // POST
        [HttpPost]
        [Route("/PostProductDetail")]
        public async Task<ActionResult<IEnumerable<ProductDetails>>> PostProductDetail(ProductDetails details)
        {
            db.ProductDetails.Add(details);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProductDetails), new { Id = details.Id }, details);
        }

        // POST by ID
        [HttpPatch]
        [Route("/PutProductDetailById/{Id}")]
        public async Task<ActionResult> PutProductDetailById(int Id, ProductDetails details)
        {
            if (details.Id != Id)
            {
                return BadRequest();
            }
            db.Entry(details).State = EntityState.Modified;
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
        [Route("/DeleteProductDetailById/{Id}")]
        public async Task<ActionResult> DeleteProductDetailById(int Id)
        {
            if (db.ProductDetails == null)
            {
                return NotFound();
            }
            var details = await db.ProductDetails.FindAsync(Id);
            if (details == null)
            {
                return NotFound();
            }
            db.Entry(details).State = EntityState.Deleted;
            await db.SaveChangesAsync();
            return Ok();
        }
    }
}
