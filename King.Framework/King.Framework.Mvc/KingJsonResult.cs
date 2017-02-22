namespace King.Framework.Mvc
{
    using Newtonsoft.Json;
    using System;
    using System.Web;
    using System.Web.Mvc;

    public class KingJsonResult : JsonResult
    {
        public KingJsonResult()
        {
        }

        public KingJsonResult(object data)
        {
            base.Data = data;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            HttpResponseBase response = context.HttpContext.Response;
            if (!string.IsNullOrEmpty(base.ContentType))
            {
                response.ContentType = base.ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (base.ContentEncoding != null)
            {
                response.ContentEncoding = base.ContentEncoding;
            }
            if (base.Data != null)
            {
                response.Write(JsonConvert.SerializeObject(base.Data));
            }
        }
    }
}

