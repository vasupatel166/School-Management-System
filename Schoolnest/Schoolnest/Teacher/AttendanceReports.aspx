<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AttendanceReports.aspx.cs" Inherits="Schoolnest.Teacher.AttendanceReports" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<form id="attendanceReportForm" runat="server" class="w-100">
    <div class="row">
        <div class="col-md-12">
            <div class="card mb-0">
                <div class="card-header">
                    <div class="card-title">Attendance Reports</div>
                </div>

                <div class="card-body">
                    <!-- Row with Teacher Name, Std, Section -->
                    <div class="form-group row">
                        <!-- Teacher Name Label -->
                        <div class="col-md-4">
                            <asp:Label runat="server" AssociatedControlID="lblTeacher" Text="Teacher Name"></asp:Label>
                            <asp:Label ID="lblTeacher" runat="server" CssClass="form-control"></asp:Label>
                        </div>

                        <!-- Std Label -->
                        <div class="col-md-4">
                            <asp:Label runat="server" AssociatedControlID="lblStd" Text="Standard"></asp:Label>
                            <asp:Label ID="lblStd" runat="server" CssClass="form-control"></asp:Label>
                        </div>

                        <!-- Section Label -->
                        <div class="col-md-4">
                            <asp:Label runat="server" AssociatedControlID="lblDivision" Text="Division"></asp:Label>
                            <asp:Label ID="lblDivision" runat="server" CssClass="form-control"></asp:Label>
                        </div>
                    </div>

                    <!-- Row with From Date and To Date -->
                    <div class="form-group row">
                        <!-- From Date Picker -->
                        <div class="col-md-6">
                            <asp:Label runat="server" AssociatedControlID="txtFromDate" Text="From Date"></asp:Label>
                            <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date" placeholder="Select From Date"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvFromDate" runat="server" ControlToValidate="txtFromDate" Display="Dynamic" ErrorMessage="Please select a from date" CssClass="text-danger" />
                        </div>

                        <!-- To Date Picker -->
                        <div class="col-md-6">
                            <asp:Label runat="server" AssociatedControlID="txtToDate" Text="To Date"></asp:Label>
                            <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date" placeholder="Select To Date"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvToDate" runat="server" ControlToValidate="txtToDate" Display="Dynamic" ErrorMessage="Please select a to date" CssClass="text-danger" />
                        </div>
                    </div>

                    <!-- Buttons: Generate Report, Show Attendance, Cancel -->
                    <div class="card-footer text-center mt-4 pt-4">
                        <asp:Button ID="btnGenerateReport" runat="server" Text="Generate Report" CssClass="btn btn-primary" OnClick="btnGenerateReport_Click" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</form>

</asp:Content>
