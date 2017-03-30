using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using DemoWebApi.Extension;
using DemoWebApi.Interface;
using DemoWebApi.Models;
using DemoWebApi.Models.DataTransferObject;
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
        /// 取得單一顧客資料
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

        /// <summary>
        /// 取得所有顧客及其相關的訂單
        /// </summary>
        /// <returns></returns>
        [Route("api/customersWithOrder")]
        [ResponseType(typeof(IQueryable<Customer>))]
        public IHttpActionResult GetCustomersWithOrder()
        {
            var customers = _customerRepository.GetAll().Include(x => x.Orders);
            return Ok(customers);
        }

        /// <summary>
        /// 取得該名顧客特定月份的分頁訂單資料，並透過 OrderSearchFilter 過濾
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="pageNumber"></param>
        /// <param name="searchFilter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customers/{customerId}/orders/{year:int}/{month:int}/page/{pageNumber:int=1}")]
        [ResponseType(typeof(List<OrderDto>))]
        public IHttpActionResult GetCustomerOrdersBy(string customerId, int year, int month, int pageNumber, OrderSearchFilter searchFilter)
        {
            if (searchFilter == null)
            {
                searchFilter = new OrderSearchFilter();
            }
            var startTime = new DateTime(year, month, 1);
            var days = DateTime.DaysInMonth(year, month);
            var endTime = new DateTime(year, month, days, 23, 59, 59);
            //取得顧客該月訂單
            var monthOrders = _customerRepository
                .GetAll()
                .Include(x => x.Orders)
                .FirstOrDefault(x => x.CustomerID == customerId)?
                .Orders
                .Where(x => x.OrderDate >= startTime
                            && x.OrderDate <= endTime );
            //搜尋過濾
            var filteredOrders = monthOrders?.Where(searchFilter.GetPredicate());   //[LinqKit]
            var result = Tools.AutoMapperConfig.Mapper       //[AutoMapper]
                        .Map<List<OrderDto>>(filteredOrders);// 投影到 Dto 物件隱藏導覽屬性

            return Ok(result.AsQueryable()
                        .OrderBy(searchFilter.SortColumnName) //[DynamicQuery]
                        .ToPagedList(pageNumber, pageSize: 5)); //[PagedList]
        }

        
        /// <summary>
        /// 取得所有顧客資料 CSV 檔
        /// </summary>
        /// <returns></returns>
        [Route("api/customersCsv")]
        public IHttpActionResult GetCustomersCsv()
        {
            var customers = _customerRepository.GetAll();
            return this.Csv  //擴充 ApiController, 回傳自定義的 CsvContentResult
                (
                    entities: customers, 
                    fileName: "Customers.csv"
                );
        }

        /// <summary>
        /// 建立一筆顧客資料
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 更新一筆顧客資料
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 刪除一筆顧客資料
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [Route("api/customers/{customerId}")]
        [HttpDelete]
        public IHttpActionResult DeleteCustomer(string customerId)
        {
            _customerRepository.Delete(new Customer {CustomerID = customerId});
            return Ok($"Customer:{customerId} 已被移除");
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
