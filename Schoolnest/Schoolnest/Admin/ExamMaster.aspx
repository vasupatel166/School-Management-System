<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ExamMaster.aspx.cs" Inherits="Schoolnest.Admin.ExamMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="examForm" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-header">
                        <div class="card-title">Exam Management</div>
                    </div>
                    <div class="card-body">
                        <!-- Exam Form -->
                        <div class="row">

                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label AssociatedControlID="txtExamName" Text="Exam Name" runat="server" />
                                    <asp:TextBox ID="txtExamName" CssClass="form-control" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvExamName" ControlToValidate="txtExamName" runat="server" ErrorMessage="Exam Name is required." CssClass="text-danger" />
                                </div>
                            </div>

                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label AssociatedControlID="txtExamDescription" Text="Description" runat="server" />
                                    <asp:TextBox ID="txtExamDescription" TextMode="MultiLine" CssClass="form-control" runat="server" />
                                </div>
                            </div>

                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label AssociatedControlID="txtTotalMarks" Text="Total Marks" runat="server" />
                                    <asp:TextBox ID="txtTotalMarks" CssClass="form-control" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvTotalMarks" ControlToValidate="txtTotalMarks" runat="server" ErrorMessage="Total Marks are required." CssClass="text-danger" />
                                </div>
                            </div>
                        </div>

                        <div class="card-footer text-center my-4 pt-4">
                            <asp:Button ID="btnSave" Text="Save" CssClass="btn btn-success" OnClick="btnSave_Click" runat="server" />
                            <asp:Button ID="btnReset" Text="Reset" CssClass="btn btn-danger" OnClick="btnReset_Click" runat="server" CausesValidation="False" />
                        </div>

                        <!-- Exam Grid -->
                        <div class="row mt-4">
                            <div class="col-md-12">
                                <asp:GridView ID="gvExams" CssClass="table table-bordered" AutoGenerateColumns="False" runat="server"
                                    OnRowCommand="gvExams_RowCommand">
                                    <Columns>
                                        <asp:BoundField DataField="ExamName" HeaderText="Exam Name" />
                                        <asp:BoundField DataField="ExamDescription" HeaderText="Description" />
                                        <asp:BoundField DataField="TotalMarks" HeaderText="Total Marks" />
                                        <asp:TemplateField HeaderText="Actions">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkEdit" CommandName="EditExam" CssClass="btn btn-primary btn-sm" CommandArgument='<%# Eval("ExamID") %>' runat="server" CausesValidation="false">
                                                    Edit
                                                </asp:LinkButton>
                                                <asp:LinkButton ID="lnkDelete" CommandName="DeleteExam" CssClass="btn btn-danger btn-sm" CommandArgument='<%# Eval("ExamID") %>' runat="server" OnClientClick="return confirm('Are you sure want to delete?');" CausesValidation="false">
                                                    Delete
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
