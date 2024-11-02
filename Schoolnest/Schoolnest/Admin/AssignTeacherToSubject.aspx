<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AssignTeacherToSubject.aspx.cs" Inherits="Schoolnest.Admin.AssignTeacherToSubject" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
    <div class="row">
        <div class="col-md-12">
            <div class="card mb-0">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <div class="card-title">Assign Teacher to Subject</div>
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group mb-0">
                                <asp:DropDownList ID="ddlSearchAssignedStandard" runat="server" CssClass="form-control" AutoPostBack="true">

                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group mb-0">
                                <asp:DropDownList ID="ddlSearchAssignedDivision" runat="server" CssClass="form-control" AutoPostBack="true" Enabled="false">

                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="ddlStandard" Text="Select Standard"></asp:Label>
                                <asp:DropDownList ID="ddlStandard" runat="server" CssClass="form-control" AutoPostBack="true">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvStandard" runat="server" ControlToValidate="ddlStandard" ValidationGroup="AddTeacher" Display="Dynamic" ErrorMessage="Standard selection is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="ddlDivision" Text="Select Division"></asp:Label>
                                <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-control" AutoPostBack="true">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvDivision" runat="server" ControlToValidate="ddlDivision" ValidationGroup="AddTeacher" Display="Dynamic" ErrorMessage="Division selection is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                    </div>

                    <!-- Dynamic Rows Container -->
                    <asp:Panel ID="teacherRowsContainer" runat="server" CssClass="container mt-4 p-0 ms-0">
                        <asp:Repeater ID="rptSubjectsTeacher" runat="server" OnItemCommand="rptSubjectsTeacher_ItemCommand" OnItemDataBound="rptSubjectsTeacher_ItemDataBound">
                            <ItemTemplate>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <asp:DropDownList ID="ddlSubject" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <asp:DropDownList ID="ddlTeacher" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <asp:Button ID="btnRemove" runat="server" Text="Remove" CssClass="btn btn-danger" CausesValidation="false" CommandName="Remove" CommandArgument='<%# Container.ItemIndex %>' />
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </asp:Panel>

                    <div class="row">
                        <div class="col-md-2">
                            <div class="form-group">
                                <div class="text-left mt-3">
                                    <asp:Button ID="btnAddRow" runat="server" Text="Add Teacher" CssClass="btn btn-primary" ValidationGroup="AddTeacher" CausesValidation="true" OnClick="btnAddRow_Click" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="card-footer text-center mt-4 pt-4">
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-success" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" CausesValidation="false"/>
                    </div>
                </div>
            </div>
        </div>
    </div>
</form>
</asp:Content>
