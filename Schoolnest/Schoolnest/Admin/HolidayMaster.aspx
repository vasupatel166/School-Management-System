<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="HolidayMaster.aspx.cs" Inherits="Schoolnest.Admin.HolidayMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <form id="holidayScheduleForm" runat="server" class="w-100">
    <div class="row">
        <div class="col-md-12">
            <div class="card mb-0">
                <div class="card-header">
                    <div class="card-title">Holiday Schedule</div>
                </div>

                <div class="card-body">
                    <!-- Holiday Name -->
                    <div class="form-group row">
                        <div class="col-md-6">
                            <asp:Label runat="server" AssociatedControlID="txtHolidayName" Text="Holiday Name"></asp:Label>
                            <asp:TextBox ID="txtHolidayName" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvHolidayName" runat="server" ControlToValidate="txtHolidayName" Display="Dynamic" ErrorMessage="Please enter the Holiday Name" CssClass="text-danger" />
                        </div>
                    </div>

                    <!-- Holiday Date -->
                    <div class="form-group row">
                        <div class="col-md-6">
                            <asp:Label runat="server" AssociatedControlID="dpHolidayDate" Text="Holiday Date"></asp:Label>
                            <asp:TextBox ID="dpHolidayDate" runat="server" CssClass="form-control" TextMode="Date" />
                            <asp:RequiredFieldValidator ID="rfvHolidayDate" runat="server" ControlToValidate="dpHolidayDate" Display="Dynamic" ErrorMessage="Please select the Holiday Date" CssClass="text-danger" />
                        </div>
                    </div>

                    <!-- Academic Year -->
                    <div class="form-group row">
                        <div class="col-md-6">
                            <asp:Label runat="server" AssociatedControlID="txtAcademicYear" Text="Academic Year"></asp:Label>
                            <asp:TextBox ID="txtAcademicYear" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvAcademicYear" runat="server" ControlToValidate="txtAcademicYear" Display="Dynamic" ErrorMessage="Please enter the Academic Year" CssClass="text-danger" />
                        </div>
                    </div>

                    <!-- Buttons -->
                    <div class="form-group row mt-4">
                        <div class="col-md-6">
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" CausesValidation="false" OnClientClick="window.history.back();" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</form>

</asp:Content>
