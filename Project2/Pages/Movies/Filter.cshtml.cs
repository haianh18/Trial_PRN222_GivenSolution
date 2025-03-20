using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Project2.Models;

namespace Project2.Pages.Movies
{
    [IgnoreAntiforgeryToken]
    public class FilterModel : PageModel
    {
        private PePrn222TrialContext _context;
        private IHubContext<ChatHub> _hubContext;

        public FilterModel(PePrn222TrialContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public List<Movie> Movies { get; set; } = new List<Movie>();
        public List<Director> Directors { get; set; } = new List<Director>();
        public List<Star> Stars { get; set; } = new List<Star>();
        public List<Genre> Genres { get; set; } = new List<Genre>();

        [BindProperty(SupportsGet = true)]
        public int? DirectorId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? StarId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? GenreId { get; set; }

        public void OnGet()
        {
            // Load filter options
            Directors = _context.Directors.OrderBy(d => d.FullName).ToList();
            Stars = _context.Stars.OrderBy(s => s.FullName).ToList();
            Genres = _context.Genres.OrderBy(g => g.Title).ToList();

            // Create base query
            var query = _context.Movies
                .Include(m => m.Director)
                .Include(m => m.Producer)
                .Include(m => m.Stars)
                .Include(m => m.Genres)
                .AsQueryable();

            // Apply filters if set
            if (DirectorId.HasValue)
            {
                query = query.Where(m => m.DirectorId == DirectorId);
            }

            if (StarId.HasValue)
            {
                query = query.Where(m => m.Stars.Any(s => s.Id == StarId));
            }

            if (GenreId.HasValue)
            {
                query = query.Where(m => m.Genres.Any(g => g.Id == GenreId));
            }

            Movies = query.ToList();
        }

        public IActionResult OnGetDelete(int movieId, int? directorId, int? starId, int? genreId)
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

            // Redirect back to the filter page with all filters preserved
            return RedirectToPage(new { directorId, starId, genreId });
        }
    }
}