﻿using YourVitebskWebServiceApp.Controllers;
using YourVitebskWebServiceApp.Interfaces;
using YourVitebskWebServiceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace YourVitebskWebServiceApp.Tests.ControllerTests
{
    public class NewsControllerTests
    {
        private readonly Mock<IImageRepository<News>> _mockRepo;
        private readonly NewsController _controller;

        public NewsControllerTests()
        {
            _mockRepo = new Mock<IImageRepository<News>>();
            var optionsBuilder = new DbContextOptionsBuilder<YourVitebskDBContext>();
            _controller = new NewsController(_mockRepo.Object);
            _mockRepo.Setup(repo => repo.Get()).Returns(new List<News>()
            {
                new News
                {
                    NewsId = 1,
                    Title = "TestTitle1",
                    Description = "TestDescription1",
                    ExternalLink = null
                },
                new News
                {
                    NewsId = 2,
                    Title = "TestTitle2",
                    Description = "TestDescription2",
                    ExternalLink = null
                }
            });

            _mockRepo.Setup(repo => repo.Get(2)).Returns(new News
            {
                NewsId = 2,
                Title = "TestTitle2",
                Description = "TestDescription2",
                ExternalLink = null
            });
        }

        [Fact]
        public void Index_ReturnsView()
        {
            var result = _controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Index_ReturnsExactNumberOfObjects()
        {
            var result = _controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var objects = Assert.IsType<List<News>>(viewResult.Model);
            Assert.Equal(2, objects.Count);
        }

        [Fact]
        public void Create_ReturnsView()
        {
            var result = _controller.Create();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Create_RedirectsToIndex()
        {
            var obj = new News
            {
                NewsId = null,
                Title = "TestTitle3",
                Description = "TestDescription3",
                ExternalLink = null
            };

            var result = _controller.Create(obj, null);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public void Edit_ReturnsView()
        {
            var result = _controller.Edit(2);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Edit_InvalidId_ReturnsNotFoundResult()
        {
            var result = _controller.Edit(-1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_RedirectsToIndex()
        {
            var newObj = new News
            {
                NewsId = 2,
                Title = "TestTitle3",
                Description = "TestDescription2",
                ExternalLink = null
            };

            var result = _controller.Edit(newObj, null);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public void Delete_ReturnsView()
        {
            var result = _controller.ConfirmDelete(2);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Delete_InvalidId_ReturnsNotFoundResult()
        {
            var result = _controller.ConfirmDelete(-1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_ActionExecuted_RedirectsToIndexAction()
        {
            var result = _controller.Delete(2);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}