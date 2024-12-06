<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ExamSchedule.aspx.cs" Inherits="Schoolnest.Student.ExamSchedule" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
    <div class="row">
        <div class="col-md-12">
            <div class="card mb-0">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <div class="card-title">View Exam Schedule</div>
                </div>
                <div class="card-body">

                    <!-- Display student name and division -->
                    <div class="row mb-3">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="control-label">Standard:</label>
                                <asp:Label ID="lblStandard" runat="server" CssClass="form-control-static"></asp:Label>
                                <asp:Label ID="lblstandardID" runat="server" CssClass="form-control-static"></asp:Label>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="control-label">Division:</label>
                                <asp:Label ID="lblDivision" runat="server" CssClass="form-control-static"></asp:Label>
                                <asp:Label ID="lblDivisionID" runat="server" CssClass="form-control-static"></asp:Label>
                            </div>
                        </div>
                    </div>

                    <!-- Subject Filter Section -->
                    <div class="row mb-3">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="ddlSubjectFilter" class="control-label">Filter by Subject:</label>
                                <asp:DropDownList ID="ddlSubjectFilter" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlSubjectFilter_SelectedIndexChanged" AutoPostBack="True">
                                    <asp:ListItem Text="-- Select Subject --" Value="" />
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <!-- GridView for Exam Schedule -->
                    <div class="row">
                        <asp:GridView ID="gvExamSchedule" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered">
                            <Columns>
                                <asp:BoundField DataField="ExamDate" HeaderText="Exam Date" DataFormatString="{0:yyyy-MM-dd}" />
                                <asp:BoundField DataField="ExamName" HeaderText="Exam Name" />
                                <asp:BoundField DataField="Marks" HeaderText="Marks" />
                                <asp:BoundField DataField="SubjectName" HeaderText="Subject" />
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="alert alert-info text-center" role="alert">
                                    No exams are scheduled.
                                </div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>

</form>
</asp:Content>
