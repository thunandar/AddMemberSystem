using Microsoft.AspNetCore.Mvc.Rendering;

namespace AddMemberSystem.Controllers.Setting
{
    public class InitialPositionController : Controller
    {
        private readonly AppDBContext _context;

        public InitialPositionController(AppDBContext context)
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

            List<TB_InitialPosition> positions = _context.TB_InitialPositions.ToList();

            ViewBag.DepartmentId = departmentList;
            ViewBag.Positions = positions;

            return View("~/Views/Setting/Position/InitialPositionCrud.cshtml");
        }

        [HttpGet]
        public IActionResult GetPositionsByDepartment(int departmentId)
        {
            List<TB_InitialPosition> positions = _context.TB_InitialPositions
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
        public IActionResult Create(TB_InitialPosition pos, string actionType)
        {
            if (ModelState.IsValid)
            {
                if (actionType == "Create")
                {
                    if (_context.TB_InitialPositions.Any(p => p.InitialPosition == pos.InitialPosition && p.DepartmentId == pos.DepartmentId))
                    {
                        ModelState.AddModelError("InitialPosition", "Position with this name already exists in the selected department.");
                        ViewBag.DepartmentId = GetDepartments();
                        return View("~/Views/Setting/Position/InitialPositionCrud.cshtml", pos);
                    }
                    _context.TB_InitialPositions.Add(pos);
                }
                else if (actionType == "Edit")
                {
                    if (_context.TB_InitialPositions.Any(p => p.InitialPosition == pos.InitialPosition && p.DepartmentId == pos.DepartmentId))
                    {
                        ModelState.AddModelError("InitialPosition", "Edit Position with this name already exists in the selected department.");
                        ViewBag.DepartmentId = GetDepartments();
                        return View("~/Views/Setting/Position/InitialPositionCrud.cshtml", pos);
                    }

                    var existingPosition = _context.TB_InitialPositions.Find(pos.InitialPositionPkid);
                    if (existingPosition != null)
                    {
                        existingPosition.InitialPosition = pos.InitialPosition;
                        _context.TB_InitialPositions.Update(existingPosition);
                    }
                }

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.DepartmentId = GetDepartments();

            return View("~/Views/Setting/Position/InitialPositionCrud.cshtml", pos);
        }

        [HttpGet]
        public IActionResult GetAllPositionsAndDepartments()
        {
            List<TB_InitialPosition> positions = _context.TB_InitialPositions
                .Include(p => p.Department)
                 .Where(position => position.isDeleted == false)
                 .Where(dep => dep.isDeleted == false)
                .ToList();

            return Json(positions);
        }

        [HttpGet]
        public IActionResult GetDepartmentAndPositionData(int positionId)
        {
            var position = _context.TB_InitialPositions
                .Include(p => p.Department)
                .FirstOrDefault(p => p.InitialPositionPkid == positionId);

            if (position != null)
            {
                var data = new
                {
                    department = position.Department.Department,
                    position = position.InitialPosition
                };

                return Json(data);
            }

            return Json(null);
        }

        private TB_InitialPosition GetPosition(int Id)
        {
            TB_InitialPosition position = _context.TB_InitialPositions
              .Where(p => p.InitialPositionPkid == Id).FirstOrDefault();
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

            TB_InitialPosition position = GetPosition(positionId);

            if (position != null)
            {

                position.isDeleted = true;

                _context.Entry(position).State = EntityState.Modified;

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));

            }

            return View("~/Views/Setting/Position/InitialPositionCrud.cshtml", position);
        }
    }
}
