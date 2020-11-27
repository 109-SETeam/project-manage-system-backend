﻿using Microsoft.EntityFrameworkCore;
using project_manage_system_backend.Models;
using project_manage_system_backend.Shares;
using project_manage_system_backend.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Services
{
    public class UserService : BaseService
    {
        public UserService(PMSContext dbContext) : base(dbContext) { }

        public void CreateUser(User model)
        {
            _dbContext.Users.Add(model);
            if (_dbContext.SaveChanges() == 0)
            {
                throw new Exception("create user fail");
            }
        }

        public bool CheckUserExist(string account)
        {
            var Users = _dbContext.Users.Where(u => u.Account.Equals(account));
            if (Users.Count() > 0) return true;
            return false;
        }

        public UserInfoDto GetUser(string account)
        {
            var user = _dbContext.Users.Find(account);

            return new UserInfoDto { Id = user.Account, Name = user.Name, AvatarUrl = user.AvatarUrl };
        }

        public User GetUserModel(string account)
        {
            var user = _dbContext.Users.Find(account);

            return new User 
            { 
                Account = user.Account, 
                Name = user.Name, 
                AvatarUrl = user.AvatarUrl,
                Authority = user.Authority,
                Projects = user.Projects
            };
        }
    }
}
