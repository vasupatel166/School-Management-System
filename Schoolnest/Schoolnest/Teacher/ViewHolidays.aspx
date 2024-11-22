<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewHolidays.aspx.cs" Inherits="Schoolnest.Teacher.ViewHolidays" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">All Holidays</div>
                        <div class="form-group mb-0">
                            <asp:DropDownList ID="ddlAcademicYear" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddlAcademicYear_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-12">
                                <table class="table table-head-bg-primary">
                                    <thead class="table-dark">
                                        <tr>
                                            <th scope="col">#</th>
                                            <th scope="col">Holiday Name</th>
                                            <th scope="col">Date</th>
                                        </tr>
                                    </thead>
                                    <tbody id="HolidaysTableBody" runat="server">
                                        <!-- Holidays will be dynamically populated -->
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>


