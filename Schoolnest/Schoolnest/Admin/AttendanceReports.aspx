<%@ Page Title="Attendance Reports" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AttendanceReports.aspx.cs" Inherits="Schoolnest.Admin.ViewAttendenceReports" %>
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
                        <!-- Month and Year Selection -->
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlUser" Text="Select User"></asp:Label>
                                    <asp:DropDownList ID="ddlUser" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="Teacher" Value="Teacher"></asp:ListItem>
                                        <asp:ListItem Text="Student" Value="Student"></asp:ListItem>
                                        <asp:ListItem Text="Both" Value="Both"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlMonth" Text="Select Month"></asp:Label>
                                    <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlYear" Text="Select Year"></asp:Label>
                                    <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="2024" Value="2024"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Button ID="btnGenerateReport" runat="server" Text="View" CssClass="btn btn-primary" OnClick="btnGenerateReport_Click" />
                                </div>
                            </div>
                        </div>  
                        <div class="row">
                            <div class="col-md-12">
                                <asp:GridView ID="gvAttendanceReport" runat="server" CssClass="table table-bordered" AutoGenerateColumns="False">
                                    <Columns>
                                        <asp:BoundField DataField="TeacherName" HeaderText="Teacher Name" />
                                        <asp:BoundField DataField="Date" HeaderText="Date" />
                                        <asp:BoundField DataField="Status" HeaderText="Status" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
