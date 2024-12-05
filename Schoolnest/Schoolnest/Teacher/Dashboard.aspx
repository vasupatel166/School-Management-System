<%@ Page Title="Teacher Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Schoolnest.Teacher.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- FullCalendar Stylesheet -->
    <link href="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.4/main.min.css" rel="stylesheet" />

    <style>
        .fc-prev-button, .fc-next-button{
            background:#1a2035 !important;
        }
        .fc-today-button {
            background-color:#1a2035 !important;
        }
    </style>

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
                                        <i class="fas fa-calendar-day text-warning"></i>
                                    </div>
                                </div>
                                <div class="col-7 col-stats">
                                    <div class="numbers">
                                        <p class="card-category">Today's Classes</p>
                                        <h4 class="card-title" id="ToadyClasses" runat="server">0</h4>
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
                                        <i class="fas fa-calendar-alt text-success"></i>
                                    </div>
                                </div>
                                <div class="col-7 col-stats">
                                    <div class="numbers">
                                        <p class="card-category">Total Weekly Classes</p>
                                        <h4 class="card-title" id="TotalClasses" runat="server">0</h4>
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
            </div>

            <!-- Top 5 Students Performance Chart If Class Teacher Logged In -->
            <div class="row" id="PerformanceChart" runat="server">
                <div class="col-md-12">
                    <div class="card">
                        <div class="card-header d-flex justify-content-between align-items-center">
                            <h4 class="card-title">Top 5 Students Performance</h4>
                            <div class="form-group mb-0">
                                <h5 class="card-subtitle" id="ClassDetails" runat="server"></h5>
                            </div>
                        </div>
                        <div class="card-body">
                            <asp:Literal ID="TopStudentsChart" runat="server"></asp:Literal>
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
                                    <table class="table table-bordered">
                                        <thead>
                                            <tr>
                                                <th>Time</th>
                                                <th>Subject</th>
                                                <th>Class</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("Time") %></td>
                                        <td><%# Eval("Subject") %></td>
                                        <td><%# Eval("Class") %></td>
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

                <!-- Attendance -->
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
                            <div id="attendanceCalendar" runat="server"></div>
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
                            <asp:HyperLink ID="EventsLink" runat="server" CssClass="btn btn-sm btn-primary float-end" NavigateUrl="~/Teacher/ViewEvents.aspx">View All Events</asp:HyperLink>
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
                            <asp:HyperLink runat="server" ID="HolidaysLink" CssClass="btn btn-sm btn-primary float-end" NavigateUrl="~/Teacher/ViewHolidays.aspx">View All Holidays</asp:HyperLink>
                        </div>
                        <div class="card-body">
                            <ul class="list-group" id="UpcomingHolidays" runat="server"></ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>

    <!-- FullCalendar Script -->
    <script src='https://cdn.jsdelivr.net/npm/fullcalendar@6.1.15/index.global.min.js'></script>

    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            const hiddenField = document.getElementById('<%= AttendanceDataHiddenField.ClientID %>');
            console.log('Raw Attendance Data:', hiddenField.value);

            let attendanceData;
            try {
                attendanceData = JSON.parse(hiddenField.value || '[]');
            } catch (error) {
                console.error('JSON Parsing Error:', error);
                attendanceData = [];
            }

            const calendarEl = document.getElementById('<%= attendanceCalendar.ClientID %>');
            const calendar = new FullCalendar.Calendar(calendarEl, {
                initialView: 'dayGridMonth',
                headerToolbar: {
                    left: 'prev today',
                    center: 'title',
                    right: 'next'
                },
                events: attendanceData,
                dayMaxEventRows: 2, // Limit number of events per day
                height: 'auto', // Adjust calendar height automatically
                contentHeight: 400 // Set a fixed content height
            });

            calendar.render();
        });
    </script>

</asp:Content>
