<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgetPassword.aspx.cs" Inherits="Schoolnest.ForgetPassword" Async="true"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Forgot Password</title>
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
            max-width: 450px;
            position: relative;
        }
        .form-header {
            text-align: center;
        }
        .form-header img {
            max-width: 130px;
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
            display: block;
        }

        .panel-slide-out {
            animation: slideOut 0.5s forwards;
            display: block;
        }
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <div class="d-flex align-items-center justify-content-center h-screen bg-light">
            <div class="form-container">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <!-- Email input panel -->
                        <asp:Panel ID="pnlEmail" runat="server" CssClass="panel-slide-in">
                            <!-- Form Header -->
                            <div class="form-header">
                                <asp:Image ImageUrl="~/assets/img/Forgot-Password.svg" ID="ForgetPasswordImg" runat="server" />
                                <h2>Forgot Password ?</h2>
                                <p>Select your role and enter your username or email to get new otp.</p>
                            </div>
                            <div class="form-group">
                                <asp:DropDownList ID="RoleDropdown" runat="server" CssClass="form-select"></asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvRole" runat="server" ControlToValidate="RoleDropdown" ErrorMessage="Role is required." CssClass="text-danger" Display="Dynamic" ValidationGroup="Panel-1" />
                            </div>
                            <div class="form-group">
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" Placeholder="Username / email address"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Username or Email is required." CssClass="text-danger" Display="Dynamic" ValidationGroup="Panel-1"/>
                            </div>

                            <!-- Submit button -->
                            <div class="form-group">
                                <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-success w-100 my-3" Text="Send OTP" OnClick="btnSubmit_Click" ValidationGroup="Panel-1" OnClientClick="return validatePanel1();"/>
                            </div>
                        </asp:Panel>

                        <!-- 6-Digit Passcode input panel -->
                        <asp:Panel ID="pnlPasscode" runat="server" CssClass="panel-hidden">
                            <!-- Form Header -->
                            <div class="form-header">
                                <h2>Enter Passcode</h2>
                                <p>Please enter the 6-digit passcode sent to your email.</p>
                            </div>
                            <asp:TextBox ID="txtPasscode" runat="server" CssClass="form-control mb-3" Placeholder="Enter 6-digit passcode" MaxLength="6"></asp:TextBox>
                            <div class="text-center mb-3">
                                <span id="timer" class="text-danger fw-bold"></span>
                            </div>
                            <asp:RequiredFieldValidator ID="rfvPasscode" runat="server" ControlToValidate="txtPasscode" ErrorMessage="Passcode is required." CssClass="text-danger" Display="Dynamic" ValidationGroup="Panel-2"/>

                            <!-- Range validator for passcode (ensures it is a 6-digit number) -->
                            <asp:RangeValidator ID="rvPasscode" runat="server" ControlToValidate="txtPasscode" MinimumValue="100000" MaximumValue="999999" Type="Integer" ErrorMessage="Passcode must be a 6-digit number." CssClass="text-danger" Display="Dynamic" ValidationGroup="Panel-2" />

                            <asp:Button ID="btnVerify" runat="server" CssClass="btn btn-success w-100 mb-3" Text="Verify" OnClick="btnVerify_Click"  ValidationGroup="Panel-2"/>
                            <asp:Button ID="btnResendOTP" runat="server" CssClass="btn btn-primary w-100 mb-3 d-none" Text="Resend OTP" OnClick="btnResendOTP_Click" CausesValidation="false" />
                        </asp:Panel>

                        <!-- Success message panel -->
                        <asp:Panel ID="pnlSuccess" runat="server" CssClass="panel-hidden">
                            <!-- Form Header -->
                            <div class="form-header">
                                <i class="fas fa-check-circle text-success fs-1"></i>
                                <h2>Verified!</h2>
                                <p>Your otp has been verified. We have sent you a temporary password, please check your mail.</p>
                                <p>Redirecting to login page...</p>
                            </div>
                        </asp:Panel>

                        <!-- Back to Login Link -->
                        <div class="form-group text-center" id="forget_password_link">
                            <asp:HyperLink ID="HyperLink1" NavigateUrl="~/Login.aspx" runat="server" CssClass="btn-link"><i class="fas fa-arrow-left"></i> Back to Login</asp:HyperLink>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnVerify" EventName="Click" />
                    </Triggers>
            </asp:UpdatePanel>
            </div>
        </div>

        <!-- Timer for redirection after verification -->
        <asp:Timer ID="Timer1" runat="server" Interval="5000" OnTick="Timer1_Tick" Enabled="false"></asp:Timer>
    </form>

    <!-- Core JS Files -->
    <script src="<%= ResolveUrl("~/assets/js/core/jquery-3.7.1.min.js") %>"></script>
    <script src="<%= ResolveUrl("~/assets/js/core/popper.min.js") %>"></script>
    <script src="<%= ResolveUrl("~/assets/js/core/bootstrap.min.js") %>"></script>

   <!-- Add this script before the closing body tag -->
    <script type="text/javascript">
    var timeLeft;
    var timerId;

    function startTimer(duration) {
        timeLeft = duration;
        timerId = setInterval(updateTimer, 1000);
        updateTimer();
    }

    function updateTimer() {
        if (timeLeft <= 0) {
            clearInterval(timerId);
            var timerElement = document.getElementById('timer');
            var verifyBtn = document.getElementById('btnVerify');
            var resendBtn = document.getElementById('btnResendOTP');
            var pnlEmail = document.getElementById('pnlEmail');
            var pnlPasscode = document.getElementById('pnlPasscode');
            var txtPasscode = document.getElementById("txtPasscode");
            
            // Update timer text
            if (timerElement) timerElement.innerHTML = 'OTP Expired';
        
            // Ensure passcode panel stays visible
            if (pnlPasscode) {
                pnlPasscode.classList.remove('panel-hidden');
                pnlPasscode.classList.add('panel-slide-in');
            }
        
            // Ensure email panel stays hidden
            if (pnlEmail) {
                pnlEmail.classList.add('panel-hidden');
                pnlEmail.classList.remove('panel-slide-in');
            }

            txtPasscode.setAttribute('readonly', true);
        
            // Toggle buttons
            if (verifyBtn) verifyBtn.classList.add('d-none');
            if (resendBtn) resendBtn.classList.remove('d-none');

        }
        else {
            var minutes = Math.floor(timeLeft / 60);
            var seconds = timeLeft % 60;
            document.getElementById('timer').textContent =
                'Time remaining: ' + minutes + ':' + (seconds < 10 ? '0' : '') + seconds;
            timeLeft--;
        }
    }

    function showNextPanel(currentPanelId, nextPanelId) {
        var currentPanel = document.getElementById(currentPanelId);
        var nextPanel = document.getElementById(nextPanelId);
        var pnlEmail = document.getElementById('pnlEmail');

        var allPanels = document.querySelectorAll('.panel-slide-in, .panel-slide-out');
        allPanels.forEach(function (panel) {
            panel.classList.add('panel-hidden');
            panel.classList.remove('panel-slide-in', 'panel-slide-out');
        });

        // Hide the current panel with animation
        if (currentPanel) {
            currentPanel.classList.remove('panel-hidden');
            currentPanel.classList.add('panel-slide-out');
        }

        // Special handling for success panel
        if (nextPanelId === 'pnlSuccess') {
            if (pnlEmail) {
                pnlEmail.classList.add('panel-hidden');
                pnlEmail.classList.remove('panel-slide-in');
            }
        }

        setTimeout(function () {
            if (currentPanel) {
                currentPanel.classList.add('panel-hidden');
                currentPanel.classList.remove('panel-slide-in', 'panel-slide-out');
            }

            // Show the next panel
            if (nextPanel) {
                nextPanel.classList.remove('panel-hidden');
                nextPanel.classList.add('panel-slide-in');
            }

            if (nextPanelId === 'pnlPasscode') {
                resetTimer();
            }
        }, 500);
    }

    function resetTimer() {
        clearInterval(timerId);
        var verifyBtn = document.getElementById('btnVerify');
        var resendBtn = document.getElementById('btnResendOTP');
        if (verifyBtn) verifyBtn.classList.remove('d-none');
        if (resendBtn) resendBtn.classList.add('d-none');
        startTimer(60);
    }

    // Function to be called when showing the OTP panel
    function showOTPPanel(currentPanelId, nextPanelId) {
        showNextPanel(currentPanelId, nextPanelId);
        resetTimer();
    }
</script>
</body>
</html>
