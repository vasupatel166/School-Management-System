<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentLogin.aspx.cs" Inherits="Schoolnest.Student.StudentLogin" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Student Login</title>
    <meta content="width=device-width, initial-scale=1.0, shrink-to-fit=no" name="viewport" />
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/css/bootstrap.min.css") %>" />
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
    <form id="StudentLoginForm" runat="server">
        <asp:ScriptManager ID="StudentScriptManager1" runat="server"></asp:ScriptManager>
        <div class="container mt-5">
            <div class="row justify-content-center">
                <div class="col-md-6">
                    <div class="card">
                        <div class="card-header text-center">
                            <h3>Student Login</h3>
                        </div>
                        <div class="card-body">
                            <!-- Username/Email Field -->
                            <div class="form-group">
                                <label for="txtUsername">Username/Email</label>
                                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter Username or Email"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="StudentRfvUsername" runat="server" ControlToValidate="txtUsername" ErrorMessage="Username or Email is required" CssClass="text-danger" Display="Dynamic" />
                            </div>

                            <!-- Password Field -->
                            <div class="form-group">
                                <label for="txtPassword">Password</label>
                                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Enter Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="StudentRfvPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required" CssClass="text-danger" Display="Dynamic" />
                            </div>

                            <!-- Buttons and Link -->
                            <div class="form-group text-center">
                                <asp:Button ID="StudentBtnLogin" runat="server" Text="Login" CssClass="btn btn-success" OnClick="StudentBtnLogin_Click" />
                                <div class="mt-3">
                                    <asp:HyperLink ID="StudentLnkForgetPassword" NavigateUrl="~/Student/ForgetPassword.aspx" runat="server" CssClass="btn-link">Forgot Password?</asp:HyperLink>
                                </div>
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


</body>
</html>
