using System;

namespace jwt_employee.Interface
{
    public interface IOTPService
    {
        void SendOTP(string mobileNo, string OTP);
    }
}
