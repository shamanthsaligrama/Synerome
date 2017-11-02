using SyneromeServices.Domain;
using SyneromeServicesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyneromeServicesAPI.Services
{
    public interface ISyneromeAuthServices
    {
        IEnumerable<SyneromeServices.Domain.Users> GetAllUsers();

        IEnumerable<SyneromeServices.Domain.Nutritionists> GetAllNutritionists();

        SyneromeServices.Domain.Users GetUserById(int id);

        SyneromeServices.Domain.Nutritionists GetNutritionistById(int id);

        SyneromeServices.Domain.Users CreateUsers(SyneromeServices.Domain.Users user, string password);

        SyneromeServices.Domain.Nutritionists CreateNutritionist(SyneromeServices.Domain.Nutritionists nutritionist, string password);

        void UpdateUsers(SyneromeServices.Domain.Users userParam, string password = null);

        void UpdateNutritionist(SyneromeServices.Domain.Nutritionists nutritionistParam, string password = null);

        SyneromeServices.Domain.Users AuthenticateUser(string username, string password);

        SyneromeServices.Domain.Nutritionists AuthenticateNutritionist(string username, string password);
    }
}
