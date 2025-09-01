using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Text;

using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace AddMemberSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(AppDBContext context, IWebHostEnvironment hostEnvironment, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            int totalStaffCount = _context.TB_Staffs.Where(b => b.isDeleted == false).Count();
            int totalPositionCount = _context.TB_Positions.Where(b => b.IsDeleted == false).Count();
            int totalDepartmentCount = _context.TB_Departments.Where(b => b.isDeleted == false).Count();
            int totalStaffPunishmentCount = _context.TB_StaffPunishments.Where(b => b.IsDeleted == false).Count();
            int totalStaffLeaveCount = _context.TB_StaffLeaves.Where(b => b.IsDeleted == false).Count();

            ViewBag.totalStaffCount = totalStaffCount;
            ViewBag.totalPositionCount = totalPositionCount;
            ViewBag.totalDepartmentCount = totalDepartmentCount;
            ViewBag.totalStaffPunishmentCount = totalStaffPunishmentCount;
            ViewBag.totalStaffLeaveCount = totalStaffLeaveCount;

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


            var searchCriteriaItems = new List<SelectListItem>
            {
                new SelectListItem { Value = "Name", Text = "Name" },
                new SelectListItem { Value = "Department", Text = "Department" },
                new SelectListItem { Value = "Position", Text = "Current Position" },
            };

            List<TB_Staff> staffs = _context.TB_Staffs
                .Where(staff => staff.isDeleted == false)
                 .OrderByDescending(s => s.StaffPkid)
                .ToList();

            var staffIds = staffs.Select(s => s.StaffID).ToList();
            
            var currentJobs = _context.TB_JobHistorys
                .Where(j => staffIds.Contains(j.StaffID) && j.IsCurrent)
                .Include(j => j.Department)
                .Include(j => j.Position)
                .ToList();

            foreach (var staff in staffs)
            {
                var currentJob = currentJobs.FirstOrDefault(j => j.StaffID == staff.StaffID);
                if (currentJob != null)
                {
                    staff.CurrentDepartment = currentJob.Department?.Department;
                    staff.CurrentPosition = currentJob.Position?.Position;
                }
            }

            const int pageSize = 20;
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

        private string GetStaffBenefitsName(int StaffBenefitPkid)
        {
            string BenefitTypeName = _context.TB_StaffBenefit.Where(p => p.StaffBenefitPkid == StaffBenefitPkid).SingleOrDefault().BenefitName;
            return BenefitTypeName;
        }


        private bool IsNrcOrPassportNumberUnique(string nrc)
        {
            return !_context.TB_Staffs.Any(m => m.NRC == nrc);
        }

        private List<SelectListItem> GetStaffBenefits()
        {
            var result = new List<SelectListItem>();

            List<TB_StaffBenefit> data = _context.TB_StaffBenefit.Where(b => !b.IsDeleted).ToList();

            result = data.Select(d => new SelectListItem()
            {
                Value = d.StaffBenefitPkid.ToString(),
                Text = d.BenefitName
            }).ToList();

            var defItem = new SelectListItem()
            {
                Value = "",
                Text = "----အကျိုးခံစားခွင့်အမျိုးအစား ရွေးချယ်ပါ----"
            };

            result.Insert(0, defItem);

            return result;
        }

        private Dictionary<int, string> GetStaffBenefitAmounts()
        {
            return _context.TB_StaffBenefit.Where(b => !b.IsDeleted).ToDictionary(b => b.StaffBenefitPkid, b => b.Amount);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_Staff Staff = new TB_Staff();
            ViewBag.StaffBenefitId = GetStaffBenefits();
            ViewBag.BenefitAmounts = GetStaffBenefitAmounts();
            return View(Staff);
        }

        private string GenerateStaffId()
        {
            string prefix = "D" + DateTime.UtcNow.ToString("yyMM"); 
            int maxSequence = 0;

            var existingIds = _context.TB_Staffs
                .Where(s => s.StaffID.StartsWith(prefix) && s.StaffID.Length == 8)
                .Select(s => s.StaffID.Substring(5, 3))
                .ToList();

            foreach (var idSuffix in existingIds)
            {
                if (int.TryParse(idSuffix, out int sequence) && sequence > maxSequence)
                {
                    maxSequence = sequence;
                }
            }

            int nextSequence = maxSequence + 1;
            return prefix + nextSequence.ToString("000"); 
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(TB_Staff staff)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StaffBenefitId = GetStaffBenefits();
                ViewBag.BenefitAmounts = GetStaffBenefitAmounts();
                return View(staff);
            }

            if (staff.StaffType == "နေ့စား")
            {
                staff.StaffID = GenerateStaffId(); 
            }

            DateTime today = DateTime.UtcNow.Date;
            if (staff.DateOfBirth.HasValue && staff.DateOfBirth.Value > today)
            {
                ViewBag.StaffBenefitId = GetStaffBenefits();
                ViewBag.BenefitAmounts = GetStaffBenefitAmounts();

                ModelState.AddModelError(nameof(TB_Staff.DateOfBirth), "မွေးသက္ကရာဇ်သည် ယနေ့ရက်စွဲထက် ကြီးမြှင့်မရပါ");

                return View("Create");
            }

            if (!IsNrcOrPassportNumberUnique(staff.NRC))
            {
                ModelState.AddModelError(nameof(TB_Staff.NRC), "မှတ်ပုံတင် နံပါတ်သည် သီးသန့်ဖြစ်ရန် လိုအပ်ပါသည်");

                //ViewBag.DepartmentPkid = GetDepartments();
                //ViewBag.PositionPkid = GetPositions();
                ViewBag.StaffBenefitId = GetStaffBenefits();
                ViewBag.BenefitAmounts = GetStaffBenefitAmounts();

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
        private int GetStaffBenefitAmount(int staffBenefitId)
        {
            string amountStr = _context.TB_StaffBenefit
                                       .Where(b => b.StaffBenefitPkid == staffBenefitId)
                                       .Select(b => b.Amount)
                                       .SingleOrDefault();

            if (int.TryParse(amountStr, out int amount))
            {
                return amount;
            }
            return 0;
        }

        private string GetStaffBenefitsAmount(int StaffBenefitPkid)
        {
            string BenefitTypeAmount = _context.TB_StaffBenefit.Where(p => p.StaffBenefitPkid == StaffBenefitPkid).SingleOrDefault().Amount;
            return BenefitTypeAmount;
        }

        [HttpGet]
        public IActionResult Details(int Id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            TB_Staff staff = GetStaff(Id);
            //ViewBag.Department = GetDepartmentName(staff.DepartmentId);
            //ViewBag.Position = staff.PositionId.HasValue ? GetPositionName(staff.PositionId.Value) : "Position Not Assigned";

            ViewBag.StaffBenefitId = staff.StaffBenefitId.HasValue ? GetStaffBenefitsName(staff.StaffBenefitId.Value) : "Benefit Not Found.";


            ViewBag.StaffBenefitAmount = staff.StaffBenefitId.HasValue ? GetStaffBenefitsAmount(staff.StaffBenefitId.Value) : "Amount Not Found.";


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

            //ViewBag.DepartmentPkid = GetDepartments();
            //ViewBag.PositionPkid = GetPositions(staff.DepartmentId);
            ViewBag.StaffBenefitId = GetStaffBenefits();
            ViewBag.BenefitAmounts = GetStaffBenefitAmounts();


            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TB_Staff editedStaff)
        {
            if (!ModelState.IsValid)
            {
                //ViewBag.DepartmentPkid = GetDepartments();
                //ViewBag.PositionPkid = GetPositions(editedStaff.DepartmentId);
                ViewBag.StaffBenefitId = GetStaffBenefits();
                ViewBag.BenefitAmounts = GetStaffBenefitAmounts();
                return View(editedStaff);
            }

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
            }

            existingMember.SerialNo = editedStaff.SerialNo;
            existingMember.StaffID = editedStaff.StaffID;
            existingMember.Name = editedStaff.Name;
            existingMember.StaffType = editedStaff.StaffType;
            existingMember.FatherName = editedStaff.FatherName;
            existingMember.DateOfBirth = editedStaff.DateOfBirth;
            existingMember.NRC = editedStaff.NRC;
            existingMember.Age = editedStaff.Age;
            existingMember.Gender = editedStaff.Gender;
            existingMember.Religion = editedStaff.Religion;
            existingMember.VisibleMark = editedStaff.VisibleMark;
            existingMember.Address = editedStaff.Address;
            existingMember.Phone = editedStaff.Phone;
            //existingMember.PositionId = editedStaff.PositionId;
            //existingMember.DepartmentId = editedStaff.DepartmentId;
            existingMember.Responsibility = editedStaff.Responsibility;
            existingMember.StartedDate = editedStaff.StartedDate;
            existingMember.Remarks = editedStaff.Remarks;
            existingMember.StaffPhoto = editedStaff.StaffPhoto;
            existingMember.Salary = editedStaff.Salary;

            existingMember.SocialSecurity = editedStaff.SocialSecurity;
            if (existingMember.SocialSecurity)
            {
                existingMember.ErSSN = editedStaff.ErSSN;
                existingMember.EeSSN = editedStaff.EeSSN;
                existingMember.Minc = editedStaff.Minc;
                existingMember.SS1EeRate = editedStaff.SS1EeRate;
                existingMember.SS1ErRate = editedStaff.SS1ErRate;
                existingMember.SS1EeConAmt = editedStaff.SS1EeConAmt;
                existingMember.SS1ErConAmt = editedStaff.SS1ErConAmt;
                existingMember.SS2EeRate = editedStaff.SS2EeRate;
                existingMember.SS2ErRate = editedStaff.SS2ErRate;
                existingMember.SS2EeConAmt = editedStaff.SS2EeConAmt;
                existingMember.SS2ErConAmt = editedStaff.SS2ErConAmt;
                existingMember.TotalConAmt = editedStaff.TotalConAmt;
            }

            // RiceOil handling
            existingMember.RiceOil = editedStaff.RiceOil;
            if (existingMember.RiceOil)
            {
                existingMember.StaffBenefitId = editedStaff.StaffBenefitId;
            }

            existingMember.ChangeAmount = editedStaff.ChangeAmount;
            if (existingMember.ChangeAmount)
            {
                existingMember.CustomBenefitAmount = editedStaff.CustomBenefitAmount;

            }

            existingMember.CreatedDate = DateTime.UtcNow;

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
            staff.CreatedDate = DateTime.UtcNow;
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

        private void SetSearchCriteriaItemsInViewBag(string searchCriteria)
        {
            var searchCriteriaItems = new List<SelectListItem>
        {
            new SelectListItem { Value = "Name", Text = "Name" },
            new SelectListItem { Value = "Department", Text = "Department" },
            new SelectListItem { Value = "Position", Text = "Current Position" },
        };

            searchCriteriaItems.ForEach(item => item.Selected = item.Value == searchCriteria);

            ViewBag.SearchCriteriaItems = searchCriteriaItems;
        }

        //private IQueryable<TB_Staff> BuildQuery(string searchCriteria, string searchTerm)
        //{
        //    var query = _context.TB_Staffs
        //        .Where(m => m.isDeleted == false);
        //        //.Include("Department")
        //        //.Include("Position");

        //    if (!string.IsNullOrEmpty(searchTerm)) 
        //    {
        //        if (!string.IsNullOrEmpty(searchCriteria))
        //        {
        //            switch (searchCriteria)
        //            {
        //                case "Name":
        //                    query = query.Where(m => m.Name.Contains(searchTerm));
        //                    break;
        //                //case "Department":
        //                //    query = query.Where(m => m.Department.Department.Contains(searchTerm));
        //                //    break;
        //                //case "Position":
        //                //    query = query.Where(m => m.Position.Position.Contains(searchTerm));
        //                //    break;
        //                default:
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            query = query.Where(m =>
        //                m.Name.Contains(searchTerm)); 
        //        //m.Department.Department.Contains(searchTerm) ||
        //        //m.Position.Position.Contains(searchTerm));
        //        }
        //    }

        //    return query;
        //}

        private IQueryable<TB_Staff> BuildQuery(string searchCriteria, string searchTerm)
        {
            var query = _context.TB_Staffs
                .Where(m => m.isDeleted == false);

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
                            // Search in current job's department
                            query = query.Where(s => _context.TB_JobHistorys
                                .Any(j => j.StaffID == s.StaffID
                                       && j.IsCurrent
                                       && j.Department.Department.Contains(searchTerm)));
                            break;
                        case "Position":
                            // Search in current job's position
                            query = query.Where(s => _context.TB_JobHistorys
                                .Any(j => j.StaffID == s.StaffID
                                       && j.IsCurrent
                                       && j.Position.Position.Contains(searchTerm)));
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    // Search across Name, Department, or Position
                    query = query.Where(m =>
                        m.Name.Contains(searchTerm) ||
                        _context.TB_JobHistorys.Any(j => j.StaffID == m.StaffID
                                                        && j.IsCurrent
                                                        && j.Department.Department.Contains(searchTerm)) ||
                        _context.TB_JobHistorys.Any(j => j.StaffID == m.StaffID
                                                        && j.IsCurrent
                                                        && j.Position.Position.Contains(searchTerm))
                    );
                }
            }

            return query;
        }

        [HttpGet]
        public IActionResult SearchStaffs(string searchCriteria, string searchTerm, int pg = 1)
        {
            var query = BuildQuery(searchCriteria, searchTerm);

            // Fetch current job histories for the search results
            var staffIds = query.Select(s => s.StaffID).ToList();
            var currentJobs = _context.TB_JobHistorys
                .Where(j => staffIds.Contains(j.StaffID) && j.IsCurrent)
                .Include(j => j.Department)
                .Include(j => j.Position)
                .ToList();
          
            // Get total count of search results
            int resultCount = query.Count();

            const int pageSize = 20;
            var pager = new Pager(query.Count(), pg, pageSize);

            var recSkip = (pg - 1) * pageSize;
            var searchResults = query
                .Skip(recSkip)
                .Take(pager.PageSize)
                .ToList();

            foreach (var staff in searchResults)
            {
                var currentJob = currentJobs.FirstOrDefault(j => j.StaffID == staff.StaffID);
                staff.CurrentDepartment = currentJob?.Department?.Department;
                staff.CurrentPosition = currentJob?.Position?.Position;
            }

            SetSearchCriteriaItemsInViewBag(searchCriteria);


            ViewBag.Pager = pager;
            ViewBag.searchCriteria = searchCriteria;
            ViewBag.searchTerm = searchTerm;

            ViewBag.ResultCount = resultCount;

            return View("List", searchResults);
        }

        private byte[] GenerateExcelData(List<TB_Staff> staffList, string worksheetName)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(worksheetName);

                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;

                worksheet.Cell(1, 1).Value = "No";
                worksheet.Cell(1, 2).Value = "Staff Id";
                worksheet.Cell(1, 3).Value = "Name";
                worksheet.Cell(1, 4).Value = "Department";
                worksheet.Cell(1, 5).Value = "Position";
                worksheet.Cell(1, 6).Value = "FatherName";
                worksheet.Cell(1, 7).Value = "DateOfBirth";
                worksheet.Cell(1, 8).Value = "NRC";
                worksheet.Cell(1, 9).Value = "Age";
                worksheet.Cell(1, 10).Value = "Religion";
                worksheet.Cell(1, 11).Value = "VisibleMark";
                worksheet.Cell(1, 12).Value = "Address";
                worksheet.Cell(1, 13).Value = "Phone";
                worksheet.Cell(1, 14).Value = "StartedDate";
                worksheet.Cell(1, 15).Value = "Responsibility";

                headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                for (int i = 0; i < staffList.Count; i++)
                {
                    var staff = staffList[i];

                    worksheet.Cell(i + 2, 1).Value = i + 1;
                    worksheet.Cell(i + 2, 2).Value = staff.StaffID;
                    worksheet.Cell(i + 2, 3).Value = staff.Name;
                    //worksheet.Cell(i + 2, 4).Value = staff.Department?.Department;
                    //worksheet.Cell(i + 2, 5).Value = staff.Position?.Position;
                    worksheet.Cell(i + 2, 4).Value = staff.CurrentDepartment; 
                    worksheet.Cell(i + 2, 5).Value = staff.CurrentPosition;
                    worksheet.Cell(i + 2, 6).Value = staff.FatherName;
                    worksheet.Cell(i + 2, 7).Value = staff.DateOfBirth;
                    worksheet.Cell(i + 2, 8).Value = staff.NRC;
                    worksheet.Cell(i + 2, 9).Value = staff.Age;
                    worksheet.Cell(i + 2, 10).Value = staff.Religion;
                    worksheet.Cell(i + 2, 11).Value = staff.VisibleMark;
                    worksheet.Cell(i + 2, 12).Value = staff.Address;
                    worksheet.Cell(i + 2, 13).Value = staff.Phone;
                    worksheet.Cell(i + 2, 14).Value = staff.StartedDate;
                    worksheet.Cell(i + 2, 15).Value = staff.Responsibility;

                    worksheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                worksheet.Columns().Width = 20;


                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    return stream.ToArray();
                }
            }
        }
          
        [HttpGet]
        public IActionResult ExcelAllStaffExport()
        {
            // Fetch non-deleted staffs
            var allStaff = _context.TB_Staffs
                .Where(staff => staff.isDeleted == false).OrderByDescending(s => s.StaffPkid)
                .ToList();

            // Get current job data for all staff
            var staffIds = allStaff.Select(s => s.StaffID).ToList();
            var currentJobs = _context.TB_JobHistorys
               .Where(j => staffIds.Contains(j.StaffID) && j.IsCurrent)
                .Include(j => j.Department)
                .Include(j => j.Position)
                .ToList();

            // Assign CurrentDepartment/CurrentPosition
            foreach (var staff in allStaff)
            {
                var currentJob = currentJobs.FirstOrDefault(j => j.StaffID == staff.StaffID);
                staff.CurrentDepartment = currentJob?.Department?.Department;
                staff.CurrentPosition = currentJob?.Position?.Position;
            }

            var excelData = GenerateExcelData(allStaff, "AllStaff");
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AllStaff.xlsx");
        }

        [HttpGet]
        public IActionResult ExcelExportSearchResult(string searchCriteria, string searchTerm)
        {
            var query = BuildQuery(searchCriteria, searchTerm);
            var searchResults = query.ToList();

            // Get current job data for search results
            var staffIds = searchResults.Select(s => s.StaffID).ToList();
            var currentJobs = _context.TB_JobHistorys
               .Where(j => staffIds.Contains(j.StaffID) && j.IsCurrent)
                .Include(j => j.Department)
                .Include(j => j.Position)
                .ToList();

            // Assign CurrentDepartment/CurrentPosition
            foreach (var staff in searchResults)
            {
                var currentJob = currentJobs.FirstOrDefault(j => j.StaffID == staff.StaffID);
                staff.CurrentDepartment = currentJob?.Department?.Department;
                staff.CurrentPosition = currentJob?.Position?.Position;
            }

            var excelData = GenerateExcelData(searchResults, "SearchResults");
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SearchResults.xlsx");
        }

        [HttpGet]
        public IActionResult StaffBenefitCalculation(string staffID, int month)
        {
            if (string.IsNullOrEmpty(staffID)) return NotFound();

            // Include StaffBenefit relation
            var staff = _context.TB_Staffs
                .Include(s => s.StaffBenefit)
                .FirstOrDefault(s => s.StaffID == staffID);

            if (staff == null)
            {
                TempData["ErrorMessage"] = "The Staff ID does not exist.";
                return RedirectToAction("List", "Home");
            }

            // Set current year (2025 as requested)
            int year = 2025;
            DateTime monthStart = new DateTime(year, month, 1);
            DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);

            // Fetch excess leaves within the selected month
            var excessLeaves = _context.TB_StaffLeaves
                .Include(l => l.LeaveType)
                .Where(l => l.StaffID == staff.StaffID &&
                            l.LeaveType.LeaveTypeName == "Excess Leave" &&
                            l.LeaveDateFrom <= monthEnd &&
                            l.LeaveDateTo >= monthStart)
                .ToList();

            // Calculate leave days within the selected month
            int totalExcessLeaveDays = 0;
            foreach (var leave in excessLeaves)
            {
                // Adjust dates to fall within the selected month
                DateTime leaveStart = leave.LeaveDateFrom > monthStart ? leave.LeaveDateFrom.Value : monthStart;
                DateTime leaveEnd = leave.LeaveDateTo < monthEnd ? leave.LeaveDateTo.Value : monthEnd;

                // Calculate days (inclusive)
                totalExcessLeaveDays += (int)(leaveEnd - leaveStart).TotalDays + 1;
            }

            ViewBag.TotalExcessLeaveDays = totalExcessLeaveDays;
            ViewBag.SelectedMonth = month;
            ViewBag.SelectedYear = year;
            ViewBag.MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

            return View(staff);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSalary(TB_Salary salary, string redirectStaffID, int redirectMonth)
        {
            try
            {
                salary.IsDeleted = false;
                salary.CreatedDate = DateTime.Now;

                _context.TB_Salaries.Add(salary);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Salary record saved successfully!";

                return RedirectToAction("StaffBenefitCalculation", new
                {
                    staffID = redirectStaffID,
                    month = redirectMonth
                });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error saving salary: {ex.Message}";
                return RedirectToAction("StaffBenefitCalculation", new
                {
                    staffID = redirectStaffID,
                    month = redirectMonth
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(TB_Payroll payroll, string redirectStaffID, int redirectMonth)
        {
            try
            {
                if (payroll == null || !ModelState.IsValid)
                {
                    return BadRequest("Invalid data.");
                }

                // Find existing payroll record for same staff, month and year
                var existingPayroll = await _context.TB_Payrolls
                    .FirstOrDefaultAsync(p =>
                        p.StaffID == payroll.StaffID &&
                        p.MonthOfSalary == payroll.MonthOfSalary &&
                        p.YearOfSalary == payroll.YearOfSalary);

                if (existingPayroll != null)
                {
                    // Update existing record
                    existingPayroll.BaseSalary = payroll.BaseSalary;
                    existingPayroll.SocialSecurityDeduction = payroll.SocialSecurityDeduction;
                    existingPayroll.RiceOilDeduction = payroll.RiceOilDeduction;
                    existingPayroll.LeaveDeduction = payroll.LeaveDeduction;
                    existingPayroll.Deductions = payroll.Deductions;
                    existingPayroll.NetSalary = payroll.NetSalary;
                    existingPayroll.PaymentDate = DateTime.Now; 

                    _context.TB_Payrolls.Update(existingPayroll);
                }
                else
                {
                    // Add new record
                    payroll.PaymentDate = DateTime.Now;
                    payroll.IsDeleted = false;
                    payroll.CreatedDate = DateTime.Now;

                    _context.TB_Payrolls.Add(payroll);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Payment processed successfully!";

                return RedirectToAction("StaffBenefitCalculation", new
                {
                    staffID = redirectStaffID,
                    month = redirectMonth
                });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error processing payment: {ex.Message}";
                return RedirectToAction("StaffBenefitCalculation", new
                {
                    staffID = redirectStaffID,
                    month = redirectMonth
                });
            }
        }

        public IActionResult SsbList(int pg = 1)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Account");
            }

            List<TB_Staff> result = _context.TB_Staffs
               .Where(p => p.isDeleted == false).OrderByDescending(s => s.StaffPkid).ToList();

            const int pageSize = 20;
            if (pg < 1)
                pg = 1;

            int recsCount = result.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = result.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            return View(data);
        }

        private static DateTime? ParseMyanmarDate(string myanmarDate)
        {
            if (string.IsNullOrWhiteSpace(myanmarDate))
                return null;

            // Myanmar digit map (Unicode characters)
            var myanmarDigits = new Dictionary<char, char>
            {
                {'၀', '0'}, {'၁', '1'}, {'၂', '2'}, {'၃', '3'}, {'၄', '4'},
                {'၅', '5'}, {'၆', '6'}, {'၇', '7'}, {'၈', '8'}, {'၉', '9'}
            };

            // Convert Myanmar digits to Western digits
            var englishDate = new string(myanmarDate.Select(c =>
                myanmarDigits.ContainsKey(c) ? myanmarDigits[c] : c
            ).ToArray());

            // Parse as dd-MM-yyyy (day first)
            if (DateTime.TryParseExact(englishDate,
                new[] { "dd-MM-yyyy", "d-MM-yyyy", "dd-M-yyyy", "d-M-yyyy" },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime result))
            {
                return result;
            }

            return null; // Return null if parsing fails
        }

        private static string CalculateAge(DateTime? dateOfBirth)
        {
            if (!dateOfBirth.HasValue)
                return null;

            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Value.Year;

            if (dateOfBirth.Value.Date > today.AddYears(-age))
                age--;

            return age.ToString(); 
        }

        private int GetNextSequenceNumber(string prefix)
        {
            int maxSequence = 0;

            var existingIds = _context.TB_Staffs
                .Where(s => s.StaffID.StartsWith(prefix) && s.StaffID.Length == 8)
                .Select(s => s.StaffID.Substring(5, 3))
                .ToList();

            foreach (var idSuffix in existingIds)
            {
                if (int.TryParse(idSuffix, out int sequence) && sequence > maxSequence)
                {
                    maxSequence = sequence;
                }
            }

            return maxSequence + 1;
        }

        public IActionResult ImportStaffData()
        {
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "excel", "staff3.xlsx");

            using (var workbook = new XLWorkbook(filePath))
            {
                foreach (var worksheet in workbook.Worksheets)
                {
                    foreach (var row in worksheet.RowsUsed().Skip(2))
                    {
                        if (row.Cell(2).IsEmpty()) continue;

                        var dob = ParseMyanmarDate(row.Cell(9).GetString());
                        string staffId = row.Cell(2).GetString();

                        var staff = new TB_Staff
                        {
                            StaffID = staffId,
                            StaffType = staffId.StartsWith("D25") ? "နေ့စား" : "အချိန်ပြည့်", // Conditional assignment
                            Name = row.Cell(3).GetString(),
                            FatherName = row.Cell(6).GetString(),
                            MotherName = row.Cell(7).GetString(),
                            NRC = row.Cell(8).GetString(),
                            DateOfBirth = dob,
                            Age = CalculateAge(dob),
                            LevelOfEducation = row.Cell(10).GetString(),
                            SpouseAndChildrenNames = row.Cell(11).GetString(),
                            Address = row.Cell(12).GetString(),
                            Phone = row.Cell(13).GetString(),
                            Remarks = row.Cell(15).GetString(),
                            CreatedDate = DateTime.Now,
                            CreatedBy = 0,
                            isDeleted = false
                        };
                        _context.Add(staff);
                    }
                }
            }
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Import successful!";
            return RedirectToAction("Index");
        }
        public IActionResult ImportDepartmentAndPosition()
        {
            _logger?.LogInformation("Logging test: ImportDepartmentAndPosition method was triggered.");
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "excel", "staff4.xlsx");

            // Create logs directory if it doesn't exist
            string logDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "excel", "logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Create log file with timestamp
            string logFilePath = Path.Combine(logDirectory, $"Import_Log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

            using (var logFile = new StreamWriter(logFilePath))
            {
                logFile.WriteLine($"Import Log - {DateTime.Now:M/d/yyyy h:mm:ss tt}");
                logFile.WriteLine("===================================");
                logFile.WriteLine();

                var departments = _context.TB_Departments
                    .Where(d => !d.isDeleted)
                    .AsEnumerable()
                    .GroupBy(d => NormalizeString(d.Department))
                    .ToDictionary(g => g.Key, g => g.First().DepartmentPkid);

                var positions = _context.TB_Positions
                    .Where(p => !p.IsDeleted)
                    .AsEnumerable()
                    .GroupBy(p => NormalizeString(p.Position))
                    .ToDictionary(g => g.Key, g => g.First().PositionPkid);

                int successCount = 0;
                int errorCount = 0;
                var importErrors = new List<string>();
                var missingDepartments = new HashSet<string>();
                var missingPositions = new HashSet<string>();

                using (var workbook = new XLWorkbook(filePath))
                {
                    foreach (var worksheet in workbook.Worksheets)
                    {
                        foreach (var row in worksheet.RowsUsed().Skip(2))
                        {
                            string staffId = row.Cell(2).GetString();
                            string rawPosition = row.Cell(4).GetString();
                            string rawDepartment = row.Cell(14).GetString();
                            string positionName = NormalizeString(rawPosition);
                            string departmentName = NormalizeString(rawDepartment);
                            DateTime? fromDate = ParseMyanmarDate(row.Cell(5).GetString());

                            bool hasError = false;
                            string errorMessage = "";

                            if (!positions.ContainsKey(positionName))
                            {
                                errorMessage = $"[ERROR] Row {row.RowNumber()} (Staff: {staffId}): Missing position '{rawPosition}'";
                                missingPositions.Add(rawPosition);
                                hasError = true;
                            }

                            if (!departments.ContainsKey(departmentName))
                            {
                                errorMessage = $"[ERROR] Row {row.RowNumber()} (Staff: {staffId}): Missing department '{rawDepartment}'";
                                missingDepartments.Add(rawDepartment);
                                hasError = true;
                            }

                            if (hasError)
                            {
                                logFile.WriteLine(errorMessage);
                                importErrors.Add(errorMessage);
                                errorCount++;
                                continue;
                            }

                            _context.TB_JobHistorys.Add(new TB_JobHistory
                            {
                                StaffID = staffId,
                                FromDate = fromDate,
                                PositionId = positions[positionName],
                                DepartmentId = departments[departmentName],
                                IsCurrent = true,
                                CreatedDate = DateTime.Now,
                                CreatedBy = 0,
                                IsDeleted = false
                            });

                            successCount++;
                        }
                    }
                }

                _context.SaveChanges();

                // Write summary to log file
                logFile.WriteLine();
                logFile.WriteLine("Import Summary:");
                logFile.WriteLine("===============");
                logFile.WriteLine($"✅ Successfully imported: {successCount} rows");
                logFile.WriteLine($"❌ Failed to import: {errorCount} rows");
                logFile.WriteLine($"❌ Missing departments: {string.Join(", ", missingDepartments)}");
                logFile.WriteLine($"❌ Missing positions: {string.Join(", ", missingPositions)}");
            }

            return RedirectToAction("Index");
        }
        private static string NormalizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string clean = input
                .Replace("\r\n", " ")
                .Replace("\n", " ")
                .Replace("\r", " ")
                .Replace("\t", " ")
                .Trim();

            // Collapse multiple spaces to a single space
            while (clean.Contains("  "))
                clean = clean.Replace("  ", " ");

            // Optional: Use regex for collapsing all whitespace
            // clean = Regex.Replace(clean, @"\s+", " ");

            return clean
                .Replace('\u106A', '\u1009')  // Normalize specific Myanmar characters
                .Replace('\u106B', '\u100A')
                .Replace('\u108F', '\u1014')
                .Normalize(NormalizationForm.FormC)
                .ToLowerInvariant();
        }

        public IActionResult ImportSocialSecurityData()
        {
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "excel", "ssb3.xlsx");
            int successCount = 0;
            var importErrors = new List<string>();

            using (var workbook = new XLWorkbook(filePath))
            {
                foreach (var worksheet in workbook.Worksheets)
                {
                    foreach (var row in worksheet.RowsUsed().Skip(2))
                    {
                        try
                        {
                            if (row.Cell(2).IsEmpty())
                            {
                                importErrors.Add($"Skipped row {row.RowNumber()}: Missing StaffID");
                                continue;
                            }

                            string staffId = row.Cell(2).GetString();
                            var staff = _context.TB_Staffs.FirstOrDefault(s => s.StaffID == staffId);

                            if (staff == null)
                            {
                                importErrors.Add($"Row {row.RowNumber()}: Staff not found (ID: {staffId})");
                                continue;
                            }

                            staff.SocialSecurity = true;
                            staff.ErSSN = row.Cell(5).GetString();
                            staff.EeSSN = row.Cell(7).GetString();
                            staff.Gender = row.Cell(11).GetString();
                            staff.Minc = GetDecimalValue(row.Cell(12));
                            staff.SS1EeRate = GetDecimalValue(row.Cell(13));
                            staff.SS1ErRate = GetDecimalValue(row.Cell(14));
                            staff.SS1EeConAmt = GetDecimalValue(row.Cell(15));
                            staff.SS1ErConAmt = GetDecimalValue(row.Cell(16));
                            staff.SS2EeRate = GetDecimalValue(row.Cell(17));
                            staff.SS2ErRate = GetDecimalValue(row.Cell(18));
                            staff.SS2EeConAmt = GetDecimalValue(row.Cell(19));
                            staff.SS2ErConAmt = GetDecimalValue(row.Cell(20));
                            staff.TotalConAmt = GetDecimalValue(row.Cell(21));

                            _context.Update(staff);
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            importErrors.Add($"Row {row.RowNumber()}: {ex.Message}");
                        }
                    }
                }
                _context.SaveChanges();
            }

            if (importErrors.Any())
            {
                TempData["ErrorMessage"] = $"Imported {successCount} records with {importErrors.Count} errors: " +
                                           string.Join("; ", importErrors);
            }
            else
            {
                TempData["SuccessMessage"] = $"Successfully imported {successCount} social security records";
            }

            return RedirectToAction("Index");
        }

        private decimal? GetDecimalValue(IXLCell cell)
        {
            if (cell.IsEmpty()) return null;

            return cell.DataType switch
            {
                XLDataType.Number => cell.GetValue<decimal>(),
                XLDataType.Text => decimal.TryParse(cell.GetString(), out decimal result) ? result : null,
                _ => null
            };
        }


    }

}