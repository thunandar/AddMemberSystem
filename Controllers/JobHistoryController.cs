using AddMemberSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AddMemberSystem.Controllers
{
    public class JobHistoryController : Controller
    {
        private readonly ILogger<JobHistoryController> _logger;
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public JobHistoryController(AppDBContext context, IWebHostEnvironment hostEnvironment)
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

        public IActionResult List(string staffID, int pg = 1)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            List<TB_JobHistory> result = _context.TB_JobHistorys
               .Where(data => data.IsDeleted == false).OrderByDescending(data => data.JobHistoryPkid).ToList();

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

        private List<SelectListItem> GetDepartments()
        {
            var result = new List<SelectListItem>();

            List<TB_Department> TB_Departments = _context.TB_Departments.Where(d => d.isDeleted == false).ToList();

            result = TB_Departments.Select(d => new SelectListItem()
            {
                Value = d.DepartmentPkid.ToString(),
                Text = d.Department ?? "Unknown Department"
            }).ToList();

            var defItem = new SelectListItem()
            {
                Value = "",
                Text = "----ဌာနအမျိုးအစား ရွေးချယ်ပါ----"
            };

            result.Insert(0, defItem);

            return result;
        }

        private List<SelectListItem> GetPositions()
        {
            var result = new List<SelectListItem>();

            List<TB_Position> TB_Positions =_context.TB_Positions.Where(d => d.IsDeleted == false).ToList();

            result = TB_Positions.Select(d => new SelectListItem()
            {
                Value = d.PositionPkid.ToString(),
                Text = d.Position ?? "Unknown Position"
            }).ToList();

            var defItem = new SelectListItem()
            {
                Value = "",
                Text = "----ရာထူးအမျိုးအစား ရွေးချယ်ပါ----"
            };

            result.Insert(0, defItem);

            return result;
        }

        private TB_Staff GetStaffInfoByStaffID(string staffID)
        {
            return _context.TB_Staffs.FirstOrDefault(s => s.StaffID == staffID);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_JobHistory result = new TB_JobHistory();
            ViewBag.DepartmentId = GetDepartments();
            ViewBag.PositionId = GetPositions();
            
            return View(result);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(TB_JobHistory JobHistory, string staffID)
        {
            JobHistory.IsDeleted = false;
            JobHistory.CreatedDate = DateTime.UtcNow;

            ViewBag.DepartmentId = GetDepartments();
            ViewBag.PositionId = GetPositions();

            if (JobHistory.IsCurrent)
            {
                bool alreadyHasCurrent = _context.TB_JobHistorys
                    .Any(jh => jh.StaffID == JobHistory.StaffID && jh.IsCurrent);
                if (alreadyHasCurrent)
                {
                    ModelState.AddModelError(
                        nameof(TB_JobHistory.IsCurrent),
                        "အဆိုပါအလုပ်သမား ID အတွက် 'isCurrent' ကို တစ်ကြိမ်သာ စစ်ဆေးနိုင်ပါသည်။"
                    );
                }
            }

            if (!ModelState.IsValid)
            {
                return View(JobHistory);
            }

            try
            {
                _context.Add(JobHistory);
                _context.SaveChanges();
                return RedirectToAction(nameof(List));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving the data.");
                return View(JobHistory);
            }
        }

        private TB_JobHistory GetJobHistory(int Id)
        {
            TB_JobHistory result = _context.TB_JobHistorys
                .Where(m => m.JobHistoryPkid == Id).FirstOrDefault();
            return result;
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_JobHistory result = GetJobHistory(id);

            ViewBag.DepartmentId = GetDepartments();
            ViewBag.PositionId = GetPositions();

            return View(result);
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TB_JobHistory editedData)
        {
            TB_JobHistory existingData = GetJobHistory(editedData.JobHistoryPkid);

            ViewBag.DepartmentId = GetDepartments();
            ViewBag.PositionId = GetPositions();

            if (editedData.IsCurrent)
            {
                bool otherCurrentExists = _context.TB_JobHistorys
                    .Any(jh =>
                        jh.StaffID == editedData.StaffID
                        && jh.IsCurrent
                        && jh.JobHistoryPkid != editedData.JobHistoryPkid
                    );
                if (otherCurrentExists)
                {
                    ModelState.AddModelError(
                        nameof(TB_JobHistory.IsCurrent),
                        "အဆိုပါအလုပ်သမား ID အတွက် 'isCurrent' ကို တစ်ကြိမ်သာ စစ်ဆေးနိုင်ပါသည်။"
                    );
                }
            }

            existingData.StaffID = editedData.StaffID;
            existingData.FromDate = editedData.FromDate;
            existingData.ToDate = editedData.ToDate;          
            existingData.JobYear = editedData.JobYear;          
            existingData.JobMonth = editedData.JobMonth;
            existingData.JobDay = editedData.JobDay;
            existingData.Duration = editedData.Duration;
            existingData.Remark = editedData.Remark;

            existingData.DepartmentId = editedData.DepartmentId;          
            existingData.PositionId = editedData.PositionId;
            existingData.CreatedDate = editedData.CreatedDate;
            existingData.IsCurrent = editedData.IsCurrent;

            if (!ModelState.IsValid)
            {
                return View(editedData);
            }

            try
            {
                _context.SaveChanges();
                return RedirectToAction(nameof(List));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving the data.");
                return View(editedData);
            }
        }

        private string GetDepartmentName(int DepartmentPkid)
        {
            string name = _context.TB_Departments.Where(p => p.DepartmentPkid == DepartmentPkid).SingleOrDefault().Department;
            return name;
        }

        private string GetPositionName(int PositionPkid)
        {
            string name = _context.TB_Positions.Where(p => p.PositionPkid == PositionPkid).SingleOrDefault().Position;
            return name;
        }

        [HttpGet]
        public IActionResult Details(int Id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_JobHistory result = GetJobHistory(Id);
            ViewBag.DepartmentId = GetDepartmentName(result.JobHistoryPkid);
            ViewBag.PositionId = GetPositionName(result.JobHistoryPkid);

            return View(result);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Delete(int Id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_JobHistory result = GetJobHistory(Id);
            result.IsDeleted = true;
            result.CreatedDate = DateTime.UtcNow;
            _context.Attach(result);
            _context.Entry(result).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction(nameof(List));
        }

    }
}
