﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace AddMemberSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeController(AppDBContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }  

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

            var searchCriteriaItems = new List<SelectListItem>
            {
                new SelectListItem { Value = "Name", Text = "၀န်ထမ်းအမည်" },
                new SelectListItem { Value = "Department", Text = "ဌာန" },
                new SelectListItem { Value = "Position", Text = "ရာထူး" },
            };

            List<TB_Staff> staffs = _context.TB_Staffs
                .Where(staff => staff.isDeleted == false)
                .Include(d => d.Department)
                .Include(p => p.Position)
                .ToList();

            const int pageSize = 5;
            if (pg < 1)
                pg = 1;

            int recsCount = staffs.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = staffs.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;
            ViewBag.SearchCriteriaItems = searchCriteriaItems;

            return View(data);
        }
        private TB_Staff GetStaff(int Id)
        {
            TB_Staff staff = _context.TB_Staffs
                .Where(m => m.StaffPkid == Id).FirstOrDefault();
            return staff;
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

        [HttpGet]
        public JsonResult GetPositionsByDepartment(int DepartmentPkid)
        {
            List<SelectListItem> positions = GetPositions(DepartmentPkid);
            return Json(positions);
        }

        private bool IsNrcOrPassportNumberUnique(string nrc)
        {
            return !_context.TB_Staffs.Any(m => m.NRC == nrc);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_Staff Staff = new TB_Staff();
            ViewBag.DepartmentPkid = GetDepartments();
            ViewBag.PositionPkid = GetPositions();
            return View(Staff);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(TB_Staff staff)
        {
            if (!IsNrcOrPassportNumberUnique(staff.NRC))
            {
                ModelState.AddModelError(nameof(TB_Staff.NRC), "မှတ်ပုံတင် နံပါတ်သည် သီးသန့်ဖြစ်ရန် လိုအပ်ပါသည်");

                ViewBag.DepartmentPkid = GetDepartments();

                return View("Create");
            }

            if (staff.ImageFile != null)
            {
                string fileName = GetFileName(staff.ImageFile);
                SaveFile(staff.ImageFile, fileName);
                staff.StaffPhoto = fileName;
            }
            else
            {
                staff.StaffPhoto = "default.jpeg";
            }

            staff.CreatedDate = DateTime.UtcNow;

            _context.Add(staff);
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

            TB_Staff staff = GetStaff(Id);
            ViewBag.Department = GetDepartmentName(staff.DepartmentId);
            ViewBag.Position = GetPositionName(staff.PositionId);
            return View(staff);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_Staff staff = GetStaff(id);
            ViewBag.DepartmentPkid = GetDepartments();
            ViewBag.PositionPkid = GetPositions(staff.DepartmentId);
            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TB_Staff editedStaff)
        {
            TB_Staff existingMember = GetStaff(editedStaff.StaffPkid);

            string newNrc = editedStaff.NRC;
            string oldNrc = existingMember.NRC;

            if (newNrc != oldNrc)
            {
                TB_Staff staff = _context.TB_Staffs.FirstOrDefault(staff => staff.NRC == newNrc);
                if (staff != null)
                {
                    TempData["Message"] = "မှတ်ပုံတင် နံပါတ်သည် သီးသန့်ဖြစ်ရန် လိုအပ်ပါသည်";
                    TempData["CssColor"] = "alert-danger";
                    return RedirectToAction(nameof(Edit));
                }
            }

            if (editedStaff.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(editedStaff.StaffPhoto))
                {
                    string oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, "StaffPhoto", editedStaff.StaffPhoto);

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                string newFileName = GetUniqueFileName(editedStaff.ImageFile.FileName);

                SaveFile(editedStaff.ImageFile, newFileName);

                editedStaff.StaffPhoto = newFileName;
            }
            else
            {
                editedStaff.StaffPhoto = existingMember.StaffPhoto; 
                Console.WriteLine("NO image found" + editedStaff.StaffPhoto);
                Console.WriteLine("NO image found" + existingMember.StaffPhoto);
    }

            existingMember.SerialNo = editedStaff.SerialNo;
            existingMember.StaffID = editedStaff.StaffID;
            existingMember.Name = editedStaff.Name;
            existingMember.FatherName = editedStaff.FatherName;
            existingMember.DateOfBirth = editedStaff.DateOfBirth;
            existingMember.NRC = editedStaff.NRC;
            existingMember.Age = editedStaff.Age;
            existingMember.Religion = editedStaff.Religion;
            existingMember.VisibleMark = editedStaff.VisibleMark;
            existingMember.Address = editedStaff.Address;
            existingMember.Phone = editedStaff.Phone;
            existingMember.PositionId = editedStaff.PositionId;
            existingMember.DepartmentId = editedStaff.DepartmentId;
            existingMember.Responsibility = editedStaff.Responsibility;
            existingMember.StartedDate = editedStaff.StartedDate;
            existingMember.Remarks = editedStaff.Remarks;
            existingMember.StaffPhoto = editedStaff.StaffPhoto;

            _context.SaveChanges();

            return RedirectToAction(nameof(List));

        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Delete(int Id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_Staff staff = GetStaff(Id);
            staff.isDeleted = true;
            _context.Attach(staff);
            _context.Entry(staff).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction(nameof(List));
        }

        public String GetFileName(IFormFile ImageFile)
        {
            string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
            return fileName + DateTime.Now.ToString("yymmssfff") + GetFileExtension(ImageFile);
        }

        public String GetFileExtension(IFormFile ImageFile)
        {
            string extension = Path.GetExtension(ImageFile.FileName);
            return extension;
        }

        public async void SaveFile(IFormFile ImageFile, string fileName)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string extension = Path.GetExtension(ImageFile.FileName);
            string path = Path.Combine(wwwRootPath + "/StaffPhoto/", fileName);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await ImageFile.CopyToAsync(fileStream);
            }
        }
        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                + "_" + Guid.NewGuid().ToString().Substring(0, 4)
                + Path.GetExtension(fileName);
        }

        [HttpGet]
        public IActionResult SearchStaffs(string searchCriteria, string searchTerm, int pg = 1)
        {
            IQueryable<TB_Staff> query = _context.TB_Staffs
                .Where(m => m.isDeleted == false)
                .Include("Department")
                .Include("Position");

            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (!string.IsNullOrEmpty(searchCriteria))
                {
                    switch (searchCriteria)
                    {
                        case "Name":
                            query = query.Where(m => m.Name.Contains(searchTerm));
                            break;                       
                        case "Department":
                            query = query.Where(m => m.DepartmentId.ToString().Contains(searchTerm));
                            break;
                        case "Position":
                            query = query.Where(m => m.PositionId.ToString().Contains(searchTerm));
                            break;
                        
                        default:
                            break;
                    }
                }
                else
                {
                    query = query.Where(m =>
                        m.Name.Contains(searchTerm) ||                       
                        m.DepartmentId.ToString().Contains(searchTerm) ||
                        m.PositionId.ToString().Contains(searchTerm));
                }
            }

            const int pageSize = 5;
            if (pg < 1)
                pg = 1;

            int recsCount = query.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var searchResults = query
                .Skip(recSkip)
                .Take(pager.PageSize)
                .ToList();

            var searchCriteriaItems = new List<SelectListItem>
            {
               new SelectListItem { Value = "Name", Text = "၀န်ထမ်းအမည်" },
                new SelectListItem { Value = "Department", Text = "ဌာန" },
                new SelectListItem { Value = "Position", Text = "ရာထူး" },
            };

            foreach (var item in searchCriteriaItems)
            {
                if (item.Value == searchCriteria)
                {
                    item.Selected = true;
                }
            }

            ViewBag.SearchCriteriaItems = searchCriteriaItems;

            pager = new Pager(recsCount, pg, pageSize);

            ViewBag.Pager = pager;
            ViewBag.searchCriteria = searchCriteria;
            ViewBag.searchTerm = searchTerm;

            return View("List", searchResults);
        }




    }
}