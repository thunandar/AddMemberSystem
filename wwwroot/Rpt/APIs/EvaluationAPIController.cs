using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RepositoryServicePattern.Core.Models;
using RepositoryServicePattern.Core.Services;
using RepositoryServicePattern.Core.Domain;
using Newtonsoft.Json;
namespace MOBA_PMS.APIs
{

    public class EvaluationAPIController : Controller
    {
       

   
        public class ResErrorMsg
        {
            public string ResCode { get; set; }
            public string ResDesc { get; set; }
        }

        public AllowProjectService allowprojectServices = new AllowProjectService();


        [HttpPost]
        [Route("api/User/Login")]

        public IActionResult UserLogIn([FromQuery]string UserName, [FromQuery] string Password)
        {
            ResApiUserModel resApiUserModel = new ResApiUserModel();

            if (string.IsNullOrEmpty(UserName))
            {
                resApiUserModel = new ResApiUserModel();
                resApiUserModel.resModel.ResDesc = "UserName must be contain.";
                resApiUserModel.resModel.ResCode = "012";
                return new JsonResult(new
                {
                    resApiUserModel,
                    resApiUserModel.resModel

                });
            }
            if (string.IsNullOrEmpty(Password))
            {             
                resApiUserModel = new ResApiUserModel();
                resApiUserModel.resModel.ResDesc = "Password must be contain.";
                resApiUserModel.resModel.ResCode = "012";
                return new JsonResult(new
                {
                    resApiUserModel,
                    resApiUserModel.resModel

                });
            }
            UserModel requestModel = new UserModel();

            requestModel.UserName = UserName;
            requestModel.UserPwd = Password;
            LogInProcessService business = new LogInProcessService();
            var dataReturn = business.LogInProcess(requestModel, HelperModels.ConfigSettings.conStr.ToString());
            if (dataReturn.ResCode == "000")
            {
                resApiUserModel = new ResApiUserModel();
                resApiUserModel.UserFullName = dataReturn.userModels[0].UserFullName;
                resApiUserModel.DeptName = dataReturn.userModels[0].DeptName;
                resApiUserModel.DeptId = dataReturn.userModels[0].DeptId;
                resApiUserModel.SectionId = dataReturn.userModels[0].SectionId;
                resApiUserModel.SectionName = dataReturn.userModels[0].SectionName;
                resApiUserModel.SubSectionId = dataReturn.userModels[0].SubSectionId;
                resApiUserModel.SubSectionName = dataReturn.userModels[0].SubSectionName;
                resApiUserModel.RegionId = dataReturn.userModels[0].RegionId;
                resApiUserModel.RegionName = dataReturn.userModels[0].RegionName;
                resApiUserModel.DistrictId = dataReturn.userModels[0].DistrictId;
                resApiUserModel.DistrictName = dataReturn.userModels[0].DistrictName;
                resApiUserModel.TownshipId = dataReturn.userModels[0].TownshipId;
                resApiUserModel.TownshipName = dataReturn.userModels[0].TownshipName;
                resApiUserModel.UserId = dataReturn.userModels[0].UserId;
                resApiUserModel.UserName = dataReturn.userModels[0].UserName;
                resApiUserModel.UserStatus = dataReturn.userModels[0].UserStatus;
                resApiUserModel.AccessKey = dataReturn.userModels[0].AccessKey;
                resApiUserModel.AccessRegion = dataReturn.userModels[0].AccessRegion;
                resApiUserModel.AccessRegionSubSectionId = dataReturn.userModels[0].AccessRegionSubSectionId;
                resApiUserModel.PositionName = dataReturn.userModels[0].PositionName;
                resApiUserModel.ParentId = dataReturn.userModels[0].ParentId;
                resApiUserModel.StaticLvl = dataReturn.userModels[0].StaticLvl;
                resApiUserModel.RoleId = dataReturn.userModels[0].RoleId;
                resApiUserModel.RoleName = dataReturn.userModels[0].RoleName;

                resApiUserModel.resModel.ResDesc = "Log In Successfully.";
                resApiUserModel.resModel.ResCode = "000";
            }
            else
            {
             
                resApiUserModel = new ResApiUserModel();
                resApiUserModel.resModel.ResDesc = "UserName and Password are wrong!";
                resApiUserModel.resModel.ResCode = "017";
                return new JsonResult(new
                {
                    resApiUserModel,
                    resApiUserModel.resModel

                });
            }


            return new JsonResult(new
            {
                resApiUserModel,
                resApiUserModel.resModel

            });

        }
        [HttpPost]
        [Route("api/Evaluation/EvaluationProjectLists")]
        public IActionResult EvaluationProjectList([FromHeader]string AccessKey, [FromQuery] string UserId, [FromQuery] string PTGpTypeGrpId, [FromQuery] string SId)
        {
            AllowProjectModelForApi resAllowProject = new AllowProjectModelForApi();
            if (string.IsNullOrEmpty(AccessKey))
            {
                resAllowProject = new AllowProjectModelForApi();
                resAllowProject.resModel.ResCode = "012";
                resAllowProject.resModel.ResDesc = "Invalid Access!";

                return new JsonResult(new
                {
                    resAllowProject.allowProjectModels,
                    resAllowProject.resModel

                });
            }
            if (string.IsNullOrEmpty(UserId))
            {
                resAllowProject = new AllowProjectModelForApi();
                resAllowProject.resModel.ResCode = "012";
                resAllowProject.resModel.ResDesc = "Invalid User!";
                return new JsonResult(new
                {
                    resAllowProject.allowProjectModels,
                    resAllowProject.resModel

                });
            }
            UserModel requestModel = new UserModel();

            resAllowProject.allowProjectModels = allowprojectServices.AllowProjectListing(UserId, HelperModels.ConfigSettings.conStr.ToString()).AllowProjectModels;
            resAllowProject.allowProjectModels = resAllowProject.allowProjectModels.Where(x => x.PTypeGrpId == PTGpTypeGrpId && x.SubPTGId == SId).ToList();
            //var dataReturn = allowprojectServices.AllowProjectListing(UserId, HelperModels.ConfigSettings.conStr.ToString());

            resAllowProject.resModel.ResCode = "000";
            resAllowProject.resModel.ResDesc = "Success";
            if(resAllowProject.allowProjectModels == null)
            {
                resAllowProject = new AllowProjectModelForApi();
                resAllowProject.resModel.ResCode = "011";
                resAllowProject.resModel.ResDesc = "There is no Data to show!";
                return new JsonResult(new
                {
                    resAllowProject.allowProjectModels,
                    resAllowProject.resModel

                });
            }

            return new JsonResult(new
            {
                resAllowProject.allowProjectModels,
                resAllowProject.resModel,

            });

        }

