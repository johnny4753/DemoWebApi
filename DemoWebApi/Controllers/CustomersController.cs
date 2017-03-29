using System.Data.Entity;
using System.Web.Http;
using DemoWebApi.Models.Domain;
using DemoWebApi.Repository;

namespace DemoWebApi.Controllers
{
    public class CustomersController : ApiController
    {
        private Repository<Customer> _customerRepository;

        public CustomersController()
        {
            _customerRepository = new Repository<Customer>();
        }

        

        [Route("api/customers/{id}")]
        public IHttpActionResult Get(string id)
        {
            var customer = _customerRepository.Get(x => x.CustomerID == id);
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
