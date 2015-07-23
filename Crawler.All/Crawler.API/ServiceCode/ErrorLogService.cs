using AutoMapper;
using Crawler.API.Helper;
using Crawler.Data.DbContext;
using Crawler.DTO.RequestDTO;
using Crawler.DTO.ResponseDTO;
using ServiceStack.ServiceClient.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crawler.API.ServiceCode
{
    public partial class CrawlerService : ServiceBase
    {
        public EnvelopeDTO<ErrorLogDTO> Any(ErrorLogCreateRequestDTO req)
        {
            var ErrorLog = req.ErrorLog;
            using (var entities = new CrawlerStorage())
            {
                try
                {
                    var errorlog = new ErrorLog();

                    errorlog.Error_Address = ErrorLog.Error_Address;
                    errorlog.Error_URL = ErrorLog.Error_URL;
                    errorlog.Error_Details = ErrorLog.Error_Details;
                    errorlog.Hresult = ErrorLog.Hresult;

                    entities.ErrorLogs.Add(errorlog);

                    entities.SaveChanges();
                    return Succeeded(new ErrorLogDTO());
                }

                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }
            }
        }
   }
}