        [HttpPost]
        [Route("api/Evaluation/RecordHistoryList")]
        public IActionResult RecordHistoryList([FromHeader] string AccessKey, [FromQuery] string UserId)
        {
            AllowProjectModelForApi resAllowProject = new AllowProjectModelForApi();
            if (string.IsNullOrEmpty(AccessKey))
            {
                resAllowProject = new AllowProjectModelForApi();
                resAllowProject.resModel.ResCode = "012";
                resAllowProject.resModel.ResDesc = "Invalid Access!";

                return new JsonResult(new
                {
                    resAllowProject.allowProjectModels,
                    resAllowProject.resModel

                });
            }
            if (string.IsNullOrEmpty(UserId))
            {
                resAllowProject = new AllowProjectModelForApi();
                resAllowProject.resModel.ResCode = "012";
                resAllowProject.resModel.ResDesc = "Invalid User!";
                return new JsonResult(new
                {
                    resAllowProject.allowProjectModels,
                    resAllowProject.resModel

                });
            }
            UserModel requestModel = new UserModel();

            RepositoryServicePattern.Core.Services.NewProposalService ProposalLogic = new NewProposalService();
            var dataReturn = ProposalLogic.ProposalListingForSubmitEvaluation(UserId, HelperModels.ConfigSettings.conStr.ToString()).evaluationHeadDetailDatas;

            RecordHistoryListModelForApi recordHistoryList = new RecordHistoryListModelForApi();
            recordHistoryList.evaluationHeadDetailDatas = dataReturn;

            recordHistoryList.resModel.ResCode = "000";
            recordHistoryList.resModel.ResDesc = "Success";
            if (recordHistoryList.evaluationHeadDetailDatas == null)
            {
                recordHistoryList = new RecordHistoryListModelForApi();
                recordHistoryList.resModel.ResCode = "011";
                recordHistoryList.resModel.ResDesc = "There is no Data to show!";
                return new JsonResult(new
                {
                    resAllowProject.resModel

                });
            }

            return new JsonResult(new
            {
                recordHistoryList.evaluationHeadDetailDatas,
                recordHistoryList.resModel,

            });

        }

