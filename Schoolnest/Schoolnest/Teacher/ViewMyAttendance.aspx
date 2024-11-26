<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewMyAttendance.aspx.cs" Inherits="Schoolnest.Teacher.ViewMyAttendance" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">My Attendance</div>
                    </div>
                    <div class="card-body">
                        <div class="row mb-3">
                            <div class="col-md-3">
                                <label>Select Duration:</label>
                                <asp:DropDownList ID="ddlDuration" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlDuration_SelectedIndexChanged">
                                    <asp:ListItem Text="Weekly" Value="weekly" />
                                    <asp:ListItem Text="Monthly" Value="monthly" />
                                    <asp:ListItem Text="Yearly" Value="yearly" />
                                    <asp:ListItem Text="Custom Date" Value="custom" />
                                </asp:DropDownList>
                            </div>
                            
                            <div class="col-md-3" id="customDateDiv" runat="server" visible="false">
                                <label>From Date:</label>
                                <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                            
                            <div class="col-md-3" id="customDateToDiv" runat="server" visible="false">
                                <label>To Date:</label>
                                <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                            
                            <div class="col-md-3 align-self-end" id="searchDiv" runat="server" visible="false">
                                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                            </div>
                        </div>
                        
                        <div class="row">
                            <div class="col-md-12">
                                <asp:GridView ID="gvAttendance" runat="server" CssClass="table table-striped table-bordered"
                                    AutoGenerateColumns="false" AllowPaging="true" PageSize="10" 
                                    OnPageIndexChanging="gvAttendance_PageIndexChanging">
                                    <Columns>
                                        <asp:BoundField DataField="SrNo" HeaderText="Sr. No." />
                                        <asp:BoundField DataField="TeacherName" HeaderText="Teacher Name" />
                                        <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:dd/MM/yyyy}" />
                                        <asp:BoundField DataField="Status" HeaderText="Status" />
                                    </Columns>
                                    <PagerStyle CssClass="pagination-ys" />
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>