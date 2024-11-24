<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Announcements.aspx.cs" Inherits="Schoolnest.Admin.Announcements" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Manage Announcements</h4>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtTitle" Text="Announcement Title"></asp:Label>
                                    <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="txtTitle" 
                                        Display="Dynamic" ErrorMessage="Title is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlTargetAudience" Text="Target Audience"></asp:Label>
                                    <asp:DropDownList ID="ddlTargetAudience" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="Select Audience" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Teacher" Value="Teacher"></asp:ListItem>
                                        <asp:ListItem Text="Student" Value="Student"></asp:ListItem>
                                        <asp:ListItem Text="Both" Value="Both"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvTargetAudience" runat="server" 
                                        ControlToValidate="ddlTargetAudience" Display="Dynamic" 
                                        ErrorMessage="Target audience is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="row mt-3">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtDescription" Text="Description"></asp:Label>
                                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" 
                                        TextMode="MultiLine" Rows="4"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvDescription" runat="server" 
                                        ControlToValidate="txtDescription" Display="Dynamic" 
                                        ErrorMessage="Description is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="row mt-3">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:CheckBox ID="chkIsActive" runat="server" Text="Is Active" CssClass="form-check-input" />
                                </div>
                            </div>
                        </div>
                        <div class="card-footer text-center mt-4">
                            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" 
                                CausesValidation="false" OnClick="btnCancel_Click" />
                            <asp:HiddenField ID="hdnAnnouncementId" runat="server" Value="0" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row mt-4">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Announcement List</h4>
                    </div>
                    <div class="card-body">
                        <asp:GridView ID="gvAnnouncements" runat="server" AutoGenerateColumns="False" 
                            CssClass="table table-bordered table-striped" OnRowCommand="gvAnnouncements_RowCommand" 
                            DataKeyNames="AnnouncementID">
                            <Columns>
                                <asp:BoundField DataField="AnnouncementTitle" HeaderText="Title" />
                                <asp:BoundField DataField="AnnouncementDescription" HeaderText="Description" />
                                <asp:BoundField DataField="TargetAudience" HeaderText="Target Audience" />
                                <asp:CheckBoxField DataField="IsActive" HeaderText="Is Active" />
                                <asp:BoundField DataField="CreatedDateTime" HeaderText="Created Date" 
                                    DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                                <asp:TemplateField HeaderText="Actions">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEdit" runat="server" CssClass="btn btn-primary btn-sm" 
                                            CommandName="EditAnnouncement" CommandArgument='<%# Eval("AnnouncementID") %>' CausesValidation="false">
                                            Edit
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-danger btn-sm" 
                                            CommandName="DeleteAnnouncement" CommandArgument='<%# Eval("AnnouncementID") %>'
                                            OnClientClick="return confirm('Are you sure you want to delete this announcement?');" CausesValidation="false">
                                            Delete
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>