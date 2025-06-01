
using AddMemberSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.Intrinsics.Arm;

namespace AddMemberSystem.Controllers.Setting
{
    public class StaffBenefitController : Controller
    {
        private readonly AppDBContext _context;

        public StaffBenefitController(AppDBContext context)
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

            List<TB_StaffBenefit> result = _context.TB_StaffBenefit.ToList();

            SelectList List = new SelectList(result, "StaffBenefitPkid", "StffBenefitName");

            ViewBag.StaffBenefitId = List;

            return View("~/Views/Setting/StaffBenefit/StaffBenefitCrud.cshtml");
        }

        [HttpGet]

        public IActionResult GetAllStaffBenefits(int? staffBenefitId)
        {
            if (staffBenefitId.HasValue)
            {
                var result = _context.TB_StaffBenefit.Find(staffBenefitId.Value);                

                if (result != null)
                {
                    return Json(new { benefit = result.BenefitName, amount = result.Amount });
                }
            }

            var benefits = _context.TB_StaffBenefit.ToList();
            return Json(benefits);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TB_StaffBenefit lt, string actionType)
        {
            if (ModelState.IsValid)
            {
                if (actionType == "Create")
                {
                    if (_context.TB_StaffBenefit.Any(d => d.BenefitName == lt.BenefitName))
                    {
                        ModelState.AddModelError("BenefitName", "Benefit with this name already exists.");
                        return View("~/Views/Setting/StaffBenefit/StaffBenefitCrud.cshtml", lt);
                    }

                    _context.TB_StaffBenefit.Add(lt);
                }
                else if (actionType == "Edit")
                {
                    //if (_context.TB_StaffBenefit.Any(d => d.BenefitName == lt.BenefitName))
                    //{
                    //    ModelState.AddModelError("BenefitName", "Edit Benefit with this name already exists.");
                    //    return View("~/Views/Setting/StaffBenefit/StaffBenefitCrud.cshtml", lt);
                    //}

                    var existingSettingName = _context.TB_StaffBenefit.Find(lt.StaffBenefitPkid);

                    if (existingSettingName != null)
                    {
                        if (_context.TB_StaffBenefit.Any(d => d.BenefitName == lt.BenefitName && d.StaffBenefitPkid != lt.StaffBenefitPkid))
                        {
                            ModelState.AddModelError("BenefitName", "Edit Benefit with this name already exists.");
                            return View("~/Views/Setting/StaffBenefit/StaffBenefitCrud.cshtml", lt);
                        }

                        existingSettingName.BenefitName = lt.BenefitName;
                        existingSettingName.Amount = lt.Amount;

                        _context.TB_StaffBenefit.Update(existingSettingName);
                    }
                }

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Setting/StaffBenefit/StaffBenefitCrud.cshtml", lt);
        }

        private TB_StaffBenefit GetStaffBenefit(int Id)
        {
            TB_StaffBenefit lt = _context.TB_StaffBenefit
              .Where(p => p.StaffBenefitPkid == Id).FirstOrDefault();
            return lt;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Delete(int staffBenefitId)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_StaffBenefit lt = GetStaffBenefit(staffBenefitId);


            if (lt != null)
            {
                _context.Remove(lt);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));

            }

            return View("~/Views/Setting/StaffBenefit/StaffBenefitCrud.cshtml", lt);
        }



    }
}
