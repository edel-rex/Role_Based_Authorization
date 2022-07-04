using System;
using jwt_employee.Data;
using jwt_employee.Interface;
using jwt_employee.Models;
using Microsoft.EntityFrameworkCore;

namespace jwt_employee.Repository
{
    public class EmployeeRepository : IEmployees
    {
        readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }
        public void AddEmployee(Employee employee)
        {
            try{
                _context.Employees!.Add(employee);
                _context.SaveChanges();
            }
            catch{
                throw;
            }
        }

        public bool CheckEmployee(int id)
        {
            return _context.Employees!.Any(e => e.EmployeeID == id);
        }

        public Employee DeleteEmployee(int id)
        {
            try{
                Employee? employee = _context.Employees!.Find(id);
                if (employee != null){
                    _context.Employees.Remove(employee);
                    _context.SaveChanges();
                    return employee;
                }
                else{
                    throw new ArgumentNullException();
                }
            }catch{
                throw;
            }
        }

        public List<Employee> GetEmployeeDetails()
        {
            try{
                return _context.Employees!.ToList();
            }
            catch{
                throw;
            }
        }

        public Employee GetEmployeeDetails(int id)
        {
            try{
                Employee? employee = _context.Employees!.Find(id);
                if (employee != null){
                    return employee;
                }
                else{
                    throw new ArgumentNullException();
                }
            }catch{
                throw;
            }
        }

        public void UpdateEmployee(Employee employee)
        {
            try{
                _context.Entry(employee).State = EntityState.Modified;
                _context.SaveChanges();
            }catch{
                throw;
            }
        }
    }
}