        [HttpPost]
        [Route("api/Evaluation/RecordHistoryDetailList")]
        public IActionResult RecordHistoryDetailList([FromHeader] string AccessKey, [FromQuery] string EvaluationRecordId)
        {
            AllowProjectModelForApi resAllowProject = new AllowProjectModelForApi();
            if (string.IsNullOrEmpty(AccessKey))
            {
                resAllowProject = new AllowProjectModelForApi();
                resAllowProject.resModel.ResCode = "012";
                resAllowProject.resModel.ResDesc = "Invalid Access!";

                return new JsonResult(new
                {
                    resAllowProject.allowProjectModels,
                    resAllowProject.resModel

                });
            }
            if (string.IsNullOrEmpty(EvaluationRecordId))
            {
                resAllowProject = new AllowProjectModelForApi();
                resAllowProject.resModel.ResCode = "012";
                resAllowProject.resModel.ResDesc = "Invalid EvaluationRecord!";
                return new JsonResult(new
                {
                    resAllowProject.allowProjectModels,
                    resAllowProject.resModel

                });
            }
            UserModel requestModel = new UserModel();

            EvaluationRecordService evaluation = new EvaluationRecordService();
            var dataReturn = evaluation.EvaluationList(HelperModels.ConfigSettings.conStr.ToString()).evaluationDetailDatas.Where(x => x.EvaluationRecordId == EvaluationRecordId).ToList();

            RecordHistoryDetailListModelForApi recordHistoryDetailList = new RecordHistoryDetailListModelForApi();
            recordHistoryDetailList.evaluationDetailDatas = dataReturn;

            recordHistoryDetailList.resModel.ResCode = "000";
            recordHistoryDetailList.resModel.ResDesc = "Success";
            if (recordHistoryDetailList.evaluationDetailDatas == null)
            {
                recordHistoryDetailList = new RecordHistoryDetailListModelForApi();
                recordHistoryDetailList.resModel.ResCode = "011";
                recordHistoryDetailList.resModel.ResDesc = "There is no Data to show!";
                return new JsonResult(new
                {
                    resAllowProject.resModel

                });
            }

            return new JsonResult(new
            {
                recordHistoryDetailList.evaluationDetailDatas,
                recordHistoryDetailList.resModel,

            });

        }

