using Microsoft.AspNetCore.Mvc;

namespace aspnetcore.blog.Extensions
{
    public static class AlertExtensions
    {
        public static IActionResult Success_alert(this IActionResult result, string alert_body, string alert_title)
        {
            return Alert(result, "success", alert_body, alert_title);
        }

        public static IActionResult Info_alert(this IActionResult result, string alert_body, string alert_title)
        {
            return Alert(result, "info", alert_body, alert_title);
        }

        public static IActionResult Warning_alert(this IActionResult result, string alert_body, string alert_title)
        {
            return Alert(result, "warning", alert_body, alert_title);
        }

        public static IActionResult Danger_alert(this IActionResult result, string alert_body, string alert_title)
        {
            return Alert(result, "danger", alert_body, alert_title);
        }

        private static IActionResult Alert(IActionResult result, string type, string alert_body, string alert_title)
        {
            return new AlertDecoratorResult(result, type, alert_body, alert_title);
        }
    }
}
