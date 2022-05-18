﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourVitebskWebServiceApp.Interfaces;
using YourVitebskWebServiceApp.Models;

namespace YourVitebskWebServiceApp.Controllers
{
    [Authorize]
    public class NewsController : Controller
    {
        private readonly IRepository<News> _repository;

        public NewsController(IRepository<News> repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            return View(_repository.Get());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(News newNews)
        {
            if (ModelState.IsValid)
            {
                var news = new News
                {
                    NewsId = null,
                    Title = newNews.Title,
                    Description = newNews.Description,
                    ExternalLink = newNews.ExternalLink ?? ""
                };

                _repository.Create(news);
                return RedirectToAction("Index");
            }

            return View(newNews);
        }

        public ActionResult Edit(int id)
        {
            News news = _repository.Get(id);
            if (news != null)
            {
                return View(news);
            }

            return NotFound();
        }

        [HttpPost]
        public ActionResult Edit(News newNews)
        {
            News news = _repository.Get((int)newNews.NewsId);
            if (ModelState.IsValid)
            {
                news.Title = newNews.Title;
                news.Description = newNews.Description;
                news.ExternalLink = newNews.ExternalLink ?? "";
                _repository.Update(news);
                return RedirectToAction("Index");
            }

            return View(newNews);
        }

        [HttpGet]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(int id)
        {
            News news = _repository.Get(id);
            if (news != null)
            {
                return View(news);
            }

            return NotFound();
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _repository.Delete(id);
            return RedirectToAction("Index");
        }
    }
}