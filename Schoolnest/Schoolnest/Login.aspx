<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Schoolnest.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Login</title>
    <meta content="width=device-width, initial-scale=1.0, shrink-to-fit=no" name="viewport" />
    <link rel="icon" href="<%= ResolveUrl("~/assets/img/favicon.ico") %>" type="image/x-icon" />

    <!-- Fonts and icons -->
    <script src="https://cdn.jsdelivr.net/npm/webfontloader@1.6.28/webfontloader.js"></script>
    <script>
        WebFont.load({
            google: { families: ["Public Sans:300,400,500,600,700"] },
            custom: {
                families: ["Font Awesome 5 Solid", "Font Awesome 5 Regular", "Font Awesome 5 Brands", "simple-line-icons"],
                urls: ["https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css"]
            },
            active: function () {
                sessionStorage.fonts = true;
            }
        });
    </script>

    <!-- CSS Files -->
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/css/bootstrap.min.css") %>" />
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/css/plugins.min.css") %>" />
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/css/kaiadmin.min.css") %>" />
</head>
<body>
    <form id="loginform" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div class="container mt-5">
            <div class="row justify-content-center">
                <div class="col-md-6">
                    <div class="card">
                        <div class="card-header text-center">
                            <h3>Login</h3>
                        </div>
                        <div class="card-body">
                            <div class="form-group">
                                <label for="userType">User Type</label>
                                <asp:DropDownList ID="ddlUserType" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="ddlUserType_SelectedIndexChanged">
                                    <asp:ListItem Text="Select User Type" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Super admin" Value="SA"></asp:ListItem>
                                    <asp:ListItem Text="Admin" Value="A"></asp:ListItem>
                                    <asp:ListItem Text="Teacher" Value="T"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvUserType" runat="server" ControlToValidate="ddlUserType" InitialValue="" ErrorMessage="User Type is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group" id="school_id_field">
                                <label for="schoolId">School ID</label>
                                <asp:TextBox ID="txtSchoolId" runat="server" CssClass="form-control" placeholder="Enter School ID"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvSchoolId" runat="server" ControlToValidate="txtSchoolId" ErrorMessage="School ID is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group">
                                <label for="username">Username/Email</label>
                                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter Username or email"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ControlToValidate="txtUsername" ErrorMessage="Username is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group">
                                <label for="password">Password</label>
                                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Enter Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group text-center">
                                <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-success" OnClick="btnLogin_Click" />
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary ml-2" CausesValidation="false" OnClick="btnCancel_Click" />
                            </div>
                            <div class="form-group text-center" id="forget_password_link">
                                <asp:HyperLink ID="HyperLink1" NavigateUrl="~/ForgetPassword.aspx" runat="server" CssClass="btn-link">Forget Password</asp:HyperLink>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </form>

    <!-- Core JS Files -->
    <script src="<%= ResolveUrl("~/assets/js/core/jquery-3.7.1.min.js") %>"></script>
    <script src="<%= ResolveUrl("~/assets/js/core/popper.min.js") %>"></script>
    <script src="<%= ResolveUrl("~/assets/js/core/bootstrap.min.js") %>"></script>

    <script>
        $(document).ready(function () {
            $("#school_id_field").hide();

            // Function to check user role and show/hide elements accordingly
            function checkUserRole(role) {
                if (role == "A" || role == "T") {
                    $("#school_id_field").show();
                } else {
                    $("#school_id_field").hide();
                }
            }

            checkUserRole($("#ddlUserType").val());

            $("#ddlUserType").change(function () {
                checkUserRole($(this).val());
            });
        });
    </script>


</body>
</html>
