<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EventMaster.aspx.cs" Inherits="Schoolnest.Admin.EventMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="eventForm" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header">
                        <div class="card-title">Create Event</div>
                    </div>

                    <div class="card-body">
                        <!-- Event Title -->
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtEventTitle" Text="Event Title"></asp:Label>
                            <asp:TextBox ID="txtEventTitle" runat="server" CssClass="form-control" placeholder="Enter Event Title"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvEventTitle" runat="server" ControlToValidate="txtEventTitle" Display="Dynamic" ErrorMessage="Event title is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                        </div>

                        <!-- Event Description -->
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtEventDescription" Text="Event Description"></asp:Label>
                            <asp:TextBox ID="txtEventDescription" runat="server" CssClass="form-control" TextMode="MultiLine" placeholder="Enter Event Description"></asp:TextBox>
                        </div>

                        <!-- Event Date -->
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtEventDate" Text="Event Date"></asp:Label>
                            <asp:TextBox ID="txtEventDate" runat="server" CssClass="form-control" TextMode="Date" placeholder="Select Event Date"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvEventDate" runat="server" ControlToValidate="txtEventDate" Display="Dynamic" ErrorMessage="Event date is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                        </div>

                        <!-- Event Time -->
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtEventTime" Text="Event Time"></asp:Label>
                            <asp:TextBox ID="txtEventTime" runat="server" CssClass="form-control" TextMode="Time"  placeholder="Select Event Time"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvEventTime" runat="server" ControlToValidate="txtEventTime" Display="Dynamic" ErrorMessage="Event time is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                        </div>

                        <!-- Academic Year (Read-Only) -->
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtAcademicYear" Text="Academic Year"></asp:Label>
                            <asp:TextBox ID="txtAcademicYear" runat="server" CssClass="form-control" ReadOnly="true" placeholder="2023-2024"></asp:TextBox>
                        </div>

                        <!-- Submit and Cancel Buttons -->
                        <div class="card-footer text-center mt-4 pt-4">
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-success"  />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger"  />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
