using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Project2.Models;

namespace Project2.Pages.Movies
{
    [IgnoreAntiforgeryToken]
    public class SearchModel : PageModel
    {
        private PePrn222TrialContext _context;
        private IHubContext<ChatHub> _hubContext;

        public SearchModel(PePrn222TrialContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public List<Movie> Movies { get; set; } = new List<Movie>();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public void OnGet()
        {
            var query = _context.Movies
                .Include(p => p.Director)
                .Include(p => p.Producer)
                .Include(p => p.Stars)
                .Include(p => p.Genres)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                string searchTermLower = SearchTerm.ToLower();

                // Fetch all movies first, then filter in memory to avoid TEXT data type issues
                var allMovies = query.ToList();

                // Filter in memory
                Movies = allMovies.Where(m =>
                    // Search in Title
                    (m.Title != null && m.Title.ToLower().Contains(searchTermLower)) ||
                    // Search in Director name
                    (m.Director != null && m.Director.FullName.ToLower().Contains(searchTermLower)) ||
                    // Search in Description (this field is TEXT type)
                    (m.Description != null && m.Description.ToLower().Contains(searchTermLower)) ||
                    // Search in Genre title
                    m.Genres.Any(g => g.Title.ToLower().Contains(searchTermLower)) ||
                    // Search in Language
                    (m.Language != null && m.Language.ToLower().Contains(searchTermLower)) ||
                    // Search in Producer name
                    (m.Producer != null && m.Producer.Name.ToLower().Contains(searchTermLower)) ||
                    // Search in Star names
                    m.Stars.Any(s => s.FullName.ToLower().Contains(searchTermLower))
                ).ToList();
            }
            else
            {
                Movies = query.ToList();
            }
        }

        public IActionResult OnGetDelete(int movieId, string searchTerm)
        {
            var movie = _context.Movies
                .Include(p => p.Genres)
                .Include(p => p.Stars)
                .FirstOrDefault(p => p.Id == movieId);

            if (movie != null)
            {
                // Remove relationships before deleting the movie
                movie.Stars.Clear();
                movie.Genres.Clear();

                _context.Movies.Remove(movie);
                _context.SaveChanges();

                // Notify clients to update their view
                _hubContext.Clients.All.SendAsync("Update");
            }

            // Redirect back to the search page with the search term preserved
            return RedirectToPage(new { searchTerm });
        }
    }
}