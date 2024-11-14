<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Announcements.aspx.cs" Inherits="Schoolnest.Admin.Announcements" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <form id="form1" runat="server" class="w-100">
     <div class="row">
         <!-- Recent Announcements Section -->
         <div class="col-md-6">
             <div class="card">
                 <div class="card-header">
                     <h4 class="card-title">Recent Announcements</h4>
                 </div>
                 <div class="card-body">
                     <!-- Display if there are no announcements -->
                     <asp:Panel ID="pnlNoAnnouncements" runat="server" Visible="false">
                         <p class="text-muted">No recent announcements</p>
                     </asp:Panel>
                     
                     <ul class="list-group">
                         <asp:Repeater ID="rptRecentAnnouncements" runat="server">
                             <ItemTemplate>
                                 <li class="list-group-item">
                                     <%# Eval("AnnouncementText") %> - 
                                     <small class="text-muted"><%# Eval("AnnouncementDate", "{0:dd MMM yyyy}") %></small>
                                 </li>
                             </ItemTemplate>
                         </asp:Repeater>
                     </ul>
                 </div>
             </div>
         </div>

         <!-- Main Announcement Section -->
         <div class="col-md-6">
             <div class="card">
                 <div class="card-header">
                     <h4 class="card-title">Publish New Announcement</h4>
                 </div>
                 <div class="card-body">
                     <div class="form-group">
                         <asp:Label runat="server" AssociatedControlID="txtAnnouncement" Text="Announcement Text"></asp:Label>
                         <asp:TextBox ID="txtAnnouncement" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5"></asp:TextBox>
                         <asp:RequiredFieldValidator ID="rfvAnnouncement" runat="server" ControlToValidate="txtAnnouncement" Display="Dynamic" ErrorMessage="Announcement text is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                     </div>
                 </div>
                 <div class="card-footer text-center">
                     <asp:Button ID="btnPublish" runat="server" Text="Publish" CssClass="btn btn-success" OnClick="btnPublish_Click" />
                 </div>
             </div>
         </div>
     </div>
 </form>
</asp:Content>
