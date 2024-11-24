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

                    <div class="row">
                        <div class="col-md-4">
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="txtHolidayName" Text="Holiday Name"></asp:Label>
                                <asp:TextBox ID="txtHolidayName" runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="rfvHolidayName" runat="server" ControlToValidate="txtHolidayName" Display="Dynamic" ErrorMessage="Please enter the Holiday Name" CssClass="text-danger" />
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="dpHolidayDate" Text="Holiday Date"></asp:Label>
                                <asp:TextBox ID="dpHolidayDate" runat="server" CssClass="form-control" TextMode="Date" />
                                <asp:RequiredFieldValidator ID="rfvHolidayDate" runat="server" ControlToValidate="dpHolidayDate" Display="Dynamic" ErrorMessage="Please select the Holiday Date" CssClass="text-danger" />
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="txtAcademicYear" Text="Academic Year"></asp:Label>
                                <asp:TextBox ID="txtAcademicYear" runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="rfvAcademicYear" runat="server" ControlToValidate="txtAcademicYear" Display="Dynamic" ErrorMessage="Please enter the Academic Year" CssClass="text-danger" />
                            </div>
                        </div>
                    </div>

                    <!-- Buttons -->
                    <div class="card-footer text-center mt-4 pt-4">
                        <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-success" OnClick="btnSave_Click" />
                        <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-danger" CausesValidation="False" OnClick="btnReset_Click" />
                    </div>

                    <div class="row mt-4">
                        <div class="col-md-12">
                            <asp:GridView ID="gvHolidays" runat="server" AutoGenerateColumns="False" 
                                CssClass="table table-striped table-bordered" OnRowCommand="gvHolidays_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="HolidayID" HeaderText="ID" />
                                    <asp:BoundField DataField="HolidayName" HeaderText="Holiday Name" />
                                    <asp:BoundField DataField="HolidayDate" HeaderText="Date" DataFormatString="{0:dd-MM-yyyy}" />
                                    <asp:BoundField DataField="AcademicYear" HeaderText="Academic Year" />
                                    <asp:TemplateField HeaderText="Actions">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkEdit" runat="server" CssClass="btn btn-primary btn-sm"
                                                CommandName="EditHoliday" CommandArgument='<%# Eval("HolidayID") %>' CausesValidation="false">
                                                <i class="fas fa-edit"></i> Edit
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-danger btn-sm"
                                                CommandName="DeleteHoliday" CommandArgument='<%# Eval("HolidayID") %>'
                                                OnClientClick="return confirm('Are you sure you want to delete this holiday?');" CausesValidation="false">
                                                <i class="fas fa-trash"></i> Delete
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
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
