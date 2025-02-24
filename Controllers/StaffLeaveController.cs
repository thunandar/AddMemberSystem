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

            const int pageSize = 5;
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

        private List<SelectListItem> GetDepartments()
        {
            var lstDepartments = new List<SelectListItem>();

            List<TB_Department> TB_Departments = _context.TB_Departments.Where(d => d.isDeleted == false).ToList();

            lstDepartments = TB_Departments.Select(d => new SelectListItem()
            {
                Value = d.DepartmentPkid.ToString(),
                Text = d.Department ?? "Unknown Department"
            }).ToList();

            var defItem = new SelectListItem()
            {
                Value = "",
                Text = "----ဌာနရွေးချယ်ပါ----"
            };

            lstDepartments.Insert(0, defItem);

            return lstDepartments;
        }

        private List<SelectListItem> GetPositions(int DepartmentPkid = 1)
        {

            List<SelectListItem> lstPositions = _context.TB_Positions
                .Where(d => d.DepartmentId == DepartmentPkid)
                .OrderBy(p => p.Position)
                .Select(p =>
                new SelectListItem
                {
                    Value = p.PositionPkid.ToString(),
                    Text = p.Position ?? "DefaultTextIfNull"
                }).ToList();

            var defItem = new SelectListItem()
            {
                Value = "",
                Text = "----ရာထူးရွေးချယ်ပါ----"
            };

            lstPositions.Insert(0, defItem);

            return lstPositions;
        }


        private string GetDepartmentName(int DepartmentPkid)
        {
            string departmentName = _context.TB_Departments.Where(d => d.DepartmentPkid == DepartmentPkid).SingleOrDefault().Department;
            return departmentName;
        }

        private string GetPositionName(int PositionPkid)
        {
            string positionName = _context.TB_Positions.Where(p => p.PositionPkid == PositionPkid).SingleOrDefault().Position;
            return positionName;
        }

        private string GetLeaveTypeName(int LeaveTypePkid)
        {
            string LeaveTypeName = _context.TB_LeaveTypes.Where(p => p.LeaveTypePkid == LeaveTypePkid).SingleOrDefault().LeaveTypeName;
            return LeaveTypeName;
        }

        [HttpGet]
        public JsonResult GetPositionsByDepartment(int DepartmentPkid)
        {
            List<SelectListItem> positions = GetPositions(DepartmentPkid);
            return Json(positions);
        }

        private TB_Staff GetStaffInfoByStaffID(string staffID)
        {
            return _context.TB_Staffs.FirstOrDefault(s => s.StaffID == staffID);
        }

        private string GetSelectedDepartment(int selectedDepartmentId)
        {
            var department = _context.TB_Departments
                .Where(d => d.DepartmentPkid == selectedDepartmentId && !d.isDeleted)
                .Select(d => d.Department)
                .FirstOrDefault();

            return department ?? "Unknown Department";
        }

        private string GetSelectedPosition(int selectedPositionId)
        {
            var position = _context.TB_Positions
                .Where(d => d.PositionPkid == selectedPositionId)
                .Select(d => d.Position)
                .FirstOrDefault();
            return position ?? "Unknown Position";
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
            int totalLeaveDays = 36;

            var staffLeaveRecords = _context.TB_StaffLeaves.Where(s => s.StaffID == staffID).Where(s => s.IsDeleted == false).ToList();
            int takenLeaveDays = staffLeaveRecords.Sum(s => s.LeaveDays);
            int remainingLeaveDays = totalLeaveDays - takenLeaveDays;

            var staffInfo = GetStaffInfoByStaffID(staffID);

            var staffName = _context.TB_Staffs.FirstOrDefault(s => s.StaffPkid == staffInfo.StaffPkid);
            ViewBag.StaffName = staffName.Name;

            ViewBag.SelectedDepartment = GetSelectedDepartment(staffInfo.DepartmentId);

            // ViewBag.SelectedPosition = GetSelectedPosition(staffInfo.PositionId);
            ViewBag.SelectedPosition = staffInfo.PositionId.HasValue
        ? GetSelectedPosition(staffInfo.PositionId.Value)
        : "Position Not Assigned";

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


            // Server-side validation for LeaveDateFrom and LeaveDateTo
            if (!staffL.LeaveDateFrom.HasValue || staffL.LeaveDateFrom.Value.Date < today)
            {
                ModelState.AddModelError(nameof(TB_StaffLeave.LeaveDateFrom), "ခွင့်ယူသည့်ရက်မှ အနည်းဆုံး ယနေ့မှစရပါမည်။");
            }

            if (!staffL.LeaveDateTo.HasValue || staffL.LeaveDateTo.Value.Date < today)
            {
                ModelState.AddModelError(nameof(TB_StaffLeave.LeaveDateTo), "ခွင့်ယူသည့်ရက်ထိ အနည်းဆုံး ယနေ့မှစရပါမည်။");
            }

            // Ensure LeaveDateTo is after or equal to LeaveDateFrom
            if (staffL.LeaveDateFrom.HasValue && staffL.LeaveDateTo.HasValue && staffL.LeaveDateTo < staffL.LeaveDateFrom)
            {
                ModelState.AddModelError(nameof(TB_StaffLeave.LeaveDateTo), "ခွင့်ယူသည့်ရက်ထိ သည် ခွင့်ယူသည့်ရက်မှ နှင့် အချိန်ကျပါမည်။");
            }

            if (ModelState.ErrorCount > 0)
            {
                SetViewDataAndViewBag(staffID, 36, 0, 36);
                return View(staffL);
            }

            staffL.LeaveDays = (staffL.LeaveDateTo.Value - staffL.LeaveDateFrom.Value).Days + 1;

            int totalLeaveDays = 36;
            var staffLeaveRecords = _context.TB_StaffLeaves.Where(s => s.StaffID == staffID).Where(s => s.IsDeleted == false).ToList();
            int takenLeaveDays = staffLeaveRecords.Sum(s => s.LeaveDays);
            int isValidLeaveDays = takenLeaveDays + staffL.LeaveDays;
            int remainingLeaveDays = totalLeaveDays - takenLeaveDays;

            var staffInfo = GetStaffInfoByStaffID(staffID);

            var Department = _context.TB_Departments.FirstOrDefault(d => d.DepartmentPkid == staffInfo.DepartmentId);
            staffL.DepartmentId = Department.DepartmentPkid;

            var Position = _context.TB_Positions.FirstOrDefault(d => d.PositionPkid == staffInfo.PositionId);
            staffL.PositionId = Position?.PositionPkid;


            if (isValidLeaveDays > totalLeaveDays)
            {
                ModelState.AddModelError(nameof(TB_StaffLeave.LeaveDays), "ခွင့်ပေးထားသည့်ရက်၃၆ရက်ထက် ခွင့်ယူထားသည့်ရက်များက ကျော်လွန်နေပါသည်");
                SetViewDataAndViewBag(staffID, totalLeaveDays, takenLeaveDays, remainingLeaveDays);
                return View(staffL);
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
            ViewBag.DepartmentPkid = GetDepartments();
            ViewBag.PositionPkid = GetPositions();
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
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

           TB_StaffLeave staffL = GetStaffLeave(id);

            var staffInfo = _context.TB_Staffs.FirstOrDefault(s => s.StaffID == staffL.StaffID); 

            int totalLeaveDays = 36;

            var staffLeaveRecords = _context.TB_StaffLeaves.Where(s => s.StaffID == staffL.StaffID).Where(s => s.IsDeleted == false).ToList();

            int takenLeaveDays = staffLeaveRecords.Sum(s => s.LeaveDays);
            int remainingLeaveDays = totalLeaveDays - takenLeaveDays;

            ViewBag.StaffID = staffInfo.StaffID;

            ViewBag.StaffName = staffInfo.Name;

            ViewBag.SelectedDepartment = GetSelectedDepartment(staffInfo.DepartmentId);
            // ViewBag.SelectedPosition = GetSelectedPosition(staffInfo.PositionId);
            ViewBag.SelectedPosition = staffInfo.PositionId.HasValue
        ? GetSelectedPosition(staffInfo.PositionId.Value)
        : "Position Not Assigned";
            ViewBag.TotalLeaveDays = totalLeaveDays;
            ViewBag.TakenLeaveDays = takenLeaveDays;
            ViewBag.RemaningLeaveDays = remainingLeaveDays;

            ViewBag.DepartmentPkid = GetDepartments();
            ViewBag.PositionPkid = GetPositions(staffL.DepartmentId);
            ViewBag.LeaveTypeId = GetLeaveTypes();
            return View(staffL);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TB_StaffLeave editedStaffL)
        {
            TB_StaffLeave existingStaffL = GetStaffLeave(editedStaffL.StaffLeavePkid);

            int totalLeaveDays = 36;
            var staffLeaveRecords = _context.TB_StaffLeaves.Where(s => s.StaffID == existingStaffL.StaffID).Where(s => s.IsDeleted == false).ToList();

            int takenLeaveDays = staffLeaveRecords.Sum(s => s.LeaveDays);
            int remainingLeaveDays = totalLeaveDays - takenLeaveDays;

            DateTime today = DateTime.UtcNow.Date;

            // Server-side validation for LeaveDateFrom and LeaveDateTo
            if (!editedStaffL.LeaveDateFrom.HasValue || editedStaffL.LeaveDateFrom.Value.Date < today)
            {
                ModelState.AddModelError(nameof(TB_StaffLeave.LeaveDateFrom), "ခွင့်ယူသည့်ရက်မှ အနည်းဆုံး ယနေ့မှစရပါမည်။");
            }

            if (!editedStaffL.LeaveDateTo.HasValue || editedStaffL.LeaveDateTo.Value.Date < today)
            {
                ModelState.AddModelError(nameof(TB_StaffLeave.LeaveDateTo), "ခွင့်ယူသည့်ရက်ထိ အနည်းဆုံး ယနေ့မှစရပါမည်။");
            }

            // Ensure LeaveDateTo is after or equal to LeaveDateFrom
            if (editedStaffL.LeaveDateFrom.HasValue && editedStaffL.LeaveDateTo.HasValue && editedStaffL.LeaveDateTo < editedStaffL.LeaveDateFrom)
            {
                ModelState.AddModelError(nameof(TB_StaffLeave.LeaveDateTo), "ခွင့်ယူသည့်ရက်ထိ သည် ခွင့်ယူသည့်ရက်မှ နှင့် အချိန်ကျပါမည်။");
            }

            if (!ModelState.IsValid)
            {
                SetViewDataAndViewBag(totalLeaveDays, takenLeaveDays, remainingLeaveDays);
                return View(editedStaffL);
            }

            // Calculate LeaveDays dynamically
            editedStaffL.LeaveDays = (editedStaffL.LeaveDateTo.Value - editedStaffL.LeaveDateFrom.Value).Days + 1;


            int isValidLeaveDays = takenLeaveDays + editedStaffL.LeaveDays;
            if (isValidLeaveDays > totalLeaveDays)
            {
                ModelState.AddModelError(nameof(TB_StaffLeave.LeaveDays), "ခွင့်ပေးထားသည့်ရက် ၃၆ ရက်ထက် ကျော်လွန်နေပါသည်");
                SetViewDataAndViewBag(totalLeaveDays, takenLeaveDays, remainingLeaveDays);
                return View(editedStaffL);
            }


            ViewBag.TotalLeaveDays = totalLeaveDays;
            ViewBag.TakenLeaveDays = takenLeaveDays;
            ViewBag.RemaningLeaveDays = remainingLeaveDays;
            ViewBag.DepartmentPkid = GetDepartments();
            ViewBag.PositionPkid = GetPositions();
            ViewBag.LeaveTypeId = GetLeaveTypes();

            existingStaffL.StaffID = editedStaffL.StaffID;
            existingStaffL.StaffLeaveName = editedStaffL.StaffLeaveName;
            existingStaffL.DepartmentId = editedStaffL.DepartmentId;
            existingStaffL.PositionId = editedStaffL.PositionId;
            existingStaffL.CreatedDate = editedStaffL.CreatedDate;
            existingStaffL.LeaveDateFrom = editedStaffL.LeaveDateFrom;
            existingStaffL.LeaveDateTo = editedStaffL.LeaveDateTo;
            existingStaffL.LeaveDays = editedStaffL.LeaveDays;
            existingStaffL.LeaveAddress = editedStaffL.LeaveAddress;
            existingStaffL.DutyAssignedTo = editedStaffL.DutyAssignedTo;
            existingStaffL.DutyAssignPosition = editedStaffL.DutyAssignPosition;
            existingStaffL.LeaveTypeId = editedStaffL.LeaveTypeId;

            if (!ModelState.IsValid)
            {
                return View(editedStaffL);
            }

            try
            {
                _context.SaveChanges();
                return RedirectToAction(nameof(List));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving the data.");
                return View(editedStaffL);
            }
        }

        private void SetViewDataAndViewBag(int totalLeaveDays, int takenLeaveDays, int remainingLeaveDays)
        {
            ViewBag.TotalLeaveDays = totalLeaveDays;
            ViewBag.TakenLeaveDays = takenLeaveDays;
            ViewBag.RemaningLeaveDays = remainingLeaveDays;
            ViewBag.DepartmentPkid = GetDepartments();
            ViewBag.PositionPkid = GetPositions();
            ViewBag.LeaveTypeId = GetLeaveTypes();
        }


        [HttpGet]
        public IActionResult Details(int Id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_StaffLeave staffL = GetStaffLeave(Id);

            var staffInfo = _context.TB_Staffs.FirstOrDefault(s => s.StaffID == staffL.StaffID);


            ViewBag.StaffName = staffInfo.Name;

            ViewBag.Department = GetDepartmentName(staffL.DepartmentId);
           // ViewBag.Position = GetPositionName(staffL.PositionId);
            ViewBag.Position = staffL.PositionId.HasValue ? GetPositionName(staffL.PositionId.Value): "Position Not Assigned";
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
            _context.Attach(staffL);
            _context.Entry(staffL).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction(nameof(List));
        }

    }
}
