using jwt_employee.Constants;
using jwt_employee.Interface;
using jwt_employee.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace jwt_employee.Controllers
{

    [Route(DbConstants.Route_Employee)]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployees _IEmployee;

        public EmployeeController(IEmployees IEmployee)
        {
            _IEmployee = IEmployee;
        }

        [Authorize(Roles = DbConstants.Role_Admin)]
        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAll()
        {
            return await Task.FromResult(_IEmployee.GetEmployeeDetails());
        }

        [Authorize(Roles = DbConstants.Role_Employee), Authorize(Roles = DbConstants.Role_Admin)]
        [HttpGet]
        public async Task<ActionResult<Employee>> Get()
        {
            var user_id = Convert.ToInt32(HttpContext.Items[DbConstants.Claims_UserID]);
            var employees = await Task.FromResult(_IEmployee.GetEmployeeDetails(1));
            if (employees == null)
            {
                return NotFound();
            }
            return employees;
        }

        [Authorize(Roles = DbConstants.Role_Admin)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> Get(int id)
        {
            var employees = await Task.FromResult(_IEmployee.GetEmployeeDetails(id));
            if (employees == null)
            {
                return NotFound();
            }
            return employees;
        }


        [HttpPost]
        [Authorize(Roles = DbConstants.Role_Admin)]
        public async Task<ActionResult<Employee>> Post(Employee employee)
        {
            _IEmployee.AddEmployee(employee);
            return await Task.FromResult(employee);
        }

        [Authorize(Roles = DbConstants.Role_Admin)]
        [HttpPut("{id}")]
        public async Task<ActionResult<Employee>> Put(int id, Employee employee)
        {
            if (id != employee.EmployeeID)
            {
                return BadRequest();
            }
            try
            {
                _IEmployee.UpdateEmployee(employee);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return await Task.FromResult(employee);
        }

        [Authorize(Roles = DbConstants.Role_Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Employee>> Delete(int id)
        {
            var employee = _IEmployee.DeleteEmployee(id);
            return await Task.FromResult(employee);
        }


        private bool EmployeeExists(int id)
        {
            return _IEmployee.CheckEmployee(id);
        }


    }
}
