<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Schoolnest.Admin.Dashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server">
                    <div class="content col-md-9 col-lg-10">
                    <div class="header d-flex justify-content-between align-items-center">
                        <h2><asp:Label ID="schoolNameLabel" runat="server" CssClass="text-black"><%= schoolNameLabel.Text %></asp:Label></h2>
                     </div>

                    <div class="row">
                        <!-- KPIs Section -->
                       <div class="col-md-6 mb-4">
                    <div class="card dashboard-card p-3 text-center bg-light">
                        <i class="icon fas fa-user-graduate text-primary"></i>
                        <h5>Total Students</h5>
                        <asp:Label ID="totalStudentsLabel" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="col-md-6 mb-4">
                    <div class="card dashboard-card p-3 text-center bg-light">
                        <i class="icon fas fa-chalkboard-teacher text-success"></i>
                        <h5>Total Teachers</h5>
                        <asp:Label ID="totalTeachersLabel" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="col-md-6 mb-4">
                    <div class="card dashboard-card p-3 text-center bg-light">
                        <i class="icon fas fa-school text-warning"></i>
                        <h5>Active Classes</h5>
                        <asp:Label ID="activeClassesLabel" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="col-md-6 mb-4">
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
                                        <asp:Repeater ID="UpcomingEventsRepeater" runat="server">
                                            <ItemTemplate>
                                                <li class="list-group-item"><%# Eval("EventTitle") %> - <%# Eval("EventDate", "{0:MMM dd}") %></li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                    <asp:Label ID="NoEventsLabel" runat="server" Text="" ForeColor="Red" />
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
                        <div class="col-md-6 mb-4">
                        <div class="card dashboard-card p-3 bg-light">
                            <h5>Budget Utilization</h5>
                            <p id="budgetUtilizationText"></p>
                            <canvas id="budgetChart"></canvas>
                            </div>
                        </div>

                        <div class="col-md-6 mb-4">
                            <div class="card dashboard-card p-3 bg-light">
                                <h5>Settings</h5>
                                <ul class="list-group">
                                    <li class="list-group-item">
                                        <a href="ProfileSettings.aspx">Edit Profile</a>
                                    </li>
                                    
                                    <li class="list-group-item">
                                        <a href="SchoolSettings.aspx">School Information</a>
                                    </li>
                                   
                                    <li class="list-group-item">
                                        <a href="BackupRestore.aspx">Backup & Restore</a>
                                    </li>

                                     <li class="list-group-item">
                                        <a href="SecuritySettings.aspx">Security Settings</a>
                                     </li>
                                     <li class="list-group-item">
                                        <a href="AnalyticsDashboard.aspx">Analytics Dashboard</a>
                                     </li>
                                </ul>
                            </div>

                        </div>


                        <!-- Chart Section (Existing Fees Chart) -->
                        <div class="row">
                        <div class="col-md-12 mb-4">
    <div class="card dashboard-card bg-light">
        <h5>Defaulter Student List</h5>
        <table class="table table-bordered">
            <asp:Repeater ID="RepeaterDefaulters" runat="server">
                <HeaderTemplate>
                    <thead>
                        <tr>
                            <th>Student Name</th>
                            <th>Standard</th>
                            <th>Division</th>
                            <th>Fee Type</th>
                            <th>Total Fees Pending</th>
                        </tr>
                    </thead>
                    <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("StudentName") %></td>
                        <td><%# Eval("Standard") %></td>
                        <td><%# Eval("Division") %></td>
                        <td><%# Eval("FeeDesc") %></td>
                        <td><%# Eval("Pending_Fees") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
                </FooterTemplate>
            </asp:Repeater>
        </table>
    </div>
</div>
                        </div>

                        


                    </div>

                    <!-- Include Chart.js Script -->
                    <!-- Include Chart.js Library -->
                        <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

<!-- Your Custom JavaScript Code -->
<script>
    
    // Fetch attendance data from the server
    function fetchAttendanceData() {
        $.ajax({
            type: "POST",
            url: "Dashboard.aspx/GetAttendanceData", // Adjust the URL to match your page
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var data = JSON.parse(response.d);
                var labels = data.map(function (item) {
                    var date = new Date(item.AttendanceDate);
                    var weekday = date.toLocaleDateString('en-US', { weekday: 'short' }); // Get short day name (e.g., 'Mon', 'Tue')
                    var day = date.toLocaleDateString('en-US', { day: 'numeric', month: 'short' }); // Get day and month (e.g., 'Oct 01')
                    return `${weekday} (${day})`; // Combine weekday and date
                });
                var studentAttendance = data.map(function (item) {
                    return item.TotalStudentsPresent;
                });
                var teacherAttendance = data.map(function (item) {
                    return item.TotalTeachersPresent;
                });

                // Create the chart with fetched data
                createAttendanceChart(labels, studentAttendance, teacherAttendance);
            },
            error: function (error) {
                console.error("Error fetching attendance data:", error);
            }
        });
    }

    // Function to create the chart
    function createAttendanceChart(labels, studentData, teacherData) {
        var attendanceCtx = document.getElementById('attendanceChart').getContext('2d');
        var attendanceChart = new Chart(attendanceCtx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Student Attendance',
                    data: studentData,
                    backgroundColor: 'rgba(54, 162, 235, 0.2)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    fill: true,
                    borderWidth: 1
                }, {
                    label: 'Teacher Attendance',
                    data: teacherData,
                    backgroundColor: 'rgba(255, 99, 132, 0.2)',
                    borderColor: 'rgba(255, 99, 132, 1)',
                    fill: true,
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }

    // Fetch budget Data from server

    function fetchBudgetData() {
        $.ajax({
            type: "POST",
            url: "Dashboard.aspx/GetBudgetData", // Adjust the URL to match your page
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var data = JSON.parse(response.d);
                var labels = data.map(function (item) {
                    return item.CategoryName;
                });
                var actualExpenditures = data.map(function (item) {
                    return item.ActualExpenditure;
                });
                var totalBudget = data.reduce((acc, b) => acc + b.Amount, 0);
                var totalExpenditure = data.reduce((acc, b) => acc + b.ActualExpenditure, 0);
                var utilizationPercentage = (totalBudget > 0) ? (totalExpenditure / totalBudget) * 100 : 0;

                document.getElementById('budgetUtilizationText').innerText =
                    `Budget Utilized: ${utilizationPercentage.toFixed(2)}%`;

                createBudgetChart(labels, actualExpenditures);
            },
            error: function (error) {
                console.error("Error fetching budget data:", error);
            }
        });
    }

    function createBudgetChart(labels, data) {
        var budgetCtx = document.getElementById('budgetChart').getContext('2d');
        var budgetChart = new Chart(budgetCtx, {
            type: 'pie',
            data: {
                labels: labels,
                datasets: [{
                    data: data,
                    backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF'],
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                    },
                }
            }
        });
    }

    // Call the fetch function to load data when the page is ready
    $(document).ready(function () {
        fetchAttendanceData();
        fetchBudgetData();
        
    });


    
</script>

                   
                </div>
        </form>
</asp:Content>
