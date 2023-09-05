using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using POS_API.Interface;
using POS_API.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace POS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly POSDBContext db;
        private readonly IGlobalService global;
        public OrderController(POSDBContext _db, IGlobalService _global)
        {
            db = _db;
            global = _global;
        }

        // GET
        [HttpGet]
        [Route("/GetOrder")]
        public async Task<ActionResult<IEnumerable<OrderDetails>>> GetOrder()
        {
            if (db.OrderDetails == null)
            {
                return NotFound();
            }
            return await db.OrderDetails.ToListAsync();
        }

        // GET by Id
        [HttpGet]
        [Route("/GetOrderByUserId/{Id}")]
        public async Task<ActionResult<OrderDetails>> GetOrderByUserId(int Id)
        {
            if (Id == 0)
            {
                return NotFound();
            }
            var list = db.OrderDetails.Where(x => x.IsCheckout == false && x.UserId == Id).ToList();
            List<object> data = new List<object>();
            foreach (var item in list)
            {
                var prod = db.Products.Include(p => p.Details).FirstOrDefault(x => x.Id == item.ProductId);
                var obj = new
                {
                    Id = item.ProductId,
                    Description = prod == null ? "" : prod.Description,
                    Quantity = item.Quantity,
                    Subtotal = item.Quantity * prod.Details.Price,
                };
                data.Add(obj);
            }

            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        // POST
        [HttpPost]
        [Route("/PostOrder")]
        public async Task<ActionResult> PostOrder(OrderDetails order)
        {
            if (order.Id == 0)
            {
                var prod = db.Products.Include(p => p.Details).FirstOrDefault(p => p.Id == order.ProductId);
                order.DateOrdered = DateTime.Now;
                order.IsPaid = false;
                order.IsCheckout = false;
                order.Price = prod.Details.Price;
                order.Subtotal = order.Quantity * prod.Details.Price;
                db.OrderDetails.Add(order);
            }
            else
            {
                db.Entry(order).State = EntityState.Modified;
            }
            await db.SaveChangesAsync();
            return Ok();
        }

        // POST
        [HttpPost]
        [Route("/Checkout/{Id}")]
        public async Task<ActionResult> Checkout(int Id)
        {
            string dataTo = string.Empty;
            var details = db.OrderDetails.Where(x => x.IsCheckout == false && x.UserId == Id).ToList();
            foreach (var item in details)
            {
                item.IsCheckout = true;
                await db.SaveChangesAsync();

                var prod = db.Products.Include(p => p.Details).FirstOrDefault(p => p.Id == item.ProductId);
                if (prod?.Details != null)
                {
                    prod.Details.Quantity = prod.Details.Quantity - item.Quantity;
                    await db.SaveChangesAsync();
                    dataTo += "Description = " + db.Products.Where(x => x.Id == item.ProductId).FirstOrDefault()?.Description + ", Quantity = " + item.Quantity + ", ";
                }
            }

            TransactionLogs Tlogs = new TransactionLogs();
            Tlogs.UserId = Id;
            Tlogs.Module = "POS/Checkout";
            Tlogs.DataFrom = string.Empty;
            Tlogs.DataTo = dataTo;
            Tlogs.Action = "Checkout";
            Tlogs.TransactionDate = DateTime.Now;

            global.SaveTransactionLogs(Tlogs);
            return Ok();
        }

        // DELETE by Id
        [HttpDelete]
        [Route("/DeleteOrderById/{Id}")]
        public async Task<ActionResult> DeleteOrderById(int Id)
        {
            if (db.OrderDetails == null)
            {
                return NotFound();
            }
            var order = await db.OrderDetails.Where(x => x.ProductId == Id && x.IsCheckout == false).SingleOrDefaultAsync();
            if (order == null)
            {
                return NotFound();
            }
            db.Entry(order).State = EntityState.Deleted;
            await db.SaveChangesAsync();
            return Ok();
        }

        // GET by Id
        [HttpGet]
        [Route("/GetReceiptBySalesId/{Id}")]
        public async Task<ActionResult<ReceiptDTO>> GetReceiptBySalesId(int Id)
        {
            if (Id == 0)
            {
                return NotFound();
            }

            var data = await (from a in db.OrderDetails
                              join b in db.Sales on a.SalesId equals b.Id
                              join c in db.Products on a.ProductId equals c.Id
                              where b.Id == Id
                              select new ReceiptDTO
                              { 
                                  Id = b.Id,
                                  UserId = b.UserId,
                                  ProductId = c.Id,
                                  Description = c.Description,
                                  Quantity = a.Quantity,
                                  Price = a.Price,
                                  Subtotal = a.Subtotal,
                                  AmountPaid = b.AmountPaid,
                                  Discount = b.Discount,
                                  Change = b.Change,
                                  VAT = 0.00M,
                                  DateInvoiced = b.DateInvoiced,
                                  DateOrdered = a.DateOrdered
                              }).ToListAsync();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
    }
}
