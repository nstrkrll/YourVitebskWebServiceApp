﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YourVitebskWebServiceApp.APIServiceInterfaces;

namespace YourVitebskWebServiceApp.APIServices
{
    public class UsersService : IUsersService
    {
        private readonly YourVitebskDBContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public UsersService(YourVitebskDBContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public async Task<IEnumerable<APIModels.UsersListItem>> GetAllUsers(int id)
        {
            var result = new List<APIModels.UsersListItem>();
            IEnumerable<Models.User> users = await _context.Users.Where(x => x.IsVisible == true && x.UserId != id).ToListAsync();
            foreach (var user in users)
            {
                string image = "";
                if (Directory.Exists($"{_appEnvironment.WebRootPath}/images/users/{user.UserId}"))
                {
                    image = Directory.GetFiles($"{_appEnvironment.WebRootPath}/images/users/{user.UserId}").Select(x => Path.GetFileName(x)).First();
                }

                result.Add(new APIModels.UsersListItem()
                {
                    UserId = (int)user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Image = image
                });
            }

            return result;
        }
    }
}
