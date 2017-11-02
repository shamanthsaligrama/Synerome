using System;
using System.Collections.Generic;
using System.Linq;

using SyneromeServices.Data;
using SyneromeServices.Domain;

namespace SyneromeServicesAPI.Services
{
    public class SyneromeAuthServices : ISyneromeAuthServices
    {
        #region Declarations
        private SyneromeServicesContext _synDBContext;
        #endregion Declarations

        #region Constructor
        public SyneromeAuthServices(SyneromeServicesContext syneromeDbContext)
        {
            _synDBContext = syneromeDbContext;
        }
        #endregion Constructor

        #region Get Users and Nutritionists
        public IEnumerable<Users> GetAllUsers()
        {
            return _synDBContext.Users;
        }

        public IEnumerable<Nutritionists> GetAllNutritionists()
        {
            return _synDBContext.Nutritionists;
        }

        public Users GetUserById(int id)
        {
            return _synDBContext.Users.Find(id);
        }

        public Nutritionists GetNutritionistById(int id)
        {
            return _synDBContext.Nutritionists.Find(id);
        }
        #endregion Get Users and Nutritionists

        #region Create Users and Nutritionists
        public Users CreateUsers(Users user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Password is required");

            if (_synDBContext.Users.Any(u => u.Email == user.Email))
                throw new Exception("User already registered using this email");

            byte[] passwordSalt, passwordHash;
            SyneromeAuthServices.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash; 
            user.PasswordSalt = passwordSalt;
            user.EmailVerified = false;
            user.PhoneVerified = false;
            user.LastLoginTime = System.DateTime.Now;

            _synDBContext.Users.Add(user);
            _synDBContext.SaveChanges();
            return user;
        }
        
        public Nutritionists CreateNutritionist(Nutritionists nutritionist, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Password is required");

            if (_synDBContext.Nutritionists.Any(u => u.Email == nutritionist.Email))
                throw new Exception("Nutritionist already registered using this email");

            byte[] passwordSalt, passwordHash;
            SyneromeAuthServices.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            nutritionist.PasswordHash = passwordHash;
            nutritionist.PasswordSalt = passwordSalt;            
            nutritionist.EmailVerified = false;
            nutritionist.PhoneVerified = false;
            nutritionist.LastLoginTime = System.DateTime.Now;

            _synDBContext.Nutritionists.Add(nutritionist);
            _synDBContext.SaveChanges();
            return nutritionist;
        }
        #endregion Create Users and Nutritionists

        #region Update Users and Nutritionists
        public void UpdateUsers(Users userParam, string password = null)
        {
            var user = _synDBContext.Users.Find(userParam.Id);

            if (user == null)
                throw new Exception("User not found");

            if (userParam.Email != user.Email)
            {
                // username has changed so check if the new username is already taken
                if (_synDBContext.Users.Any(x => x.Email == userParam.Email))
                    throw new Exception("Username " + userParam.Email + " is already taken");
            }

            // update user properties
            user.FirstName = userParam.FirstName;
            user.LastName = userParam.LastName;
            user.DOB = userParam.DOB;
            user.City = userParam.City;
            user.State = userParam.State;
            user.Zip = userParam.Zip;
            user.Country = userParam.Country;
            user.Phone = userParam.Phone;
            user.EmailVerified = true;
            user.PhoneVerified = false;
            user.LastUpdatedTime = System.DateTime.Now;
            
            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash,passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _synDBContext.Users.Update(user);
            _synDBContext.SaveChanges();
        }

        public void UpdateNutritionist(Nutritionists nutritionistParam, string password = null)
        {
            var nutritionist = _synDBContext.Nutritionists.Find(nutritionistParam.Id);

            if (nutritionist == null)
                throw new Exception("Nutritionist not found");

            if (nutritionistParam.Email != nutritionist.Email)
            {
                // username has changed so check if the new username is already taken
                if (_synDBContext.Nutritionists.Any(x => x.Email == nutritionistParam.Email))
                    throw new Exception("Nutritionists " + nutritionistParam.Email + " is already taken");
            }

            // update user properties
            nutritionist.FirstName = nutritionistParam.FirstName;
            nutritionist.LastName = nutritionistParam.LastName;
            nutritionist.DOB = nutritionistParam.DOB;
            nutritionist.City = nutritionistParam.City;
            nutritionist.State = nutritionistParam.State;
            nutritionist.Zip = nutritionistParam.Zip;
            nutritionist.Country = nutritionistParam.Country;
            nutritionist.Phone = nutritionistParam.Phone;
            nutritionist.EmailVerified = true;
            nutritionist.PhoneVerified = false;
            nutritionist.LastUpdatedTime = System.DateTime.Now;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash,passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                nutritionist.PasswordHash = passwordHash;
                nutritionist.PasswordSalt = passwordSalt;
            }

            _synDBContext.Nutritionists.Update(nutritionist);
            _synDBContext.SaveChanges();
        }
        #endregion Update Users and Nutritionists

        #region Authenticate Users and Nutritionists
        public Users AuthenticateUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _synDBContext.Users.SingleOrDefault(x => x.Email == username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public Nutritionists AuthenticateNutritionist(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var nutritionist = _synDBContext.Nutritionists.SingleOrDefault(x => x.Email == username);

            // check if username exists
            if (nutritionist == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, nutritionist.PasswordHash, nutritionist.PasswordSalt))
                return null;

            // authentication successful
            return nutritionist;
        }
        #endregion Authenticate Users and Nutritionists

        #region private helper methods
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
        #endregion private helper methods
    }
}
