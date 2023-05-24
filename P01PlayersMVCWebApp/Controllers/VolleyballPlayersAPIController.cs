using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using P01PlayersMVCWebApp.Configuration;
using P01PlayersMVCWebApp.Models;
using P01PlayersMVCWebApp.Services;

namespace P01PlayersMVCWebApp.Controllers
{
    public class VolleyballPlayersAPIController : Controller
    {
        private readonly VolleyballWebContext _context;
        private readonly HttpClient _client;
        private readonly ApiSettings _apiSettings;
        private readonly string _resourcePath;
        private readonly ScoreCalculator _scoreCalculator;
        public VolleyballPlayersAPIController(IHttpClientFactory clientFactory, IOptions<ApiSettings> apiSettings)
        {
            _client = clientFactory.CreateClient();
            _apiSettings = apiSettings.Value;
            _resourcePath = "/volleyballplayers";  // Definiowanie ścieżki zasobu
        }

        // GET: VolleyballPlayersAPI
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string sort = "")
        {
            try
            {
                var playersResponse = await _client.GetAsync($"{_apiSettings.BaseUrl}{_resourcePath}");
                if (!playersResponse.IsSuccessStatusCode)
                {
                    return Problem("Cannot access API to retrieve players.", statusCode: (int)playersResponse.StatusCode);
                }

                var playersContent = await playersResponse.Content.ReadAsStringAsync();
                var volleyballPlayers = JsonConvert.DeserializeObject<List<VolleyballPlayer>>(playersContent);


                foreach (VolleyballPlayer player in volleyballPlayers)
                {
                    // TODO: Implement score calculation logic for each player
                    if (player.MatchesPlayed > 0)
                    {
                        player.Score = 5 * (double)player.PointsScored / player.MatchesPlayed + 100 * player.MedalsWon;
                        player.Score = Math.Round(player.Score, 2);
                    }
                    else
                    {
                        player.Score = 0;
                    }
                }


                // Sort the players based on score
                switch (sort)
                {
                    case "score":
                        volleyballPlayers = volleyballPlayers.OrderBy(p => p.Score).ToList();
                        break;
                    case "score_desc":
                        volleyballPlayers = volleyballPlayers.OrderByDescending(p => p.Score).ToList();
                        break;
                    case "id":
                        volleyballPlayers = volleyballPlayers.OrderBy(p => p.Id).ToList();
                        break;
                    case "id_desc":
                        volleyballPlayers = volleyballPlayers.OrderByDescending(p => p.Id).ToList();
                        break;
                    case "name":
                        volleyballPlayers = volleyballPlayers.OrderBy(p => p.Name).ToList();
                        break;
                    case "name_desc":
                        volleyballPlayers = volleyballPlayers.OrderByDescending(p => p.Name).ToList();
                        break;
                    default:
                        // No sort specified, use the default order
                        // Add your default sorting logic here
                        break;
                }

                // Calculate total count and totalPages based on the retrieved players
                var totalCount = volleyballPlayers.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                // Validate page number
                if (page < 1 || page > totalPages)
                {
                    return BadRequest("Invalid page number.");
                }

                // Get players for the specified page
                var startIndex = (page - 1) * pageSize;
                var playersForPage = volleyballPlayers.Skip(startIndex).Take(pageSize).ToList();

                // Store the sort parameter in the ViewBag
                ViewBag.Sort = sort;

                // Create pager object for the specified page
                var pager = new Pager<VolleyballPlayer>(totalCount, pageSize, page, playersForPage);

                return View(pager);
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while processing your request, " + ex.Message, statusCode: 500);
            }
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _client.GetAsync($"{_apiSettings.BaseUrl}{_resourcePath}/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var volleyballPlayer = JsonConvert.DeserializeObject<VolleyballPlayer>(content);
                return View(volleyballPlayer);
            }
            else
            {
                return NotFound();
            }
        }

        // GET: VolleyballPlayersAPI/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VolleyballPlayersAPI/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Position,Number,MatchesPlayed,PointsScored,MedalsWon")] VolleyballPlayer volleyballPlayer)
        {
            if (ModelState.IsValid)
            {
                var response = await _client.PostAsJsonAsync($"{_apiSettings.BaseUrl}{_resourcePath}", volleyballPlayer);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(volleyballPlayer);
        }

        // GET: VolleyballPlayersAPI/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _client.GetAsync($"{_apiSettings.BaseUrl}{_resourcePath}/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var volleyballPlayer = JsonConvert.DeserializeObject<VolleyballPlayer>(content);
                return View(volleyballPlayer);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: VolleyballPlayersAPI/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Position,Number,MatchesPlayed,PointsScored,MedalsWon")] VolleyballPlayer volleyballPlayer)
        {
            if (id != volleyballPlayer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var response = await _client.PutAsJsonAsync($"{_apiSettings.BaseUrl}{_resourcePath}/{id}", volleyballPlayer);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View(volleyballPlayer);
                }
            }
            return View(volleyballPlayer);
        }

        // GET: VolleyballPlayersAPI/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _client.GetAsync($"{_apiSettings.BaseUrl}{_resourcePath}/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var volleyballPlayer = JsonConvert.DeserializeObject<VolleyballPlayer>(content);
                return View(volleyballPlayer);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: VolleyballPlayersAPI/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _client.DeleteAsync($"{_apiSettings.BaseUrl}{_resourcePath}/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }

        private bool VolleyballPlayerExists(int id)
        {
          return (_context.VolleyballPlayers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
