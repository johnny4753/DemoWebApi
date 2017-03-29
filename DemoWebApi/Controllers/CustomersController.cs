using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using DemoWebApi.Interface;
using DemoWebApi.Models.Domain;
using DemoWebApi.Repository;

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


        [Route("api/customers/{id}")]
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

        [Route("api/customers")]
        [HttpPost]
        public IHttpActionResult CreateCustomer([FromBody]Customer customer)
        {
            _customerRepository.Create(customer);
            return Ok(customer);
        }

        [Route("api/customers/{id}")]
        [HttpPut]
        public IHttpActionResult UpdateCustomer(string id, [FromBody]Customer customer)
        {
            customer.CustomerID = id;
            var updatedCustomer = _customerRepository.Update(customer);
            return Ok(updatedCustomer);
        }

        [Route("api/customers/{id}")]
        [HttpDelete]
        public void DeleteCustomer(string id)
        {
            _customerRepository.Delete(new Customer {CustomerID = id});
        }
    }
}
