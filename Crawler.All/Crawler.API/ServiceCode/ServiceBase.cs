
using Crawler.Data.DbContext;
using Crawler.DTO.ResponseDTO;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Crawler.API.ServiceCode
{
    public static class ShortGuid
    {
        public static string ToShortGuid(this Guid newGuid)
        {
            string modifiedBase64 = Convert.ToBase64String(newGuid.ToByteArray())
                .Replace('+', '-').Replace('/', '_') // avoid invalid URL characters
                .Substring(0, 22);
            return modifiedBase64;
        }

        public static Guid ParseShortGuid(string shortGuid)
        {
            string base64 = shortGuid.Replace('-', '+').Replace('_', '/') + "==";
            Byte[] bytes = Convert.FromBase64String(base64);
            return new Guid(bytes);
        } 
    }

    public partial class CrawlerService : ServiceBase
    {

    }

    public abstract class ServiceBase : Service
    {


        protected CrawlerStorage context;

        public ServiceBase()
        {
            context = new CrawlerStorage();
        }

        ~ServiceBase()
        {
            context.Dispose();
        }

        //protected BasecampUserSession UserSession
        //{
        //    get
        //    {
        //        return base.SessionAs<BasecampUserSession>();
        //    }
        //}

     

        //public bool HasRole(string abbr)
        //{


        //    return UserSession.GivenRoles.Any(r => r.Abbr.Equals(abbr));
        //}

        // SHA256 Hash computing function
        public static byte[] GetSHA256Hash(string input)
        {
            var hash = SHA256.Create();
            var data = hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            return data;
        }

        // Service result helper functions

        protected EnvelopeDTO<T> Succeeded<T>(T returnBody = null) where T : class
        {
            return new EnvelopeDTO<T>
            {
                ResultType = ResultTypeDTO.Success,
                ReturnBody = returnBody
            };
        }

        protected EnvelopeDTO<T> Fail<T>(string errorMessage) where T : class
        {
            return new EnvelopeDTO<T>
            {
                ResultType = ResultTypeDTO.Fail,
                ErrorMessage = errorMessage
            };
        }

        protected EnvelopeDTO<T> Fail<T>(int errorType, string errorMessage) where T : class
        {
            return new EnvelopeDTO<T>
            {
                ResultType = (ResultTypeDTO)errorType,
                ErrorMessage = errorMessage
            };
        }

        protected EnvelopeDTO<T> Exception<T>(System.Exception e) where T : class
        {
            if (e.InnerException != null)
            {
                return new EnvelopeDTO<T>
                {
                    ResultType = ResultTypeDTO.Exception,
                    ErrorMessage = e.InnerException.Message
                };
            }
            else
            {
                return new EnvelopeDTO<T>
                {
                    ResultType = ResultTypeDTO.Exception,
                    ErrorMessage = e.Message
                };
            }
        }

        protected EnvelopeDTO<T> IntegrityError<T>() where T : class
        {
            return new EnvelopeDTO<T>
            {
                ResultType = ResultTypeDTO.IntegrityError
            };
        }

        protected EnvelopeDTO_struct<T> Succeeded_struct<T>(T returnBody) where T : struct
        {
            return new EnvelopeDTO_struct<T>
            {
                ResultType = ResultTypeDTO.Success,
                ReturnBody = returnBody
            };
        }

        protected EnvelopeDTO_struct<T> Fail_struct<T>(string errorMessage) where T : struct
        {
            return new EnvelopeDTO_struct<T>
            {
                ResultType = ResultTypeDTO.Fail,
                ErrorMessage = errorMessage
            };
        }

        protected EnvelopeDTO_struct<T> Fail_struct<T>(int errorType, string errorMessage) where T : struct
        {
            return new EnvelopeDTO_struct<T>
            {
                ResultType = (ResultTypeDTO)errorType,
                ErrorMessage = errorMessage
            };
        }

        protected EnvelopeDTO_struct<T> Exception_struct<T>(System.Exception e) where T : struct
        {
            if (e.InnerException != null)
            {
                return new EnvelopeDTO_struct<T>
                {
                    ResultType = ResultTypeDTO.Exception,
                    ErrorMessage = e.InnerException.Message
                };
            }
            else
            {
                return new EnvelopeDTO_struct<T>
                {
                    ResultType = ResultTypeDTO.Exception,
                    ErrorMessage = e.Message
                };
            }
        }

        protected EnvelopeDTO_struct<T> IntegrityError_struct<T>() where T : struct
        {
            return new EnvelopeDTO_struct<T>
            {
                ResultType = ResultTypeDTO.IntegrityError
            };
        }

    }
}