        [HttpPost]
        [Route("api/Evaluation/ProjectTypeList")]
        public IActionResult ProjectTypeList([FromHeader]string AccessKey)
        {
            RepositoryServicePattern.Core.Services.PTypeGrpService pTypeGrpService = new PTypeGrpService();
            ProjectTypeListForApi ResData = new ProjectTypeListForApi();

            if (string.IsNullOrEmpty(AccessKey))
            {
                ResData = new ProjectTypeListForApi();
                ResData.resModel.ResCode = "012";
                ResData.resModel.ResDesc = "Invalid Access!";

                return new JsonResult(new
                {
                    ResData.PTypeGrpResponseModel.PTypeGrpModels,
                    ResData.resModel

                });
            }

            try
            {

                ResData.PTypeGrpResponseModel = pTypeGrpService.PTypeGrpListing(null, HelperModels.ConfigSettings.conStr.ToString());
                if (ResData.PTypeGrpResponseModel != null)
                {
                    if(ResData.PTypeGrpResponseModel.PTypeGrpModels != null)
                    {
                        if(ResData.PTypeGrpResponseModel.PTypeGrpModels.Count() > 0)
                        {
                            ResData.resModel.ResCode = "000";
                            ResData.resModel.ResDesc = "ProjectTypeListAPI successfully acheived!";
                        }
                    }
                }
                return new JsonResult(new
                {

                    ResData.PTypeGrpResponseModel.PTypeGrpModels,
                    ResData.resModel,

                });
            }
            catch (Exception ex)
            {
                ResData = new ProjectTypeListForApi();
                ResData.resModel.ResCode = "999";
                ResData.resModel.ResDesc = ex.Message.ToString();

                return new JsonResult(new
                {
                    ResData.PTypeGrpResponseModel.PTypeGrpModels,
                    ResData.resModel

                });
            }




        }
        [HttpPost]
        [Route("api/Evaluation/SubProjectTypeList")]
        public IActionResult SubProjectTypeList([FromHeader]string AccessKey, [FromQuery] string PTGpTypeGrpId)
        {
            RepositoryServicePattern.Core.Services.SubPTGService subPTGService = new SubPTGService();
            SubProjectTypeListForApi ResData = new SubProjectTypeListForApi();

            if (string.IsNullOrEmpty(AccessKey))
            {
                ResData = new SubProjectTypeListForApi();
                ResData.resModel.ResCode = "012";
                ResData.resModel.ResDesc = "Invalid Access!";

                return new JsonResult(new
                {
                    ResData.SubPTGResponseModel.SubPTGModels,
                    ResData.resModel

                });
            }
            if (string.IsNullOrEmpty(PTGpTypeGrpId))
            {
                ResData = new SubProjectTypeListForApi();
                ResData.resModel.ResCode = "012";
                ResData.resModel.ResDesc = "Invalid ProjectTypeGroup!";

                return new JsonResult(new
                {
                    ResData.SubPTGResponseModel.SubPTGModels,
                    ResData.resModel

                });
            }
            try
            {

                ResData.SubPTGResponseModel = subPTGService.SubPTGListForApi(PTGpTypeGrpId, HelperModels.ConfigSettings.conStr.ToString());
                ResData.resModel.ResCode = ResData.SubPTGResponseModel.ResCode;
                ResData.resModel.ResDesc = ResData.SubPTGResponseModel.ResDesc;
                if (ResData.SubPTGResponseModel != null)
                {
                    if (ResData.SubPTGResponseModel.SubPTGModels != null)
                    {
                        if (ResData.SubPTGResponseModel.SubPTGModels.Count() > 0)
                        {
                            ResData.resModel.ResCode = "000";
                            ResData.resModel.ResDesc = "SubProjectTypeListAPI successfully acheived!";
                        }
                    }
                }
                return new JsonResult(new
                {

                    ResData.SubPTGResponseModel.SubPTGModels,
                    ResData.resModel,

                });
            }
            catch (Exception ex)
            {
                ResData = new SubProjectTypeListForApi();
                ResData.resModel.ResCode = "999";
                ResData.resModel.ResDesc = ex.Message.ToString();

                return new JsonResult(new
                {
                    ResData.SubPTGResponseModel.SubPTGModels,
                    ResData.resModel

                });
            }




        }

