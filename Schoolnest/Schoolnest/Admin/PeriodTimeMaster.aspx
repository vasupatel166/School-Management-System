<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PeriodTimeMaster.aspx.cs" Inherits="Schoolnest.Admin.PeriodTimeMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <form id="periodForm" runat="server" class="w-100">
    <div class="row">
        <div class="col-md-12">
            <div class="card mb-0">
                <div class="card-header">
                    <div class="card-title">Period Details</div>
                </div>

                <div class="card-body">
                    <!-- Period Time -->
                    <div class="form-group row">
                        <div class="col-md-6">
                            <asp:Label runat="server" AssociatedControlID="ddlPeriodTime" Text="Period Time"></asp:Label>
                            <asp:DropDownList ID="ddlPeriodTime" runat="server" CssClass="form-control">
                                <asp:ListItem Text="Select Period Time" Value="" />
                                <asp:ListItem Text="08:00 AM - 09:00 AM" Value="08:00-09:00" />
                                <asp:ListItem Text="09:00 AM - 10:00 AM" Value="09:00-10:00" />
                                <asp:ListItem Text="10:00 AM - 11:00 AM" Value="10:00-11:00" />
                              
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvPeriodTime" runat="server" ControlToValidate="ddlPeriodTime" InitialValue="" Display="Dynamic" ErrorMessage="Please select a Period Time" CssClass="text-danger" />
                        </div>
                    </div>

                    <!-- Is Break Time -->
                    <div class="form-group row">
                        <div class="col-md-6">
                            <asp:CheckBox ID="chkIsBreakTime" runat="server" CssClass="form-check-input" />
                            <asp:Label runat="server" AssociatedControlID="chkIsBreakTime" Text="Is Break Time" CssClass="form-check-label ml-2"></asp:Label>
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
