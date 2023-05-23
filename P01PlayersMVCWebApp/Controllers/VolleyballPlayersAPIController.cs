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
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            try
            {
                var totalCountResponse = await _client.GetAsync($"{_apiSettings.BaseUrl}{_resourcePath}/all");
                if (!totalCountResponse.IsSuccessStatusCode)
                {
                    return Problem("Cannot access API to retrieve size data.", statusCode: (int)totalCountResponse.StatusCode);
                }

                var totalCountContent = await totalCountResponse.Content.ReadAsStringAsync();
                var totalCountData = JObject.Parse(totalCountContent);
                if (!int.TryParse(totalCountData["totalCount"].ToString(), out var totalCount))
                {
                    return Problem("Invalid size data received from API.", statusCode: 500);
                }


                // Calculate total pages
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                // Validate page number
                if (page < 1 || page > totalPages)
                {
                    return BadRequest("Invalid page number.");
                }

                // Get volleyball players for the specified page
                var playersResponse = await _client.GetAsync($"{_apiSettings.BaseUrl}{_resourcePath}/{page}/{pageSize}");
                if (!playersResponse.IsSuccessStatusCode)
                {
                    return Problem("Cannot access API to retrieve players.", statusCode: (int)playersResponse.StatusCode);
                }

                var playersContent = await playersResponse.Content.ReadAsStringAsync();
                var volleyballPlayers = JsonConvert.DeserializeObject<List<VolleyballPlayer>>(playersContent);

                foreach( VolleyballPlayer player in volleyballPlayers )
                {

                    // TODO implement it as method
                    if (player.MatchesPlayed > 0)
                        player.Score = 5 * (double)player.PointsScored / player.MatchesPlayed + 100 * player.MedalsWon;
                    else
                        player.Score = 0;
                }

                // Create pager object
                var pager = new Pager<VolleyballPlayer>(totalCount, pageSize, page, volleyballPlayers);

                return View(pager);
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while processing your request, "+ex.Message, statusCode: 500);
            }
        }        // GET: VolleyballPlayersAPI/Details/5
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
