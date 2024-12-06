<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MarkTeacherAttendance.aspx.cs" Inherits="Schoolnest.Admin.MarkTeacherAttendance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            const dateInput = document.getElementById('<%= txtDate.ClientID %>');
            const today = new Date();

            // Format the date as YYYY-MM-DD
            const formatDate = (date) => {
                const year = date.getFullYear();
                const month = String(date.getMonth() + 1).padStart(2, '0');
                const day = String(date.getDate()).padStart(2, '0');
                return `${year}-${month}-${day}`;
            };

            const maxDate = formatDate(today);

            // Set max date to today
            dateInput.max = maxDate;

            // Set min date to 7 days ago
            const minDate = new Date(today);
            minDate.setDate(minDate.getDate() - 7);
            dateInput.min = formatDate(minDate);
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="attendanceForm" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header">
                        <div class="card-title">Mark Teacher Attendance</div>
                    </div>
                    <div class="card-body">
                        <div class="form-group row">
                            <div class="col-md-3">
                                <asp:Label runat="server" AssociatedControlID="txtDate" Text="Date"></asp:Label>
                                <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvDate" runat="server" ControlToValidate="txtDate" Display="Dynamic" ErrorMessage="Please select a date" CssClass="text-danger" />
                            </div>
                            <div class="col-md-3">
                                <asp:Button ID="btnView" runat="server" Text="View" CssClass="btn btn-primary mt-4" OnClick="btnView_Click" />
                            </div>
                        </div>
                        <asp:GridView ID="gvTeacher" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered" EnableViewState="true">
                            <Columns>
                                <asp:BoundField DataField="TeacherName" HeaderText="Teacher Name" SortExpression="TeacherName" />

                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hfTeacherID" runat="server" Value='<%# Eval("TeacherID") %>' />
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
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
