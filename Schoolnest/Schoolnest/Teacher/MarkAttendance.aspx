<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MarkAttendance.aspx.cs" Inherits="Schoolnest.Teacher.MarkAttendance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            const dateInput = document.getElementById('<%= txtDate.ClientID %>');
            const today = new Date();
            const maxDate = today.toISOString().split('T')[0];

            // Set max date to today
            dateInput.max = maxDate;

            // Set min date to 7 days ago
            const minDate = new Date(today);
            minDate.setDate(minDate.getDate() - 7);
            dateInput.min = minDate.toISOString().split('T')[0];

            // Only set default date if no date is selected
            if (!dateInput.value) {
                dateInput.value = maxDate;
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="attendanceForm" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header">
                        <div class="card-title">Mark Student Attendance</div>
                    </div>
                    <div class="card-body">
                        <div class="form-group row">
                            <div class="col-md-3">
                                <asp:Label runat="server" AssociatedControlID="lblTeacher" Text="Class Teacher"></asp:Label>
                                <asp:Label ID="lblTeacher" runat="server" CssClass="form-control"></asp:Label>
                            </div>
                            <div class="col-md-3">
                                <asp:Label runat="server" AssociatedControlID="lblStd" Text="Std"></asp:Label>
                                <asp:Label ID="lblStd" runat="server" CssClass="form-control"></asp:Label>
                            </div>
                            <div class="col-md-3">
                                <asp:Label runat="server" AssociatedControlID="lblDivision" Text="Division"></asp:Label>
                                <asp:Label ID="lblDivision" runat="server" CssClass="form-control"></asp:Label>
                            </div>
                            <div class="col-md-3">
                                <asp:Label runat="server" AssociatedControlID="txtDate" Text="Date"></asp:Label>
                                <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date" AutoPostBack="true" OnTextChanged="txtDate_TextChanged"></asp:TextBox>
                                <asp:HiddenField ID="hfSelectedDate" runat="server" />
                                <asp:RequiredFieldValidator ID="rfvDate" runat="server" ControlToValidate="txtDate" Display="Dynamic" ErrorMessage="Please select a date" CssClass="text-danger" />
                            </div>
                        </div>
                        <asp:HiddenField ID="hfStandardID" runat="server" />
                        <asp:HiddenField ID="hfDivisionID" runat="server" />
                        <asp:GridView ID="gvStudents" runat="server" AutoGenerateColumns="false"
                            CssClass="table table-bordered" OnRowDataBound="gvStudents_RowDataBound">
                            <Columns>
                                <asp:TemplateField HeaderText="Student Name">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hfStudentID" runat="server" Value='<%# Eval("StudentID") %>' />
                                        <%# Eval("Student_FullName") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Attendance">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlAttendance" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="Present" Value="Present"></asp:ListItem>
                                            <asp:ListItem Text="Absent" Value="Absent"></asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <div class="card-footer text-center mt-4 pt-4">
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-primary" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
