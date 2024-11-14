<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AssignTeacherToSubject.aspx.cs" Inherits="Schoolnest.Admin.AssignTeacherToSubject" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdnjs.cloudflare.com/ajax/libs/sweetalert2/11.7.32/sweetalert2.all.min.js"></script>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/sweetalert2/11.7.32/sweetalert2.min.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Assign Teacher to Subject</div>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlStandard" Text="Standard"></asp:Label>
                                    <asp:DropDownList ID="ddlStandard" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlStandard_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvStandard" runat="server" ControlToValidate="ddlStandard" ValidationGroup="ValidateSubject" Display="Dynamic" ErrorMessage="Standard is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlDivision" Text="Division"></asp:Label>
                                    <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged" Enabled="false"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvDivision" runat="server" ControlToValidate="ddlDivision" ValidationGroup="ValidateSubject" Display="Dynamic" ErrorMessage="Division is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <asp:GridView ID="gvSubjects" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-striped mt-3">
                            <Columns>
                                <asp:BoundField DataField="SubjectName" HeaderText="Subject" ReadOnly="true" />
                                <asp:TemplateField HeaderText="Assign Teacher">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlTeacher" runat="server" CssClass="form-control" AutoPostBack="true">
                                        </asp:DropDownList>
                                        <asp:HiddenField ID="hdnSubjectDetailId" runat="server" Value='<%# Eval("SubjectDetailID") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <div class="card-footer text-center mt-4 pt-4">
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-success" OnClick="btnSubmit_Click" ValidationGroup="ValidateSubject" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" CausesValidation="false" OnClick="btnCancel_Click"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>