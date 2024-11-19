<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EnterGrades.aspx.cs" Inherits="Schoolnest.Teacher.EnterGrades" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Enter Grades</div>
                    </div>
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
                    <asp:Label runat="server" AssociatedControlID="ddlSubject" Text="Subject"></asp:Label>
                    <asp:DropDownList ID="ddlSubject" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
            </div>
            <div class="col-md-6">
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="ddlExam" Text="Exam"></asp:Label>
                    <asp:DropDownList ID="ddlExam" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-md-6">
                <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn btn-primary" CausesValidation="False" OnClick="btnFilter_Click" />
            </div>
        </div>
        <!-- GridView for displaying students and entering grades -->
        <asp:GridView ID="gvStudents" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered" OnRowDataBound="gvStudents_RowDataBound" EmptyDataText="No data found for the selected criteria.">
            <columns>
                <asp:BoundField DataField="Student_FullName" HeaderText="Student Name" />
                <asp:BoundField DataField="TotalMarks" HeaderText="Total Marks" />
                <asp:TemplateField HeaderText="Enter Grade">
                    <itemtemplate>
                        <asp:TextBox ID="txtGrade" runat="server" CssClass="form-control" />
                    </itemtemplate>
                </asp:TemplateField>
            </columns>
        </asp:GridView>


        <div class="card-footer">
            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
            <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-primary" CausesValidation="False" OnClick="btnReset_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-primary" CausesValidation="False" OnClick="btnCancel_Click" />
        </div>
    </form>

    <script type="text/javascript">
        function validateGrade(textbox) {
            // Find the row that contains the textbox
            var row = textbox.closest("tr");

            // Get the total marks from the second cell in the row (assuming Total Marks is in the second column)
            var totalMarks = parseInt(row.cells[1].innerText.trim());

            // Get the entered grade
            var enteredGrade = parseInt(textbox.value.trim());

            // Validate that entered grade does not exceed total marks
            if (enteredGrade > totalMarks) {
                alert("Entered grade cannot be greater than total marks (" + totalMarks + ").");
                textbox.value = ""; // Clear the invalid entry
                textbox.focus();
            }
        }
</script>
</asp:Content>
