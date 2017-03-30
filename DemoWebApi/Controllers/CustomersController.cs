using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using DemoWebApi.Extension;
using DemoWebApi.Interface;
using DemoWebApi.Models;
using DemoWebApi.Models.Domain;
using DemoWebApi.Repository;
using PagedList;

namespace DemoWebApi.Controllers
{
    public class CustomersController : ApiController
    {
        private readonly IRepository<Customer> _customerRepository;

        public CustomersController() : this(new Repository<Customer>())
        {
        }

        public CustomersController(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }


        /// <summary>
        /// 取得單一顧客
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/customers/{id}")]
        [ResponseType(typeof(Customer))]
        public IHttpActionResult Get(string id)
        {
            var customer = _customerRepository.GetAll()
                                            .FirstOrDefault(x => x.CustomerID == id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [Route("api/customersWithOrder")]
        public IHttpActionResult GetCustomersWithOrder()
        {
            var customers = _customerRepository.GetAll().Include(x => x.Orders);
            return Ok(customers);
        }

        [HttpPost]
        [Route("api/customers/{customerId}/orders/{year:int}/{month:int}/page/{pageNumber:int=1}")]
        public IHttpActionResult GetCustomerOrdersBy(string customerId, int year, int month, int pageNumber, OrderSearchFilter searchFilter)
        {
            var startTime = new DateTime(year, month, 1);
            var endTime = new DateTime(year, month + 1, 1);
            //取得顧客該月訂單
            var monthOrders = _customerRepository
                .GetAll()
                .Include(x => x.Orders)
                .FirstOrDefault(x => x.CustomerID == customerId)?
                .Orders.AsQueryable()
                .Where(x => x.OrderDate >= startTime
                            && x.OrderDate < endTime );
            //搜尋過濾
            var filteredOrders = monthOrders?.Where(searchFilter.GetPredicate());

            return Ok(filteredOrders
                        .OrderBy(searchFilter.SortColumnName)
                        .ToPagedList(pageNumber, pageSize: 5));
        }

        

        [Route("api/customersCsv")]
        public IHttpActionResult GetCustomersCsv()
        {
            var customers = _customerRepository.GetAll();
            return this.Csv
                (
                    entities: customers, 
                    fileName: $"{customers.FirstOrDefault()?.CompanyName}_Customers.csv"
                );
        }

        [Route("api/customers")]
        [HttpPost]
        public IHttpActionResult CreateCustomer([FromBody]Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetModelValidationErrorMsg());
            }
            var createdCustomer = _customerRepository.Create(customer);
            return Ok($"Customer:{createdCustomer.CustomerID} 已被新增");
        }

        [Route("api/customers")]
        [HttpPut]
        public IHttpActionResult UpdateCustomer([FromBody]Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetModelValidationErrorMsg());
            }
            var updatedCustomer = _customerRepository.Update(customer);
            return Ok($"Customer:{updatedCustomer.CustomerID} 已被更新");
        }

        [Route("api/customers/{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteCustomer(string id)
        {
            _customerRepository.Delete(new Customer {CustomerID = id});
            return Ok($"Customer:{id} 已被移除");
        }

        private string GetModelValidationErrorMsg()
        {
            var sb = new StringBuilder();
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    sb.Append(error.ErrorMessage);
                }
            }
            return sb.ToString();
        }
    }
}
