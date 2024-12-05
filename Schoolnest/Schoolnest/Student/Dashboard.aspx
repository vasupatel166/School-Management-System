<%@ Page Title="Student Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Schoolnest.Student.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="container-fluid">
            <!-- School Name -->
            <div class="row mb-3">
                <div class="col-12">
                    <div class="alert alert-success d-flex justify-content-left align-items-center" role="alert">
                        <i class="fas fa-school fs-4"></i>
                        <h3 class="mb-0 ms-4" id="SchoolNameHeader" runat="server"></h3>
                    </div>
                </div>
            </div>

            <!-- Cards for Summary -->
            <div class="row">
                <div class="col-sm-6 col-md-3">
                    <div class="card card-stats card-round">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-5">
                                    <div class="icon-big text-center">
                                        <i class="fas fa-chalkboard-teacher text-warning"></i>
                                    </div>
                                </div>
                                <div class="col-7 col-stats">
                                    <div class="numbers">
                                        <p class="card-category">Class</p>
                                        <h6 class="fs-4" id="StudentStandardName" runat="server"></h6>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-md-3">
                    <div class="card card-stats card-round">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-5">
                                    <div class="icon-big text-center">
                                        <i class="fas fa-check-circle text-primary"></i>
                                    </div>
                                </div>
                                <div class="col-7 col-stats">
                                    <div class="numbers">
                                        <p class="card-category">Days Present</p>
                                        <h4 class="card-title" id="DaysPresent" runat="server">0</h4>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-md-3">
                    <div class="card card-stats card-round">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-5">
                                    <div class="icon-big text-center">
                                        <i class="fas fa-times-circle text-danger"></i>
                                    </div>
                                </div>
                                <div class="col-7 col-stats">
                                    <div class="numbers">
                                        <p class="card-category">Days Absent</p>
                                        <h4 class="card-title" id="DaysAbsent" runat="server">0</h4>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-md-3">
                    <div class="card card-stats card-round">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-5">
                                    <div class="icon-big text-center">
                                        <i class="fas fa-wallet text-success"></i>
                                    </div>
                                </div>
                                <div class="col-7 col-stats">
                                    <div class="numbers">
                                        <p class="card-category">Total Remaining Fees</p>
                                        <h4 class="card-title" id="FeesRemaining" runat="server">0</h4>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Row for Timetable and Events -->
            <div class="row">
                <!-- Today's Timetable -->
                <div class="col-md-6">
                    <div class="card">
                        <div class="card-header">
                            <h4 class="card-title">Today's Timetable</h4>
                        </div>
                        <div class="card-body">
                            <asp:Repeater ID="TodaysTimetableRepeater" runat="server">
                                <HeaderTemplate>
                                    <table class="table table-bordered timetable">
                                        <thead>
                                            <tr>
                                                <th>Time</th>
                                                <th>Subject</th>
                                                <th>Teacher</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("Time") %></td>
                                        <td><%# Eval("Subject") %></td>
                                        <td><%# Eval("Teacher") %></td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>

                <!-- Attendance Pie Chart -->
                <div class="col-md-6">
                    <div class="card">
                        <div class="card-header d-flex justify-content-between align-items-center">
                            <h4 class="card-title">Attendance</h4>
                            <div class="form-group mb-0">
                                <asp:DropDownList ID="ddlAttendanceType" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddlAttendanceType_SelectedIndexChanged">
                                    <asp:ListItem Text="Weekly" Value="Weekly" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Monthly" Value="Monthly"></asp:ListItem>
                                    <asp:ListItem Text="Yearly" Value="Yearly"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:HiddenField ID="AttendanceDataHiddenField" runat="server" />
                            </div>
                        </div>
                        <div class="card-body">
                            <canvas id="attendancePieChart" class="piechart"></canvas>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <!-- Upcoming 5 Events -->
                <div class="col-md-6">
                    <div class="card">
                        <div class="card-header d-flex justify-content-between align-items-center">
                            <h4 class="card-title">Upcoming Events</h4>
                            <asp:HyperLink ID="EventsLink" runat="server" CssClass="btn btn-sm btn-primary float-end" NavigateUrl="~/Student/UpcomingEvents.aspx">View All Events</asp:HyperLink>
                        </div>
                        <div class="card-body">
                            <ul class="list-group" id="UpcomingEvents" runat="server"></ul>
                        </div>
                    </div>
                </div>

                <!-- Upcoming 5 Holidays -->
                <div class="col-md-6">
                    <div class="card">
                        <div class="card-header d-flex justify-content-between align-items-center">
                            <h4 class="card-title">Upcoming Holidays</h4>
                        </div>
                        <div class="card-body">
                            <ul class="list-group" id="UpcomingHolidays" runat="server"></ul>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Upcoming Exams and Last Exam Grades -->
            <div class="row">
                <!-- Upcoming Exams -->
                <div class="col-md-6">
                    <div class="card">
                        <div class="card-header d-flex justify-content-between align-items-center">
                            <h4 class="card-title">Upcoming Exams</h4>
                        </div>
                        <div class="card-body">
                            <div class="list-group" id="UpcomingExams" runat="server"></div>
                        </div>
                    </div>
                </div>

                <!-- Last Exam Grades -->
                <div class="col-md-6">
                    <div class="card">
                        <div class="card-header d-flex justify-content-between align-items-center">
                            <h4 class="card-title">Last Exam Grades</h4>
                        </div>
                        <div class="card-body">
                            <div class="list-group" id="LastExamGrades" runat="server"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>

    <!-- Include Chart.js -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

    <script type="text/javascript">
        function updateChart() {
            var attendanceData = JSON.parse('<%= AttendanceDataHiddenField.Value %>');

            var daysPresent = attendanceData.DaysPresent;
            var daysAbsent = attendanceData.DaysAbsent;

            var ctx = document.getElementById('attendancePieChart').getContext('2d');
            var chart = new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: ['Present', 'Absent'],
                    datasets: [{
                        label: 'Attendance',
                        data: [daysPresent, daysAbsent],
                        backgroundColor: ['#4caf50', '#f44336'],
                        borderColor: ['#388e3c', '#d32f2f'],
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'top',
                        },
                        tooltip: {
                            callbacks: {
                                label: function (tooltipItem) {
                                    return tooltipItem.label + ': ' + tooltipItem.raw + ' days';
                                }
                            }
                        }
                    }
                }
            });
        }

        window.onload = updateChart;
    </script>
</asp:Content>
