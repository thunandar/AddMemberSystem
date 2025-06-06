 using AddMemberSystem.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AddMemberSystem.Controllers
{
    public class StaffPunishmentController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public StaffPunishmentController(AppDBContext context, IWebHostEnvironment hostEnvironment)
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

            List<TB_StaffPunishment> staffLeaves = _context.TB_StaffPunishments
               .Where(staff => staff.IsDeleted == false).ToList();

            const int pageSize = 10;
            if (pg < 1)
                pg = 1;

            int recsCount = staffLeaves.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = staffLeaves.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            return View(data);
        }

        private List<SelectListItem> GetPunishmentTypes()
        {
            var lstPunishmentTypes = new List<SelectListItem>();

            List<TB_PunishmentType> TB_PunishmentTypes = _context.TB_PunishmentType.Where(d => d.IsDeleted == false).ToList();

            lstPunishmentTypes = TB_PunishmentTypes.Select(d => new SelectListItem()
            {
                Value = d.PunishmentTypePkid.ToString(),
                Text = d.Punishment ?? "Unknown Punishment"
            }).ToList();

            var defItem = new SelectListItem()
            {
                Value = "",
                Text = "----ပြစ်မှုအမျိုးအစား ရွေးချယ်ပါ----"
            };

            lstPunishmentTypes.Insert(0, defItem);

            return lstPunishmentTypes;
        }

        private TB_Staff GetStaffInfoByStaffID(string staffID)
        {
            var staff = _context.TB_Staffs
                .FirstOrDefault(s => s.StaffID == staffID);

            if (staff != null)
            {
                // Get current job history
                var currentJob = _context.TB_JobHistorys
                    .Where(j => j.StaffID == staffID && j.IsCurrent)
                    .Include(j => j.Department)
                    .Include(j => j.Position)
                    .FirstOrDefault();

                if (currentJob != null)
                {
                    staff.CurrentDepartment = currentJob.Department?.Department;
                    staff.CurrentPosition = currentJob.Position?.Position;
                }
            }

            return staff;
        }


        [HttpGet]
        public IActionResult Create(string staffID)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            var staff = _context.TB_Staffs.FirstOrDefault(s => s.StaffID == staffID);
            if (staff == null)
            {
                TempData["ErrorMessage"] = "The Staff ID does not exist.";
                return RedirectToAction("List");
            }

            TB_StaffPunishment StaffPunishment = new TB_StaffPunishment();

            var staffInfo = GetStaffInfoByStaffID(staffID);
            var staffName = _context.TB_Staffs.FirstOrDefault(s => s.StaffPkid == staffInfo.StaffPkid);

            ViewBag.StaffID = staffInfo.StaffID;
            ViewBag.StaffName = staffName.Name;
            ViewBag.PunishmentTypeId = GetPunishmentTypes();
            ViewBag.SelectedDepartment = staffInfo.CurrentDepartment ?? "N/A";
            ViewBag.SelectedPosition = staffInfo.CurrentPosition ?? "N/A";

            return View(StaffPunishment);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(TB_StaffPunishment StaffPunishment, string staffID)
        {
            StaffPunishment.IsDeleted = false;

            var staffInfo = GetStaffInfoByStaffID(staffID);
            StaffPunishment.StaffId = staffInfo.StaffPkid;


            if (!ModelState.IsValid)
            {
                return View(StaffPunishment);
            }

            try
            {
                _context.Add(StaffPunishment);
                _context.SaveChanges();
                return RedirectToAction(nameof(List));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving the data.");
                return View(StaffPunishment);
            }
        }

        private TB_StaffPunishment GetStaffPunishment(int Id)
        {
            TB_StaffPunishment staffPunish = _context.TB_StaffPunishments
                .Where(m => m.StaffPunishmentPkid == Id).FirstOrDefault();
            return staffPunish;


        }

        [HttpGet]
        public IActionResult Edit(int id, string staffID)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_StaffPunishment staffPunish = GetStaffPunishment(id);

            var staffInfo = _context.TB_Staffs.FirstOrDefault(s => s.StaffPkid == staffPunish.StaffId);

            ViewBag.StaffID = staffInfo.StaffID;

            ViewBag.StaffName = staffInfo.Name ?? "N/A";
            ViewBag.PunishmentTypeId = GetPunishmentTypes();

            var staffInfo2 = GetStaffInfoByStaffID(staffInfo.StaffID);
            ViewBag.SelectedDepartment = staffInfo2.CurrentDepartment ?? "N/A";
            ViewBag.SelectedPosition = staffInfo2.CurrentPosition ?? "N/A";
 

            return View(staffPunish);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TB_StaffPunishment editedStaffPunishment)
        {
            TB_StaffPunishment existingStaffPunishment = GetStaffPunishment(editedStaffPunishment.StaffPunishmentPkid);

            existingStaffPunishment.PunishmentDate = editedStaffPunishment.PunishmentDate;
            existingStaffPunishment.Punishment = editedStaffPunishment.Punishment;
            existingStaffPunishment.PunishmentTypeId = editedStaffPunishment.PunishmentTypeId;
            existingStaffPunishment.CreatedDate = editedStaffPunishment.CreatedDate;

            if (!ModelState.IsValid)
            {
                return View(editedStaffPunishment);
            }

            try
            {
                _context.SaveChanges();
                return RedirectToAction(nameof(List));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving the data.");
                return View(editedStaffPunishment);
            }
        }

        private string GetPunishmentTypeName(int PunishmentTypePkid)
        {
            string PunishmentTypeName = _context.TB_PunishmentType.Where(p => p.PunishmentTypePkid == PunishmentTypePkid).SingleOrDefault().Punishment;
            return PunishmentTypeName;
        }

        [HttpGet]
        public IActionResult Details(int Id, string staffID)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_StaffPunishment staffPunishment = GetStaffPunishment(Id);

            var staffInfo = _context.TB_Staffs.FirstOrDefault(s => s.StaffPkid == staffPunishment.StaffId);

            ViewBag.StaffID = staffInfo.StaffID;

            ViewBag.StaffName = staffInfo.Name;
            ViewBag.PunishmentTypeId = GetPunishmentTypes();
            ViewBag.PunishmentTypeId = GetPunishmentTypeName(staffPunishment.PunishmentTypeId);

            var staffInfo2 = GetStaffInfoByStaffID(staffInfo.StaffID);
            ViewBag.SelectedDepartment = staffInfo2.CurrentDepartment ?? "N/A";
            ViewBag.SelectedPosition = staffInfo2.CurrentPosition ?? "N/A";
            return View(staffPunishment);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Delete(int Id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_StaffPunishment staffPunishment = GetStaffPunishment(Id);
            staffPunishment.IsDeleted = true;
            staffPunishment.CreatedDate = DateTime.UtcNow;
            _context.Attach(staffPunishment);
            _context.Entry(staffPunishment).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction(nameof(List));
        }

    }
}
