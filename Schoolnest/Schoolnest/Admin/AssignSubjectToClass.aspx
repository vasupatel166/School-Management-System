<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AssignSubjectToClass.aspx.cs" Inherits="Schoolnest.Admin.AssignSubjectToClass" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Assign Subject to Class</div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group mb-0">
                                    <asp:DropDownList ID="ddlSearchAssignedStandard" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlSearchAssignedStandard_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group mb-0">
                                    <asp:DropDownList ID="ddlSearchAssignedDivision" runat="server" CssClass="form-control" AutoPostBack="true" Enabled="false" OnSelectedIndexChanged="ddlSearchAssignedDivision_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlStandard" Text="Standard"></asp:Label>
                                    <asp:DropDownList ID="ddlStandard" runat="server" CssClass="form-control" AutoPostBack="true"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvStandard" runat="server" ControlToValidate="ddlStandard" InitialValue="" Display="Dynamic" ErrorMessage="Standard is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlDivision" Text="Division"></asp:Label>
                                    <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-control" AutoPostBack="true"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvDivision" runat="server" ControlToValidate="ddlDivision" InitialValue="" Display="Dynamic" ErrorMessage="Division is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>

                        <!-- Dynamic Rows Container -->
                        <asp:Panel ID="subjectRowsContainer" runat="server" CssClass="container mt-4 p-0 ms-0">
                            <asp:Repeater ID="rptSubjects" runat="server" OnItemCommand="rptSubjects_ItemCommand">
                                <ItemTemplate>
                                    <div class="row">
                                        <div class="col-md-4">
                                            <div class="form-group">
                                                <asp:DropDownList ID="ddlSubject" runat="server" CssClass="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-md-2">
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
                                        <asp:Button ID="btnAddRow" runat="server" Text="Add Subject" CssClass="btn btn-primary" OnClick="btnAddRow_Click" CausesValidation="false" />
                                    </div>
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