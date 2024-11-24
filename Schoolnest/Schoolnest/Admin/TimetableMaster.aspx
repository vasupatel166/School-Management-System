<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TimetableMaster.aspx.cs" Inherits="Schoolnest.Admin.TimetableMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .timetable-container {
            margin-top: 20px;
            overflow-x: auto;
        }

        .timetable {
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 20px;
        }

        .timetable th {
            background-color: #f8f9fa;
            font-weight: 600;
            text-align: center;
            padding: 12px 8px;
            border: 1px solid #dee2e6;
        }

        .timetable td {
            padding: 12px 8px;
            border: 1px solid #dee2e6;
            text-align: center;
            vertical-align: middle;
            min-width: 120px;
        }

        .timetable th:first-child {
            min-width: 100px;
        }

        .assigned-period {
            background-color: #e3f2fd;
            cursor: help;
            transition: background-color 0.2s;
        }

        .assigned-period:hover {
            background-color: #bbdefb;
        }

        .assigned-period[title] {
            position: relative;
            cursor: help;
        }

        .assigned-period[title]:hover::after {
            content: attr(title);
            position: absolute;
            bottom: 100%;
            left: 50%;
            transform: translateX(-50%);
            padding: 5px 10px;
            background: rgba(0, 0, 0, 0.8);
            color: white;
            border-radius: 4px;
            font-size: 14px;
            white-space: nowrap;
            z-index: 100;
        }

        .break-period {
            background-color: #f8f9fa;
            font-weight: 500;
            color: #6c757d;
            font-style: italic;
        }
    </style>


    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

    <script type="text/javascript">
         function openTimetableModal() {
             $('#timetableModal').modal('show');
             return false;
         }

         function closeModal() {
             $('#timetableModal').modal('hide');
             return false;
         }

         $(document).ready(function () {
             $('#timetableModal').modal({
                 backdrop: 'static',
                 keyboard: false,
                 show: false
             });

             // Bind click events for modal close buttons
             $('.modal-header .close, .modal-footer .btn-secondary').on('click', function () {
                 closeModal();
             });

             // Handle Bootstrap modal events
             $('#timetableModal').on('hidden.bs.modal', function () {
                 // Clear form fields or perform any cleanup if needed
                 $(this).find('select').val('0');
             });
         });
     </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="row">
                    <div class="col-md-12">
                        <div class="card mb-0">
                            <div class="card-header d-flex justify-content-between align-items-center">
                                <div class="card-title">Set Timetable</div>
                            </div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="ddlStandard" Text="Standard"></asp:Label>
                                            <asp:DropDownList ID="ddlStandard" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlStandard_SelectedIndexChanged"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvStandard" runat="server" ControlToValidate="ddlStandard" ValidationGroup="ValidateTeacher" Display="Dynamic" ErrorMessage="Standard is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="ddlDivision" Text="Division"></asp:Label>
                                            <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged" Enabled="false"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvDivision" runat="server" ControlToValidate="ddlDivision" ValidationGroup="ValidateTeacher" Display="Dynamic" ErrorMessage="Division is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </div>
                                <asp:GridView ID="gvSubjects" runat="server" AutoGenerateColumns="false" 
                                    CssClass="table table-bordered table-striped mt-3 p-4" 
                                    OnRowCommand="gvSubjects_RowCommand" 
                                    OnRowDataBound="gvSubjects_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="SubjectName" HeaderText="Subject" ReadOnly="true" />
                                        <asp:BoundField DataField="TeacherName" HeaderText="Teacher" ReadOnly="true" />
                                        <asp:BoundField DataField="SubjectDetailID" ReadOnly="true" Visible="false" />
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnSetTimetable" runat="server" 
                                                    Text="Set Timetable" 
                                                    CommandName="SetTimetable" 
                                                    CommandArgument='<%# Eval("SubjectDetailID") %>' 
                                                    CssClass="btn btn-primary"
                                                     />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row mt-5">
                    <div class="col-md-12">
                        <div class="card mb-0">
                            <div class="card-header d-flex justify-content-between align-items-center">
                                <div class="card-title">Class Timetable</div>
                            </div>
                            <div class="card-body">
                                <div class="timetable-container">
                                    <asp:Table ID="ClassTimetable" CssClass="timetable table-bordered" runat="server">
                                    </asp:Table>
                                </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>

        <%--Modal--%>
        <div class="modal fade" id="timetableModal" tabindex="-1" role="dialog" aria-labelledby="timetableModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div class="modal-header">
                                <h5 class="modal-title" id="timetableModalLabel">Set Timetable</h5>
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close" onclick="return closeModal();">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div class="modal-body">
                                <asp:HiddenField ID="HiddenField1" runat="server" />
                                <table class="table table-bordered">
                                    <thead>
                                        <tr>
                                            <th>Day of the Week</th>
                                            <th>Period Time</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>Monday</td>
                                            <td><asp:DropDownList ID="ddlMonday" runat="server" CssClass="form-control"></asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <td>Tuesday</td>
                                            <td><asp:DropDownList ID="ddlTuesday" runat="server" CssClass="form-control"></asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <td>Wednesday</td>
                                            <td><asp:DropDownList ID="ddlWednesday" runat="server" CssClass="form-control"></asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <td>Thursday</td>
                                            <td><asp:DropDownList ID="ddlThursday" runat="server" CssClass="form-control"></asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <td>Friday</td>
                                            <td><asp:DropDownList ID="ddlFriday" runat="server" CssClass="form-control"></asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <td>Saturday</td>
                                            <td><asp:DropDownList ID="ddlSaturday" runat="server" CssClass="form-control"></asp:DropDownList></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" onclick="return closeModal();">Close</button>
                                <asp:Button ID="btnSaveTimetable" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSaveTimetable_Click" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
