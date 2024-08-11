
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AddMemberSystem.Controllers.Setting
{
    public class PositionController : Controller
    {
        private readonly AppDBContext _context;

        public PositionController(AppDBContext context)
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

            return View("~/Views/Setting/Position/PositionCrud.cshtml");
        }

        [HttpGet]
        public IActionResult GetPositionsByDepartment(int departmentId)
        {
            List<TB_Position> positions = _context.TB_Positions
                .Where(p => p.DepartmentId == departmentId)
                .Include(p => p.Department)
                 .Where(position => position.isDeleted == false)
                 .Where(dep => dep.isDeleted == false)
                .ToList();

            return Json(positions);
        }

        private List<SelectListItem> GetDepartments()
        {
            List<TB_Department> departments = _context.TB_Departments.Where(dep => dep.isDeleted == false).ToList();

            List<SelectListItem> departmentList = departments
                .Select(d => new SelectListItem()
                {
                    Value = d.DepartmentPkid.ToString(),
                    Text = d.Department ?? "Unknown Department"
                })
                .ToList();

            return departmentList;
        }


        [HttpGet]
        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            ViewBag.DepartmentId = GetDepartments();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TB_Position pos, string actionType)
        {          
            if (ModelState.IsValid)
            {
                if (actionType == "Create")
                {
                    if (_context.TB_Positions.Any(p => p.Position == pos.Position && p.DepartmentId == pos.DepartmentId))
                    {
                        ModelState.AddModelError("Position", "Position with this name already exists in the selected department.");
                        ViewBag.DepartmentId = GetDepartments();
                        return View("~/Views/Setting/Position/PositionCrud.cshtml", pos);
                    }
                    _context.TB_Positions.Add(pos);
                }
                else if (actionType == "Edit")
                {
                    if (_context.TB_Positions.Any(p => p.Position == pos.Position && p.DepartmentId == pos.DepartmentId))
                    {
                        ModelState.AddModelError("Position", "EDit Position with this name already exists in the selected department.");
                        ViewBag.DepartmentId = GetDepartments();
                        return View("~/Views/Setting/Position/PositionCrud.cshtml", pos);
                    }

                    var existingPosition = _context.TB_Positions.Find(pos.PositionPkid);
                    if (existingPosition != null)
                    {
                        existingPosition.Position = pos.Position;
                        _context.TB_Positions.Update(existingPosition);
                    }
                }

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.DepartmentId = GetDepartments();

            return View("~/Views/Setting/Position/PositionCrud.cshtml", pos);
        }

        [HttpGet]
        public IActionResult GetAllPositionsAndDepartments()
        {
            List<TB_Position> positions = _context.TB_Positions
                .Include(p => p.Department)
                 .Where(position => position.isDeleted == false)
                 .Where(dep => dep.isDeleted == false)
                .ToList();

            return Json(positions);
        }

        [HttpGet]
        public IActionResult GetDepartmentAndPositionData(int positionId)
        {
            var position = _context.TB_Positions
                .Include(p => p.Department)
                .FirstOrDefault(p => p.PositionPkid == positionId);

            if (position != null)
            {
                var data = new
                {
                    department = position.Department.Department,
                    position = position.Position
                };

                return Json(data);
            }

            return Json(null);
        }

        private TB_Position GetPosition(int Id)
        {
            TB_Position position = _context.TB_Positions
              .Where(p => p.PositionPkid == Id).FirstOrDefault();
            return position;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult PositionDelete(int positionId)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_Position position = GetPosition(positionId);

            if (position != null)
            {

                position.isDeleted = true;

                _context.Entry(position).State = EntityState.Modified;

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));

            }

            return View("~/Views/Setting/Position/PositionCrud.cshtml", position);
        }
    }
}
