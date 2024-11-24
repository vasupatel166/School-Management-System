<%@ Page Title="Send Notifications" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SendNotifications.aspx.cs" Inherits="Schoolnest.Admin.SendEmailNotifications" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <asp:ScriptManager ID="ScriptManager1" runat="server"/>
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Send Notifications</div>
                    </div>

                    <div class="card-body">
                        <div class="row mb-3">
                            <div class="col-md-4">
                                <asp:Label runat="server" AssociatedControlID="ddlUserType" Text="Select User Type"></asp:Label>
                                <asp:DropDownList ID="ddlUserType" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlUserType_SelectedIndexChanged">
                                    <asp:ListItem Text="Teacher" Value="Teacher" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Student" Value="Student"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <asp:Panel ID="pnlStudentFilters" runat="server" Visible="false">
                            <div class="row mb-3">
                                <div class="col-md-6">
                                    <asp:Label runat="server" AssociatedControlID="ddlStandard" Text="Select Standard"></asp:Label>
                                    <asp:DropDownList ID="ddlStandard" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlStandard_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvStandard" runat="server" ControlToValidate="ddlStandard" 
                                        ErrorMessage="Standard is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="StudentGroup"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-6">
                                    <asp:Label runat="server" AssociatedControlID="ddlDivision" Text="Select Division"></asp:Label>
                                    <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-control" AutoPostBack="true" 
                                        Enabled="false" OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvDivision" runat="server" ControlToValidate="ddlDivision" 
                                        ErrorMessage="Division is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="StudentGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </asp:Panel>

                        <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered mt-4" DataKeyNames="Email">
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkSelectAll" runat="server" OnCheckedChanged="chkSelectAll_CheckedChanged" AutoPostBack="true" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelect" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Name" HeaderText="Name" />
                                <asp:BoundField DataField="Email" HeaderText="Email" />
                                <asp:TemplateField HeaderText="Profile Image">
                                    <ItemTemplate>
                                        <div class="avatar-lg">
                                            <asp:Image ID="imgProfile" CssClass="avatar-img rounded w-100" ImageUrl='<%# Eval("ProfileImage") %>' 
                                                runat="server" AlternateText="User Image" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>

                        <div class="card-footer text-center mt-4 pt-4">
                            <asp:Button ID="btnSendNotification" runat="server" Text="Send Notification" CssClass="btn btn-primary" 
                                OnClick="btnSendNotification_Click" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" 
                                CausesValidation="false" OnClick="btnCancel_Click"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>