        [HttpPost]
        [Route("api/Evaluation/EvaluationProjectById")]
        public IActionResult AllowProjectByIdAPI([FromHeader]string AccessKey, [FromQuery] string TenderProjectId)
        {

            AllowProjectModelForApi resAllowProject = new AllowProjectModelForApi();
            if (string.IsNullOrEmpty(AccessKey))
            {
                resAllowProject = new AllowProjectModelForApi();
                resAllowProject.resModel.ResCode = "012";
                resAllowProject.resModel.ResDesc = "Invalid Access!";

                return new JsonResult(new
                {
                    resAllowProject.allowProjectModels,
                    resAllowProject.resModel

                });
            }

             try
            {

                var dataReturn = allowprojectServices.AllowProjectById(TenderProjectId, HelperModels.ConfigSettings.conStr.ToString());
                var EvaluationIdWithSub = dataReturn.subEvaluationModels.Select(x => x.EvaluationId).ToList();
                EvaluationTypeService evaluationTypeService = new EvaluationTypeService();
                dataReturn.evaluationTypeModels = evaluationTypeService.EvaluationTypeListing(string.Empty, HelperModels.ConfigSettings.conStr.ToString()).EvaluationTypeModels;
                
                var pairsEvaluation = from bItem in dataReturn.evaluationModels
                                      join aItem in dataReturn.subEvaluationModels
                                on new { bItem.EvaluationId } equals new { aItem.EvaluationId }
                                      select new { bItem, aItem };
                foreach (var pair in pairsEvaluation)
                    pair.bItem.HaveSub = 1;


                resAllowProject.resModel.ResCode = dataReturn.ResCode;
                resAllowProject.resModel.ResDesc = dataReturn.ResDesc;
                if (dataReturn != null)
                {
                    if (dataReturn.AllowProjectModels != null)
                    {
                        if (dataReturn.AllowProjectModels.Count() > 0)
                        {
                            if (dataReturn.AllowProjectModels[0].MeasureExtra == 2)
                            {
                                foreach (var subEvaluation in dataReturn.subEvaluationModels)
                                {
                                    subEvaluation.SubEvaluationValue = subEvaluation.SubEvaluationValue * dataReturn.AllowProjectModels[0].PropProjectUnit + subEvaluation.SubEvaluationValue * (dataReturn.AllowProjectModels[0].PropProjectSub / 8);
                                }
                            }
                        }
                    }
                }
                return new JsonResult(new
                {

                    dataReturn.AllowProjectModels,
                    dataReturn.evaluationModels,
                    dataReturn.subEvaluationModels,
                    dataReturn.evaluationTypeModels,
                    resAllowProject.resModel,

                });
            }
            catch(Exception ex)
            {
                resAllowProject = new AllowProjectModelForApi();
                resAllowProject.resModel.ResCode = "999";
                resAllowProject.resModel.ResDesc = ex.Message.ToString();

                return new JsonResult(new
                {
                    resAllowProject.allowProjectModels,
                    resAllowProject.resModel

                });
            }
    

          

        }


