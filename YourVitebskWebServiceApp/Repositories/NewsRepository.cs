﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using YourVitebskWebServiceApp.Helpers;
using YourVitebskWebServiceApp.Interfaces;
using YourVitebskWebServiceApp.Models;

namespace YourVitebskWebServiceApp.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly YourVitebskDBContext _context;
        private readonly ImageService _imageService;
        private readonly RolePermissionManager _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private bool _disposed = false;

        public NewsRepository(YourVitebskDBContext context, IWebHostEnvironment appEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _imageService = new ImageService(appEnvironment);
            _roleManager = new RolePermissionManager(_context);
            _httpContextAccessor = httpContextAccessor;
        }

        public bool CheckRolePermission(string permission)
        {
            return _roleManager.HasPermission(_httpContextAccessor.HttpContext.User.Identity.Name, permission);
        }

        public IEnumerable<News> Get()
        {
            return _context.News.ToList();
        }

        public News Get(int id)
        {
            return _context.News.FirstOrDefault(x => x.NewsId == id);
        }

        public void Create(News news, IFormFileCollection uploadedFiles)
        {
            _context.News.Add(news);
            _context.SaveChanges();
            _imageService.SaveImages("news", (int)news.NewsId, uploadedFiles);
        }

        public void Update(News news, IFormFileCollection uploadedFiles)
        {
            _context.News.Update(news);
            _context.SaveChanges();
            _imageService.SaveImages("news", (int)news.NewsId, uploadedFiles);
        }

        public void Delete(int id)
        {
            _context.News.Remove((News)Get(id));
            _context.SaveChanges();
            _imageService.DeleteImages("news", id);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
