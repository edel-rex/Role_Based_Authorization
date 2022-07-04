using System;
using jwt_employee.Data;
using jwt_employee.Interface;
using jwt_employee.Models;
using Microsoft.EntityFrameworkCore;

namespace jwt_employee.Repository
{
    public class UserInfoRepository : IUserInfos
    {
        readonly AppDbContext _context;

        public UserInfoRepository(AppDbContext context)
        {
            _context = context;
        }
        public void AddUser(UserInfo user)
        {
            try
            {
                _context.Add(user);
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public UserInfo DeleteUser(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(UserInfo user)
        {
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
    }
}
