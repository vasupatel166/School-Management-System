<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AssignTeacher.aspx.cs" Inherits="Schoolnest.Admin.AssignTeacher" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Assign Teacher</div>
                    </div>

                    <div class="card-body">
                        <!-- Form for assigning teachers -->
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtAssignmentID" Text="Assignment ID"></asp:Label>
                                    <asp:TextBox ID="txtAssignmentID" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlTeacher" Text="Teacher"></asp:Label>
                                    <asp:DropDownList ID="ddlTeacher" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlStandard" Text="Standard"></asp:Label>
                                    <asp:DropDownList ID="ddlStandard" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlDivision" Text="Division"></asp:Label>
                                    <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlSection" Text="Section"></asp:Label>
                                    <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Buttons -->
                    <div class="card-footer text-center mt-4 pt-4">
                        <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-success" OnClick="btnSave_Click" />
                        <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-danger" CausesValidation="False" OnClick="btnReset_Click" />
                    </div>
                </div>
            </div>
        </div>

        <div class="row mt-4">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">All Assigned Teacher</div>
                    </div>

                    <div class="card-body">

                        <asp:GridView ID="gvAssignments" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
                            DataKeyNames="AssignmentID"
                            OnRowEditing="gvAssignments_RowEditing"
                            OnRowDeleting="gvAssignments_RowDeleting"
                            OnRowDataBound="gvAssignments_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="AssignmentID" HeaderText="Assignment ID" />
                                <asp:BoundField DataField="TeacherName" HeaderText="Teacher" />
                                <asp:BoundField DataField="Standard" HeaderText="Standard" />
                                <asp:BoundField DataField="Division" HeaderText="Division" />
                                <asp:BoundField DataField="Section" HeaderText="Section" />
                                <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" />
                            </Columns>
                        </asp:GridView>

                    </div>
                </div>
            </div>
        </div>


    </form>
</asp:Content>
