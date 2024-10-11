<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="Schoolnest.Profile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .profile-header {
            background: #2C3E50;
            color: white;
            padding: 20px;
            border-radius: 10px 10px 0 0;
        }

        .profile-card {
            border: none;
            box-shadow: 0 0 20px rgba(0, 0, 0, 0.1);
            border-radius: 10px;
        }

        .profile-body {
            padding: 30px;
        }

        .form-control:read-only {
            background-color: #f9f9f9;
        }

        .edit-section {
            border: 2px dashed #3498db;
            padding: 20px;
            border-radius: 10px;
            background-color: #f0f9ff;
        }

        .edit-section h4 {
            color: #3498db;
        }

        .readonly-section {
            margin-bottom: 30px;
        }

        .readonly-section h4 {
            color: #4b6584;
        }

        .edit-btn {
            color: #3498db;
            cursor: pointer;
            margin-left: 10px;
        }

        .edit-btn:hover {
            text-decoration: underline;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
    <div class="container mt-5">
        <!-- Profile Header -->
        <div class="profile-header d-flex justify-content-between align-items-center">
            <h2 id="profile-title">User Profile</h2>
            <div>
                <asp:Button ID="btnEditProfile" runat="server" CssClass="btn btn-primary" Text="Edit Profile" OnClientClick="return false;" OnClick="btnEditProfile_Click" />
                <asp:Button ID="btnSaveProfile" runat="server" CssClass="btn btn-success" Text="Save Changes" OnClick="btnSaveProfile_Click" Style="display: none;" />
                <asp:Label ID="lblChangePasswordMessage" runat="server" CssClass="text-success ml-3" Text="" />
            </div>
        </div>

        <!-- Profile Card -->
        <div class="card profile-card mt-4">
            <div class="card-body profile-body">
                <!-- SuperAdmin Profile Panel -->
                <asp:Panel ID="pnlSuperAdminProfile" runat="server" Visible="false">
                    <div class="readonly-section">
                        <h4>SuperAdmin Profile</h4>
                        <hr />
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Name</label>
                                    <asp:TextBox ID="txtSuperAdminName" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                            </div>
                            <div class="col-md-6 text-center">
                                <asp:Image ID="imgSuperAdminProfile" runat="server" CssClass="profile-image img-fluid rounded-circle" Width="150px" />
                            </div>
                        </div>
                    </div>

                    <!-- Editable Section -->
                    <div class="edit-section">
                        <h4>Edit SuperAdmin Details</h4>
                        <hr />
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Email</label>
                                    <asp:TextBox ID="txtSuperAdminEmail" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                                <div class="form-group">
                                    <label>Mobile Number</label>
                                    <asp:TextBox ID="txtSuperAdminMobile" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                                <div class="form-group">
                                    <label>Address</label>
                                    <asp:TextBox ID="txtSuperAdminAddress" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Admin Profile Panel -->
                <asp:Panel ID="pnlAdminProfile" runat="server" Visible="false">
                    <div class="readonly-section">
                        <h4>Admin Profile</h4>
                        <hr />
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Name</label>
                                    <asp:TextBox ID="txtAdminName" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                            </div>
                            <div class="col-md-6 text-center">
                                <asp:Image ID="imgAdminProfile" runat="server" CssClass="profile-image img-fluid rounded-circle" Width="150px" />
                            </div>
                        </div>
                    </div>

                    <!-- Editable Section -->
                    <div class="edit-section">
                        <h4>Edit Admin Details</h4>
                        <hr />
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Email</label>
                                    <asp:TextBox ID="txtAdminEmail" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                                <div class="form-group">
                                    <label>Mobile Number</label>
                                    <asp:TextBox ID="txtAdminMobile" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                                <div class="form-group">
                                    <label>Address</label>
                                    <asp:TextBox ID="txtAdminAddress" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Teacher Profile Panel -->
                <asp:Panel ID="pnlTeacherProfile" runat="server" Visible="false">
                    <div class="readonly-section">
                        <h4>Teacher Profile</h4>
                        <hr />
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Name</label>
                                    <asp:TextBox ID="txtTeacherName" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                                <div class="form-group">
                                    <label>Gender</label>
                                    <asp:TextBox ID="txtTeacherGender" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                                <div class="form-group">
                                    <label>Date of Birth</label>
                                    <asp:TextBox ID="txtTeacherDOB" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                            </div>
                            <div class="col-md-6 text-center">
                                <asp:Image ID="imgTeacherProfile" runat="server" CssClass="profile-image img-fluid rounded-circle" Width="150px" />
                            </div>
                        </div>
                    </div>

                    <!-- Editable Section -->
                    <div class="edit-section">
                        <h4>Edit Teacher Details</h4>
                        <hr />
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Email</label>
                                    <asp:TextBox ID="txtTeacherEmail" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                                <div class="form-group">
                                    <label>Mobile Number</label>
                                    <asp:TextBox ID="txtTeacherMobile" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                                <div class="form-group">
                                    <label>Address</label>
                                    <asp:TextBox ID="txtTeacherAddress" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Student Profile Panel -->
                <asp:Panel ID="pnlStudentProfile" runat="server" Visible="false">
                    <div class="readonly-section">
                        <h4>Student Profile</h4>
                        <hr />
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Name</label>
                                    <asp:TextBox ID="txtStudentName" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                                <div class="form-group">
                                    <label>Gender</label>
                                    <asp:TextBox ID="txtStudentGender" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                                <div class="form-group">
                                    <label>Date of Birth</label>
                                    <asp:TextBox ID="txtStudentDOB" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                            </div>
                            <div class="col-md-6 text-center">
                                <asp:Image ID="imgStudentProfile" runat="server" CssClass="profile-image img-fluid rounded-circle" Width="150px" />
                            </div>
                        </div>
                    </div>

                    <!-- Editable Section -->
                    <div class="edit-section">
                        <h4>Edit Student Details</h4>
                        <hr />
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Email</label>
                                    <asp:TextBox ID="txtStudentEmail" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                                <div class="form-group">
                                    <label>Mobile Number</label>
                                    <asp:TextBox ID="txtStudentMobile" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                                <div class="form-group">
                                    <label>Address</label>
                                    <asp:TextBox ID="txtStudentAddress" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
        </form>
</asp:Content>