        public EvaluationRecordService evaluationrecordServices = new EvaluationRecordService();

    
        [HttpPost]
        [Route("api/Evaluation/NewEvaluationImageData")]
        public ActionResult NewEvaluationRecordSave([FromHeader]string AccessKey, [FromHeader]string UserId, [FromBody]List<EvaluationImageApiModel> evaluationImageApiModel)
        {
            ResModel resModel = new ResModel();


            if (string.IsNullOrEmpty(AccessKey))
            {


                resModel.ResCode = "010";
                resModel.ResDesc = "Request Parameter must not be null value";
                return new JsonResult(new
                {
                    resModel

                });
            }

            else if (evaluationImageApiModel == null)
            {
                resModel.ResCode = "010";
                resModel.ResDesc = "Request Parameter must not be null value";
                return new JsonResult(new
                {
                    resModel

                });
            }
      


            try
            {
                EvaluationRecordModel evaluationRecordModel = new EvaluationRecordModel();
                EvaluationImageModel evaluationImageModel = new EvaluationImageModel();
                RepositoryServicePattern.Core.Domain.EvaluationDBCommon.EvaluatoinImageDBCommon evaluationImageLogic = new EvaluationDBCommon.EvaluatoinImageDBCommon();

                foreach(var a in evaluationImageApiModel)
                {
                    evaluationImageModel = new EvaluationImageModel();
                    evaluationImageModel.EvaluationImageId = a.EvaluationImageId;
                    evaluationImageModel.EvaluationRecordId = a.EvaluationRecordId;
                    evaluationImageModel.OriginalName = a.OriginalName;
                    evaluationImageModel.FilePath = a.FilePath;
                    evaluationImageModel.FileType = a.FileType;
                    evaluationImageModel.ImageData = a.ImageData;
                    evaluationImageModel.InPara = 1;
                    evaluationImageModel.CreatedBy = UserId;
                    evaluationRecordModel.evaluationImageModels.Add(evaluationImageModel);
                }
                evaluationRecordModel.InPara = 1;


                var DataReturn = evaluationImageLogic.NewEvaluationImageSaveUpdateDelete(evaluationRecordModel, HelperModels.ConfigSettings.conStr.ToString());

                if(DataReturn == true)
                {
                    resModel.ResCode = "000";
                    resModel.ResDesc = "Image Data Save Successfully";
                    return new JsonResult(new
                    {
                        resModel,
                        

                    });
                }
                else
                {
                    resModel.ResCode = "999";
                    resModel.ResDesc = "Error Occured!";
                    return new JsonResult(new
                    {
                        resModel

                    });
                }


           


            }
            catch (Exception ex)
            {
                resModel.ResCode = "999";
                resModel.ResDesc = ex.Message;
                return new JsonResult(new
                {
                    resModel

                });
            }
        }
        [HttpPost]
        [Route("api/Evaluation/NewEvaluationRecordData")]
        public ActionResult NewEvaluationRecordSave([FromHeader]string AccessKey, [FromHeader]string UserId, [FromBody]EvaluationRecordApiModel evaluationRecordApiModel)
        {
            ResModel resModel = new ResModel();


            if (string.IsNullOrEmpty(AccessKey))
            {
               

                resModel.ResCode = "010";
                resModel.ResDesc = "Request Parameter must not be null value";
                return new JsonResult(new
                {
                    resModel

                });
            }
       
            else if (evaluationRecordApiModel == null)
            {
                resModel.ResCode = "010";
                resModel.ResDesc = "Request Parameter must not be null value";
                return new JsonResult(new
                {
                    resModel

                });
            }
        
       

            try
            {
                EvaluationRecordModel evaluationRecordModel = new EvaluationRecordModel();
                evaluationRecordModel.EvaluationRecordId = evaluationRecordApiModel.EvaluationRecordId;
                evaluationRecordModel.EvaluationRuleId = evaluationRecordApiModel.EvaluationRuleId;
                evaluationRecordModel.EvaluationTypeId = evaluationRecordApiModel.EvaluationTypeId;
                evaluationRecordModel.ProjectId = evaluationRecordApiModel.ProjectId;
                evaluationRecordModel.PTypeGrpId = evaluationRecordApiModel.PTypeGrpId;
                evaluationRecordModel.SubPTGId = evaluationRecordApiModel.SubPTGId;
                evaluationRecordModel.CompletedPer = evaluationRecordApiModel.CompletedPer;
                evaluationRecordModel.EvaluationRecordRemark = evaluationRecordApiModel.EvaluationRecordRemark;
                evaluationRecordModel.CreatedBy = UserId;
                evaluationRecordModel.InPara = 1;
                var dataReturns = evaluationrecordServices.NewEvaluationRecordSaveUpdateDelete(evaluationRecordModel, HelperModels.ConfigSettings.conStr.ToString());

             
                    resModel.ResCode = dataReturns.ResCode;
                    resModel.ResDesc = dataReturns.ResDesc;


                    return new JsonResult(new
                    {
                        resModel,
                        dataReturns.OutputEvaluationRecordId
                    });
               

            }
            catch (Exception ex)
            {
                resModel.ResCode = "999";
                resModel.ResDesc = ex.Message;
                return new JsonResult(new
                {
                    resModel

                });
            }
        }

