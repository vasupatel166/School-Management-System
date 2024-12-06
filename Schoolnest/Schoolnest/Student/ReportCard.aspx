<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReportCard.aspx.cs" Inherits="Schoolnest.Student.ReportCard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="attendanceForm" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header">
                        <div class="card-title">Download Report Card</div>
                    </div>
                    <div class="card-body">
                        <!-- Row 1: Filter By -->
                        <div class="form-group row">
                            <div class="col-md-6 offset-md-3">
                                <asp:Label runat="server" AssociatedControlID="ddlFilter" Text="Filter by" CssClass="form-label"></asp:Label>
                                <asp:DropDownList ID="ddlFilter" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddlFilter_SelectedIndexChanged">
                                    <asp:ListItem Value="S" Text="--Select--" Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="E" Text="Examwise"></asp:ListItem>
                                    <asp:ListItem Value="F" Text="Full Report Card"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="form-group row">
                            <div class="col-md-6 offset-md-3">
                                <asp:Label runat="server" AssociatedControlID="ddlExam" Text="Exam" ID="lblExam"></asp:Label>
                                <asp:DropDownList ID="ddlExam" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="card-footer text-center">
                            <!-- View Button -->
                            <asp:Button ID="btnView" runat="server" Text="View" CssClass="btn btn-primary" OnClick="btnView_Click" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-primary" OnClick="btnCancel_Click" />
                            <asp:Button ID="btnGenerateReport" runat="server" Text="Download Report Card" CssClass="btn btn-primary" OnClick="btnGenerateReport_Click" />
                        </div>

                    </div>

                    <div class="container mt-4">
                        <!-- Student Details Section (Initially hidden) -->
                        <div class="form-group row" id="studentDetailsSection" runat="server" style="display: none;">
                            <div class="form-group row">
                                <div class="col-md-4">
                                    <asp:Label ID="lblStudentName" runat="server" Text="Student Name" CssClass="form-label"></asp:Label>
                                </div>
                                <div class="col-md-8">
                                    <asp:Label ID="lblStudentNameValue" runat="server" CssClass="form-control" ReadOnly="True"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group row">
                                <div class="col-md-4">
                                    <asp:Label ID="lblStandard" runat="server" Text="Standard" CssClass="form-label"></asp:Label>
                                </div>
                                <div class="col-md-8">
                                    <asp:Label ID="lblStandardValue" runat="server" CssClass="form-control" ReadOnly="True"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group row">
                                <div class="col-md-4">
                                    <asp:Label ID="lblDivision" runat="server" Text="Division" CssClass="form-label"></asp:Label>
                                </div>
                                <div class="col-md-8">
                                    <asp:Label ID="lblDivisionValue" runat="server" CssClass="form-control" ReadOnly="True"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <!-- Grades and Results Section (Initially hidden) -->
                        <div class="form-group row" id="resultsSection" runat="server" style="display: none;">
                            <div class="col-md-12">
                                <asp:GridView ID="gvGrades" runat="server" CssClass="table table-bordered" AutoGenerateColumns="False" OnRowDataBound="gvGrades_RowDataBound">
                                    <columns>
                                        <asp:BoundField DataField="SubjectName" HeaderText="Subject Name" SortExpression="SubjectName" />
                                        <asp:BoundField DataField="MarksObtained" HeaderText="Marks Obtained" SortExpression="MarksObtained" />
                                        <asp:BoundField DataField="TotalMarks" HeaderText="Total Marks" SortExpression="TotalMarks" />
                                    </columns>
                                </asp:GridView>

                                <!-- Row 1: Total Marks and Percentage (All in one row) -->
                                <div class="row mt-2">
                                    <div class="col-md-4">
                                        <label for="lblTotal" class="form-label">Total</label>
                                    </div>
                                    <div class="col-md-4">
                                        <asp:Label ID="lblMarksObtainedSum" runat="server" CssClass="form-control" Text="0"></asp:Label>
                                    </div>
                                    <div class="col-md-4">
                                        <asp:Label ID="lblTotalMarksSum" runat="server" CssClass="form-control" Text="0"></asp:Label>
                                    </div>
                                </div>

                                <!-- Row 2: Percentage (on the same row as Total Marks) -->
                                <div class="row mt-2">
                                    <div class="col-md-4">
                                        <label for="lblPercentage" class="form-label">Percentage:</label>
                                    </div>
                                    <div class="col-md-4">
                                        <asp:Label ID="lblPercentage" runat="server" CssClass="form-control" Text="0%"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
