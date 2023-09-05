using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using POS_API.Interface;
using POS_API.Models;
using System.Data;

namespace POS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly POSDBContext db;
        private readonly IGlobalService global;
        public SalesController(POSDBContext _db, IGlobalService _global)
        {
            db = _db;
            global = _global;
        }

        // GET
        [HttpGet]
        [Route("/GetSales")]
        public async Task<ActionResult<IEnumerable<Sales>>> GetSales()
        {
            if (db.Sales == null)
            {
                return NotFound();
            }
            return await db.Sales.ToListAsync();
        }

        // GET by Id
        [HttpGet]
        [Route("/GetSalesByUserId/{Id}")]
        public async Task<ActionResult<Sales>> GetSalesByUserId(int Id)
        {
            if (Id == 0)
            {
                return NotFound();
            }
            var res = await db.Sales.FindAsync(Id);
            if (res == null)
            {
                return NotFound();
            }
            return Ok(res);
        }

        // POST
        [HttpPost]
        [Route("/PostSales")]
        public async Task<ActionResult> PostSales(Sales sales)
        {
            string dataTo = string.Empty;
            var msg = "";
            if (sales.AmountPaid >= sales.TotalAmount)
            {
                sales.DateInvoiced = DateTime.Now;
                sales.Change = sales.AmountPaid - sales.TotalAmount;
                db.Sales.Add(sales);
                await db.SaveChangesAsync();

                dataTo = "Subtotal = " + sales.TotalAmount +
                    ", AmountPaid = " + sales.AmountPaid +
                    ", Discount = " + sales.Discount +
                    ", Change = " + sales.Change +
                    ", DateInvoiced = " + sales.DateInvoiced + ".";

                var details = db.OrderDetails.Where(x => x.IsPaid == false && x.UserId == sales.UserId).ToList();

                foreach (var item in details)
                {
                    item.IsPaid = true;
                    item.SalesId = sales.Id;
                    db.SaveChanges();
                }
                msg = "success";

                TransactionLogs Tlogs = new TransactionLogs();
                Tlogs.UserId = sales.UserId;
                Tlogs.Module = "POS/PlaceOrder";
                Tlogs.DataFrom = string.Empty;
                Tlogs.DataTo = dataTo;
                Tlogs.Action = "PlaceOrder";
                Tlogs.TransactionDate = DateTime.Now;

                global.SaveTransactionLogs(Tlogs);
            }
            else
            {
                msg = "The Amount Paid should be greater than or equal to the Total Amount.";
            }
            return Ok(sales);
        }

        // DELETE by Id
        [HttpDelete]
        [Route("/DeleteSalesById/{Id}")]
        public async Task<ActionResult> DeleteSalesById(int Id)
        {
            if (db.Sales == null)
            {
                return NotFound();
            }
            var Sales = await db.Sales.FindAsync(Id);
            if (Sales == null)
            {
                return NotFound();
            }
            db.Entry(Sales).State = EntityState.Deleted;
            await db.SaveChangesAsync();
            return Ok();
        }

        // GET by Date Invoiced
        [HttpGet]
        [Route("/GetSalesByDateInvoiced")]
        public async Task<ActionResult<ReceiptDTO>> GetSalesByDateInvoiced(int Id, string DateFrom, string DateTo)
        {
            if (Id == 0)
            {
                DateTime startDate = DateTime.Parse(DateFrom);
                DateTime endDate = DateTime.Parse(DateTo);

                var data = await (from a in db.OrderDetails
                                  join b in db.Sales on a.SalesId equals b.Id
                                  join c in db.Products on a.ProductId equals c.Id
                                  where b.DateInvoiced.Value.Date >= startDate.Date && b.DateInvoiced.Value.Date <= endDate.Date
                                  group new { a, b, c } by c.Id into productGroup
                                  select new ReceiptDTO
                                  {
                                      Id = productGroup.Select(x => x.b.Id).FirstOrDefault(),
                                      UserId = productGroup.Select(x => x.b.UserId).FirstOrDefault(),
                                      ProductId = productGroup.Key,
                                      Description = productGroup.Select(x => x.c.Description).FirstOrDefault(),
                                      Quantity = productGroup.Sum(x => x.a.Quantity),
                                      Price = productGroup.Sum(x => x.a.Price),
                                      Subtotal = productGroup.Sum(x => x.a.Subtotal),
                                      AmountPaid = productGroup.Select(x => x.b.AmountPaid).FirstOrDefault(),
                                      Discount = productGroup.Select(x => x.b.Discount).FirstOrDefault(),
                                      Change = productGroup.Select(x => x.b.Change).FirstOrDefault(),
                                      VAT = 0.00M,
                                      DateInvoiced = productGroup.Select(x => x.b.DateInvoiced).FirstOrDefault(),
                                      DateOrdered = productGroup.Select(x => x.a.DateOrdered).FirstOrDefault(),

                                  }).ToListAsync();

                if (data == null)
                {
                    return NotFound();
                }
                return Ok(data);
            }
            else
            {
                DateTime startDate = DateTime.Parse(DateFrom);
                DateTime endDate = DateTime.Parse(DateTo);

                var data = await (from a in db.OrderDetails
                                  join b in db.Sales on a.SalesId equals b.Id
                                  join c in db.Products on a.ProductId equals c.Id
                                  where b.DateInvoiced.Value.Date >= startDate.Date && b.DateInvoiced.Value.Date <= endDate.Date
                                  && c.Id == Id
                                  group new { a, b, c } by c.Id into productGroup
                                  select new ReceiptDTO
                                  {
                                      Id = productGroup.Select(x => x.b.Id).FirstOrDefault(),
                                      UserId = productGroup.Select(x => x.b.UserId).FirstOrDefault(),
                                      ProductId = productGroup.Key,
                                      Description = productGroup.Select(x => x.c.Description).FirstOrDefault(),
                                      Quantity = productGroup.Sum(x => x.a.Quantity),
                                      Price = productGroup.Sum(x => x.a.Price),
                                      Subtotal = productGroup.Sum(x => x.a.Subtotal),
                                      AmountPaid = productGroup.Select(x => x.b.AmountPaid).FirstOrDefault(),
                                      Discount = productGroup.Select(x => x.b.Discount).FirstOrDefault(),
                                      Change = productGroup.Select(x => x.b.Change).FirstOrDefault(),
                                      VAT = 0.00M,
                                      DateInvoiced = productGroup.Select(x => x.b.DateInvoiced).FirstOrDefault(),
                                      DateOrdered = productGroup.Select(x => x.a.DateOrdered).FirstOrDefault(),

                                  }).ToListAsync();

                if (data == null)
                {
                    return NotFound();
                }
                return Ok(data);
            }

        }

    }
}
