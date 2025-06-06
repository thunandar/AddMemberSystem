using AddMemberSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace AddMemberSystem.Controllers
{
    public class StaffLeaveController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public StaffLeaveController(AppDBContext context, IWebHostEnvironment hostEnvironment)
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

            List<TB_StaffLeave> staffLeaves = _context.TB_StaffLeaves
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

        private List<SelectListItem> GetLeaveTypes()
        {
            var lstLeaveTypes = new List<SelectListItem>();

            List<TB_LeaveType> LeaveTypes = _context.TB_LeaveTypes.ToList();

            lstLeaveTypes = LeaveTypes.Select(d => new SelectListItem()
            {
                Value = d.LeaveTypePkid.ToString(),
                Text = d.LeaveTypeName
            }).ToList();

            var defItem = new SelectListItem()
            {
                Value = "",
                Text = "----ခွင့်အမျိုးအစား ရွေးချယ်ပါ----"
            };

            lstLeaveTypes.Insert(0, defItem);

            return lstLeaveTypes;
        }
 
        private string GetLeaveTypeName(int LeaveTypePkid)
        {
            string LeaveTypeName = _context.TB_LeaveTypes.Where(p => p.LeaveTypePkid == LeaveTypePkid).SingleOrDefault().LeaveTypeName;
            return LeaveTypeName;
        }
  

        private TB_Staff GetStaffInfoByStaffID(string staffID)
        {
            return _context.TB_Staffs.FirstOrDefault(s => s.StaffID == staffID);
        }
 
        [HttpGet]
        public JsonResult GetTakenLeaveDays(string staffID, int leaveTypeId)
        {
            var records = _context.TB_StaffLeaves
                .Where(s => s.StaffID == staffID
                    && s.LeaveTypeId == leaveTypeId
                    && s.IsDeleted == false)
                .ToList();

            return Json(new
            {
                takenLeaveDays = records.Sum(s => s.LeaveDays)
            });
        }

        private TB_Staff GetStaffInfo2ByStaffID(string staffID)
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

            var staffLeaveModel = new TB_StaffLeave();
            var leaveTypes = _context.TB_LeaveTypes.ToList();
            ViewBag.LeaveTypes = leaveTypes;

            int totalLeaveDays = 0;
            int takenLeaveDays = 0;
            int remainingLeaveDays = 0;

            if (leaveTypes.Any())
            {
                var defaultLeaveType = leaveTypes.First();
                totalLeaveDays = defaultLeaveType.LeaveDays;

                // Filter by leave type
                var staffLeaveRecords = _context.TB_StaffLeaves
                         .Where(s => s.StaffID == staffID
                             && s.LeaveTypeId == defaultLeaveType.LeaveTypePkid
                             && s.IsDeleted == false)
                         .ToList();

                takenLeaveDays = staffLeaveRecords.Sum(s => s.LeaveDays);
                remainingLeaveDays = totalLeaveDays - takenLeaveDays;
            }

            var staffInfo = GetStaffInfoByStaffID(staffID);

            var staffName = _context.TB_Staffs.FirstOrDefault(s => s.StaffPkid == staffInfo.StaffPkid);
            ViewBag.StaffName = staffName.Name;

            var staffInfo2 = GetStaffInfo2ByStaffID(staffID);
            ViewBag.SelectedDepartment = staffInfo2.CurrentDepartment ?? "N/A";
            ViewBag.SelectedPosition = staffInfo2.CurrentPosition ?? "N/A";


            SetViewDataAndViewBag(staffID, totalLeaveDays, takenLeaveDays, remainingLeaveDays);

            return View(staffLeaveModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(TB_StaffLeave staffL, string staffID)
        {

            staffL.CreatedDate = DateTime.UtcNow;
            staffL.IsDeleted = false;

            DateTime today = DateTime.UtcNow.Date;

            var staffInfo2 = GetStaffInfo2ByStaffID(staffID);
            ViewBag.SelectedDepartment = staffInfo2.CurrentDepartment ?? "N/A";
            ViewBag.SelectedPosition = staffInfo2.CurrentPosition ?? "N/A";

            var staffInfoFirst = GetStaffInfoByStaffID(staffID);

            var staffName = _context.TB_Staffs.FirstOrDefault(s => s.StaffPkid == staffInfoFirst.StaffPkid);

            ViewBag.LeaveTypes = _context.TB_LeaveTypes.ToList();

            // Server-side validation for LeaveDateFrom and LeaveDateTo
            if (!staffL.LeaveDateFrom.HasValue || staffL.LeaveDateFrom.Value.Date < today)
            {
                ViewBag.StaffName = staffName.Name;
 
                ModelState.AddModelError(nameof(TB_StaffLeave.LeaveDateFrom), "ခွင့်ယူသည့်ရက်မှ အနည်းဆုံး ယနေ့မှစရပါမည်။");
            }

            // Ensure LeaveDateTo is after or equal to LeaveDateFrom
            if (staffL.LeaveDateFrom.HasValue && staffL.LeaveDateTo.HasValue && staffL.LeaveDateTo < staffL.LeaveDateFrom)
            {
                ViewBag.StaffName = staffName.Name;
  

                ModelState.AddModelError(nameof(TB_StaffLeave.LeaveDateTo), "ခွင့်ယူသည့်ရက်ထိ သည် ခွင့်ယူသည့်ရက်မှ နှင့် အချိန်ကျပါမည်။");
            }


            var selectedLeaveType = _context.TB_LeaveTypes.Find(staffL.LeaveTypeId);
            if (selectedLeaveType == null)
            {
                ModelState.AddModelError("LeaveTypeId", "Invalid leave type selected.");
                ViewBag.LeaveTypes = _context.TB_LeaveTypes.ToList();
                return View(staffL);
            }
            int totalLeaveDays = selectedLeaveType.LeaveDays;

            var staffLeaveRecords = _context.TB_StaffLeaves
                   .Where(s => s.StaffID == staffID
                       && s.LeaveTypeId == staffL.LeaveTypeId
                       && s.IsDeleted == false)
                   .ToList();

            int takenLeaveDays = staffLeaveRecords.Sum(s => s.LeaveDays);
            int isValidLeaveDays = takenLeaveDays + staffL.LeaveDays;
            int remainingLeaveDays = totalLeaveDays - takenLeaveDays;

            var staffInfo = GetStaffInfoByStaffID(staffID);

            //var Department = _context.TB_Departments.FirstOrDefault(d => d.DepartmentPkid == staffInfo.DepartmentId);
            //staffL.DepartmentId = Department.DepartmentPkid;

            //var Position = _context.TB_Positions.FirstOrDefault(d => d.PositionPkid == staffInfo.PositionId);
            //staffL.PositionId = Position?.PositionPkid;

            if (isValidLeaveDays > totalLeaveDays)
            {

                ViewBag.LeaveTypes = _context.TB_LeaveTypes.ToList();
                SetViewDataAndViewBag(staffID, totalLeaveDays, takenLeaveDays, remainingLeaveDays);
                ModelState.AddModelError(nameof(TB_StaffLeave.LeaveTypeId), "ခွင့်ပေးထားသည့်ရက်ထက် ခွင့်ယူထားသည့်ရက်များက ကျော်လွန်နေပါသည်");
            }


            SetViewDataAndViewBag(staffID, totalLeaveDays, takenLeaveDays, remainingLeaveDays);

            if (!ModelState.IsValid)
            {
                return View(staffL);
            }

            try
            {
                _context.Add(staffL);
                _context.SaveChanges();
                return RedirectToAction(nameof(List));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving the data.");
                return View(staffL);
            }
        }

        private void SetViewDataAndViewBag(string staffID, int totalLeaveDays, int takenLeaveDays, int remainingLeaveDays)
        {
            ViewData["StaffID"] = staffID;
            ViewBag.TotalLeaveDays = totalLeaveDays;
            ViewBag.TakenLeaveDays = takenLeaveDays;
            ViewBag.RemaningLeaveDays = remainingLeaveDays;
  
            ViewBag.LeaveTypeId = GetLeaveTypes();
        }

        private TB_StaffLeave GetStaffLeave(int Id)
        {
            TB_StaffLeave staffL = _context.TB_StaffLeaves
                .Where(m => m.StaffLeavePkid == Id).FirstOrDefault();
            return staffL;
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsUserLoggedIn()) return RedirectToAction("Index", "Account");

            var staffLeave = _context.TB_StaffLeaves
                .Include(s => s.LeaveType)
                .FirstOrDefault(s => s.StaffLeavePkid == id);

            if (staffLeave == null) return NotFound();

            var staffInfo = _context.TB_Staffs.FirstOrDefault(s => s.StaffID == staffLeave.StaffID);
            if (staffInfo == null) return NotFound();
 
            ViewBag.StaffName = staffInfo.Name ?? "N/A";

            var leaveTypes = _context.TB_LeaveTypes.ToList();
            var currentLeaveType = leaveTypes.FirstOrDefault(lt => lt.LeaveTypePkid == staffLeave.LeaveTypeId);

            ViewBag.LeaveTypes = leaveTypes;
            ViewBag.SelectedLeaveTypeId = staffLeave.LeaveTypeId;
            ViewBag.InitialTotalDays = currentLeaveType?.LeaveDays ?? 0;

            var query = _context.TB_StaffLeaves
        .Where(s => s.StaffID == staffLeave.StaffID
                 && s.LeaveTypeId == staffLeave.LeaveTypeId
                 && s.IsDeleted == false
                );

            var InitialTakenDays = query.Sum(s => (int?)s.LeaveDays) ?? 0;
            ViewBag.InitialTakenDays = InitialTakenDays;

            var staffInfo2 = GetStaffInfo2ByStaffID(staffInfo.StaffID);
            ViewBag.SelectedDepartment = staffInfo2.CurrentDepartment ?? "N/A";
            ViewBag.SelectedPosition = staffInfo2.CurrentPosition ?? "N/A";


            return View(staffLeave);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TB_StaffLeave editedStaffL)
        {
            var existingStaffL = _context.TB_StaffLeaves
                .FirstOrDefault(s => s.StaffLeavePkid == editedStaffL.StaffLeavePkid);

            var staffInfo = _context.TB_Staffs.FirstOrDefault(s => s.StaffID == editedStaffL.StaffID);
            if (staffInfo == null) return NotFound();
            ViewBag.StaffID = staffInfo.StaffID;
            ViewBag.StaffName = staffInfo.Name ?? "N/A";

            var staffInfo2 = GetStaffInfo2ByStaffID(editedStaffL.StaffID);
            ViewBag.SelectedDepartment = staffInfo2.CurrentDepartment ?? "N/A";
            ViewBag.SelectedPosition = staffInfo2.CurrentPosition ?? "N/A";

            if (existingStaffL == null) return NotFound();

            // Populate essential ViewBag data
            ViewBag.LeaveTypes = _context.TB_LeaveTypes.ToList();
 
            // Validation logic
            var leaveType = _context.TB_LeaveTypes.Find(editedStaffL.LeaveTypeId)
                ?? throw new ArgumentException("Invalid leave type");

            // Date validations
            var today = DateTime.UtcNow.Date;
            if (editedStaffL.LeaveDateFrom?.Date < today)
                ModelState.AddModelError("LeaveDateFrom", "ခွင့်ယူသည့်ရက်မှ အနည်းဆုံး ယနေ့မှစရပါမည်။");

            if (editedStaffL.LeaveDateTo < editedStaffL.LeaveDateFrom)
                ModelState.AddModelError("LeaveDateTo", "ခွင့်ယူသည့်ရက်ထိ သည် ခွင့်ယူသည့်ရက်မှ နှင့် အချိန်ကျပါမည်။");

            // Calculate leave days
            editedStaffL.LeaveDays = (editedStaffL.LeaveDateTo.Value - editedStaffL.LeaveDateFrom.Value).Days + 1;

            // Taken days calculation (exclude current record)
            var takenDays = _context.TB_StaffLeaves
                .Where(s => s.StaffID == existingStaffL.StaffID
                         && s.LeaveTypeId == editedStaffL.LeaveTypeId
                          && s.IsDeleted == false)
                .Sum(s => s.LeaveDays);

            var remainingDays = leaveType.LeaveDays - takenDays;

            // Consistency with Create page validation
            if (editedStaffL.LeaveDays > remainingDays)
            {
                ModelState.AddModelError(
                    nameof(TB_StaffLeave.LeaveTypeId),
                    "ခွင့်ပေးထားသည့်ရက်ထက် ခွင့်ယူထားသည့်ရက်များက ကျော်လွန်နေပါသည်"
                );
            }

            if (!ModelState.IsValid)
            {
                // Maintain view state consistency
                ViewBag.TotalLeaveDays = leaveType.LeaveDays;
                ViewBag.TakenLeaveDays = takenDays;
                ViewBag.RemainingLeaveDays = remainingDays;
                return View(editedStaffL);
            }

            // Update record
            existingStaffL.LeaveTypeId = editedStaffL.LeaveTypeId;
            existingStaffL.LeaveDateFrom = editedStaffL.LeaveDateFrom;
            existingStaffL.LeaveDateTo = editedStaffL.LeaveDateTo;
            existingStaffL.LeaveDays = editedStaffL.LeaveDays;
            existingStaffL.LeaveAddress = editedStaffL.LeaveAddress;
            existingStaffL.DutyAssignedTo = editedStaffL.DutyAssignedTo;
            existingStaffL.DutyAssignPosition = editedStaffL.DutyAssignPosition;

            try
            {
                _context.SaveChanges();
                return RedirectToAction(nameof(List));
            }
            catch
            {
                ModelState.AddModelError("", "အချက်အလက်များသိမ်းဆည်းရာတွင်အမှားတစ်ခုဖြစ်ပေါ်ခဲ့သည်။ ကျေးဇူးပြု၍ ထပ်ကြိုးစားပါ။");
                return View(editedStaffL);
            }
        }

        private void SetViewDataAndViewBag(int totalLeaveDays, int takenLeaveDays, int remainingLeaveDays)
        {
            ViewBag.TotalLeaveDays = totalLeaveDays;
            ViewBag.TakenLeaveDays = takenLeaveDays;
            ViewBag.RemaningLeaveDays = remainingLeaveDays;
 
            ViewBag.LeaveTypeId = GetLeaveTypes();
        }


        [HttpGet]
        public IActionResult Details(int Id, string staffID)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_StaffLeave staffL = GetStaffLeave(Id);

            var staffInfo = _context.TB_Staffs.FirstOrDefault(s => s.StaffID == staffL.StaffID);


            ViewBag.StaffName = staffInfo.Name;

            var staffInfo2 = GetStaffInfo2ByStaffID(staffInfo.StaffID);
            ViewBag.SelectedDepartment = staffInfo2.CurrentDepartment ?? "N/A";
            ViewBag.SelectedPosition = staffInfo2.CurrentPosition ?? "N/A";

            ViewBag.LeaveTypeId = GetLeaveTypeName(staffL.LeaveTypeId);
            return View(staffL);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Delete(int Id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_StaffLeave staffL = GetStaffLeave(Id);
            staffL.IsDeleted = true;
            staffL.CreatedDate = DateTime.UtcNow;
            _context.Attach(staffL);
            _context.Entry(staffL).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction(nameof(List));
        }

    }
}
