
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

            List<TB_Position> positionTypes = _context.TB_Positions.Where(dep => dep.IsDeleted == false).ToList();

            SelectList positionTypeList = new SelectList(positionTypes, "PositionPkid", "Position");

            ViewBag.LeaveTypeId = positionTypeList;

            return View("~/Views/Setting/Position/PositionCrud.cshtml");
        }

        [HttpGet]

        public IActionResult GetAllPositionTypes(int? positionTypeId)
        {
            if (positionTypeId.HasValue)
            {
                var positionType = _context.TB_Positions.Find(positionTypeId.Value);
                Console.WriteLine("PSI" + positionType);

                if (positionType != null)
                {
                    return Json(new { positionType = positionType.Position });
                }
            }

            var positionTypes = _context.TB_Positions.Where(dep => dep.IsDeleted == false).ToList();
            return Json(positionTypes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TB_Position lt, string actionType)
        {
            if (ModelState.IsValid)
            {
                if (actionType == "Create")
                {
                    if (_context.TB_Positions.Any(d => d.Position == lt.Position && !d.IsDeleted))
                    {
                        ModelState.AddModelError("Position", "Position with this name already exists.");
                        return View("~/Views/Setting/Position/PositionCrud.cshtml");
                    }

                    _context.TB_Positions.Add(lt);
                }
                else if (actionType == "Edit")
                {
                    if (_context.TB_Positions.Any(d => d.Position == lt.Position && !d.IsDeleted))
                    {
                        ModelState.AddModelError("Position", "Edit Position with this name already exists.");
                        return View("~/Views/Setting/Position/PositionCrud.cshtml");
                    }

                    var existingSettingName = _context.TB_Positions.Find(lt.PositionPkid);

                    if (existingSettingName != null)
                    {
                        existingSettingName.Position = lt.Position;

                        _context.TB_Positions.Update(existingSettingName);
                    }
                }

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Setting/Position/PositionCrud.cshtml");
        }

        private TB_Position GetPositionType(int Id)
        {
            TB_Position lt = _context.TB_Positions
              .Where(p => p.PositionPkid == Id).FirstOrDefault();
            return lt;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Delete(int positionTypeId)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_Position lt = GetPositionType(positionTypeId);


            if (lt != null)
            {
                lt.IsDeleted = true;

                _context.Entry(lt).State = EntityState.Modified;

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));

            }

            return View("~/Views/Setting/Position/PositionCrud.cshtml", lt);
        }



    }
}
