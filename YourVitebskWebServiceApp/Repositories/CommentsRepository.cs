﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using YourVitebskWebServiceApp.Helpers;
using YourVitebskWebServiceApp.Interfaces;
using YourVitebskWebServiceApp.Models;
using YourVitebskWebServiceApp.ViewModels;

namespace YourVitebskWebServiceApp.Repositories
{
    public class CommentsRepository : ICommentRepository
    {
        private readonly YourVitebskDBContext _context;
        private readonly RolePermissionManager _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private bool _disposed = false;

        public CommentsRepository(YourVitebskDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _roleManager = new RolePermissionManager(_context);
            _httpContextAccessor = httpContextAccessor;
        }

        public bool CheckRolePermission(string permission)
        {
            return _roleManager.HasPermission(_httpContextAccessor.HttpContext.User.Identity.Name, permission);
        }

        public IEnumerable<CommentViewModel> Get()
        {
            var result = new List<CommentViewModel>();
            IEnumerable<Comment> comments = _context.Comments.ToList();
            foreach (var comment in comments)
            {
                result.Add(Get((int)comment.CommentId));
            }

            return result;
        }

        public CommentViewModel Get(int id)
        {
            Comment comment = _context.Comments.FirstOrDefault(x => x.CommentId == id);
            if (comment == null)
            {
                return null;
            }

            object service = null;
            string serviceTypeName = null;
            var itemName = "";
            switch (comment.ServiceId)
            {
                case 1:
                    service = _context.Cafes.First(x => x.CafeId == comment.ItemId);
                    serviceTypeName = _context.CafeTypes.First(x => x.CafeTypeId == (service as Cafe).CafeTypeId).Name;
                    itemName = $"{serviceTypeName} {(service as Cafe).Title}";
                    break;
                case 2:
                    service = _context.Posters.First(x => x.PosterId == comment.ItemId);
                    serviceTypeName = _context.PosterTypes.First(x => x.PosterTypeId == (service as Poster).PosterTypeId).Name;
                    itemName = $"{serviceTypeName} {(service as Poster).Title}";
                    break;
            }

            return new CommentViewModel
            {
                CommentId = (int)comment.CommentId,
                UserEmail = _context.Users.First(x => x.UserId == comment.UserId).Email,
                Service = _context.Services.First(x => x.ServiceId == comment.ServiceId).Name,
                ItemName = itemName,
                IsRecommend = comment.IsRecommend ? "Рекомендует" : "Не рекомендует",
                Message = comment.Message,
                PublishDate = comment.PublishDate.ToString("D", new CultureInfo("ru-RU")) + comment.PublishDate.ToString(" HH:mm")
            };
        }

        public void Delete(int id)
        {
            _context.Comments.Remove(_context.Comments.FirstOrDefault(x => x.CommentId == id));
            _context.SaveChanges();
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
