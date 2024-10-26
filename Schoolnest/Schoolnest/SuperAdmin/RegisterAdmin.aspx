<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegisterAdmin.aspx.cs" Inherits="Schoolnest.SuperAdmin.RegisterAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Register Admin</div>
                        <!-- Search Dropdown -->
                        <div class="form-group mb-0">
                            <asp:DropDownList ID="ddlSearchAdmin" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlSearchAdmin_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="card-body">
                        <!-- Admin First Name, Last Name and Gender -->
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name"></asp:Label>
                                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="Enter First Name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" Display="Dynamic" ErrorMessage="First Name is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name"></asp:Label>
                                    <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" placeholder="Enter Last Name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" Display="Dynamic" ErrorMessage="Last Name is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlGender" Text="Gender"></asp:Label>
                                    <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="Select Gender" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Male" Value="Male"></asp:ListItem>
                                        <asp:ListItem Text="Female" Value="Female"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvGender" runat="server" ControlToValidate="ddlGender" Display="Dynamic" ErrorMessage="Gender is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>

                        <!-- Admin Email and Mobile Number -->
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtEmail" Text="Email"></asp:Label>
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter Email"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" Display="Dynamic" ErrorMessage="Email is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail" Display="Dynamic" ErrorMessage="Invalid email format" ValidationExpression="\w+@\w+\.\w+" CssClass="text-danger"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtMobileNumber" Text="Mobile Number"></asp:Label>
                                    <asp:TextBox ID="txtMobileNumber" runat="server" CssClass="form-control" placeholder="Enter Mobile Number"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvMobileNumber" runat="server" ControlToValidate="txtMobileNumber" Display="Dynamic" ErrorMessage="Mobile Number is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>

                        <!-- Select School for the Admin -->
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlSchool" Text="Select School"></asp:Label>
                                    <asp:DropDownList ID="ddlSchool" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvSchool" runat="server" ControlToValidate="ddlSchool" Display="Dynamic" ErrorMessage="School selection is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="fileProfileImage" Text="Profile Image (optional)"></asp:Label>
                                    <asp:FileUpload ID="fileProfileImage" runat="server" CssClass="form-control" />
                                </div>
                            </div>
                             <div class="col-md-4">
                                 <div class="form-check mt-2">
                                     <asp:CheckBox
                                             ID="chkIsActive"
                                             runat="server"
                                             CssClass="form-check-input border-0"
                                             Checked="true"
                                         />
                                     <label class="form-check-label" for="flexCheckDefault">
                                     Active
                                     </label>
                                 </div>
                             </div>
                        </div>

                        <div class="card-footer text-center mt-4 pt-4">
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-success" OnClick="btnSubmit_Click" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" CausesValidation="false" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

   </form>

</asp:Content>
