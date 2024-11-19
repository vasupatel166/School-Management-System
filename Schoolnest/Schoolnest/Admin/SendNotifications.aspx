<%@ Page Title="Send Notifications" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SendNotifications.aspx.cs" Inherits="Schoolnest.Admin.SendEmailNotifications" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <!-- Send Notification Form -->
            <div class="col-md-6">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Send Notification</h4>
                    </div>
                    <div class="card-body">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtSubject" Text="Notification Subject"></asp:Label>
                            <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvSubject" runat="server" ControlToValidate="txtSubject" Display="Dynamic" ErrorMessage="Subject is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtMessage" Text="Notification Message"></asp:Label>
                            <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvMessage" runat="server" ControlToValidate="txtMessage" Display="Dynamic" ErrorMessage="Message is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="ddlRecipientGroup" Text="Recipient Group"></asp:Label>
                            <asp:DropDownList ID="ddlRecipientGroup" runat="server" CssClass="form-control">
                                <asp:ListItem Value="" Text="Select Group" />
                                <asp:ListItem Value="Teachers" Text="Teachers" />
                                <asp:ListItem Value="Students" Text="Students" />
                                <asp:ListItem Value="Parents" Text="Parents" />
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvRecipientGroup" runat="server" ControlToValidate="ddlRecipientGroup" Display="Dynamic" InitialValue="" ErrorMessage="Please select a recipient group" CssClass="text-danger"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="card-footer text-center">
                        <asp:Button ID="btnSendNotification" runat="server" Text="Send Notification" CssClass="btn btn-success" OnClick="btnSendNotification_Click" />
                    </div>
                </div>
            </div>

            <!-- Notifications History Section -->
            <div class="col-md-6">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Sent Notifications</h4>
                    </div>
                    <div class="card-body">
                        <ul class="list-group">
                            <asp:Repeater ID="rptNotificationsHistory" runat="server">
                                <ItemTemplate>
                                    <li class="list-group-item">
                                        <strong><%# Eval("Subject") %></strong> - 
                                        <%# Eval("DateSent", "{0:dd MMM yyyy HH:mm}") %> 
                                        <br />
                                        <small class="text-muted"><%# Eval("Message") %></small>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                        <asp:Panel ID="pnlNoNotifications" runat="server" Visible="false">
                            <p class="text-muted">No notifications have been sent yet.</p>
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
