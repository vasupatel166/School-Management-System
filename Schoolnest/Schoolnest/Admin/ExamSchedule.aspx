<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ExamSchedule.aspx.cs" Inherits="Schoolnest.Admin.ExamSchedule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Exam Schedule</div>
                    </div>

                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtExamScheduleID" Text="Exam Schedule ID"></asp:Label>
                                    <asp:TextBox ID="txtExamScheduleID" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlSubject" Text="Subject Name"></asp:Label>
                                    <asp:DropDownList ID="ddlSubject" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlStandard" Text="Standard"></asp:Label>
                                    <asp:DropDownList ID="ddlStandard" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlDivision" Text="Division"></asp:Label>
                                    <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlSection" Text="Section"></asp:Label>
                                    <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlExam" Text="Exam"></asp:Label>
                                    <asp:DropDownList ID="ddlExam" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 form-group">
                            <asp:Label runat="server" AssociatedControlID="txtDateOfExam" Text="Date of Exam"></asp:Label>
                            <asp:TextBox ID="txtDateOfExam" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>

                    </div>

                    <!-- Buttons -->
                    <div class="card-footer text-center my-4 pt-4">
                        <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-success" OnClick="btnSave_Click" />
                        <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-danger" CausesValidation="False" OnClick="btnReset_Click" />
                    </div>
                </div>
            </div>
        </div>

        <div class="row mt-4">
            <asp:GridView ID="gvExamSchedule" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
                DataKeyNames="ExamScheduleID"
                OnRowCommand="gvExamSchedule_RowCommand"
                OnRowDataBound="gvExamSchedule_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="ExamScheduleID" HeaderText="SR No" />
                    <asp:BoundField DataField="ExamName" HeaderText="Exam Name" />
                    <asp:BoundField DataField="ExamDate" HeaderText="Exam Date" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:BoundField DataField="Subject" HeaderText="Subject" />
                    <asp:BoundField DataField="Standard" HeaderText="Standard" />
                    <asp:BoundField DataField="Division" HeaderText="Division" />
                    <asp:BoundField DataField="Section" HeaderText="Section" />
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkEdit" runat="server" CssClass="btn btn-primary btn-sm"
                                CommandName="EditExamSchedule" CommandArgument='<%# Eval("ExamScheduleID") %>' CausesValidation="false">
                                <i class="fas fa-edit"></i> Edit
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-danger btn-sm"
                                CommandName="DeleteExamSchedule" CommandArgument='<%# Eval("ExamScheduleID") %>'
                                OnClientClick="return confirm('Are you sure you want to delete this exam schedule?');" CausesValidation="false">
                                <i class="fas fa-trash"></i> Delete
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div style="text-align: center; padding: 10px;">
                        No records found.
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>

    </form>
</asp:Content>
