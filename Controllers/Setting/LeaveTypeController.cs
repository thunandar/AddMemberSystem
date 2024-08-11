
using AddMemberSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.Intrinsics.Arm;

namespace AddMemberSystem.Controllers.Setting
{
    public class LeaveTypeController : Controller
    {
        private readonly AppDBContext _context;

        public LeaveTypeController(AppDBContext context)
        {
            _context = context;
        }
        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetString("loginUser") != null || Request.Cookies.ContainsKey("staySignedIn");
        }

        public IActionResult Index()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            List<TB_LeaveType> leaveTypes = _context.TB_LeaveTypes.ToList();

            SelectList leaveTypeList = new SelectList(leaveTypes, "LeaveTypePkid", "LeaveTypeName");

            ViewBag.LeaveTypeId = leaveTypeList;

            return View("~/Views/Setting/LeaveType/LeaveTypeCrud.cshtml");
        }

        [HttpGet]

        public IActionResult GetAllLeaveTypes(int? leaveTypeId)
        {
            if (leaveTypeId.HasValue)
            {
                var leaveType = _context.TB_LeaveTypes.Find(leaveTypeId.Value);

                if (leaveType != null)
                {
                    return Json(new { leaveType = leaveType.LeaveTypeName });
                }
            }

            var leaveTypes = _context.TB_LeaveTypes.ToList();
            return Json(leaveTypes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TB_LeaveType lt, string actionType)
        {
            if (ModelState.IsValid)
            {
                if (actionType == "Create")
                {
                    if (_context.TB_LeaveTypes.Any(d => d.LeaveTypeName == lt.LeaveTypeName))
                    {
                        ModelState.AddModelError("LeaveTypeName", "LeaveType with this name already exists.");
                        return View("~/Views/Setting/LeaveType/LeaveTypeCrud.cshtml", lt);
                    }

                    _context.TB_LeaveTypes.Add(lt);
                }
                else if (actionType == "Edit")
                {
                    if (_context.TB_LeaveTypes.Any(d => d.LeaveTypeName == lt.LeaveTypeName))
                    {
                        ModelState.AddModelError("LeaveTypeName", "Edit LeaveType with this name already exists.");
                        return View("~/Views/Setting/LeaveType/LeaveTypeCrud.cshtml", lt);
                    }

                    var existingSettingName = _context.TB_LeaveTypes.Find(lt.LeaveTypePkid);

                    if (existingSettingName != null)
                    {
                        existingSettingName.LeaveTypeName = lt.LeaveTypeName;

                        _context.TB_LeaveTypes.Update(existingSettingName);
                    }
                }

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Setting/LeaveType/LeaveTypeCrud.cshtml", lt);
        }

        private TB_LeaveType GetLeaveType(int Id)
        {
            TB_LeaveType lt = _context.TB_LeaveTypes
              .Where(p => p.LeaveTypePkid == Id).FirstOrDefault();
            return lt;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Delete(int leaveTypeId)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_LeaveType lt = GetLeaveType(leaveTypeId);


            if (lt != null)
            {
                _context.Remove(lt);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));

            }

            return View("~/Views/Setting/LeaveType/LeaveTypeCrud.cshtml", lt);
        }



    }
}
