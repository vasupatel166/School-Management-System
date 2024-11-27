<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Timetable.aspx.cs" Inherits="Schoolnest.Student.Timetable" %>

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
                        <h2 class="card-title">Student Timetable</h2>
                    </div>

                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label ID="lblStandard" runat="server" CssClass="standard-label fs-3 fw-bold"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="timetable-container">
                                    <asp:Table ID="StudentTimetable" CssClass="timetable" runat="server"></asp:Table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>

