﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YourVitebskWebServiceApp.APIServiceInterfaces;
using System;
using YourVitebskWebServiceApp.APIModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace YourVitebskWebServiceApp.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Register user
        [HttpPost("register")]
        public async Task<ActionResult<ResponseModel>> Register(UserRegisterDTO user)
        {
            if (string.IsNullOrWhiteSpace(user.Email) 
                || string.IsNullOrWhiteSpace(user.Password) 
                || string.IsNullOrWhiteSpace(user.FirstName) 
                || string.IsNullOrWhiteSpace(user.LastName))
            {
                return BadRequest(ResponseModel.CreateResponseWithError("Заполните все обязательные поля"));
            }

            try
            {
                string token = await _authService.Register(user);
                return Ok(ResponseModel.CreateResponseWithContent(token));
            }
            catch (ArgumentException e)
            {
                return BadRequest(ResponseModel.CreateResponseWithError(e.Message));
            }
        }

        // Authenticate user
        [HttpPost("login")]
        public async Task<ActionResult<ResponseModel>> Login(UserLoginDTO user)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest(ResponseModel.CreateResponseWithError("Введите email"));
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest(ResponseModel.CreateResponseWithError("Введите пароль"));
            }

            try
            {
                string token = await _authService.Login(user);
                return Ok(ResponseModel.CreateResponseWithContent(token));
            }
            catch (ArgumentException e)
            {
                return BadRequest(ResponseModel.CreateResponseWithError(e.Message));
            }
        }

        // Restore password
        [HttpGet("restorepassword")]
        public async Task<ActionResult<ResponseModel>> RestorePassword(string email, string firstName)
        {
            try
            {
                await _authService.RestorePassword(email, firstName);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(ResponseModel.CreateResponseWithError(e.Message));
            }
        }

        // Update user
        [HttpPost("update")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ResponseModel>> Update(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest(ResponseModel.CreateResponseWithError("Введите email"));
            }

            if (string.IsNullOrWhiteSpace(user.FirstName))
            {
                return BadRequest(ResponseModel.CreateResponseWithError("Введите имя"));
            }

            if (string.IsNullOrWhiteSpace(user.LastName))
            {
                return BadRequest(ResponseModel.CreateResponseWithError("Введите фамилию"));
            }

            try
            {
                string token = await _authService.Update(user);
                return Ok(ResponseModel.CreateResponseWithContent(token));
            }
            catch (ArgumentException e)
            {
                return BadRequest(ResponseModel.CreateResponseWithError(e.Message));
            }
        }
    }
}
