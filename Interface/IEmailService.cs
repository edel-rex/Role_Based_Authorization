using System;
using jwt_employee.Models;

namespace jwt_employee.Interface
{
    public interface IEmailService
    {
        void SendEmail(Email request);
    }
}
