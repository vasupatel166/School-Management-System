<%@ Page Title="View Announcements" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewAnnouncements.aspx.cs" Inherits="Schoolnest.Teacher.ViewAnnouncements" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .announcement-item {
            cursor: pointer;
            padding: 10px;
            border-bottom: 1px solid #dee2e6;
            transition: background-color 0.2s ease;
        }
        .announcement-item:hover {
            background-color: #f8f9fa;
        }
        .announcement-item.selected {
            background-color: #e9ecef;
            border-left: 4px solid #0d6efd;
        }
    </style>
    <script type="text/javascript">
        function highlightAnnouncement(announcementId) {
            // Remove selected class from all announcements
            document.querySelectorAll('.announcement-item').forEach(item => {
                item.classList.remove('selected');
            });
            
            // Add selected class to clicked announcement
            const selectedAnnouncement = document.getElementById('announcement_' + announcementId);
            if (selectedAnnouncement) {
                selectedAnnouncement.classList.add('selected');
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-4">
                <div class="card mb-3">
                    <div class="card-header">
                        <h5 class="card-title">Announcements</h5>
                    </div>
                    <div class="card-body p-0">
                        <asp:Repeater ID="rptAnnouncements" runat="server" OnItemCommand="rptAnnouncements_ItemCommand">
                            <ItemTemplate>
                                <div class="announcement-item <%# IsSelectedAnnouncement(Eval("AnnouncementID"))? "selected" : "" %>" 
                                     id='announcement_<%# Eval("AnnouncementID") %>'>
                                    <asp:LinkButton ID="lnkAnnouncement" runat="server" 
                                        CssClass="d-block text-decoration-none text-dark"
                                        CommandName="ShowDetails" 
                                        CommandArgument='<%# Eval("AnnouncementID") %>'
                                        OnClientClick='<%# "highlightAnnouncement(" + Eval("AnnouncementID") + "); return true;" %>'>
                                        <h6 class="mb-1"><%# Eval("AnnouncementTitle") %></h6>
                                        <small class="text-muted"><%# Convert.ToDateTime(Eval("DisplayDateTime")).ToString("MMM dd, yyyy hh:mm tt") %></small>
                                    </asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
            <div class="col-md-8">
                <div class="card">
                    <div class="card-header">
                        <h5 class="card-title">Announcement Details</h5>
                    </div>
                    <div class="card-body">
                        <asp:Panel ID="pnlNoAnnouncement" runat="server" CssClass="text-center text-muted py-5">
                            <h6>Select an announcement to view details</h6>
                        </asp:Panel>
                        <asp:Panel ID="pnlAnnouncementDetails" runat="server" Visible="false">
                            <h4><asp:Label ID="lblTitle" runat="server" /></h4>
                            <div class="mb-3">
                                <small class="text-muted">
                                    Posted: <asp:Label ID="lblDateTime" runat="server" />
                                </small>
                            </div>
                            <div class="announcement-content">
                                <asp:Label ID="lblDescription" runat="server" />
                            </div>
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>