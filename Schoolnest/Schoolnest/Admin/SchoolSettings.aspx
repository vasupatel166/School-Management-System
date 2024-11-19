<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SchoolSettings.aspx.cs" Inherits="Schoolnest.Admin.SchoolSettings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <form id="classScheduleForm" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header">
                        <div class="card-title">School Settings</div>
                    </div>
                    <div class="card-body">
                        <!-- Academic Year -->
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtAcademicYear" Text="Academic Year"></asp:Label>
                                    <asp:TextBox ID="txtAcademicYear" runat="server" CssClass="form-control" />
                                    <asp:RequiredFieldValidator ID="rfvAcademicYear" runat="server" ControlToValidate="txtAcademicYear" Display="Dynamic" ErrorMessage="Please enter the Academic Year" CssClass="text-danger" />
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="tpClassStartTime" Text="Class Start Time"></asp:Label>
                                    <asp:TextBox ID="tpClassStartTime" runat="server" CssClass="form-control" TextMode="Time" />
                                    <asp:RequiredFieldValidator ID="rfvClassStartTime" runat="server" ControlToValidate="tpClassStartTime" Display="Dynamic" ErrorMessage="Please select the Class Start Time" CssClass="text-danger" />
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="tpClassEndTime" Text="Class End Time"></asp:Label>
                                    <asp:TextBox ID="tpClassEndTime" runat="server" CssClass="form-control" TextMode="Time" />
                                    <asp:RequiredFieldValidator ID="rfvClassEndTime" runat="server" ControlToValidate="tpClassEndTime" Display="Dynamic" ErrorMessage="Please select the Class End Time" CssClass="text-danger" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="tpPeriodDuration" Text="Period Duration (minutes)"></asp:Label>
                                    <asp:TextBox ID="tpPeriodDuration" runat="server" CssClass="form-control" TextMode="Number" />
                                    <asp:RequiredFieldValidator ID="rfvPeriodDuration" runat="server" ControlToValidate="tpPeriodDuration" Display="Dynamic" ErrorMessage="Please enter the Period Duration" CssClass="text-danger" />
                                    <asp:RangeValidator ID="rvPeriodDuration" runat="server" ControlToValidate="tpPeriodDuration" Type="Integer" MinimumValue="30" MaximumValue="100" Display="Dynamic" ErrorMessage="Period Duration must be equal or more than 30" CssClass="text-danger" />
                                </div>
                            </div>

                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="tpBreakStartTime" Text="Break Start Time"></asp:Label>
                                    <asp:TextBox ID="tpBreakStartTime" runat="server" CssClass="form-control" TextMode="Time"/>
                                    <asp:RequiredFieldValidator ID="rfvBreakStartTime" runat="server" ControlToValidate="tpBreakStartTime" Display="Dynamic" ErrorMessage="Please select the Break Start Time" CssClass="text-danger" />
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="tpBreakDuration" Text="Break Duration (minutes)"></asp:Label>
                                    <asp:TextBox ID="tpBreakDuration" runat="server" CssClass="form-control" TextMode="Number" />
                                    <asp:RequiredFieldValidator ID="rfvBreakDuration" runat="server" ControlToValidate="tpBreakDuration" Display="Dynamic" ErrorMessage="Please enter the Break Duration" CssClass="text-danger" />
                                    <asp:RangeValidator ID="rvBreakDuration" runat="server" ControlToValidate="tpBreakDuration" Type="Integer" MinimumValue="1" MaximumValue="100" Display="Dynamic" ErrorMessage="Break duration must be more than 0" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtFirstTermFeeDueDate" Text="First Term Fee Due Date"></asp:Label>
                                    <asp:TextBox ID="txtFirstTermFeeDueDate" runat="server" CssClass="form-control" TextMode="Date" />
                                    <asp:RequiredFieldValidator ID="rfvFirstTermFeeDueDate" runat="server"
                                        ControlToValidate="txtFirstTermFeeDueDate" Display="Dynamic"
                                        ErrorMessage="Please select the First Term Fee Due Date" CssClass="text-danger" />
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtSecondTermFeeDueDate" Text="Second Term Fee Due Date"></asp:Label>
                                    <asp:TextBox ID="txtSecondTermFeeDueDate" runat="server" CssClass="form-control" TextMode="Date" />
                                    <asp:RequiredFieldValidator ID="rfvSecondTermFeeDueDate" runat="server"
                                        ControlToValidate="txtSecondTermFeeDueDate" Display="Dynamic"
                                        ErrorMessage="Please select the Second Term Fee Due Date" CssClass="text-danger" />
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtLateFeeCharges" Text="Late Fee Charges"></asp:Label>
                                    <asp:TextBox ID="txtLateFeeCharges" runat="server" CssClass="form-control" TextMode="Number" />
                                    <asp:RequiredFieldValidator ID="rfvLateFeeCharges" runat="server"
                                        ControlToValidate="txtLateFeeCharges"
                                        Display="Dynamic"
                                        ErrorMessage="Please enter the Late Fee Charges"
                                        CssClass="text-danger" />
                                    <asp:RegularExpressionValidator ID="revLateFeeCharges" runat="server"
                                        ControlToValidate="txtLateFeeCharges"
                                        Display="Dynamic"
                                        ErrorMessage="Please enter a valid whole number"
                                        ValidationExpression="^\d+$"
                                        CssClass="text-danger" />
                                </div>
                            </div>
                        </div>
                        <div class="card-footer text-center mt-4 pt-4">
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-success" OnClick="btnSubmit_Click" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" CausesValidation="false" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>

</asp:Content>
