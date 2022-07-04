using System;
using jwt_employee.Interface;
using TextmagicRest;

namespace jwt_employee.Services
{
    public class OTPService : IOTPService
    {
        public void SendOTP(string mobileNo, string OTP)
        {
            var client = new Client("test", "my-api-key");
            var link = client.SendMessage("Hello from TextMagic API", "9361874324");
            if (link.Success)
            {
                Console.WriteLine("Message ID {0} has been successfully sent", link.Id);
            }
            else
            {
                Console.WriteLine("Message was not sent due to following exception: {0}", link.ClientException.Message);
            }
        }
    }
}
