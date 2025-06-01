using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

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

            int totalStaffCount = _context.TB_Staffs.Where(b => b.isDeleted == false).Count();
            //int totalPositionCount = _context.TB_Positions.Where(b => b.isDeleted == false).Count();
            //int totalDepartmentCount = _context.TB_Departments.Where(b => b.isDeleted == false).Count();
            int totalStaffPunishmentCount = _context.TB_StaffPunishments.Where(b => b.IsDeleted == false).Count();
            int totalStaffLeaveCount = _context.TB_StaffLeaves.Where(b => b.IsDeleted == false).Count();

            ViewBag.totalStaffCount = totalStaffCount;
            //ViewBag.totalPositionCount = totalPositionCount;
            //ViewBag.totalDepartmentCount = totalDepartmentCount;
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
                //.Include(d => d.Department)
                //.Include(p => p.Position)
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

        //private List<SelectListItem> GetDepartments()
        //{
        //    var lstDepartments = new List<SelectListItem>();

        //    List<TB_Department> TB_Departments = _context.TB_Departments.Where(d => d.isDeleted == false).ToList();

        //    lstDepartments = TB_Departments.Select(d => new SelectListItem()
        //    {
        //        Value = d.DepartmentPkid.ToString(),
        //        Text = d.Department ?? "Unknown Department"
        //    }).ToList();

        //    var defItem = new SelectListItem()
        //    {
        //        Value = "",
        //        Text = "----ဌာနရွေးချယ်ပါ----"
        //    };

        //    lstDepartments.Insert(0, defItem);

        //    return lstDepartments;
        //}

        //private string GetDepartmentName(int DepartmentPkid)
        //{
        //    string departmentName = _context.TB_Departments.Where(d => d.DepartmentPkid == DepartmentPkid).SingleOrDefault().Department;
        //    return departmentName;
        //}

        //private string GetPositionName(int PositionPkid)
        //{
        //    string positionName = _context.TB_Positions.Where(p => p.PositionPkid == PositionPkid).SingleOrDefault().Position;
        //    return positionName ?? "Position Not Found";
        //}

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
            //ViewBag.DepartmentPkid = GetDepartments();
            //ViewBag.PositionPkid = GetPositions();
            ViewBag.StaffBenefitId = GetStaffBenefits();
            ViewBag.BenefitAmounts = GetStaffBenefitAmounts();
            return View(Staff);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(TB_Staff staff)
        {
            if (!ModelState.IsValid)
            {
                //ViewBag.DepartmentPkid = GetDepartments();
                //ViewBag.PositionPkid = GetPositions();
                ViewBag.StaffBenefitId = GetStaffBenefits();
                ViewBag.BenefitAmounts = GetStaffBenefitAmounts();
                return View(staff);
            }

            DateTime today = DateTime.UtcNow.Date;
            // Validate that DateOfBirth is not in the future
            if (staff.DateOfBirth.HasValue && staff.DateOfBirth.Value > today)
            {
                //ViewBag.DepartmentPkid = GetDepartments();
                //ViewBag.PositionPkid = GetPositions();
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
            existingMember.FatherName = editedStaff.FatherName;
            existingMember.DateOfBirth = editedStaff.DateOfBirth;
            existingMember.NRC = editedStaff.NRC;
            existingMember.Age = editedStaff.Age;
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

            const int pageSize = 5;
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
                .Where(staff => staff.isDeleted == false)
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
                return RedirectToAction("List");
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
        public async Task<IActionResult> SaveSalary([FromBody] TB_Salary salary)
        {
            try
            {
                if (salary == null || !ModelState.IsValid)
                {
                    return BadRequest("Invalid data.");
                }

                // Set additional fields
                salary.IsDeleted = false;
                salary.CreatedDate = DateTime.Now;

                // Add to the database
                _context.TB_Salaries.Add(salary);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while saving salary details.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] TB_Payroll payroll)
        {
            try
            {
                if (payroll == null || !ModelState.IsValid)
                {
                    return BadRequest("Invalid data.");
                }

                // Check if a record with the same StaffID already exists
                var existingPayroll = await _context.TB_Payrolls
                    .FirstOrDefaultAsync(p => p.StaffID == payroll.StaffID);

                if (existingPayroll != null)
                {
                    // Update existing record
                    existingPayroll.NetSalary = payroll.NetSalary;
                    existingPayroll.PaymentDate = payroll.PaymentDate;
                }
                else
                {
                    // Add new record
                    payroll.IsDeleted = false;
                    _context.TB_Payrolls.Add(payroll);
                }

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while processing payment.");
            }
        }

    }

}