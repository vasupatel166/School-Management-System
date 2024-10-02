<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegisterSchool.aspx.cs" Inherits="Schoolnest.SuperAdmin.RegisterSchool" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100" >
                <div class="row">
                    <div class="col-md-12">
                        <div class="card mb-0">
                            <div class="card-header">
                                <div class="card-title">Register School</div>
                            </div>
                            <div class="card-body">
                                <div class="row">
                                    <!-- School Name and Address 1 in one row -->
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="txtSchoolName" Text="School Name"></asp:Label>
                                            <asp:TextBox ID="txtSchoolName" runat="server" CssClass="form-control" placeholder="Enter School Name"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvSchoolName" runat="server" ControlToValidate="txtSchoolName" Display="Dynamic" ErrorMessage="School name is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="txtSchoolAdd1" Text="Address Line 1"></asp:Label>
                                            <asp:TextBox ID="txtSchoolAdd1" runat="server" CssClass="form-control" placeholder="Enter Address Line 1"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvSchoolAdd1" runat="server" ControlToValidate="txtSchoolAdd1" Display="Dynamic" ErrorMessage="Address is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <!-- Address 2 and Address 3 in one row -->
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="txtSchoolAdd2" Text="Address Line 2"></asp:Label>
                                            <asp:TextBox ID="txtSchoolAdd2" runat="server" CssClass="form-control" placeholder="Enter Address Line 2"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="txtSchoolAdd3" Text="Address Line 3"></asp:Label>
                                            <asp:TextBox ID="txtSchoolAdd3" runat="server" CssClass="form-control" placeholder="Enter Address Line 3"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <!-- Phone Number and Email in one row -->
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="txtPhoneNo" Text="Phone Number"></asp:Label>
                                            <asp:TextBox ID="txtPhoneNo" runat="server" CssClass="form-control" placeholder="Enter Phone Number"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvPhoneNo" runat="server" ControlToValidate="txtPhoneNo" Display="Dynamic" ErrorMessage="Phone number is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="txtSchoolEmail" Text="Email Address"></asp:Label>
                                            <asp:TextBox ID="txtSchoolEmail" runat="server" CssClass="form-control" placeholder="Enter Email Address"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvSchoolEmail" runat="server" ControlToValidate="txtSchoolEmail" Display="Dynamic" ErrorMessage="Email address is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtSchoolEmail" Display="Dynamic" ErrorMessage="Invalid email format" ValidationExpression="\w+@\w+\.\w+" CssClass="text-danger"></asp:RegularExpressionValidator>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <!-- Website and School Type in one row -->
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="txtSchoolWebsite" Text="Website"></asp:Label>
                                            <asp:TextBox ID="txtSchoolWebsite" runat="server" CssClass="form-control" placeholder="Enter School Website"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="ddlSchoolType" Text="School Type"></asp:Label>
                                            <asp:DropDownList ID="ddlSchoolType" runat="server" CssClass="form-control">
                                                <asp:ListItem Value="">Select Type</asp:ListItem>
                                                <asp:ListItem Value="Public">Aided</asp:ListItem>
                                                <asp:ListItem Value="Private">Un-Aided</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div class="card-footer text-center mt-4 pt-4">
                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-success" />
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>


    </form>
</asp:Content>
