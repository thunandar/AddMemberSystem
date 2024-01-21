
using AddMemberSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AddMemberSystem.Controllers.Setting
{
    public class DepartmentController : Controller
    {
        private readonly AppDBContext _context;

        public DepartmentController(AppDBContext context)
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

            List<TB_Department> departments = _context.TB_Departments.Where(dep => dep.isDeleted == false).ToList();

            SelectList departmentList = new SelectList(departments, "DepartmentPkid", "Department");

            List<TB_Position> positions = _context.TB_Positions.ToList();

            ViewBag.DepartmentId = departmentList;
            ViewBag.Positions = positions;
            
            return View("~/Views/Setting/Department/DepartmentCrud.cshtml");
        }     

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TB_Department dep, string actionType)
        {
            if (ModelState.IsValid)
            {
                if (actionType == "Create")
                {
                    if (_context.TB_Departments.Any(d => d.Department == dep.Department))
                    {
                        ModelState.AddModelError("Department", "Department with this name already exists.");
                        return View("~/Views/Setting/Department/DepartmentCrud.cshtml", dep);
                    }

                    _context.TB_Departments.Add(dep);
                }
                else if (actionType == "Edit")
                {
                    if (_context.TB_Departments.Any(d => d.Department == dep.Department))
                    {
                        ModelState.AddModelError("Department", "Edit Department with this name already exists in the selected department.");
                        return View("~/Views/Setting/Department/DepartmentCrud.cshtml", dep);
                    }

                    var existingDepartment = _context.TB_Departments.Find(dep.DepartmentPkid);

                    if (existingDepartment != null)
                    {
                        existingDepartment.Department = dep.Department;

                        _context.TB_Departments.Update(existingDepartment);
                    }
                }

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Setting/Department/DepartmentCrud.cshtml", dep);
        }

       
        [HttpGet]
        
        public IActionResult GetAllDepartments(int? departmentId)
        {
            if (departmentId.HasValue)
            {
                var department = _context.TB_Departments.Find(departmentId.Value);

                if (department != null)
                {
                    return Json(new { department = department.Department });
                }
            }

            var departments = _context.TB_Departments.Where(departments => departments.isDeleted == false).ToList();
            return Json(departments);
        }

         private TB_Department GetDepartment(int Id)
         {
             TB_Department department = _context.TB_Departments
               .Where(p => p.DepartmentPkid == Id).FirstOrDefault();
             return department;
         }

         [ValidateAntiForgeryToken]
         [HttpPost]
         public IActionResult PositionDelete(int departmentId)
         {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_Department department = GetDepartment(departmentId);


             if (department != null)
             {

                 department.isDeleted = true;

                 _context.Entry(department).State = EntityState.Modified;

                 _context.SaveChanges();

                 return RedirectToAction(nameof(Index));

             }

             return View("~/Views/Setting/Department/DepartmentCrud.cshtml", department);
         }
        

    }
}
