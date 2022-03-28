﻿using AirlineReservationSystem.Core;
using AirlineReservationSystem.Core.Contracts;
using AirlineReservationSystem.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AirlineReservationSystem.Controllers
{
    public class UserController : BaseController
    {
        private readonly RoleManager<IdentityRole> roleManager;

        private readonly UserManager<ApplicationUser> userManager;

        private readonly IUserService userService;

        public UserController(
            RoleManager<IdentityRole> _roleManager,
            UserManager<ApplicationUser> _userManager,
            IUserService _userService)
        {
            roleManager = _roleManager;
            userManager = _userManager;
            userService = _userService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = UserConstants.Role.AdministratorRole)]
        public async Task<IActionResult> ManageUsers() 
        {

            var users = await userService.GetUsers();

            return Ok(users);
        
        }

        public async Task<IActionResult> CreateRole()
        {
            //await roleManager.CreateAsync(new IdentityRole()
            //{
            //    Name = UserConstants.Role.AdministratorRole
            //});

            return Ok();
        }
    }
}
