using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Project2.Models;

namespace Project2.Pages.Movies
{

    [IgnoreAntiforgeryToken]
    public class Director_MovieModel : PageModel
    {
        private PePrn222TrialContext _context;
        private IHubContext<ChatHub> _hubContext;

        public Director_MovieModel(PePrn222TrialContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }



        public List<Movie> Movies { get; set; } = new List<Movie>();
        public List<Director> Directors { get; set; } = new List<Director>();

        public int? SelectedDirectorId { get; set; }

        // Thay đổi tham số từ direcId thành id để phù hợp với route
        public void OnGet(int? id)
        {
            Directors = _context.Directors.ToList();
            Movies = GetMoviesByDirector(id);
            SelectedDirectorId = id;
        }

        public List<Movie> GetMoviesByDirector(int? direcId)
        {
            var query = _context.Movies.Include(p => p.Director).Include(p => p.Producer)
                .Include(p => p.Stars).AsQueryable();
            if (direcId.HasValue && direcId.Value > 0)
            {
                query = query.Where(p => p.DirectorId == direcId.Value);
            }
            return query.ToList();
        }

        // Xử lý việc xóa phim
        public IActionResult OnGetDelete(int movieId, int? directorId)
        {
            var movie = _context.Movies
                .Include(p => p.Genres)
                .Include(p => p.Stars).FirstOrDefault(p => p.Id == movieId);
            if (movie != null)
            {
                movie.Stars.Clear();
                movie.Genres.Clear();
                _context.Movies.Remove(movie);
                _context.SaveChanges();
                _hubContext.Clients.All.SendAsync("Update");
            }
            return RedirectToPage(new { id = directorId });
        }
    }
}