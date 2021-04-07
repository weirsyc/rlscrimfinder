using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RocketLeagueScrimFinder.Areas.Management.Models;
using RocketLeagueScrimFinder.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RocketLeagueScrimFinder.Areas.Management.Controllers
{
    [Area("Management")]
    public class ManagementController : Controller
    {
        private MatchmakingService _matchmakingService;
        
        public ManagementController(MatchmakingService matchmakingService)
        {
            _matchmakingService = matchmakingService;
        }

        // GET: HomeController
        public ActionResult Index()
        {
            var matchmakingData = new ManagementViewModel()
            {
                LobbyList = _matchmakingService.GetLobbies(),
                PlayerQueueList = _matchmakingService.GetQueueData(),
                PlayersInMatch = _matchmakingService.GetPlayersInMatch(),
                DeclineList = _matchmakingService.GetDeclineList()
            };

            return View(matchmakingData);
        }

        // GET: HomeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HomeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HomeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
