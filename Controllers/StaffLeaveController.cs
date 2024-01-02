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
            return HttpContext.Session.GetString("loginUser") != null;
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

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_StaffLeave StaffL = new TB_StaffLeave();
            ViewBag.DepartmentPkid = GetDepartments();
            ViewBag.PositionPkid = GetPositions();
            ViewBag.LeaveTypeId = GetLeaveTypes();
            return View(StaffL);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(TB_StaffLeave staffL)
        {
            
            staffL.CreatedDate = DateTime.UtcNow;
            staffL.IsDeleted = false;

            if (staffL.LeaveDays > 0 && staffL.LeaveDateFrom.HasValue && staffL.LeaveDateTo.HasValue)
            {
                var leaveFromDate = staffL.LeaveDateFrom.Value;
                var leaveToDate = staffL.LeaveDateTo.Value;

                var dateDifference = leaveToDate - leaveFromDate;

                if (dateDifference.Days != staffL.LeaveDays - 1)
                {
                    ModelState.AddModelError(nameof(TB_StaffLeave.LeaveDays), $"The difference between LeaveDateFrom and LeaveDateTo should be {staffL.LeaveDays} days.");
                }
            }

            _context.Add(staffL);
            _context.SaveChanges();
            return RedirectToAction(nameof(List));
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

            existingStaffL.SerialNo = editedStaffL.SerialNo;
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

            if (existingStaffL.LeaveDays > 0 && existingStaffL.LeaveDateFrom.HasValue && existingStaffL.LeaveDateTo.HasValue)
            {
                var leaveFromDate = existingStaffL.LeaveDateFrom.Value;
                var leaveToDate = existingStaffL.LeaveDateTo.Value;

                var dateDifference = leaveToDate - leaveFromDate;

                if (dateDifference.Days != existingStaffL.LeaveDays - 1)
                {
                    Console.WriteLine("Edit ERR");
                    ModelState.AddModelError(nameof(TB_StaffLeave.LeaveDays), $"The difference between LeaveDateFrom and LeaveDateTo should be {existingStaffL.LeaveDays} days.");
                }
            }

            _context.SaveChanges();


            return RedirectToAction(nameof(List));

        }

        [HttpGet]
        public IActionResult Details(int Id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_StaffLeave staffL = GetStaffLeave(Id);
            ViewBag.Department = GetDepartmentName(staffL.DepartmentId);
            ViewBag.Position = GetPositionName(staffL.PositionId);
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
