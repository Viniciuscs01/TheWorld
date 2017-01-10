﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Web
{
    public class AppController : Controller
    {
        private IMailService _mailService;
        private IConfigurationRoot _config;
        private IWorldRepository _repository;
        private ILogger<AppController> _logger;

        public AppController(IMailService mailService, IConfigurationRoot config, IWorldRepository repository, ILogger<AppController> logger)
        {
            _mailService = mailService;
            _config = config;
            _repository = repository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var data = _repository.getAllTrips();
                return View(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"$Failed to get trips in Index Page: {ex.Message}");
                return Redirect("/error");
            }
            
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            if (model.Email.Contains("AOL"))
            {
                ModelState.AddModelError("", "We don't support AOL Address!");
            }

            if (ModelState.IsValid)
            {
                _mailService.SendMail(_config["MailSettings:ToAddress"], "teste2@teste.com", "teste", "body");

                ModelState.Clear();
                ViewBag.UserMessage = "Message Sent";

            }
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
