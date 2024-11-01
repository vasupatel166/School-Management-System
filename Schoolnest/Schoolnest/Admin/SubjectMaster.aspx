<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SubjectMaster.aspx.cs" Inherits="Schoolnest.Admin.SubjectMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <form id="form1" runat="server" class="w-100">
     <div class="row">
         <div class="col-md-12">
             <div class="card mb-0">
                 <div class="card-header d-flex justify-content-between align-items-center">
                     <div class="card-title">Subject Master</div>
                     <!-- Search Dropdown for Subjects -->
                     <div class="form-group mb-0">
                         <asp:DropDownList ID="ddlSearchSubject" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlSearchSubject_SelectedIndexChanged">
                         </asp:DropDownList>
                     </div>
                 </div>

                 <div class="card-body">
                     <!-- Subject Name -->
                     <div class="row">
                         <div class="col-md-6">
                             <div class="form-group">
                                 <asp:Label runat="server" AssociatedControlID="txtSubjectName" Text="Subject Name"></asp:Label>
                                 <asp:TextBox ID="txtSubjectName" runat="server" CssClass="form-control" placeholder="Enter Subject Name"></asp:TextBox>
                                 <asp:RequiredFieldValidator ID="rfvSubjectName" runat="server" ControlToValidate="txtSubjectName" Display="Dynamic" ErrorMessage="Subject Name is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                             </div>
                         </div>
                         <div class="col-md-6">
                             <div class="form-group">
                                 <asp:Label runat="server" AssociatedControlID="txtSubjectCode" Text="Subject Code"></asp:Label>
                                 <asp:TextBox ID="txtSubjectCode" runat="server" CssClass="form-control" placeholder="Enter Subject Code"></asp:TextBox>
                                 <asp:RequiredFieldValidator ID="rfvSubjectCode" runat="server" ControlToValidate="txtSubjectCode" Display="Dynamic" ErrorMessage="Subject Code is required" CssClass="text-danger"></asp:RequiredFieldValidator>
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
