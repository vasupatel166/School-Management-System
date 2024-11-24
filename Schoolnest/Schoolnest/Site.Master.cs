using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace Schoolnest
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected string userName;
        protected string userEmail;
        protected string profileImage;
        private int AssignedTeacher;
        private string connectionString = Global.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAnnouncements();

                string role = Session["UserRole"]?.ToString();
                string roleName = "";
                string username = Session["Username"]?.ToString();

                if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(username))
                {
                    Response.Redirect("~/Login.aspx");
                }

                var userDetails = GetUserDetailsByRole(username, role);

                if (userDetails != null)
                {
                    // Store the values in class-level variables based on the retrieved data
                    switch (role.ToUpper())
                    {
                        case "SA":
                            userName = userDetails["Name"].ToString();
                            userEmail = userDetails["Email"].ToString();
                            profileImage = userDetails["ProfileImage"].ToString();
                            AnnouncementsLink.Visible = false;
                            roleName = "superadmin";
                            break;

                        case "A":
                            userName = $"{userDetails["Firstname"]} {userDetails["Lastname"]}";
                            userEmail = userDetails["Email"].ToString();
                            profileImage = userDetails["ProfileImage"].ToString();
                            ViewAllAnnouncements.NavigateUrl = "~/Admin/Announcements.aspx";
                            roleName = "admin";
                            break;

                        case "T":
                            userName = userDetails["TeacherName"].ToString();
                            userEmail = userDetails["Teacher_Email"].ToString();
                            AssignedTeacher = Convert.ToInt32(userDetails["AssignTeacherStatus"]);
                            Session["ClassTeacher"] = AssignedTeacher;
                            profileImage = ""; // If there's no ProfileImage column in TeacherMaster, leave it empty or provide a default
                            roleName = "teacher";
                            ViewAllAnnouncements.NavigateUrl = "~/Teacher/ViewAnnouncements.aspx";
                            break;

                        case "S":
                            userName = $"{userDetails["Student_FirstName"]} {userDetails["Student_LastName"]}";
                            userEmail = userDetails["Student_EmailID"].ToString();
                            profileImage = userDetails["Student_ProfileImage"].ToString();
                            roleName = "student";
                            ViewAllAnnouncements.NavigateUrl = "~/Student/Announcements.aspx";
                            break;

                        default:
                            break;
                    }

                    if (string.IsNullOrEmpty(profileImage))
                    {
                        profileImage = "default_user_logo.png";
                    }

                    user_heading_section.Text = $"{userName} ({roleName})";

                    var menuItems = GetMenuForRole(roleName);

                    // Bind the menu to a Repeater control
                    SidebarMenuRepeater.DataSource = menuItems;
                    SidebarMenuRepeater.DataBind();

                    // Display the information on the page
                    NavUserName.Text = userName;
                    NavUserName2.Text = userName;
                    NavUserImage.ImageUrl = ResolveUrl($"~/assets/img/user-profile-img/{profileImage}");
                    NavUserImage2.ImageUrl = ResolveUrl($"~/assets/img/user-profile-img/{profileImage}");
                    NavUserEmail.Text = userEmail;

                }
            }
        }

        private void LoadAnnouncements()
        {
            string schoolId = Session["SchoolID"]?.ToString();
            string roleId = Session["UserRole"]?.ToString();
            string TargetAudience;

            if(roleId == "T")
            {
                TargetAudience = "Teacher";
            }
            else
            {
                TargetAudience = "Student";
            }

            if (string.IsNullOrEmpty(schoolId)) return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 5 AnnouncementID, AnnouncementTitle,
                CASE 
                    WHEN UpdatedDateTime IS NULL THEN CreatedDateTime 
                    ELSE UpdatedDateTime 
                END AS DisplayDateTime
                FROM AnnouncementMaster 
                WHERE (TargetAudience = @TargetAudience OR TargetAudience = 'Both')
                AND SchoolMaster_SchoolID = @SchoolID AND IsActive = 1 
                ORDER BY DisplayDateTime DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@TargetAudience", TargetAudience);
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    try
                    {
                        // Create a DataTable to store the results
                        DataTable dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }

                        int count = dt.Rows.Count;

                        lblNotificationCount.Text = count.ToString();
                        lblAnnouncementTitle.Text = $"You have {count} new announcement{(count != 1 ? "s" : "")}";

                        rptMasterAnnouncements.DataSource = dt;
                        rptMasterAnnouncements.DataBind();
                    }
                    catch (Exception ex)
                    {
                        Response.Write("An error occurred: " + ex.Message);
                    }
                }
            }
        }

        protected string GetTimeAgo(DateTime dateTime)
        {
            TimeSpan timeSince = DateTime.Now.Subtract(dateTime);

            if (timeSince.TotalMinutes < 1)
                return "just now";
            if (timeSince.TotalMinutes < 60)
                return $"{(int)timeSince.TotalMinutes} minutes ago";
            if (timeSince.TotalHours < 24)
                return $"{(int)timeSince.TotalHours} hours ago";
            if (timeSince.TotalDays < 7)
                return $"{(int)timeSince.TotalDays} days ago";

            return dateTime.ToString("MMM dd, yyyy");
        }

        protected string GetAnnouncementUrl(string announcementId)
        {
            string role = Session["UserRole"]?.ToString();
            string baseUrl = "";

            switch (role)
            {
                case "A":
                    baseUrl = "~/Admin/Announcements.aspx";
                    return ResolveUrl($"{baseUrl}");
                case "T":
                    baseUrl = "~/Teacher/ViewAnnouncements.aspx";
                    break;
                case "S":
                    baseUrl = "~/Student/Announcements.aspx";
                    break;
                default:
                    return "#";
            }

            return ResolveUrl($"{baseUrl}?aid={announcementId}");
        }

        // Method to call the stored procedure and retrieve user details based on username and role
        private DataRow GetUserDetailsByRole(string username, string role)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetUserDetailsByRole", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add input parameters
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@RoleID", role);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);

                        con.Open();
                        da.Fill(dt);
                        con.Close();
                    }
                }

                // Return the first row if data exists
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0];
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error retrieving user details: {ex.Message}");
            }

            return null;
        }

        // Menu model
        public class MenuItem
        {
            public string Title { get; set; }
            public string Icon { get; set; }
            public string Url { get; set; }
            public List<MenuItem> SubMenus { get; set; }

            public MenuItem(string title, string icon, string url, List<MenuItem> subMenus = null)
            {
                Title = title;
                Icon = icon;
                Url = url;
                SubMenus = subMenus ?? new List<MenuItem>();
            }
        }

        // Menu data for each role
        public List<MenuItem> GetMenuForRole(string role)
        {

            var attendanceSubMenu = new List<MenuItem>();

            if (AssignedTeacher == 1)
            {
                attendanceSubMenu.Add(new MenuItem("View Attendance Reports", "fas fa-chart-line", GetResolvedUrl("Teacher/AttendanceReports.aspx")));
                attendanceSubMenu.Add(new MenuItem("Mark Attendance", "fas fa-check", GetResolvedUrl("Teacher/MarkAttendance.aspx")));
            }

            attendanceSubMenu.Add(new MenuItem("View My Attendance", "fas fa-chart-line", GetResolvedUrl("Teacher/ViewMyAttendance.aspx")));

            List<MenuItem> menuItems = new List<MenuItem>();

            switch (role.ToLower())
            {
                case "superadmin": // super admin
                    menuItems.Add(new MenuItem("Dashboard", "fas fa-home", GetResolvedUrl("SuperAdmin/Dashboard.aspx")));
                    menuItems.Add(new MenuItem("Registeration", "fas fa-users", "", new List<MenuItem> {
                        new MenuItem("School", "fas fa-school", GetResolvedUrl("SuperAdmin/RegisterSchool.aspx")),
                        new MenuItem("Admin", "fas fa-user-plus", GetResolvedUrl("SuperAdmin/RegisterAdmin.aspx"))
                    }));
                    menuItems.Add(new MenuItem("Reports", "fas fa-file-invoice", GetResolvedUrl("SuperAdmin/Reports.aspx")));
                    break;

                case "admin": // Admin
                    menuItems.Add(new MenuItem("Dashboard", "fas fa-home", GetResolvedUrl("Admin/Dashboard.aspx")));

                    menuItems.Add(new MenuItem("User Management", "fas fa-users", "", new List<MenuItem> {
                        new MenuItem("Teacher List", "fas fa-chalkboard-teacher", GetResolvedUrl("Admin/TeacherList.aspx")),
                        new MenuItem("Student List", "fas fa-user-graduate", GetResolvedUrl("Admin/StudentList.aspx"))
                    }));

                    menuItems.Add(new MenuItem("Class Management", "fas fa-school", "", new List<MenuItem> {
                        new MenuItem("Standard Master", "fas fa-list", GetResolvedUrl("Admin/StandardMaster.aspx")),
                        new MenuItem("Divison Master", "fas fa-chalkboard", GetResolvedUrl("Admin/DivisonMaster.aspx")),
                        new MenuItem("Section Master", "fas fa-calendar-alt", GetResolvedUrl("Admin/SectionMaster.aspx")),
                        new MenuItem("Assign Teacher", "fas fa-calendar-alt", GetResolvedUrl("Admin/AssignTeacher.aspx")),
                    }));

                    menuItems.Add(new MenuItem("Subject Management", "fas fa-book", "", new List<MenuItem> {
                        new MenuItem("Subject Master", "fas fa-list", GetResolvedUrl("Admin/SubjectMaster.aspx")),
                        new MenuItem("Assign Subject to Class", "fas fa-chalkboard", GetResolvedUrl("Admin/AssignSubjectToClass.aspx")),
                        new MenuItem("Assign Teacher to Subject", "fas fa-chalkboard-teacher", GetResolvedUrl("Admin/AssignTeacherToSubject.aspx"))
                    }));

                    menuItems.Add(new MenuItem("Timetable Management", "fas fa-book", "", new List<MenuItem> {
                        new MenuItem("Period Time Master", "fas fa-list", GetResolvedUrl("Admin/PeriodTimeMaster.aspx")),
                        new MenuItem("Timetable Master", "fas fa-list", GetResolvedUrl("Admin/TimetableMaster.aspx"))
                    }));

                    menuItems.Add(new MenuItem("Attendance Management", "fas fa-calendar-check", "", new List<MenuItem> {
                        new MenuItem("Mark Teacher Attendance", "fas fa-check", GetResolvedUrl("Admin/MarkTeacherAttendance.aspx")),
                        new MenuItem("View Attendance Reports", "fas fa-chart-line", GetResolvedUrl("Admin/AttendanceReports.aspx"))
                    }));

                    menuItems.Add(new MenuItem("Exam Management", "fas fa-file-alt", "", new List<MenuItem> {
                        new MenuItem("Exam Master", "fas fa-calendar-alt", GetResolvedUrl("Admin/ExamMaster.aspx")),
                        new MenuItem("Exam Schedule", "fas fa-calendar-alt", GetResolvedUrl("Admin/ExamSchedule.aspx")),
                        new MenuItem("Grade Management", "fas fa-graduation-cap", GetResolvedUrl("Admin/GradeManagement.aspx")),
                        new MenuItem("Generate Report Cards", "fas fa-file-signature", GetResolvedUrl("Admin/GenerateReportCards.aspx"))
                    }));

                    menuItems.Add(new MenuItem("Fee Management", "fas fa-dollar-sign", "", new List<MenuItem> {
                        new MenuItem("Fee Master", "fas fa-money-check-alt", GetResolvedUrl("Admin/FeeMaster.aspx")),
                        new MenuItem("Payment Report", "fas fa-file-invoice", GetResolvedUrl("Admin/PaymentReport.aspx"))
                    }));

                    menuItems.Add(new MenuItem("Miscellaneous", "fas fa-dollar-sign", "", new List<MenuItem> {
                        new MenuItem("Event Master", "fas fa-money-check-alt", GetResolvedUrl("Admin/EventMaster.aspx")),
                        new MenuItem("Budget Master", "fas fa-money-check-alt", GetResolvedUrl("Admin/BudgetMaster.aspx")),
                        new MenuItem("Budget Categories", "fas fa-money-check-alt", GetResolvedUrl("Admin/BudgetCategories.aspx")),
                        new MenuItem("Holiday Master", "fas fa-money-check-alt", GetResolvedUrl("Admin/HolidayMaster.aspx")),
                        new MenuItem("School Settings", "fas fa-money-check-alt", GetResolvedUrl("Admin/SchoolSettings.aspx"))
                    }));

                    menuItems.Add(new MenuItem("Notifications", "fas fa-bullhorn", "", new List<MenuItem> {
                        new MenuItem("Announcements Board", "fas fa-bullhorn", GetResolvedUrl("Admin/Announcements.aspx")),
                        new MenuItem("Send Email Notifications", "fas fa-envelope", GetResolvedUrl("Admin/SendNotifications.aspx"))
                    }));

                    break;

                case "teacher":
                    menuItems.Add(new MenuItem("Dashboard", "fas fa-home", GetResolvedUrl("Teacher/Dashboard.aspx")));

                    menuItems.Add(new MenuItem("Attendance Management", "fas fa-calendar-check", "", attendanceSubMenu));

                    menuItems.Add(new MenuItem("Exams & Grades Management", "fas fa-file-alt", "", new List<MenuItem> {
                        new MenuItem("View Exam Schedule", "fas fa-calendar-alt", GetResolvedUrl("Teacher/ViewExamSchedule.aspx")),
                        new MenuItem("Enter Grades", "fas fa-graduation-cap", GetResolvedUrl("Teacher/EnterGrades.aspx")),
                        new MenuItem("View Grade Reports", "fas fa-chart-line", GetResolvedUrl("Teacher/ViewGradeReports.aspx")),
                    }));

                    menuItems.Add(new MenuItem("Timetable Management", "fas fa-calendar-alt", "", new List<MenuItem> {
                        new MenuItem("View Timetable", "fas fa-table", GetResolvedUrl("Teacher/ViewTimetable.aspx")),
                    }));

                    menuItems.Add(new MenuItem("Miscellaneous", "fas fa-dollar-sign", "", new List<MenuItem> {
                        new MenuItem("All Events", "fas fa-money-check-alt", GetResolvedUrl("Teacher/ViewEvents.aspx")),
                        new MenuItem("All Holidays", "fas fa-money-check-alt", GetResolvedUrl("Teacher/ViewHolidays.aspx")),
                    }));

                    menuItems.Add(new MenuItem("Notifications", "fas fa-bullhorn", "", new List<MenuItem> {
                        new MenuItem("View Announcements", "fas fa-bullhorn", GetResolvedUrl("Teacher/ViewAnnouncements.aspx")),
                        new MenuItem("Send Notifications", "fas fa-envelope", GetResolvedUrl("Teacher/SendNotifications.aspx")),
                    }));

                    break;

                case "student":
                    menuItems.Add(new MenuItem("Dashboard", "fas fa-home", GetResolvedUrl("Student/Dashboard.aspx")));
                    menuItems.Add(new MenuItem("Timetable", "fas fa-calendar-alt", GetResolvedUrl("Student/Timetable.aspx")));
                    menuItems.Add(new MenuItem("Attendance Report", "fas fa-calendar-check", GetResolvedUrl("Student/AttendanceReport.aspx")));
                    menuItems.Add(new MenuItem("Exam Schedule", "fas fa-calendar-alt", GetResolvedUrl("Student/ExamSchedule.aspx")));
                    menuItems.Add(new MenuItem("Report Card", "fas fa-graduation-cap", GetResolvedUrl("Student/ReportCard.aspx")));
                    menuItems.Add(new MenuItem("Fee Payment", "fas fa-dollar-sign", GetResolvedUrl("Student/FeePayment.aspx")));
                    menuItems.Add(new MenuItem("Fee Payment History", "fas fa-dollar-sign", GetResolvedUrl("Student/FeePaymentHistory.aspx")));
                    menuItems.Add(new MenuItem("Announcements", "fas fa-bullhorn", GetResolvedUrl("Student/Announcements.aspx")));
                    menuItems.Add(new MenuItem("Upcoming Events", "fas fa-bullhorn", GetResolvedUrl("Student/UpcomingEvents.aspx")));

                    break;

                default:
                    break;
            }

            return menuItems;
        }

        private string GetResolvedUrl(string relativePath)
        {
            return ResolveUrl(relativePath);
        }

    }
}