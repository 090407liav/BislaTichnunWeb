using System;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;

namespace BislaTichnunWeb
{
    /// <summary>
    /// Summary description for AssignmentHandler
    /// </summary>
    public class AssignmentHandler : IHttpHandler
    {
        private static readonly JavaScriptSerializer _js =
            new JavaScriptSerializer();

        public void ProcessRequest(HttpContext ctx)
        {
            ctx.Response.ContentType = "application/json";
            ctx.Response.ContentEncoding =
                System.Text.Encoding.UTF8;

            string action =
                ctx.Request.QueryString["action"] ?? "";

            string body =
                new StreamReader(
                    ctx.Request.InputStream)
                    .ReadToEnd();

            try
            {
                dynamic data =
                    _js.DeserializeObject(body);

                switch (action)
                {
                    case "create":
                        DutyAssignment assignment =
                            new DutyAssignment
                            {
                                CourseId =
                                    Convert.ToInt32(
                                        data["courseId"]),

                                DutyTypeId =
                                    Convert.ToInt32(
                                        data["dutyTypeId"]),

                                StartDate =
                                    DateTime.Parse(
                                        (string)data["startDate"]),

                                EndDate =
                                    DateTime.Parse(
                                        (string)data["endDate"]),

                                Status =
                                    ParseAssignmentStatus(
                                        data["mode"]),

                                Notes =
                                    data.ContainsKey("notes")
                                        ? (string)data["notes"]
                                        : null
                            };

                        int newId =
                            DataAccess.CreateAssignment(
                                assignment);

                        Write(
                            ctx,
                            true,
                            new { id = newId });

                        break;

                    case "update":
                        DutyAssignment updatedAssignment =
                            new DutyAssignment
                            {
                                Id =
                                    Convert.ToInt32(
                                        data["id"]),

                                CourseId =
                                    Convert.ToInt32(
                                        data["courseId"]),

                                DutyTypeId =
                                    Convert.ToInt32(
                                        data["dutyTypeId"]),

                                StartDate =
                                    DateTime.Parse(
                                        (string)data["startDate"]),

                                EndDate =
                                    DateTime.Parse(
                                        (string)data["endDate"]),

                                Status =
                                    ParseAssignmentStatus(
                                        data["mode"]),

                                Notes =
                                    data.ContainsKey("notes")
                                        ? (string)data["notes"]
                                        : null
                            };

                        DataAccess.UpdateAssignment(
                            updatedAssignment);

                        Write(
                            ctx,
                            true,
                            new { });

                        break;

                    case "delete":
                        DataAccess.DeleteAssignment(
                            Convert.ToInt32(
                                data["id"]));

                        Write(
                            ctx,
                            true,
                            new { });

                        break;

                    case "confirm":
                        DataAccess.ConfirmAssignment(
                            Convert.ToInt32(
                                data["id"]));

                        Write(
                            ctx,
                            true,
                            new { });

                        break;

                    default:
                        ctx.Response.StatusCode = 400;

                        Write(
                            ctx,
                            false,
                            new
                            {
                                error =
                                    "Unknown action: " +
                                    action
                            });

                        break;
                }
            }
            catch (Exception ex)
            {
                ctx.Response.StatusCode = 500;

                Write(
                    ctx,
                    false,
                    new
                    {
                        error = ex.Message
                    });
            }
        }

        private static void Write(
            HttpContext ctx,
            bool ok,
            object payload)
        {
            ctx.Response.Write(
                _js.Serialize(
                    new
                    {
                        ok = ok,
                        data = payload
                    }));
        }

        public bool IsReusable
        {
            get { return true; }
        }

        private static AssignmentStatus ParseAssignmentStatus(
            object value)
        {
            string status =
                value == null
                    ? ""
                    : value.ToString();

            if (string.Equals(
                status,
                "active",
                StringComparison.OrdinalIgnoreCase))
            {
                return AssignmentStatus.Active;
            }

            return AssignmentStatus.Planning;
        }
    }
}