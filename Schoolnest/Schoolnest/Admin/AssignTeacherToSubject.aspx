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
                    <div class="form-group mb-0">
                        <asp:DropDownList ID="ddlSearchAssignedTeacher" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlSearchAssignedTeacher_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="ddlTeacher" Text="Select Teacher"></asp:Label>
                                <asp:DropDownList ID="ddlTeacher" runat="server" CssClass="form-control">
                                    
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvTeacher" runat="server" ControlToValidate="ddlTeacher" Display="Dynamic" ErrorMessage="Teacher selection is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="ddlSubject" Text="Select Subject"></asp:Label>
                                <asp:DropDownList ID="ddlSubject" runat="server" CssClass="form-control">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvSubject" runat="server" ControlToValidate="ddlSubject" Display="Dynamic" ErrorMessage="Subject selection is required" CssClass="text-danger"></asp:RequiredFieldValidator>
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
