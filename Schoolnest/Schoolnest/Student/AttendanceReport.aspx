<%@ Page Title="Attendance Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AttendanceReport.aspx.cs" Inherits="Schoolnest.Student.AttendanceReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Attendance Reports</div>
                    </div>
                    <div class="card-body">
                        <!-- Filter By Dropdown -->
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlFilterBy" Text="Filter By" CssClass="control-label"></asp:Label>
                                    <asp:DropDownList ID="ddlFilterBy" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlFilterBy_SelectedIndexChanged">
                                        <asp:ListItem Text="Select" Value="" />
                                        <asp:ListItem Text="Monthly" Value="Monthly" />
                                        <asp:ListItem Text="Custom" Value="Custom" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <!-- Monthly Field (Initially hidden) -->
                        <div class="row " id="monthlyField" runat="server" visible="false">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlMonth" Text="Select Month"></asp:Label>
                                    <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="January" Value="January" />
                                        <asp:ListItem Text="February" Value="February" />
                                        <asp:ListItem Text="March" Value="March" />
                                        <asp:ListItem Text="April" Value="April" />
                                        <asp:ListItem Text="May" Value="May" />
                                        <asp:ListItem Text="June" Value="June" />
                                        <asp:ListItem Text="July" Value="July" />
                                        <asp:ListItem Text="August" Value="August" />
                                        <asp:ListItem Text="September" Value="September" />
                                        <asp:ListItem Text="October" Value="October" />
                                        <asp:ListItem Text="November" Value="November" />
                                        <asp:ListItem Text="December" Value="December" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <!-- Custom Date Fields (Initially hidden) -->
                        <div class="row" id="customField" runat="server" visible="false">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtFromDate" Text="From Date"></asp:Label>
                                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtToDate" Text="To Date"></asp:Label>
                                    <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <!-- Buttons -->
                        <div class="row text-center">
                            <div class="col-md-12">
                                <asp:Button ID="btnView" runat="server" Text="View" CssClass="btn btn-primary" OnClick="btnView_Click" />
                                <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-warning" OnClick="btnReset_Click" />
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                            </div>
                        </div>

                        <!-- Attendance Report Grid -->
                        <div class="row mt-4 p-4">
                            <div class="col-md-12">
                                <asp:GridView ID="gvAttendanceReport" runat="server" OnRowDataBound="gvAttendanceReport_RowDataBound" CssClass="table table-bordered" AutoGenerateColumns="False" Visible="false">
                                    <Columns>
                                        <asp:BoundField DataField="Date" HeaderText="Date" />
                                        <asp:BoundField DataField="Status" HeaderText="Status" />
                                    </Columns>
                                </asp:GridView>
                                <asp:Label ID="lblNoRecordsFound" runat="server" Visible="false" CssClass="text-danger"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>