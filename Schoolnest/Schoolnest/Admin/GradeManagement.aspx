<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GradeManagement.aspx.cs" Inherits="Schoolnest.Admin.GradeManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <form id="form1" runat="server" class="w-100">
            <div class="row">
                <div class="col-md-12">
                    <div class="card mb-0">
                        <div class="card-header d-flex justify-content-between align-items-center">
                            <div class="card-title">Grade Management</div>
                        </div>

                        <div class="card-body">
                            <asp:HiddenField ID="hfCriteriaID" runat="server" />
                            <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtGrade" Text="Grade"></asp:Label>
                                        <asp:TextBox ID="txtGrade" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtMinMarks" Text="Minimum Marks"></asp:Label>
                                        <asp:TextBox ID="txtMinMarks" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtMaxMarks" Text="Maximum Marks"></asp:Label>
                                        <asp:TextBox ID="txtMaxMarks" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Buttons -->
                        <div class="card-footer">
                            <asp:Button ID="btnAdd" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                            <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-primary" CausesValidation="False" OnClick="btnReset_Click" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-primary" CausesValidation="False" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>
            </div>

            <asp:GridView ID="gvGrades" runat="server" AutoGenerateColumns="False"
                DataKeyNames="GradesID" CssClass="table table-bordered"
                OnRowEditing="gvGrades_RowEditing" OnRowDeleting="gvGrades_RowDeleting">
                <columns>
                    <asp:BoundField DataField="GradesID" HeaderText="ID" ReadOnly="True" />
                    <asp:BoundField DataField="Grade" HeaderText="Grade" />
                    <asp:BoundField DataField="MinMarks" HeaderText="Minimum Marks" />
                    <asp:BoundField DataField="MaxMarks" HeaderText="Maximum Marks" />
                    <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" />
                </columns>
            </asp:GridView>
                     
    </form>
</asp:Content>