        [HttpPost]
        [Route("api/Evaluation/NewEvaluationRecordDetail")]
        public ActionResult NewEvaluationRecordDetail([FromHeader]string AccessKey, [FromHeader]string UserId, [FromBody] List<EvaluationRecordDetailApiModel> evaluationRecordDetailApiModels)
        {
            ResEvaluationRecordDetailModel resModel = new ResEvaluationRecordDetailModel();
            EvaluationRecordDetailModel evaluationRecordDetailModel = new EvaluationRecordDetailModel();
            List<EvaluationRecordDetailModel> evaluationRecordDetailModels = new List<EvaluationRecordDetailModel>();
            if (string.IsNullOrEmpty(AccessKey))
            {


                resModel.ResCode = "010";
                resModel.ResDesc = "Request Parameter must not be null value";
                return new JsonResult(new
                {
                    resModel

                });
            }

            else if (evaluationRecordDetailApiModels == null)
            {
                resModel.ResCode = "010";
                resModel.ResDesc = "Request Parameter must not be null value";
                return new JsonResult(new
                {
                    resModel

                });
            }
            try
            {
                foreach (var e in evaluationRecordDetailApiModels)
                {
                    evaluationRecordDetailModel = new EvaluationRecordDetailModel();
                    evaluationRecordDetailModel.EvaluationRecordDetailId = Guid.NewGuid().ToString();
                    evaluationRecordDetailModel.CreatedBy = UserId;
                    evaluationRecordDetailModel.EvaluationRecordId = e.EvaluationRecordId;
                    evaluationRecordDetailModel.EvaluationId = e.EvaluationId;
                    evaluationRecordDetailModel.EvaluationRecordDetailDesc = e.EvaluationRecordDetailDesc;
                    evaluationRecordDetailModel.EvaluationRecordDetailValue = Convert.ToDecimal(e.EvaluationRecordDetailValueApi);
                    evaluationRecordDetailModel.EvaluationRecordDetailDescription = e.EvaluationRecordDetailDescription;
                    evaluationRecordDetailModel.EvaluationRecordDetailStatus = e.EvaluationRecordDetailStatus;
                    evaluationRecordDetailModel.InPara = 1;

                    evaluationRecordDetailModels.Add(evaluationRecordDetailModel);
                }
                resModel = evaluationrecordServices.NewEvaluationRecordDetailSaveUpdateDelete(evaluationRecordDetailModels, HelperModels.ConfigSettings.conStr.ToString());
            }
            catch (Exception ex)
            {
                resModel.ResCode = "999";
                resModel.ResDesc = ex.Message;
                return new JsonResult(new
                {
                    resModel

                });
            }

            //List<EvaluationRecordDetailApiModel> evaluationRecordDetailApiModels = new List<EvaluationRecordDetailApiModel>();
            //List<SubEvaluationionRecordDetailApiModel> subEvaluationionRecordDetailApiModels = new List<SubEvaluationionRecordDetailApiModel>();
            //SubEvaluationionRecordDetailApiModel subEvaluationionRecordDetailApiModel = new SubEvaluationionRecordDetailApiModel();

            //EvaluationRecordDetailApiModel res = new EvaluationRecordDetailApiModel();
            //res.EvaluationRecordDetailId = "1212";
            //res.EvaluationRecordId = "1212";
            //res.EvaluationId = "1212";
            //res.EvaluationRecordDetailDesc = "1212";
            //res.EvaluationRecordDetailValueApi = 10;
            //res.EvaluationRecordDetailDescription = "1212";
            //res.EvaluationRecordDetailStatus = 1;
            //evaluationRecordDetailApiModels.Add(res);
            //res = new EvaluationRecordDetailApiModel();
            //res.EvaluationRecordDetailId = "1212";
            //res.EvaluationRecordId = "1212";
            //res.EvaluationId = "1212";
            //res.EvaluationRecordDetailDesc = "1212";
            //res.EvaluationRecordDetailValueApi = 10;
            //res.EvaluationRecordDetailDescription = "1212";
            //res.EvaluationRecordDetailStatus = 1;
            //evaluationRecordDetailApiModels.Add(res);

            //subEvaluationionRecordDetailApiModel = new SubEvaluationionRecordDetailApiModel();
            //subEvaluationionRecordDetailApiModel.SubEvaluationRecordDetailId = "12121";
            //subEvaluationionRecordDetailApiModel.EvaluationRecordDetailId = "12121";
            //subEvaluationionRecordDetailApiModel.EvaluationRecordId = "12121";
            //subEvaluationionRecordDetailApiModel.EvaluationId = "12121";
            //subEvaluationionRecordDetailApiModel.SubEvaluationId = "12121";
            //subEvaluationionRecordDetailApiModel.SubEvaluationRecordDetailDesc = "12121";
            //subEvaluationionRecordDetailApiModel.SubEvaluationRecordDetailValueApi = 10;
            //subEvaluationionRecordDetailApiModel.SubEvaluationRecordDetailStatus = 1;
            //subEvaluationionRecordDetailApiModels.Add(subEvaluationionRecordDetailApiModel);
            //subEvaluationionRecordDetailApiModel = new SubEvaluationionRecordDetailApiModel();
            //subEvaluationionRecordDetailApiModel.SubEvaluationRecordDetailId = "12121";
            //subEvaluationionRecordDetailApiModel.EvaluationRecordDetailId = "12121";
            //subEvaluationionRecordDetailApiModel.EvaluationRecordId = "12121";
            //subEvaluationionRecordDetailApiModel.EvaluationId = "12121";
            //subEvaluationionRecordDetailApiModel.SubEvaluationId = "12121";
            //subEvaluationionRecordDetailApiModel.SubEvaluationRecordDetailDesc = "12121";
            //subEvaluationionRecordDetailApiModel.SubEvaluationRecordDetailValueApi = 10;
            //subEvaluationionRecordDetailApiModel.SubEvaluationRecordDetailStatus = 1;
            //subEvaluationionRecordDetailApiModels.Add(subEvaluationionRecordDetailApiModel);


            return new JsonResult(new
            {
                resModel

            });
        }

