﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YourVitebskWebServiceApp.APIServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using YourVitebskWebServiceApp.APIModels;

namespace YourVitebskWebServiceApp.APIControllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CafesController : ControllerBase
    {
        private readonly IService<Cafe> _cafesService;

        public CafesController(IService<Cafe> cafesService)
        {
            _cafesService = cafesService;
        }

        // Gets all cafes
        [HttpGet("cafes/all")]
        public async Task<ActionResult<IEnumerable<Cafe>>> GetAll()
        {
            IEnumerable<Cafe> cafes = await _cafesService.GetAll();
            if (cafes == null)
            {
                return NotFound();
            }

            return Ok(cafes);
        }

        // Gets cafe by id
        [HttpGet("cafes/{id}")]
        public async Task<ActionResult<ResponseModel>> Get(int id)
        {
            try
            {
                Cafe cafe = await _cafesService.GetById(id);
                return Ok(ResponseModel.CreateResponseWithContent(cafe));
            }
            catch (ArgumentException e)
            {
                return NotFound(ResponseModel.CreateResponseWithError(e.Message));
            }
        }
    }
}
