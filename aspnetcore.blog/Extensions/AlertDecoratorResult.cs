using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace aspnetcore.blog.Extensions
{
    public class AlertDecoratorResult : IActionResult
    {
        public IActionResult Alert_Result { get; }
        public string Alert_Type { get; }
        public string Alert_Title { get; }
        public string Alert_Body { get; }

        public AlertDecoratorResult(IActionResult alert_result, string alert_type, string alert_title, string alert_body)
        {
            Alert_Result = alert_result;
            Alert_Type = alert_type;
            Alert_Title = alert_title;
            Alert_Body = alert_body;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            AttachAlertMsg(context);
            await Alert_Result.ExecuteResultAsync(context);
        }
        private void AttachAlertMsg(ActionContext context)
        {
            var factory = context.HttpContext.RequestServices.GetService<ITempDataDictionaryFactory>();

            var tempData = factory.GetTempData(context.HttpContext);
            tempData["_alert.type"] = Alert_Type;
            tempData["_alert.title"] = Alert_Title;
            tempData["_alert.body"] = Alert_Body;
        }
    }
}