        [HttpPost]
        [Route("api/Evaluation/NewsubEvaluationionRecordDetail")]
        public ActionResult NewEvaluationRecordDetail([FromHeader]string AccessKey, [FromHeader]string UserId,[FromBody] List<SubEvaluationionRecordDetailApiModel> subEvaluationionRecordDetailApiModels)
        {
            ResEvaluationRecordDetailModel resModel = new ResEvaluationRecordDetailModel();
            SubEvaluationionRecordDetailModel subEvaluationionRecordDetailModel = new SubEvaluationionRecordDetailModel();
            List<SubEvaluationionRecordDetailModel> subEvaluationionRecordDetailModels = new List<SubEvaluationionRecordDetailModel>();
            if (string.IsNullOrEmpty(AccessKey))
            {


                resModel.ResCode = "010";
                resModel.ResDesc = "Request Parameter must not be null value";
                return new JsonResult(new
                {
                    resModel

                });
            }

            else if (subEvaluationionRecordDetailApiModels == null)
            {
                resModel.ResCode = "010";
                resModel.ResDesc = "Request Parameter must not be null value";
                return new JsonResult(new
                {
                    resModel

                });
            }
            try
            {
                foreach (var s in subEvaluationionRecordDetailApiModels)
                {
                    subEvaluationionRecordDetailModel.SubEvaluationRecordDetailId = Guid.NewGuid().ToString();
                    subEvaluationionRecordDetailModel.CreatedBy = UserId;
                    subEvaluationionRecordDetailModel.InPara = 1;
                    subEvaluationionRecordDetailModel.EvaluationRecordDetailId = s.EvaluationRecordDetailId;
                    subEvaluationionRecordDetailModel.EvaluationRecordId = s.EvaluationRecordId;
                    subEvaluationionRecordDetailModel.EvaluationId = s.EvaluationId;
                    subEvaluationionRecordDetailModel.SubEvaluationId = s.SubEvaluationId;
                    subEvaluationionRecordDetailModel.SubEvaluationRecordDetailDesc = s.SubEvaluationRecordDetailDesc;
                    subEvaluationionRecordDetailModel.SubEvaluationRecordDetailValue = Convert.ToDecimal(s.SubEvaluationRecordDetailValueApi);
                    subEvaluationionRecordDetailModel.SubEvaluationRecordDetailStatus = s.SubEvaluationRecordDetailStatus;
                    subEvaluationionRecordDetailModels.Add(subEvaluationionRecordDetailModel);
                }

                if (evaluationrecordServices.NewSubEvaluationRecordDetailSaveUpdateDelete(subEvaluationionRecordDetailModels, HelperModels.ConfigSettings.conStr.ToString()))
                {
                    resModel.ResCode = "000";
                    resModel.ResDesc = "လုပ်ငန်းပြီးစီးမှု အောင်မြင်စွာ ထည့်သွင် ပြီးပါပြီ!";
                }
            }
            catch (Exception ex)
            {
                resModel.ResCode = "999";
                resModel.ResDesc = ex.Message;
                return new JsonResult(new
                {
                    resModel

                });
            }
            return new JsonResult(new
            {
                resModel

            });
        }
    }
}
