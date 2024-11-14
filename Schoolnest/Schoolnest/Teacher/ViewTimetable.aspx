<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewTimetable.aspx.cs" Inherits="Schoolnest.Teacher.ViewTimetable" %>
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

        .not-assigned-period {
            background-color: #f5f5f5;
            color: #757575;
            font-style: italic;
        }

        .break-period {
            background-color: #f8f9fa;
            font-weight: 500;
            color: #6c757d;
            font-style: italic;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="attendanceReportForm" runat="server" class="w-100">
        <asp:ScriptManager runat="server" />
        
        <div class="row">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-header">
                        <h2 class="card-title">Teacher Timetable</h2>
                    </div>

                    <div class="card-body">
                        <!-- Teacher Selection Section -->
                        <div class="teacher-select-container">
                            <asp:UpdatePanel ID="TeacherSelectionPanel" runat="server">
                                <ContentTemplate>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="ddlTeacher" Text="Select Teacher" CssClass="teacher-select-label" />
                                        <asp:DropDownList ID="ddlTeacher" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlTeacher_SelectedIndexChanged"></asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvTeacher" runat="server" ControlToValidate="ddlTeacher" InitialValue="0" Display="Dynamic" ErrorMessage="Please select a teacher" CssClass="text-danger" />
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlTeacher" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>

                        <!-- Timetable Section -->
                        <asp:UpdatePanel ID="TimetablePanel" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="timetable-container">
                                    <asp:Table ID="TeacherTimetable" CssClass="timetable table-bordered" runat="server">
                                    </asp:Table>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlTeacher" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>