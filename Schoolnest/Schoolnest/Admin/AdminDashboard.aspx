<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="Schoolnest.Admin.AdminDashboard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin Dashboard</title>
    <!-- Bootstrap CSS -->
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <!-- Custom CSS -->
    <link href="../Css/Style.css" rel="stylesheet" type="text/css" />
    <!-- Font Awesome Icons -->
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-fluid">
            <div class="row no-gutters">
                <!-- Sidebar -->
                <nav class="sidebar col-md-3 col-lg-2 bg-dark">
                   <h2><asp:Label ID="lbldashboard" runat="server" CssClass="text-white">Admin Dashboard</asp:Label></h2>
        
   
                    <ul class="nav flex-column">
    <li class="nav-item">
        <a href="#" class="nav-link text-white">Dashboard</a>
    </li>
    <!-- User Management -->
    <li class="nav-item menu-group">
        <a href="#" class="nav-link text-white menu-header">User Management</a>
        <ul class="submenu nav flex-column">
            <li class="nav-item"><a href="#" class="nav-link text-white">Admin List</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">Teacher List</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">Student List</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">Parent List</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">User Role Management</a></li>
        </ul>
    </li>
    <!-- Class Management -->
    <li class="nav-item menu-group">
        <a href="#" class="nav-link text-white menu-header">Class Management</a>
        <ul class="submenu nav flex-column">
            <li class="nav-item"><a href="#" class="nav-link text-white">Class List</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">Assign Class Teacher</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">Class Timetable</a></li>
        </ul>
    </li>
    <!-- Subject Management -->
    <li class="nav-item menu-group">
        <a href="#" class="nav-link text-white menu-header">Subject Management</a>
        <ul class="submenu nav flex-column">
            <li class="nav-item"><a href="#" class="nav-link text-white">Subject List</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">Assign Subject to Class</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">Assign Teacher to Subject</a></li>
        </ul>
    </li>
    <!-- Attendance -->
    <li class="nav-item menu-group">
        <a href="#" class="nav-link text-white menu-header">Attendance</a>
        <ul class="submenu nav flex-column">
            <li class="nav-item"><a href="#" class="nav-link text-white">Mark Attendance</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">View Attendance Reports</a></li>
        </ul>
    </li>
    <!-- Exam Management -->
    <li class="nav-item menu-group">
        <a href="#" class="nav-link text-white menu-header">Exam Management</a>
        <ul class="submenu nav flex-column">
            <li class="nav-item"><a href="#" class="nav-link text-white">Exam Schedule</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">Grade Management</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">Generate Report Cards</a></li>
        </ul>
    </li>
    <!-- Fee Management -->
    <li class="nav-item menu-group">
        <a href="#" class="nav-link text-white menu-header">Fee Management</a>
        <ul class="submenu nav flex-column">
            <li class="nav-item"><a href="#" class="nav-link text-white">Fee Structure</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">Payment Management</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">Payment Report</a></li>
        </ul>
    </li>
    <!-- Library Management -->
    <li class="nav-item menu-group">
        <a href="#" class="nav-link text-white menu-header">Library Management</a>
        <ul class="submenu nav flex-column">
            <li class="nav-item"><a href="#" class="nav-link text-white">Book List</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">Issue/Return Books</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">Overdue Books/Fine Management</a></li>
        </ul>
    </li>
    <!-- Notifications -->
    <li class="nav-item menu-group">
        <a href="#" class="nav-link text-white menu-header">Notifications</a>
        <ul class="submenu nav flex-column">
            <li class="nav-item"><a href="#" class="nav-link text-white">Announcements Board</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">Send Email/SMS Notifications</a></li>
        </ul>
    </li>
    <!-- Reports & Analytics -->
    <li class="nav-item menu-group">
        <a href="#" class="nav-link text-white menu-header">Reports & Analytics</a>
        <ul class="submenu nav flex-column">
            <li class="nav-item"><a href="#" class="nav-link text-white">Attendance Report</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">Grade Report</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">Fee Report</a></li>
            <li class="nav-item"><a href="#" class="nav-link text-white">Library Report</a></li>
        </ul>
    </li>
    <!-- Settings -->
    <li class="nav-item">
        <a href="#" class="nav-link text-white">Settings</a>
    </li>
</ul>
                </nav>

                <!-- Main Content -->
                <div class="content col-md-9 col-lg-10">
                    <div class="header d-flex justify-content-between align-items-center">
                        <h2><asp:Label ID="schoolNameLabel" runat="server" CssClass="text-black"><%= schoolNameLabel.Text %></asp:Label></h2>
                        <div>
                            <button class="btn btn-primary">Profile</button>
                            <button class="btn btn-danger">Logout</button>
                        </div>
                    </div>

                    <div class="row">
                        <!-- KPIs Section -->
                       <div class="col-md-3 mb-4">
                    <div class="card dashboard-card p-3 text-center bg-light">
                        <i class="icon fas fa-user-graduate text-primary"></i>
                        <h5>Total Students</h5>
                        <asp:Label ID="totalStudentsLabel" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="col-md-3 mb-4">
                    <div class="card dashboard-card p-3 text-center bg-light">
                        <i class="icon fas fa-chalkboard-teacher text-success"></i>
                        <h5>Total Teachers</h5>
                        <asp:Label ID="totalTeachersLabel" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="col-md-3 mb-4">
                    <div class="card dashboard-card p-3 text-center bg-light">
                        <i class="icon fas fa-school text-warning"></i>
                        <h5>Active Classes</h5>
                        <asp:Label ID="activeClassesLabel" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="col-md-3 mb-4">
                    <div class="card dashboard-card p-3 text-center bg-light">
                        <i class="icon fas fa-dollar-sign text-danger"></i>
                        <h5>Pending Fees</h5>
                        <asp:Label ID="pendingFeesLabel" runat="server"></asp:Label>
                    </div>
                </div>
                    </div>

                    <div class="row">
                        <!-- Upcoming Events Calendar -->
                        <div class="col-md-6 mb-4">
                            <div class="card dashboard-card p-3 bg-light">
                                <h5>Upcoming Events</h5>
                                <ul class="list-group">
                                    <li class="list-group-item">Science Fair - Oct 15</li>
                                    <li class="list-group-item">Parent-Teacher Meeting - Oct 20</li>
                                    <li class="list-group-item">Sports Day - Nov 5</li>
                                </ul>
                            </div>
                        </div>

                        <!-- Attendance Overview -->
                        <div class="col-md-6 mb-4">
                            <div class="card dashboard-card p-3 bg-light">
                                <h5>Attendance Overview</h5>
                                <canvas id="attendanceChart"></canvas>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <!-- User Activity/Logins Chart -->
                        <div class="col-md-6 mb-4">
                            <div class="card dashboard-card p-3 bg-light">
                                <h5>User Activity Logins</h5>
                                <canvas id="userActivityChart"></canvas>
                            </div>
                        </div>

                        <!-- Chart Section (Existing Fees Chart) -->
                        <div class="col-md-6 mb-4">
                            <div class="card dashboard-card bg-light">
                                <div class="chart-container p-3">
                                    <canvas id="myChart"></canvas>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Include Chart.js Script -->
                    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
                   <script src="../Scripts/Admin.js"></script>
                </div>
            </div>
        </div>
    </form>

    <!-- Bootstrap JS and dependencies -->
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/popper.js@1.16.1/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</body>
</html>
