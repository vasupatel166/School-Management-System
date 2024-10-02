using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                // Assuming the logged-in user's role is stored in Session
                string role = Session["UserRole"]?.ToString();

                string roleName = "";

                if (role == "SA")
                {
                    roleName = "superadmin";
                } 
                else if (role == "A")
                {
                    roleName = "admin";
                }
                else if (role == "T") {
                    roleName = "teacher";
                }
                else
                {
                    roleName = "student";
                }

                user_heading_section.Text = roleName;

                var menuItems = GetMenuForRole(roleName);

                // Bind the menu to a Repeater control or any other way
                SidebarMenuRepeater.DataSource = menuItems;
                SidebarMenuRepeater.DataBind();
            }
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
                        new MenuItem("Admin List", "fas fa-user-tie", ResolveUrl("Admin/AdminList.aspx")),
                        new MenuItem("Teacher List", "fas fa-chalkboard-teacher", ResolveUrl("Admin/TeacherList.aspx")),
                        new MenuItem("Student List", "fas fa-user-graduate", ResolveUrl("Admin/StudentList.aspx")),
                        new MenuItem("Parent List", "fas fa-users", ResolveUrl("Admin/ParentList.aspx")),
                        new MenuItem("User Role Management", "fas fa-user-cog", ResolveUrl("Admin/UserRoleManagement.aspx"))
                    }));
                            menuItems.Add(new MenuItem("Class Management", "fas fa-school", "", new List<MenuItem> {
                        new MenuItem("Class List", "fas fa-list", ResolveUrl("Admin/ClassList.aspx")),
                        new MenuItem("Assign Class Teacher", "fas fa-chalkboard", ResolveUrl("Admin/AssignClassTeacher.aspx")),
                        new MenuItem("Class Timetable", "fas fa-calendar-alt", ResolveUrl("Admin/ClassTimetable.aspx")),
                    }));
                            menuItems.Add(new MenuItem("Subject Management", "fas fa-book", "", new List<MenuItem> {
                        new MenuItem("Subject List", "fas fa-list", ResolveUrl("Admin/SubjectList.aspx")),
                        new MenuItem("Assign Subject to Class", "fas fa-chalkboard", ResolveUrl("Admin/AssignSubjectToClass.aspx")),
                        new MenuItem("Assign Teacher to Subject", "fas fa-chalkboard-teacher", ResolveUrl("Admin/AssignTeacherToSubject.aspx")),
                    }));
                            menuItems.Add(new MenuItem("Attendance Management", "fas fa-calendar-check", "", new List<MenuItem> {
                        new MenuItem("Mark Attendance", "fas fa-check", ResolveUrl("Admin/MarkAttendance.aspx")),
                        new MenuItem("View Attendance Reports", "fas fa-chart-line", ResolveUrl("Admin/AttendanceReports.aspx")),
                    }));
                            menuItems.Add(new MenuItem("Exam Management", "fas fa-file-alt", "", new List<MenuItem> {
                        new MenuItem("Exam Schedule", "fas fa-calendar-alt", ResolveUrl("Admin/ExamSchedule.aspx")),
                        new MenuItem("Grade Management", "fas fa-graduation-cap", ResolveUrl("Admin/GradeManagement.aspx")),
                        new MenuItem("Generate Report Cards", "fas fa-file-signature", ResolveUrl("Admin/GenerateReportCards.aspx")),
                    }));
                            menuItems.Add(new MenuItem("Fee Management", "fas fa-dollar-sign", "", new List<MenuItem> {
                        new MenuItem("Fee Structure", "fas fa-money-check-alt", ResolveUrl("Admin/FeeStructure.aspx")),
                        new MenuItem("Payment Management", "fas fa-credit-card", ResolveUrl("Admin/PaymentManagement.aspx")),
                        new MenuItem("Payment Report", "fas fa-file-invoice", ResolveUrl("Admin/PaymentReport.aspx")),
                    }));
                            menuItems.Add(new MenuItem("Library Management", "fas fa-book-open", "", new List<MenuItem> {
                        new MenuItem("Book List", "fas fa-list", ResolveUrl("Admin/BookList.aspx")),
                        new MenuItem("Issue/Return Books", "fas fa-book-reader", ResolveUrl("Admin/IssueReturnBooks.aspx")),
                        new MenuItem("Overdue Books/Fine Management", "fas fa-exclamation-circle", ResolveUrl("Admin/OverdueBooks.aspx")),
                    }));
                            menuItems.Add(new MenuItem("Notifications", "fas fa-bullhorn", "", new List<MenuItem> {
                        new MenuItem("Announcements Board", "fas fa-bullhorn", ResolveUrl("Admin/Announcements.aspx")),
                        new MenuItem("Send Email/SMS Notifications", "fas fa-envelope", ResolveUrl("Admin/SendNotifications.aspx")),
                    }));

                    break;

                case "teacher": // teacher
                    menuItems.Add(new MenuItem("Dashboard", "fas fa-home", ResolveUrl("Teacher/Dashboard.aspx")));
                    menuItems.Add(new MenuItem("Attendance", "fas fa-calendar-check", "", new List<MenuItem> {
                        new MenuItem("Mark Attendance", "fas fa-check", ResolveUrl("Teacher/MarkAttendance.aspx")),
                        new MenuItem("View Attendance Reports", "fas fa-chart-line", ResolveUrl("Teacher/AttendanceReports.aspx")),
                    }));
                            menuItems.Add(new MenuItem("Exams & Grades", "fas fa-file-alt", "", new List<MenuItem> {
                        new MenuItem("View Exam Schedule", "fas fa-calendar-alt", ResolveUrl("Teacher/ViewExamSchedule.aspx")),
                        new MenuItem("Enter Grades", "fas fa-graduation-cap", ResolveUrl("Teacher/EnterGrades.aspx")),
                        new MenuItem("View Grade Reports", "fas fa-chart-line", ResolveUrl("Teacher/ViewGradeReports.aspx")),
                    }));
                            menuItems.Add(new MenuItem("Timetable", "fas fa-calendar-alt", "", new List<MenuItem> {
                        new MenuItem("View Class Timetable", "fas fa-table", ResolveUrl("Teacher/ViewClassTimetable.aspx")),
                        new MenuItem("View Personal Schedule", "fas fa-calendar-day", ResolveUrl("Teacher/ViewPersonalSchedule.aspx")),
                    }));
                            menuItems.Add(new MenuItem("Assignments", "fas fa-tasks", "", new List<MenuItem> {
                        new MenuItem("Create Assignment", "fas fa-plus", ResolveUrl("Teacher/CreateAssignment.aspx")),
                        new MenuItem("Grade Assignment", "fas fa-check-circle", ResolveUrl("Teacher/GradeAssignment.aspx")),
                        new MenuItem("View Assignment Submissions", "fas fa-list", ResolveUrl("Teacher/ViewAssignmentSubmissions.aspx")),
                    }));
                            menuItems.Add(new MenuItem("Notifications", "fas fa-bullhorn", "", new List<MenuItem> {
                        new MenuItem("View Announcements", "fas fa-bullhorn", ResolveUrl("Teacher/ViewAnnouncements.aspx")),
                        new MenuItem("Send Notifications", "fas fa-envelope", ResolveUrl("Teacher/SendNotifications.aspx")),
                    }));

                    break;

                case "student": // student
                    menuItems.Add(new MenuItem("Dashboard", "fas fa-home", ResolveUrl("Student/Dashboard.aspx")));
                    menuItems.Add(new MenuItem("Profile", "fas fa-user", ResolveUrl("Student/ViewEditProfile.aspx")));

                    menuItems.Add(new MenuItem("Attendance", "fas fa-calendar-check", "", new List<MenuItem> {
                        new MenuItem("View Attendance Report", "fas fa-chart-line", ResolveUrl("Student/ViewAttendanceReport.aspx")),
                    }));

                    menuItems.Add(new MenuItem("Exams & Results", "fas fa-file-alt", "", new List<MenuItem> {
                        new MenuItem("View Exam Schedule", "fas fa-calendar-alt", ResolveUrl("Student/ViewExamSchedule.aspx")),
                        new MenuItem("View Grades/Report Cards", "fas fa-graduation-cap", ResolveUrl("Student/ViewGrades.aspx")),
                        new MenuItem("Download Report Cards", "fas fa-file-download", ResolveUrl("Student/DownloadReportCard.aspx")),
                    }));
                            menuItems.Add(new MenuItem("Assignments", "fas fa-tasks", "", new List<MenuItem> {
                        new MenuItem("View Assigned Work", "fas fa-list", ResolveUrl("Student/ViewAssignedWork.aspx")),
                        new MenuItem("Submit Assignment", "fas fa-upload", ResolveUrl("Student/SubmitAssignment.aspx")),
                    }));
                            menuItems.Add(new MenuItem("Fee Management", "fas fa-dollar-sign", "", new List<MenuItem> {
                        new MenuItem("View Fee Structure", "fas fa-money-check-alt", ResolveUrl("Student/ViewFeeStructure.aspx")),
                        new MenuItem("Pay Fees", "fas fa-credit-card", ResolveUrl("Student/PayFees.aspx")),
                        new MenuItem("View Payment History", "fas fa-file-invoice-dollar", ResolveUrl("Student/ViewPaymentHistory.aspx")),
                    }));
                            menuItems.Add(new MenuItem("Library", "fas fa-book-open", "", new List<MenuItem> {
                        new MenuItem("View Issued Books", "fas fa-book", ResolveUrl("Student/ViewIssuedBooks.aspx")),
                        new MenuItem("Return Books", "fas fa-undo", ResolveUrl("Student/ReturnBooks.aspx")),
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