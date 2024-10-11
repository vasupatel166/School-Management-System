<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TeacherList.aspx.cs" Inherits="Schoolnest.Admin.TeacherList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-header">
                        <div class="card-title">Register Teacher</div>
                    </div>
                    <div class="card-body">

                        <!-- Teacher ID -->
                        <div class="row">
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtTeacherID" Text="Teacher ID"></asp:Label>
                                <asp:TextBox ID="txtTeacherID" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                            </div>
                        </div>

                        <!-- Name fields -->
                        <div class="row">
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name"></asp:Label>
                                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" AutoPostBack="True"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" ErrorMessage="First Name is required." ForeColor="Red"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtMiddleName" Text="Middle Name"></asp:Label>
                                <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-control" AutoPostBack="True"></asp:TextBox>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name"></asp:Label>
                                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" AutoPostBack="True"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" ErrorMessage="Last Name is required." ForeColor="Red"></asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <!-- Teacher Name, Gender, DOB -->
                        <div class="row">
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtTeacherName" Text="Teacher Full Name"></asp:Label>
                                <asp:TextBox ID="txtTeacherName" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvTeacherName" runat="server" ControlToValidate="txtTeacherName" ErrorMessage="Teacher name is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="ddlGender" Text="Gender"></asp:Label>
                                <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">Select Gender</asp:ListItem>
                                    <asp:ListItem Text="Male" Value="M"></asp:ListItem>
                                    <asp:ListItem Text="Female" Value="F"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtDOB" Text="Date of Birth"></asp:Label>
                                <asp:TextBox ID="txtDOB" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvDOB" runat="server" ControlToValidate="txtDOB" ErrorMessage="Date of Birth is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <!-- Type of Appointment, Marital Status, Joining Date -->
                        <div class="row">
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="ddlTypeOfAppointment" Text="Type of Appointment"></asp:Label>
                                <asp:DropDownList ID="ddlTypeOfAppointment" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">Select Type of Appointment</asp:ListItem>
                                    <asp:ListItem Text="Permanent" Value="Permanent"></asp:ListItem>
                                    <asp:ListItem Text="Contract" Value="Contract"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="ddlMaritalStatus" Text="Marital Status"></asp:Label>
                                <asp:DropDownList ID="ddlMaritalStatus" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">Select Status</asp:ListItem>
                                    <asp:ListItem Text="Single" Value="Single"></asp:ListItem>
                                    <asp:ListItem Text="Married" Value="Married"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtJoiningDate" Text="Joining Date"></asp:Label>
                                <asp:TextBox ID="txtJoiningDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvJoiningDate" runat="server" ControlToValidate="txtJoiningDate" ErrorMessage="Joining Date is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <!-- Qualification, Experience, Email -->
                        <div class="row">
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtQualification" Text="Qualification"></asp:Label>
                                <asp:TextBox ID="txtQualification" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvQualification" runat="server" ControlToValidate="txtQualification" ErrorMessage="Qualification is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtExperience" Text="Experience"></asp:Label>
                                <asp:TextBox ID="txtExperience" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvExperience" runat="server" ControlToValidate="txtExperience" ErrorMessage="Experience is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtEmail" Text="Email"></asp:Label>
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Invalid email format" ValidationExpression="\w+@\w+\.\w+" CssClass="text-danger"></asp:RegularExpressionValidator>
                            </div>
                        </div>

                        <!-- Phone Number, Address, Location -->
                        <div class="row">
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtMobileNumber" Text="Phone Number"></asp:Label>
                                <asp:TextBox ID="txtMobileNumber" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvMobileNumber" runat="server" ControlToValidate="txtMobileNumber" ErrorMessage="Phone number is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtAddress1" Text="Address Line 1"></asp:Label>
                                <asp:TextBox ID="txtAddress1" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtAddress2" Text="Address Line 2"></asp:Label>
                                <asp:TextBox ID="txtAddress2" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtAddress3" Text="Address Line 3"></asp:Label>
                                <asp:TextBox ID="txtAddress3" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="fileUploadStudentImage" Text="Upload Teacher Image"></asp:Label>
                                <asp:FileUpload ID="fileUploadStudentImage" runat="server" CssClass="form-control" />
                                <asp:Button ID="btnUploadImage" runat="server" Text="Upload" CssClass="btn btn-primary" CausesValidation="False" />
                                <asp:Button ID="btnDeleteImage" runat="server" Text="Delete Image" CssClass="btn btn-danger" Visible="false" CausesValidation="False" />
                                <asp:HiddenField ID="hfFilePath" runat="server" />
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Image ID="imgStudent" runat="server" CssClass="img-thumbnail" Height="150" Width="150" Visible="false" />
                            </div>
                        </div>

                    </div>

                    <div class="card-footer">
                        <asp:Button ID="btnSubmit" runat="server" Text="Save" CssClass="btn btn-primary" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-primary" OnClick="btnCancel_Click" CausesValidation="False" />
                        <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-primary" CausesValidation="False" />
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" CausesValidation="False" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
