
using AddMemberSystem.Models;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.Intrinsics.Arm;

namespace AddMemberSystem.Controllers.Setting
{
    public class PunishmentTypeController : Controller
    {
        private readonly AppDBContext _context;

        public PunishmentTypeController(AppDBContext context)
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

            List<TB_PunishmentType> punishmentTypes = _context.TB_PunishmentType.Where(dep => dep.IsDeleted == false).ToList();

            SelectList punishmentTypeList = new SelectList(punishmentTypes, "PunishmentTypePkid", "Punishment");

            ViewBag.LeaveTypeId = punishmentTypeList;

            return View("~/Views/Setting/PunishmentType/PunishmentTypeCrud.cshtml");
        }

        [HttpGet]

        public IActionResult GetAllPunishmentTypes(int? punishmentTypeId)
        {
            if (punishmentTypeId.HasValue)
            {
                var punishmentType = _context.TB_PunishmentType.Find(punishmentTypeId.Value);

                if (punishmentType != null)
                {
                    return Json(new { punishmentType = punishmentType.Punishment });
                }
            }

            var punishmentTypes = _context.TB_PunishmentType.Where(dep => dep.IsDeleted == false).ToList();
            return Json(punishmentTypes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TB_PunishmentType lt, string actionType)
        {
            if (ModelState.IsValid)
            {
                if (actionType == "Create")
                {
                    if (_context.TB_PunishmentType.Any(d => d.Punishment == lt.Punishment && !d.IsDeleted))
                    {
                        ModelState.AddModelError("Punishment", "Punishment with this name already exists.");
                        return View("~/Views/Setting/PunishmentType/PunishmentTypeCrud.cshtml");
                    }
                    lt.CreatedDate = DateTime.UtcNow;
                    _context.TB_PunishmentType.Add(lt);
                }
                else if (actionType == "Edit")
                {
                    if (_context.TB_PunishmentType.Any(d => d.Punishment == lt.Punishment && !d.IsDeleted))
                    {
                        ModelState.AddModelError("Punishment", "Edit Punishment with this name already exists.");
                        return View("~/Views/Setting/PunishmentType/PunishmentTypeCrud.cshtml");
                    }

                    var existingSettingName = _context.TB_PunishmentType.Find(lt.PunishmentTypePkid);

                    if (existingSettingName != null)
                    {
                        existingSettingName.Punishment = lt.Punishment;
                        existingSettingName.CreatedDate = DateTime.UtcNow;

                        _context.TB_PunishmentType.Update(existingSettingName);
                    }
                }

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Setting/PunishmentType/PunishmentTypeCrud.cshtml");
        }

        private TB_PunishmentType GetPunishmentType(int Id)
        {
            TB_PunishmentType lt = _context.TB_PunishmentType
              .Where(p => p.PunishmentTypePkid == Id).FirstOrDefault();
            return lt;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Delete(int punishmentTypeId)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_PunishmentType lt = GetPunishmentType(punishmentTypeId);


            if (lt != null)
            {
                lt.IsDeleted = true;
                lt.CreatedDate = DateTime.UtcNow;

                _context.Entry(lt).State = EntityState.Modified;

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));

            }

            return View("~/Views/Setting/PunishmentType/PunishmentTypeCrud.cshtml", lt);
        }



    }
}
