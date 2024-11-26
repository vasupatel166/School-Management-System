﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewExamSchedule.aspx.cs" Inherits="Schoolnest.Teacher.ViewExamSchedule" %>

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
                        <div class="row">
                            <asp:GridView ID="gvExamSchedule" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered">
                                <Columns>
                                    <asp:BoundField DataField="ExamDate" HeaderText="Exam Date" DataFormatString="{0:yyyy-MM-dd}" />
                                    <asp:BoundField DataField="ExamName" HeaderText="Exam Name" />
                                    <asp:BoundField DataField="TotalMarks" HeaderText="Marks" />
                                    <asp:BoundField DataField="StandardName" HeaderText="Class" />
                                    <asp:BoundField DataField="DivisionName" HeaderText="Division" />
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

