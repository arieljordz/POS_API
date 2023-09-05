using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using POS_API.Interface;
using POS_API.Models;
using System.Data;
using System;

namespace POS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly POSDBContext db;
        private readonly IGlobalService global;
        public ProductsController(POSDBContext _db, IGlobalService _global)
        {
            db = _db;
            global = _global;
        }

        // GET
        [HttpGet]
        [Route("/GetProducts")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            if (db.Products == null)
            {
                return NotFound();
            }
            return await db.Products.Include(p => p.Details).ToListAsync();

            //return await db.Products.ToListAsync();
        }

        // GET by Id
        [HttpGet]
        [Route("/GetProductById/{Id}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductById(int Id)
        {
            if (db.Products == null)
            {
                return NotFound();
            }

            if (Id != 0)
            {
                return await db.Products.Include(p => p.Details).Where(x => x.Id == Id).ToListAsync();
            }
            else if (Id == 0)
            {
                return await db.Products.Include(p => p.Details).ToListAsync();
            }
            else
            {
                return NotFound();
            }
        }

        // POST
        [HttpPost]
        [Route("/PostProduct")]
        public async Task<ActionResult> PostProduct(Product product, int userId)
        {
            var dataFrom = string.Empty;
            var dataTo = string.Empty;
            try
            {
                if (product.Id == 0)
                {
                    if (product.Details != null)
                    {
                        product.Details.DateAdded = DateTime.Now;
                        db.Products.Add(product);
                        dataTo = "Added new Product Description = " + product.Description +
                                ", Quantity = " + product.Details?.Quantity +
                                ", Price = " + product.Details?.Price +
                                ", ExpirationDate = " + product.Details?.ExpirationDate +
                                ".";

                        TransactionLogs Tlogs = new TransactionLogs();
                        Tlogs.UserId = userId;
                        Tlogs.Module = "Manage/Products";
                        Tlogs.DataFrom = dataFrom;
                        Tlogs.DataTo = dataTo;
                        Tlogs.Action = "Add";
                        Tlogs.TransactionDate = DateTime.Now;

                        global.SaveTransactionLogs(Tlogs);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    var existingProduct = db.Products.Include(p => p.Details).FirstOrDefaultAsync(p => p.Id == product.Id);
                    if (existingProduct != null)
                    {
                        if (existingProduct.Result != null)
                        {
                            if (existingProduct.Result.Details != null)
                            {
                                dataFrom = "From Product Description = " + existingProduct.Result.Description +
                                    ", Quantity = " + existingProduct.Result.Details.Quantity +
                                    ", Price = " + existingProduct.Result.Details.Price +
                                    ", ExpirationDate = " + existingProduct.Result.Details.ExpirationDate +
                                    ".";
                                dataTo = "Updated Product into Description = " + product.Description +
                                    ", Quantity = " + product.Details?.Quantity +
                                    ", Price = " + product.Details?.Price +
                                    ", ExpirationDate = " + product.Details?.ExpirationDate +
                                    ".";

                                TransactionLogs Tlogs = new TransactionLogs();
                                Tlogs.UserId = userId;
                                Tlogs.Module = "Manage/Products";
                                Tlogs.DataFrom = dataFrom;
                                Tlogs.DataTo = dataTo;
                                Tlogs.Action = "Update";
                                Tlogs.TransactionDate = DateTime.Now;

                                global.SaveTransactionLogs(Tlogs);

                                existingProduct.Result.Description = product.Description;
                                existingProduct.Result.Details.Quantity = product.Details?.Quantity ?? 0;
                                existingProduct.Result.Details.Price = product.Details?.Price ?? 0;
                                existingProduct.Result.Details.ExpirationDate = product.Details?.ExpirationDate;
                            }
                            else
                            {
                                return NotFound();
                            }

                            db.Entry(existingProduct.Result).State = EntityState.Modified;
                        }
                    }
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
        [Route("/DeleteProductById")]
        public async Task<ActionResult> DeleteProductById(int Id, int userId)
        {
            if (db.Products == null)
            {
                return NotFound();
            }
            //var product = await db.Products.FindAsync(Id);
            var product = await db.Products.Include(p => p.Details).FirstOrDefaultAsync(p => p.Id == Id);
            if (product == null)
            {
                return NotFound();
            }
            db.Entry(product).State = EntityState.Deleted;

            var dataFrom = "From Product Description = " + product.Description +
                     ", Quantity = " + product.Details?.Quantity +
                     ", Price = " + product.Details?.Price +
                     ", ExpirationDate = " + product.Details?.ExpirationDate +
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
    }
}
