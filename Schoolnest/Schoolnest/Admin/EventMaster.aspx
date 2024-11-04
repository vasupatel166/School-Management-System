<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EventMaster.aspx.cs" Inherits="Schoolnest.Admin.EventMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .badge-success {
            background-color: #28a745;
            color: white;
            padding: 5px 10px;
            border-radius: 5px;
        }

        .badge-danger {
            background-color: #dc3545;
            color: white;
            padding: 5px 10px;
            border-radius: 5px;
        }

    </style>
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
                        <div class="row">
                            <div class="col-md-6">
                                 <div class="form-group">
                                     <asp:Label runat="server" AssociatedControlID="txtEventTitle" Text="Event Title"></asp:Label>
                                     <asp:TextBox ID="txtEventTitle" runat="server" CssClass="form-control" placeholder="Enter Event Title"></asp:TextBox>
                                     <asp:RequiredFieldValidator ID="rfvEventTitle" runat="server" ControlToValidate="txtEventTitle" Display="Dynamic" ErrorMessage="Event title is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                 </div>
                            </div>
                            <div class="col-md-6">
                                 <!-- Event Description -->
                                 <div class="form-group">
                                     <asp:Label runat="server" AssociatedControlID="txtEventDescription" Text="Event Description (optional)"></asp:Label>
                                     <asp:TextBox ID="txtEventDescription" runat="server" CssClass="form-control" TextMode="MultiLine" placeholder="Enter Event Description"></asp:TextBox>
                                 </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-4">
                                <!-- Event Date -->
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtEventDate" Text="Event Date"></asp:Label>
                                    <asp:TextBox ID="txtEventDate" runat="server" CssClass="form-control" TextMode="Date" placeholder="Select Event Date"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvEventDate" runat="server" ControlToValidate="txtEventDate" Display="Dynamic" ErrorMessage="Event date is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <!-- Event Time -->
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtEventTime" Text="Event Time"></asp:Label>
                                    <asp:TextBox ID="txtEventTime" runat="server" CssClass="form-control" TextMode="Time"  placeholder="Select Event Time"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvEventTime" runat="server" ControlToValidate="txtEventTime" Display="Dynamic" ErrorMessage="Event time is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <!-- Academic Year (Read-Only) -->
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtAcademicYear" Text="Academic Year"></asp:Label>
                                    <asp:TextBox ID="txtAcademicYear" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                         </div>
                        <div class="row">
                            <div class="col-md-4">
                                 <div class="form-check mt-2">
                                     <asp:CheckBox
                                             ID="chkIsActive"
                                             runat="server"
                                             CssClass="form-check-input border-0"
                                             Checked="true"
                                         />
                                     <label class="form-check-label" for="flexCheckDefault">
                                     Active
                                     </label>
                                 </div>
                             </div>
                        </div>
                       
                        <!-- Submit and Cancel Buttons -->
                        <div class="card-footer text-center mt-4 pt-4">
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-success" OnClick="btnSubmit_Click" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Events Table -->
        <div class="row mt-5">
            <div class="col-md-12">
                <div class="card">
                  <div class="card-header">
                    <h4 class="card-title">All Events</h4>
                  </div>
                  <div class="card-body">
                    <div class="table-responsive">
                        <asp:GridView ID="gvEvents" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-hover" OnRowCommand="gvEvents_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="EventTitle" HeaderText="Title" />
                                <asp:BoundField DataField="EventDescription" HeaderText="Description" />
                                <asp:BoundField DataField="EventDate" HeaderText="Date" DataFormatString="{0:dd MMM, yyyy}" />
        
                               <asp:TemplateField HeaderText="Time">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEventTime" runat="server" Text='<%# Eval("FormattedEventTime") %>'></asp:Label>
                                    </ItemTemplate>
                               </asp:TemplateField>

                                <asp:BoundField DataField="AcademicYear" HeaderText="Academic Year" />
        
                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate>
                                        <asp:Label ID="lblStatus" runat="server" Text='<%# (bool)Eval("IsActive") ? "Active" : "Inactive" %>'
                                            CssClass='<%# (bool)Eval("IsActive") ? "badge badge-success" : "badge badge-danger" %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>
                                        <asp:Button ID="btnEdit" runat="server" CommandName="EditEvent" CommandArgument='<%# Eval("EventID") %>' CausesValidation="false" Text="Edit" CssClass="btn btn-primary btn-sm" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView> 
                    </div>
                  </div>
                </div>
              </div>
        </div>
    </form>
</asp:Content>

