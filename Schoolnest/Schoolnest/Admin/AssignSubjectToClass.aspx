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
                    <!-- Search Dropdown for Assigned Subjects -->
                    <div class="form-group mb-0">
                        <asp:DropDownList ID="ddlSearchAssigned" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlSearchAssigned_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="ddlSubject" Text="Select Subject"></asp:Label>
                                <asp:DropDownList ID="ddlSubject" runat="server" CssClass="form-control">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvSubject" runat="server" ControlToValidate="ddlSubject" Display="Dynamic" ErrorMessage="Subject selection is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="ddlClass" Text="Select Class"></asp:Label>
                                <asp:DropDownList ID="ddlClass" runat="server" CssClass="form-control">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvClass" runat="server" ControlToValidate="ddlClass" Display="Dynamic" ErrorMessage="Class selection is required" CssClass="text-danger"></asp:RequiredFieldValidator>
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
