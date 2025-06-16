namespace AddMemberSystem.Controllers
{
    public class SalaryController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public SalaryController(AppDBContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetString("loginUser") != null || Request.Cookies.ContainsKey("staySignedIn");
        }

        public IActionResult List(int pg = 1)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            List<TB_Salary> result = _context.TB_Salaries
               .Where(p => p.IsDeleted == false).OrderByDescending(s => s.SalaryPkid).ToList();

            const int pageSize = 20;
            if (pg < 1)
                pg = 1;

            int recsCount = result.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = result.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            return View(data);
        }
    }
}

