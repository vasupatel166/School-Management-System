<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegisterSchool.aspx.cs" Inherits="Schoolnest.SuperAdmin.RegisterSchool" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Register School</div>
                        <!-- Search Dropdown -->
                        <div class="form-group mb-0">
                            <asp:DropDownList ID="ddlSearchSchool" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSearchSchool_SelectedIndexChanged" CssClass="form-control">
                                <asp:ListItem Text="Select School" Value=""></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>


                    <div class="card-body">

                        <!-- School Name and Principal Name -->
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtSchoolName" Text="School Name"></asp:Label>
                                    <asp:TextBox ID="txtSchoolName" runat="server" CssClass="form-control" placeholder="Enter School Name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvSchoolName" runat="server" ControlToValidate="txtSchoolName" Display="Dynamic" ErrorMessage="School name is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                             <div class="col-md-6">
                                 <div class="form-group">
                                     <asp:Label runat="server" AssociatedControlID="txtPrincipalName" Text="Principal Name"></asp:Label>
                                     <asp:TextBox ID="txtPrincipalName" runat="server" CssClass="form-control" placeholder="Enter Principal Name"></asp:TextBox>
                                     <asp:RequiredFieldValidator ID="rfvPrincipalName" runat="server" ControlToValidate="txtPrincipalName" Display="Dynamic" ErrorMessage="Principal name is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                 </div>
                             </div>
                         </div>

                        <!--Address 1 & 2 -->
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtSchoolAdd1" Text="Address Line 1"></asp:Label>
                                    <asp:TextBox ID="txtSchoolAdd1" runat="server" CssClass="form-control" placeholder="Enter Address Line 1"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvSchoolAdd1" runat="server" ControlToValidate="txtSchoolAdd1" Display="Dynamic" ErrorMessage="Address is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtSchoolAdd2" Text="Address Line 2 (optional)"></asp:Label>
                                    <asp:TextBox ID="txtSchoolAdd2" runat="server" CssClass="form-control" placeholder="Enter Address Line 2"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <!-- School Location (State, City, Pincode) -->
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlState" Text="State"></asp:Label>
                                    <asp:DropDownList ID="ddlState" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlState_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvState" runat="server" ControlToValidate="ddlState" Display="Dynamic" ErrorMessage="State is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlCity" Text="City"></asp:Label>
                                    <asp:DropDownList ID="ddlCity" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCity_SelectedIndexChanged" Enabled="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvCity" runat="server" ControlToValidate="ddlCity" Display="Dynamic" ErrorMessage="City is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlPincode" Text="Pincode"></asp:Label>
                                    <asp:DropDownList ID="ddlPincode" runat="server" CssClass="form-control" Enabled="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvPincode" runat="server" ControlToValidate="ddlPincode" Display="Dynamic" ErrorMessage="Pincode is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>

                         <!-- School Phone, Alternate Phone, Email -->
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtPhoneNo" Text="Phone Number"></asp:Label>
                                    <asp:TextBox ID="txtPhoneNo" runat="server" CssClass="form-control" placeholder="Enter Phone Number"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvPhoneNo" runat="server" ControlToValidate="txtPhoneNo" Display="Dynamic" ErrorMessage="Phone number is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtAlterPhoneNo" Text="Alternate Phone Number (optional)"></asp:Label>
                                    <asp:TextBox ID="txtAlterPhoneNo" runat="server" CssClass="form-control" placeholder="Enter Alternate Phone Number"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtSchoolEmail" Text="Email Address"></asp:Label>
                                    <asp:TextBox ID="txtSchoolEmail" runat="server" CssClass="form-control" placeholder="Enter Email Address"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvSchoolEmail" runat="server" ControlToValidate="txtSchoolEmail" Display="Dynamic" ErrorMessage="Email address is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtSchoolEmail" Display="Dynamic" ErrorMessage="Invalid email format" ValidationExpression="\w+@\w+\.\w+" CssClass="text-danger"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                        </div>

                        <!-- Website, School Type and Board Affiliation -->
                        <div class="row">                    
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtSchoolWebsite" Text="Website (optional)"></asp:Label>
                                    <asp:TextBox ID="txtSchoolWebsite" runat="server" CssClass="form-control" placeholder="Enter School Website"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlSchoolType" Text="School Type"></asp:Label>
                                    <asp:DropDownList ID="ddlSchoolType" runat="server" AutoPostBack="true" CssClass="form-control">
                                        <asp:ListItem Value="">Select Type</asp:ListItem>
                                        <asp:ListItem Value="Aided">Aided</asp:ListItem>
                                        <asp:ListItem Value="Un-Aided">Un-Aided</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvSchoolType" runat="server" ControlToValidate="ddlSchoolType" Display="Dynamic" ErrorMessage="School Type is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>

                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlBoardAffiliation" Text="Board Affiliation (optional)"></asp:Label>
                                    <asp:DropDownList ID="ddlBoardAffiliation" runat="server" AutoPostBack="true" CssClass="form-control">
                                        <asp:ListItem Value="">Select Board</asp:ListItem>
                                        <asp:ListItem Value="ICSE">ICSE</asp:ListItem>
                                        <asp:ListItem Value="CBSE">CBSE</asp:ListItem>
                                        <asp:ListItem Value="State Board">State Board</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <!-- Established Year , School Category & Is Active -->
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlSchoolCategory" Text="School Category"></asp:Label>
                                    <asp:DropDownList ID="ddlSchoolCategory" runat="server" AutoPostBack="true" CssClass="form-control">
                                        <asp:ListItem Value="">Select Category</asp:ListItem>
                                        <asp:ListItem Value="Pre-Primary">Pre-Primary</asp:ListItem>
                                        <asp:ListItem Value="Primary">Primary</asp:ListItem>
                                        <asp:ListItem Value="Secondary">Secondary</asp:ListItem>
                                        <asp:ListItem Value="Higher Secondary">Higher Secondary</asp:ListItem>
                                        <asp:ListItem Value="Senior Secondary">Senior Secondary</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvSchoolCategory" runat="server" ControlToValidate="ddlSchoolCategory" Display="Dynamic" ErrorMessage="School Category is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>

                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtSchoolEstablishedYear" Text="Established Year (optional)"></asp:Label>
                                    <asp:TextBox ID="txtSchoolEstablishedYear" runat="server" CssClass="form-control" placeholder="Enter School Established Year"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-md-4">
                                <div class="form-check mt-2">
                                    <asp:CheckBox
                                            ID="chkIsActive"
                                            runat="server"
                                            CssClass="form-check-input border-0"
                                            Checked="true"
                                            AutoPostBack="true"
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

