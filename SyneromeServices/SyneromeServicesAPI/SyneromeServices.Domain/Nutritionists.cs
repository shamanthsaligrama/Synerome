using System;
using System.Collections.Generic;
using System.Text;

namespace SyneromeServices.Domain
{
    public class Nutritionists
    {
        public int Id { get; set; }
        public int LicenseID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public Boolean EmailVerified { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public Boolean PhoneVerified { get; set; }
        public DateTime LastLoginTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }
}
