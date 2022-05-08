﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using YourVitebskWebServiceApp.Interfaces;
using YourVitebskWebServiceApp.Models;
using YourVitebskWebServiceApp.ViewModels;

namespace YourVitebskWebServiceApp.Controllers
{
    [Authorize]
    public class PostersController : Controller
    {
        private readonly YourVitebskDBContext _context;
        private readonly IRepository<Poster> _repository;

        public PostersController(YourVitebskDBContext context, IRepository<Poster> repository)
        {
            _context = context;
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View(_repository.Get());
        }

        public ActionResult Create()
        {
            ViewBag.PosterTypes = _context.PosterTypes;
            return View();
        }

        [HttpPost]
        public IActionResult CreateAsync(PosterViewModel newPoster)
        {
            if (_context.Posters.FirstOrDefault(x => x.Title == newPoster.Title) != null)
            {
                ModelState.AddModelError("Title", "Афиша с таким именем уже используется");
            }

            if (newPoster.PosterTypeId == 0)
            {
                ModelState.AddModelError("PosterTypeId", "Выберите тип искусства");
            }

            if (!DateTime.TryParse(newPoster.DateTime, out DateTime dateTime) || dateTime >= new DateTime(2079, 6, 6))
            {
                ModelState.AddModelError("DateTime", "Некорректная дата и(или) время");
            }

            if (dateTime <= DateTime.Now)
            {
                ModelState.AddModelError("DateTime", "Необходимо указать будущуюю дату");
            }

            if (ModelState.IsValid)
            {
                var Poster = new Poster
                {
                    PosterId = null,
                    PosterTypeId = newPoster.PosterTypeId,
                    Title = newPoster.Title,
                    Description = newPoster.Description,
                    DateTime = dateTime,
                    Address = newPoster.Address,
                    ExternalLink = newPoster.ExternalLink,
                };

                _repository.Create(Poster);
                return RedirectToAction("Index");
            }

            ViewBag.PosterTypes = _context.PosterTypes;
            return View(newPoster);
        }

        public ActionResult Edit(int id)
        {
            Poster poster = _repository.Get(id);
            if (poster != null)
            {
                var viewModel = new PosterViewModel
                {
                    PosterId = poster.PosterId,
                    PosterTypeId = poster.PosterTypeId,
                    Title = poster.Title,
                    Description = poster.Description,
                    DateTime = ((DateTime)poster.DateTime).ToString("yyyy-MM-dd HH:mm"),
                    Address = poster.Address,
                    ExternalLink = poster.ExternalLink
                };

                ViewBag.PosterTypes = _context.PosterTypes;
                ViewData["PosterTypeId"] = poster.PosterTypeId;
                return View(viewModel);
            }

            return NotFound();
        }

        [HttpPost]
        public ActionResult Edit(PosterViewModel newPoster)
        {
            Poster Poster = _repository.Get((int)newPoster.PosterId);
            if (_context.Posters.FirstOrDefault(x => x.Title == newPoster.Title && newPoster.Title != Poster.Title) != null)
            {
                ModelState.AddModelError("Title", "Афиша с таким именем уже используется");
            }

            if (newPoster.PosterTypeId == 0)
            {
                ModelState.AddModelError("PosterTypeId", "Выберите тип искусства");
            }

            if (!DateTime.TryParse(newPoster.DateTime, out DateTime dateTime) || dateTime >= new DateTime(2079, 6, 6))
            {
                ModelState.AddModelError("DateTime", "Некорректная дата и(или) время");
            }

            if (dateTime <= DateTime.Now)
            {
                ModelState.AddModelError("DateTime", "Необходимо указать будущуюю дату");
            }

            if (ModelState.IsValid)
            {
                Poster.PosterTypeId = newPoster.PosterTypeId;
                Poster.Title = newPoster.Title;
                Poster.Description = newPoster.Description;
                Poster.DateTime = dateTime;
                Poster.Address = newPoster.Address;
                Poster.ExternalLink = newPoster.ExternalLink;
                _repository.Update(Poster);
                return RedirectToAction("Index");
            }

            ViewBag.PosterTypes = _context.PosterTypes;
            ViewData["PosterTypeId"] = newPoster.PosterTypeId;
            return View(newPoster);
        }

        [HttpGet]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(int id)
        {
            Poster Poster = _repository.Get(id);
            if (Poster != null)
            {
                ViewData["PosterType"] = _context.PosterTypes.First(x => x.PosterTypeId == Poster.PosterTypeId).Name;
                return View(Poster);
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