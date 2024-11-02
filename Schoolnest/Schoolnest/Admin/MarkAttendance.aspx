<%@ Page Title="Mark Attendance" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MarkAttendance.aspx.cs" Inherits="Schoolnest.Admin.MarkAttendance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Mark Attendance</div>
                    </div>

                    <div class="card-body">
                        <!-- Teacher Selection -->
                        <div class="row mb-3">
                            <div class="col-md-6">
                                <asp:Label runat="server" AssociatedControlID="ddlTeacher" Text="Select Teacher"></asp:Label>
                                <asp:DropDownList ID="ddlTeacher" runat="server" CssClass="form-control" AutoPostBack="true">
                                    <asp:ListItem Text="Select Teacher" Value=""></asp:ListItem>
                                    <asp:ListItem Value="Teacher 1" Text="Rajesh Sharma"></asp:ListItem>
                                    <asp:ListItem Value="Teacher 2" Text="Anjali Nair"></asp:ListItem>
                                    <asp:ListItem Value="Teacher 3" Text="Prakash Rao"></asp:ListItem>
                                    <asp:ListItem Value="Teacher 4" Text="Ashish Chauhan"></asp:ListItem>
                                    <asp:ListItem Value="Teacher 5" Text="Sunita Joshi"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-6">
                                <asp:Label runat="server" AssociatedControlID="txtDate" Text="Select Date"></asp:Label>
                                <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                        </div>

                        <!-- Attendance Buttons -->
                        <div class="row mb-2">
                            <div class="col-md-6">
                                <asp:Button ID="btnPresent" runat="server" Text="Present" CssClass="btn btn-success"  />
                                <asp:Button ID="btnAbsent" runat="server" Text="Absent" CssClass="btn btn-danger" />
                            </div>
                        </div>

                        <!-- GridView for Attendance Report -->
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
