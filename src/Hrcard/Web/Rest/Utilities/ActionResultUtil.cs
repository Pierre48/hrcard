using Microsoft.AspNetCore.Mvc;

namespace MyCompany.Web.Rest.Utilities {
    public static class ActionResultUtil {
        public static ActionResult WrapOrNotFound(object value)
        {
            return value != null ? (ActionResult) new OkObjectResult(value) : new NotFoundResult();
        }
    }
}
