using System;
using jwt_employee.Models;

namespace jwt_employee.Interface
{
    public interface IUserInfos
    {
        public void AddUser (UserInfo user);
        public void UpdateUser (UserInfo user);
        public UserInfo DeleteUser (int id);
    }
}
