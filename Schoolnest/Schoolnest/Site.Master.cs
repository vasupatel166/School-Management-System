using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected string userName;
        protected string userEmail;
        protected string profileImage;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Assuming the logged-in user's role and username are stored in Session
                string role = Session["UserRole"]?.ToString();
                string roleName = "";
                string username = Session["Username"]?.ToString();

                if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(username))
                {
                    Response.Redirect("~/Login.aspx");
                }

                // Retrieve user details from the database using the stored procedure
                var userDetails = GetUserDetailsByRole(username, role);

                if (userDetails != null)
                {
                    // Store the values in class-level variables based on the retrieved data
                    switch (role.ToUpper())
                    {
                        case "SA":
                            userName = userDetails["Name"].ToString(); // SuperAdmin table has "Name"
                            userEmail = userDetails["Email"].ToString();
                            profileImage = userDetails["ProfileImage"].ToString();
                            roleName = "superadmin";
                            break;

                        case "A":
                            userName = $"{userDetails["Firstname"]} {userDetails["Lastname"]}"; // Admin table has "First_Name" and "Last_Name"
                            userEmail = userDetails["Email"].ToString();
                            profileImage = userDetails["ProfileImage"].ToString();
                            roleName = "admin";
                            break;

                        case "T":
                            userName = userDetails["TeacherName"].ToString(); // TeacherMaster table has "TeacherName"
                            userEmail = userDetails["Teacher_Email"].ToString();
                            profileImage = ""; // If there's no ProfileImage column in TeacherMaster, leave it empty or provide a default
                            roleName = "teacher";
                            break;

                        case "S":
                            userName = $"{userDetails["Student_FirstName"]} {userDetails["Student_LastName"]}"; // StudentMaster has "Student_FirstName" and "Student_LastName"
                            userEmail = userDetails["Student_EmailID"].ToString();
                            profileImage = userDetails["Student_ProfileImage"].ToString();
                            roleName = "student";
                            break;

                        default:
                            break;
                    }

                    if (string.IsNullOrEmpty(profileImage))
                    {
                        // If profile image is empty, set a default image
                        profileImage = "default_user_logo.png";
                    }

                    user_heading_section.Text = $"{userName} ({roleName})";

                    var menuItems = GetMenuForRole(roleName);

                    // Bind the menu to a Repeater control or any other way
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

        // Method to call the stored procedure and retrieve user details based on username and role
        private DataRow GetUserDetailsByRole(string username, string role)
        {
            // Create a DataTable to store the result
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(Global.ConnectionString))
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
            List<MenuItem> menuItems = new List<MenuItem>();

            switch (role.ToLower())
            {
                case "superadmin": // super admin
                    menuItems.Add(new MenuItem("Dashboard", "fas fa-home", ResolveUrl("SuperAdmin/Dashboard.aspx")));
                    menuItems.Add(new MenuItem("Registeration", "fas fa-users", "", new List<MenuItem> {
                        new MenuItem("School", "fas fa-school", ResolveUrl("SuperAdmin/RegisterSchool.aspx")),
                        new MenuItem("Admin", "fas fa-user-plus", ResolveUrl("SuperAdmin/RegisterAdmin.aspx"))
                    }));
                    menuItems.Add(new MenuItem("Reports", "fas fa-file-invoice", ResolveUrl("SuperAdmin/Reports.aspx")));
                    break;

                case "admin": // Admin
                    menuItems.Add(new MenuItem("Dashboard", "fas fa-home", ResolveUrl("Admin/Dashboard.aspx")));

                    menuItems.Add(new MenuItem("User Management", "fas fa-users", "", new List<MenuItem> {
                        new MenuItem("Teacher List", "fas fa-chalkboard-teacher", ResolveUrl("Admin/TeacherList.aspx")),
                        new MenuItem("Student List", "fas fa-user-graduate", ResolveUrl("Admin/StudentList.aspx")),
                        new MenuItem("Parent List", "fas fa-users", ResolveUrl("Admin/ParentList.aspx"))
                    }));

                    menuItems.Add(new MenuItem("Class Management", "fas fa-school", "", new List<MenuItem> {
                        new MenuItem("Standard Master", "fas fa-list", ResolveUrl("Admin/StandardMaster.aspx")),
                        new MenuItem("Divison Master", "fas fa-chalkboard", ResolveUrl("Admin/DivisonMaster.aspx")),
                        new MenuItem("Section Master", "fas fa-calendar-alt", ResolveUrl("Admin/SectionMaster.aspx")),
                        new MenuItem("Assign Teacher", "fas fa-calendar-alt", ResolveUrl("Admin/AssignTeacher.aspx")),
                    }));

                    menuItems.Add(new MenuItem("Subject Management", "fas fa-book", "", new List<MenuItem> {
                        new MenuItem("Subject Master", "fas fa-list", ResolveUrl("Admin/SubjectMaster.aspx")),
                        new MenuItem("Assign Subject to Class", "fas fa-chalkboard", ResolveUrl("Admin/AssignSubjectToClass.aspx")),
                        new MenuItem("Assign Teacher to Subject", "fas fa-chalkboard-teacher", ResolveUrl("Admin/AssignTeacherToSubject.aspx"))
                    }));

                    menuItems.Add(new MenuItem("Attendance Management", "fas fa-calendar-check", "", new List<MenuItem> {
                        new MenuItem("Mark Attendance", "fas fa-check", ResolveUrl("Admin/MarkAttendance.aspx")),
                        new MenuItem("View Attendance Reports", "fas fa-chart-line", ResolveUrl("Admin/AttendanceReports.aspx"))
                    }));

                    menuItems.Add(new MenuItem("Exam Management", "fas fa-file-alt", "", new List<MenuItem> {
                        new MenuItem("Exam Schedule", "fas fa-calendar-alt", ResolveUrl("Admin/ExamSchedule.aspx")),
                        new MenuItem("Grade Management", "fas fa-graduation-cap", ResolveUrl("Admin/GradeManagement.aspx")),
                        new MenuItem("Generate Report Cards", "fas fa-file-signature", ResolveUrl("Admin/GenerateReportCards.aspx"))
                    }));

                    menuItems.Add(new MenuItem("Fee Management", "fas fa-dollar-sign", "", new List<MenuItem> {
                        new MenuItem("Fee Master", "fas fa-money-check-alt", ResolveUrl("Admin/FeeMaster.aspx")),
                        new MenuItem("Set Class Wise Fees", "fas fa-money-check-alt", ResolveUrl("Admin/ClassWiseFees.aspx")),
                        new MenuItem("Payment Report", "fas fa-file-invoice", ResolveUrl("Admin/PaymentReport.aspx"))
                    }));

                    menuItems.Add(new MenuItem("Miscellaneous", "fas fa-dollar-sign", "", new List<MenuItem> {
                        new MenuItem("Event Master", "fas fa-money-check-alt", ResolveUrl("Admin/EventMaster.aspx")),
                        new MenuItem("Budget Master", "fas fa-money-check-alt", ResolveUrl("Admin/BudgetMaster.aspx")),
                        new MenuItem("Budget Categories", "fas fa-money-check-alt", ResolveUrl("Admin/BudgetCategories.aspx"))
                    }));

                    menuItems.Add(new MenuItem("Notifications", "fas fa-bullhorn", "", new List<MenuItem> {
                        new MenuItem("Announcements Board", "fas fa-bullhorn", ResolveUrl("Admin/Announcements.aspx")),
                        new MenuItem("Send Email Notifications", "fas fa-envelope", ResolveUrl("Admin/SendNotifications.aspx"))
                    }));

                    break;

                case "teacher": // teacher
                    menuItems.Add(new MenuItem("Dashboard", "fas fa-home", ResolveUrl("Teacher/Dashboard.aspx")));
                    menuItems.Add(new MenuItem("Attendance Management", "fas fa-calendar-check", "", new List<MenuItem> {
                        new MenuItem("Mark Attendance", "fas fa-check", ResolveUrl("Teacher/MarkAttendance.aspx")),
                        new MenuItem("View Attendance Reports", "fas fa-chart-line", ResolveUrl("Teacher/AttendanceReports.aspx")),
                    }));
                            menuItems.Add(new MenuItem("Exams & Grades Management", "fas fa-file-alt", "", new List<MenuItem> {
                        new MenuItem("View Exam Schedule", "fas fa-calendar-alt", ResolveUrl("Teacher/ViewExamSchedule.aspx")),
                        new MenuItem("Enter Grades", "fas fa-graduation-cap", ResolveUrl("Teacher/EnterGrades.aspx")),
                        new MenuItem("View Grade Reports", "fas fa-chart-line", ResolveUrl("Teacher/ViewGradeReports.aspx")),
                    }));
                            menuItems.Add(new MenuItem("Timetable Management", "fas fa-calendar-alt", "", new List<MenuItem> {
                        new MenuItem("View Class Timetable", "fas fa-table", ResolveUrl("Teacher/ViewClassTimetable.aspx")),
                        new MenuItem("View Personal Schedule", "fas fa-calendar-day", ResolveUrl("Teacher/ViewPersonalSchedule.aspx")),
                    }));
                            
                            menuItems.Add(new MenuItem("Notifications", "fas fa-bullhorn", "", new List<MenuItem> {
                        new MenuItem("View Announcements", "fas fa-bullhorn", ResolveUrl("Teacher/ViewAnnouncements.aspx")),
                        new MenuItem("Send Notifications", "fas fa-envelope", ResolveUrl("Teacher/SendNotifications.aspx")),
                    }));

                    break;

                case "student": // student
                    menuItems.Add(new MenuItem("Dashboard", "fas fa-home", ResolveUrl("Student/Dashboard.aspx")));

                    menuItems.Add(new MenuItem("Attendance", "fas fa-calendar-check", "", new List<MenuItem> {
                        new MenuItem("View Attendance Report", "fas fa-chart-line", ResolveUrl("Student/ViewAttendanceReport.aspx")),
                    }));

                    menuItems.Add(new MenuItem("Exams & Results", "fas fa-file-alt", "", new List<MenuItem> {
                        new MenuItem("View Exam Schedule", "fas fa-calendar-alt", ResolveUrl("Student/ViewExamSchedule.aspx")),
                        new MenuItem("View Grades/Report Cards", "fas fa-graduation-cap", ResolveUrl("Student/ViewGrades.aspx"))
                    }));
                           
                    menuItems.Add(new MenuItem("Fee Management", "fas fa-dollar-sign", "", new List<MenuItem> {
                        new MenuItem("Pay Fees", "fas fa-credit-card", ResolveUrl("Student/PayFees.aspx")),
                        new MenuItem("View Payment History", "fas fa-file-invoice-dollar", ResolveUrl("Student/ViewPaymentHistory.aspx")),
                    }));
                           
                            menuItems.Add(new MenuItem("Notifications", "fas fa-bullhorn", "", new List<MenuItem> {
                        new MenuItem("View Announcements", "fas fa-bullhorn", ResolveUrl("Student/ViewAnnouncements.aspx")),
                    }));
                            break;

                default:
                    break;
            }

            return menuItems;
        }

    }
}