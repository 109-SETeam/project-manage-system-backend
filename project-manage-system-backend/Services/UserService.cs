﻿using Microsoft.EntityFrameworkCore;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Models;
using project_manage_system_backend.Shares;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var Users = _dbContext.Users.Where(u => u.Account.Equals(account)).ToList();
            return Users.Any();
        }

        public bool IsProjectOwner(User owner, int projectId)
        {
            var project = _dbContext.Projects.Include(p => p.Owner).Where(p => p.ID.Equals(projectId)).First();
            return project.Owner.Equals(owner);
        }

        public UserInfoDto GetUser(string account)
        {
            var user = _dbContext.Users.Find(account);

            return new UserInfoDto { Id = user.Account, Name = user.Name, AvatarUrl = user.AvatarUrl };
        }

        public List<UserInfoDto> GetAllUser(string inviterId)
        {
            return _dbContext.Users.Where(u => u.Account != inviterId).Select(u => new UserInfoDto
            {
                Id = u.Account,
                Name = u.Name,
                AvatarUrl = u.AvatarUrl
            }).ToList();
        }

        public User GetUserModel(string account)
        {
            var user = _dbContext.Users.Include(u => u.Projects).ThenInclude(p => p.Project).First(u => u.Account.Equals(account));

            return user;
        }

        public void AddProject(Invitation invitation)
        {
            var user = invitation.Applicant;
            user.Projects.Add(new UserProject { User = user, Project = invitation.InvitedProject });
            
            if (_dbContext.SaveChanges() == 0)
            {
                throw new Exception("Add project fail!");
            }
        }
        
        public List<User> GetAllUserExceptAdmin(string account)
        {
            return _dbContext.Users.Where(u => u.Account != account).ToList();
        }

        public void DeleteUserByAccount(string accouunt)
        {
            var user = _dbContext.Users.Include(u => u.Projects).FirstOrDefault(u =>  u.Account == accouunt);
            if (user == null)
                throw new Exception("User not found!");
            if (user.Projects.Any())
            {
                var userProjects = user.Projects.Where(p => p.Account == user.Account).ToList();
                ProjectService projectService = new ProjectService(_dbContext);
                userProjects.ForEach(up => projectService.DeleteProject(up.ProjectId));
            }
            _dbContext.Users.Remove(user);
            if (_dbContext.SaveChanges() == 0)
                throw new Exception("Delet user fail!");
        }
    }
}
