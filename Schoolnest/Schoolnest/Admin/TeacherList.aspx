<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TeacherList.aspx.cs" Inherits="Schoolnest.Teacher.TeacherList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Teacher Master</div>
                        <div class="col-md-4">
                            <asp:DropDownList ID="ddlSearchTeacher" runat="server" CssClass="form-control" AutoPostBack="true" Visible="false" OnSelectedIndexChanged="ddlSearchTeacher_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtTeacherID" Text="Teacher ID"></asp:Label>
                                    <asp:TextBox ID="txtTeacherID" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="fileUploadTeacherImage" Text="Upload Teacher Image  (optional)"></asp:Label>
                                <asp:FileUpload ID="fileUploadTeacherImage" runat="server" CssClass="form-control" />
                                <asp:Button ID="btnUploadImage" runat="server" Text="Upload" CssClass="btn btn-primary" CausesValidation="False" OnClick="btnUploadImage_Click" />
                                <asp:Button ID="btnDeleteImage" runat="server" Text="Delete Image" CssClass="btn btn-danger" Visible="false" CausesValidation="False" OnClick="btnDeleteImage_Click" />
                                <asp:HiddenField ID="hfFilePath" runat="server" />
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Image ID="imgTeacher" runat="server" CssClass="img-thumbnail" Height="150" Width="150" Visible="false" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name"></asp:Label>
                                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" AutoPostBack="True" OnTextChanged="txtName_TextChanged"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" Display="Dynamic" ErrorMessage="First Name is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name"></asp:Label>
                                    <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" AutoPostBack="True" OnTextChanged="txtName_TextChanged"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" Display="Dynamic" ErrorMessage="Last Name is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtFullName" Text="Full Name"></asp:Label>
                                    <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" ReadOnly="True"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlGender" Text="Gender"></asp:Label>
                                    <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="Select Gender" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Male" Value="M"></asp:ListItem>
                                        <asp:ListItem Text="Female" Value="F"></asp:ListItem>
                                        <asp:ListItem Text="Other" Value="O"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtDateOfBirth" Text="Date of Birth"></asp:Label>
                                    <asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlTeacherType" Text="Teacher Type"></asp:Label>
                                    <asp:DropDownList ID="ddlTeacherType" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="Select Type" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="Permanent" Value="Permanent"></asp:ListItem>
                                        <asp:ListItem Text="Contract" Value="Contract"></asp:ListItem>
                                        <asp:ListItem Text="Temporary" Value="Temporary"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlMaritalStatus" Text="Marital Status"></asp:Label>
                                    <asp:DropDownList ID="ddlMaritalStatus" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="Select Status" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="Single" Value="Single"></asp:ListItem>
                                        <asp:ListItem Text="Married" Value="Married"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtEmail" Text="Email"></asp:Label>
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" Display="Dynamic" ErrorMessage="Email is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail" Display="Dynamic" ErrorMessage="Invalid email format" ValidationExpression="\w+@\w+\.\w+" CssClass="text-danger"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtMobileNumber" Text="Mobile Number"></asp:Label>
                                    <asp:TextBox ID="txtMobileNumber" runat="server" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvMobileNumber" runat="server" ControlToValidate="txtMobileNumber" Display="Dynamic" ErrorMessage="Mobile Number is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtQualification" Text="Qualification"></asp:Label>
                                    <asp:TextBox ID="txtQualification" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtExperience" Text="Experience (Years)"></asp:Label>
                                    <asp:TextBox ID="txtExperience" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtDateOfJoining" Text="Date of Joining (optional)"></asp:Label>
                                    <asp:TextBox ID="txtDateOfJoining" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtAddress1" Text="Address 1"></asp:Label>
                                    <asp:TextBox ID="txtAddress1" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtAddress2" Text="Address 2  (optional)"></asp:Label>
                                    <asp:TextBox ID="txtAddress2" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <!-- Teacher Location (State, City, Pincode) -->
                        <div class="row">
                            <div class="col-md-3">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlState" Text="State"></asp:Label>
                                    <asp:DropDownList ID="ddlState" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlState_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvState" runat="server" ControlToValidate="ddlState" Display="Dynamic" ErrorMessage="State is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlCity" Text="City"></asp:Label>
                                    <asp:DropDownList ID="ddlCity" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCity_SelectedIndexChanged" Enabled="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvCity" runat="server" ControlToValidate="ddlCity" Display="Dynamic" ErrorMessage="City is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlPincode" Text="Pincode"></asp:Label>
                                    <asp:DropDownList ID="ddlPincode" runat="server" CssClass="form-control" Enabled="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvPincode" runat="server" ControlToValidate="ddlPincode" Display="Dynamic" ErrorMessage="Pincode is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="form-check mt-2">
                                    <asp:CheckBox
                                        ID="chkIsActive"
                                        runat="server"
                                        CssClass="form-check-input border-0"
                                        Checked="true" />
                                    <label class="form-check-label" for="flexCheckDefault">
                                        Active
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- Submit Button -->
                    <div class="card-footer text-center p-4">
                        <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-success" OnClick="btnSave_Click" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" CausesValidation="False" />
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" CausesValidation="False" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
