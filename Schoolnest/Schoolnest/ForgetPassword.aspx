<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgetPassword.aspx.cs" Inherits="Schoolnest.ForgetPassword1" %>


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

    <style>
        body, html {
            height: 100%;
        }
        .h-screen {
            height: 100vh;
        }
        .form-container {
            background: #fff;
            border-radius: 15px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
            padding: 40px;
            width: 100%;
            max-width: 400px;
            position: relative;
        }
        .form-header {
            text-align: center;
        }
        .form-header img {
            max-width: 130px; /* 30% larger logo */
            margin-bottom: 20px;
        }
        .form-header h2 {
            font-size: 1.75rem;
            color: #343a40;
            margin-bottom: 10px;
        }
        .form-header p {
            color: #6c757d;
            margin-bottom: 30px;
        }
        .btn-custom {
            background-color: #28a745;
            color: #fff;
            font-weight: bold;
        }
        .btn-custom:hover {
            background-color: #218838;
        }
        .text-center a {
            color: #007bff;
        }

        /* Slide Animation */
        @keyframes slideIn {
            from {
                transform: translateX(100%);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }

        @keyframes slideOut {
            from {
                transform: translateX(0);
                opacity: 1;
            }
            to {
                transform: translateX(-100%);
                opacity: 0;
            }
        }

        .panel-hidden {
            display: none;
        }

        .panel-slide-in {
            animation: slideIn 0.5s forwards;
            display: block; /* Ensure the panel is visible while sliding in */
        }

        .panel-slide-out {
            animation: slideOut 0.5s forwards;
            display: block; /* Ensure the panel is visible while sliding out */
        }
    </style>

</head>
<body>
      <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <div class="d-flex align-items-center justify-content-center h-screen bg-light">
            <div class="form-container">
                <!-- Email input panel -->
                <asp:Panel ID="pnlEmail" runat="server" CssClass="panel-slide-in">
                    <!-- Form Header -->
                    <div class="form-header">
                        <img src="Image/Logo.png" />
                        <h2>Forgot Password?</h2>
                        <p>Enter your email to get the reset link.</p>
                    </div>

                    <!-- Email input -->
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control mb-3" Placeholder="Your email address" TextMode="Email"></asp:TextBox>

                    <!-- Error message label -->
                    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" Visible="false"></asp:Label>

                    <!-- Submit button -->
                    <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-custom w-100 mb-3" Text="Send Reset Link" OnClick="btnSubmit_Click" OnClientClick="showNextPanel('pnlEmail', 'pnlPasscode'); return false;" />
                </asp:Panel>

                <!-- 6-Digit Passcode input panel -->
                <asp:Panel ID="pnlPasscode" runat="server" CssClass="panel-hidden">
                    <!-- Form Header -->
                    <div class="form-header">
                        <h2>Enter Passcode</h2>
                        <p>Please enter the 6-digit passcode sent to your email.</p>
                    </div>

                    <!-- Passcode input -->
                    <asp:TextBox ID="txtPasscode" runat="server" CssClass="form-control mb-3" Placeholder="Enter 6-digit passcode" MaxLength="6"></asp:TextBox>

                    <!-- Verify button -->
                    <asp:Button ID="btnVerify" runat="server" CssClass="btn btn-custom w-100 mb-3" Text="Verify" OnClick="btnVerify_Click" OnClientClick="showNextPanel('pnlPasscode', 'pnlSuccess'); return false;" />
                </asp:Panel>

                <!-- Success message panel -->
                <asp:Panel ID="pnlSuccess" runat="server" CssClass="panel-hidden">
                    <!-- Form Header -->
                    <div class="form-header">
                        <h2>Verified!</h2>
                        <p>Your passcode has been verified. Redirecting to login...</p>
                    </div>
                </asp:Panel>
            </div>
        </div>

        <!-- Timer for redirection after verification -->
        <asp:Timer ID="Timer1" runat="server" Interval="3000" OnTick="Timer1_Tick" Enabled="false"></asp:Timer>
    </form>

    <!-- Core JS Files -->
    <script src="<%= ResolveUrl("~/assets/js/core/jquery-3.7.1.min.js") %>"></script>
    <script src="<%= ResolveUrl("~/assets/js/core/popper.min.js") %>"></script>
    <script src="<%= ResolveUrl("~/assets/js/core/bootstrap.min.js") %>"></script>

    <!-- JavaScript to handle panel transitions -->
    <script>
        // JavaScript to handle panel transitions
        function showNextPanel(currentPanelId, nextPanelId) {
            var currentPanel = document.getElementById(currentPanelId);
            var nextPanel = document.getElementById(nextPanelId);

            // Add slide-out animation to current panel
            currentPanel.classList.add('panel-slide-out');

            // Wait for the slide-out animation to finish
            setTimeout(function () {
                currentPanel.classList.add('panel-hidden'); // Hide current panel
                currentPanel.classList.remove('panel-slide-in', 'panel-slide-out');

                nextPanel.classList.remove('panel-hidden'); // Show next panel
                nextPanel.classList.add('panel-slide-in');
            }, 500); // Match the duration of the slide-out animation
        }
    </script>
</body>
</html>
