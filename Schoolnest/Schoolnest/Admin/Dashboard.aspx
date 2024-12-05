<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Schoolnest.Admin.Dashboard" Async="true" %>

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

                <!-- Subjects Card -->
                <div class="col-sm-6 col-md-3">
                    <div class="card card-stats card-round">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-5">
                                    <div class="icon-big text-center">
                                        <i class="fas fa-book text-info"></i>
                                    </div>
                                </div>
                                <div class="col-7 col-stats">
                                    <div class="numbers">
                                        <p class="card-category">Total Subjects</p>
                                        <h6 class="fs-4" id="TotalSubjects" runat="server">0</h6>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Classes Card -->
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
                                        <p class="card-category">Active Classes</p>
                                        <h6 class="fs-4" id="TotalClasses" runat="server">0</h6>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Students Card -->
                <div class="col-sm-6 col-md-3">
                    <div class="card card-stats card-round">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-5">
                                    <div class="icon-big text-center">
                                        <i class="fas fa-users text-primary"></i>
                                    </div>
                                </div>
                                <div class="col-7 col-stats">
                                    <div class="numbers">
                                        <p class="card-category">Total Students</p>
                                        <h6 class="fs-4" id="TotalStudents" runat="server">0</h6>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Teachers Card -->
                <div class="col-sm-6 col-md-3">
                    <div class="card card-stats card-round">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-5">
                                    <div class="icon-big text-center">
                                        <i class="fas fa-chalkboard text-danger"></i>
                                    </div>
                                </div>
                                <div class="col-7 col-stats">
                                    <div class="numbers">
                                        <p class="card-category">Total Teachers</p>
                                        <h6 class="fs-4" id="TotalTeachers" runat="server">0</h6>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <!-- Buses Card -->
                <div class="col-sm-6 col-md-3">
                    <div class="card card-stats card-round">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-5">
                                    <div class="icon-big text-center">
                                        <i class="fas fa-bus text-secondary"></i>
                                    </div>
                                </div>
                                <div class="col-7 col-stats">
                                    <div class="numbers">
                                        <p class="card-category">Total Buses</p>
                                        <h6 class="fs-4" id="TotalBuses" runat="server">0</h6>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Remaining Fees Card -->
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

                <!-- Budget Remaining Card -->
                <div class="col-sm-6 col-md-3">
                    <div class="card card-stats card-round">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-5">
                                    <div class="icon-big text-center">
                                        <i class="fas fa-money-bill-wave text-success"></i>
                                    </div>
                                </div>
                                <div class="col-7 col-stats">
                                    <div class="numbers">
                                        <p class="card-category">Budget Remaining</p>
                                        <h6 class="fs-4" id="TotalBudgetRemaining" runat="server">0</h6>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <!-- Attendance Chart -->
                <div class="col-md-12">
                    <div class="card">
                        <div class="card-header d-flex justify-content-between align-items-center">
                            <h4 class="card-title">Attendance</h4>
                            <div class="form-group mb-0">
                                <asp:DropDownList ID="ddlAttendanceType" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddlAttendanceType_SelectedIndexChanged">
                                    <asp:ListItem Text="Weekly" Value="Weekly" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Monthly" Value="Monthly"></asp:ListItem>
                                    <asp:ListItem Text="Yearly" Value="Yearly"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:HiddenField ID="StudentAttendanceDataHiddenField" runat="server" />
                                <asp:HiddenField ID="TeacherAttendanceDataHiddenField" runat="server" />
                            </div>
                        </div>
                        <div class="card-body">
                            <!-- Canvas for Chart -->
                            <div class="row">
                                <div class="col-md-6 text-center">
                                    <canvas id="StudentAttendanceChart" class="chart"></canvas>
                                    <h2 class="card-title mt-5">Student Attendance</h2>
                                </div>
                                <div class="col-md-6 text-center">
                                    <canvas id="TeacherAttendanceChart" class="chart"></canvas>
                                     <h2 class="card-title mt-5">Teacher Attendance</h2>
                                </div>
                            </div>
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
                            <asp:HyperLink runat="server" ID="HolidaysLink" CssClass="btn btn-sm btn-primary float-end" NavigateUrl="~/Admin/HolidayMaster.aspx">View All Holidays</asp:HyperLink>
                        </div>
                        <div class="card-body">
                            <ul class="list-group" id="UpcomingHolidays" runat="server"></ul>
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
            var StudentAttendanceData = JSON.parse('<%= StudentAttendanceDataHiddenField.Value %>');
            var TeacherAttendanceData = JSON.parse('<%= TeacherAttendanceDataHiddenField.Value %>');

            var StudentDaysPresent = StudentAttendanceData.DaysPresent;
            var StudentDaysAbsent = StudentAttendanceData.DaysAbsent;

            var TeacherDaysPresent = TeacherAttendanceData.DaysPresent;
            var TeacherDaysAbsent = TeacherAttendanceData.DaysAbsent;

            var ctx = document.getElementById('StudentAttendanceChart').getContext('2d');
            var chart = new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: ['Present', 'Absent'],
                    datasets: [{
                        label: 'Student Attendance',
                        data: [StudentDaysPresent, StudentDaysAbsent],
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

            var ctx1 = document.getElementById('TeacherAttendanceChart').getContext('2d');
            var chart1 = new Chart(ctx1, {
                type: 'pie',
                data: {
                    labels: ['Present', 'Absent'],
                    datasets: [{
                        label: 'Teacher Attendance',
                        data: [TeacherDaysPresent, TeacherDaysAbsent],
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
