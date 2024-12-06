<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AttendanceReports.aspx.cs" Inherits="Schoolnest.Admin.AttendanceReports" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .hidden {
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="attendanceForm" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header">
                        <div class="card-title">Attendance Reports</div>
                    </div>
                    <div class="card-body">
                        <!-- Row 1: Filter By -->
                        <div class="form-group row">
                            <div class="col-md-6 offset-md-3">
                                <asp:Label runat="server" AssociatedControlID="ddlFilter" Text="Filter by" CssClass="form-label"></asp:Label>
                                <asp:DropDownList ID="ddlFilter" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddlFilter_SelectedIndexChanged">
                                    <asp:ListItem Value="Student" Text="Student" Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="Teacher" Text="Teacher"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <!-- Row 2: Standard and Division -->
                        <div class="form-group row" id="studentFilters" runat="server">
                            <div class="col-md-6">
                                <asp:Label runat="server" AssociatedControlID="ddlStandard" Text="Standard" CssClass="form-label"></asp:Label>
                                <asp:DropDownList ID="ddlStandard" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                            <div class="col-md-6">
                                <asp:Label runat="server" AssociatedControlID="ddlDivision" Text="Division" CssClass="form-label"></asp:Label>
                                <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>
                        <!-- Row 3: Date Range -->
                        <div class="form-group row">
                            <div class="col-md-6">
                                <asp:Label runat="server" AssociatedControlID="txtFromDate" Text="From Date" CssClass="form-label"></asp:Label>
                                <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-6">
                                <asp:Label runat="server" AssociatedControlID="txtToDate" Text="To Date" CssClass="form-label"></asp:Label>
                                <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="card-footer text-center">
                        <!-- View Button -->
                        <asp:Button ID="btnView" runat="server" Text="View" CssClass="btn btn-primary" OnClick="btnView_Click" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-primary" OnClick="btnCancel_Click"/>
                    </div>
                </div>
                <div class="card mt-4">
                    <div class="card-body">
                        <!-- Attendance Grid -->
                        <asp:GridView ID="gvAttendance" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered" Visible="false">
                            <Columns>
                                <asp:BoundField DataField="Name" HeaderText="Name" />
                                <asp:BoundField DataField="AttendanceDate" HeaderText="Date" />
                                <asp:BoundField DataField="AttendanceStatus" HeaderText="Attendance Status" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
