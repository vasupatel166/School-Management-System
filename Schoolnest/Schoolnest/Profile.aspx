<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="Schoolnest.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .profile-card {
            border-top-left-radius: 10px !important;
            border-top-right-radius: 10px !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="container">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-header profile-card bg-dark">
                        <h4 class="card-title text-white">Profile</h4>
                    </div>
                    <div class="card-body">
                        <ul class="nav nav-tabs nav-line nav-color-success" id="line-tab" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link active" id="line-details-tab" data-bs-toggle="pill" href="#line-details" role="tab" aria-controls="pills-home" aria-selected="true">Details</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" id="line-change-password-tab" data-bs-toggle="pill" href="#line-change-password" role="tab" aria-controls="pills-profile" aria-selected="false">Change Password</a>
                            </li>
                        </ul>
                        <div class="tab-content mt-3 mb-3" id="line-tabContent">
                            <!-- Details Tab -->
                            <div class="tab-pane fade show active" id="line-details" role="tabpanel" aria-labelledby="line-details-tab">
                                <div class="row" id="Image">
                                    <div class="col-md-4">
                                        <div class="avatar avatar-xxl">
                                            <asp:Image ID="ProfileImage" runat="server" CssClass="avatar-img rounded-circle" />
                                        </div>
                                    </div>
                                </div>
                                <div id="profileFields" runat="server">
                                    <!-- Dynamically Loaded Fields -->
                                </div>
                                <div class="row" id="LocationFields" runat="server">
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
                                <div class="form-group">
                                    <asp:Button ID="btnEditProfile" runat="server" CssClass="btn btn-primary" CausesValidation="false" Text="Edit Profile" OnClick="btnEditProfile_Click" />
                                    <asp:Button ID="btnSaveProfile" runat="server" CssClass="btn btn-success" Text="Save Changes" OnClick="btnSaveProfile_Click" Visible="false" />
                                    <asp:Button ID="btnCancelUpdate" runat="server" CssClass="btn btn-danger" CausesValidation="false" Text="Cancel" OnClick="btnCancelUpdate_Click" Visible="false" />
                                </div>
                            </div>

                            <!-- Change Password Tab -->
                            <div class="tab-pane fade" id="line-change-password" role="tabpanel" aria-labelledby="line-change-password-tab">
                                <div class="form-group">
                                    <label for="newPassword">New Password</label>
                                    <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" />
                                    <asp:RequiredFieldValidator ID="rfvChangePassword" runat="server" ControlToValidate="txtNewPassword" ValidationGroup="PasswordValidation" Display="Dynamic" CssClass="text-danger" ErrorMessage="Please enter new password"></asp:RequiredFieldValidator>
                                </div>
                                <div class="form-group">
                                    <label for="confirmPassword">Confirm New Password</label>
                                    <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" />
                                    <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword" ValidationGroup="PasswordValidation" Display="Dynamic" CssClass="text-danger" ErrorMessage="Please confirm your password"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="cvConfirmPassword" runat="server" Display="Dynamic" ControlToCompare="txtNewPassword" ValidationGroup="PasswordValidation" ControlToValidate="txtConfirmPassword" CssClass="text-danger" ErrorMessage="Password doesn't match"></asp:CompareValidator>
                                </div>
                                <div class="form-group">
                                    <asp:Button ID="btnChangePassword" runat="server" CssClass="btn btn-primary" Text="Change Password" OnClick="btnChangePassword_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
