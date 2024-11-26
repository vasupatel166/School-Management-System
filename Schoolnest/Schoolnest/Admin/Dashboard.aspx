﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Schoolnest.Admin.Dashboard" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div>
            <h3 class="fw-bold mb-3" id="SchoolNameHeading" runat="server"></h3> <%--School Name--%>
        </div>
        <%--4 Cards (Students,Teachers,Active Classes,Pending Fees)--%>
        <div class="row">
            <div class="col-sm-6 col-md-3">
            <div class="card card-stats card-round">
                <div class="card-body">
                <div class="row align-items-center">
                    <div class="col-icon">
                    <div
                        class="icon-big text-center icon-primary bubble-shadow-small"
                    >
                        <i class="fas fa-users"></i>
                    </div>
                    </div>
                    <div class="col col-stats ms-3 ms-sm-0">
                    <div class="numbers">
                        <p class="card-category">Total Students</p>
                        <asp:Label CssClass="card-title" ID="TotalStudentsCard" runat="server"></asp:Label>
                    </div>
                    </div>
                </div>
                </div>
            </div>
            </div>
            <div class="col-sm-6 col-md-3">
            <div class="card card-stats card-round">
                <div class="card-body">
                <div class="row align-items-center">
                    <div class="col-icon">
                    <div
                        class="icon-big text-center icon-info bubble-shadow-small"
                    >
                        <i class="fas fa-chalkboard-teacher"></i>
                    </div>
                    </div>
                    <div class="col col-stats ms-3 ms-sm-0">
                    <div class="numbers">
                        <p class="card-category">Total Teachers</p>
                        <asp:Label CssClass="card-title" ID="TotalTeachersCard" runat="server"></asp:Label>
                    </div>
                    </div>
                </div>
                </div>
            </div>
            </div>
            <div class="col-sm-6 col-md-3">
            <div class="card card-stats card-round">
                <div class="card-body">
                <div class="row align-items-center">
                    <div class="col-icon">
                    <div
                        class="icon-big text-center icon-success bubble-shadow-small"
                    >
                        <i class="fas fa-laptop"></i>
                    </div>
                    </div>
                    <div class="col col-stats ms-3 ms-sm-0">
                    <div class="numbers">
                        <p class="card-category">Total Active Classes</p>
                        <asp:Label CssClass="card-title" ID="TotalActiveClassesCard" runat="server"></asp:Label>
                    </div>
                    </div>
                </div>
                </div>
            </div>
            </div>
            <div class="col-sm-6 col-md-3">
            <div class="card card-stats card-round">
                <div class="card-body">
                <div class="row align-items-center">
                    <div class="col-icon">
                    <div
                        class="icon-big text-center icon-secondary bubble-shadow-small"
                    >
                        <i class="fas fa-rupee-sign"></i>
                    </div>
                    </div>
                    <div class="col col-stats ms-3 ms-sm-0">
                    <div class="numbers">
                        <p class="card-category">Total Pending Fees</p>
                        <asp:Label CssClass="card-title" ID="TotalPendingFeesCard" runat="server"></asp:Label>
                    </div>
                    </div>
                </div>
                </div>
            </div>
            </div>
        </div>

        <%--Upcoming Events & Weekly Attendace Overview of Teachers and Students--%>
        <div class="row">
            <div class="col-md-4">
                <div class="card card-round">
                    <div class="card-body">
                        <div class="card-head-row card-tools-still-right">
                            <div class="card-title">Upcoming Events</div>
                                <div class="card-tools">
                                    <div class="dropdown">
                                        <button class="btn btn-icon btn-clean me-0" type="button" id="dropdownMenuButton" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                            <i class="fas fa-ellipsis-h"></i>
                                        </button>
                                        <div class="dropdown-menu" aria-labelledby="dropdownMenuButton">
                                            <asp:HyperLink ID="AllEventsLink" NavigateUrl="~/Admin/EventMaster.aspx" runat="server" CssClass="dropdown-item">All Events</asp:HyperLink>
                                        </div>
                                   </div>
                            </div>
                        </div>
                        <div class="card-list py-0" id="UpcomingEvents" runat="server">
                        </div>
                    </div>
                </div>
            </div>   
            <div class="col-md-8">
                <div class="card card-round">
                  <div class="card-header">
                    <div class="card-head-row card-tools-still-right">
                      <div class="card-title">Weekly Attendance Overview</div>
                    </div>
                  </div>
                  <div class="card-body p-0">
                    <canvas id="attendanceChart"></canvas>
                  </div>
                </div>
              </div>
        </div>       
    </form>

    <!-- Chart JS -->
    <script src="<%= ResolveUrl("~/assets/js/plugin/chart.js/chart.min.js") %>"></script>

</asp:Content>
