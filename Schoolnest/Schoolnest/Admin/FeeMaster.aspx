<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FeeMaster.aspx.cs" Inherits="Schoolnest.Admin.FeeMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function togglePaymentSchedule() {
            var selectedOption = document.getElementById('<%= ddlPaymentScheduleType.ClientID %>').value;
            document.getElementById('quarterlySchedule').style.display = (selectedOption === 'Quarterly') ? 'block' : 'none';
            document.getElementById('halfYearlySchedule').style.display = (selectedOption === 'HalfYearly') ? 'block' : 'none';
            document.getElementById('annualSchedule').style.display = (selectedOption === 'Annual') ? 'block' : 'none';
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="formFeeMaster" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Fee Structure Master</div>
                        <div class="d-flex gap-2">
                            <div class="form-group mb-0">
                                <asp:TextBox ID="txtAcademicYear" runat="server" CssClass="form-control" placeholder="Enter Academic Year"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvAcademicYear" ControlToValidate="txtAcademicYear" runat="server" Display="Dynamic" ErrorMessage="Academic Year is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group mb-0">
                                <asp:DropDownList ID="ddlStandard" runat="server" CssClass="form-control" AutoPostBack="true">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvStandard" runat="server" ControlToValidate="ddlStandard" Display="Dynamic" ErrorMessage="Standard selection is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                    </div>

                    <div class="card-body">
                        <!-- Basic Details -->
                        <div class="row mb-4">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlSchool" Text="School"></asp:Label>
                                    <asp:DropDownList ID="ddlSchool" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvSchool" runat="server" ControlToValidate="ddlSchool" Display="Dynamic" ErrorMessage="School selection is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtFeeCode" Text="Fee Code"></asp:Label>
                                    <asp:TextBox ID="txtFeeCode" runat="server" CssClass="form-control" placeholder="Enter Fee Code"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvFeeCode" runat="server" ControlToValidate="txtFeeCode" Display="Dynamic" ErrorMessage="Fee Code is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-check mt-4">
                                    <asp:CheckBox ID="chkIsActive" runat="server" CssClass="form-check-input border-0" Checked="true" />
                                    <label class="form-check-label">Active</label>
                                </div>
                            </div>
                        </div>

                        <!-- Fee Components (Core Fees) -->
                        <div class="card mb-4">
                            <div class="card-header">
                                <h5 class="mb-0">Core Fee Components</h5>
                            </div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Admission Fee (₹)"></asp:Label>
                                            <asp:TextBox ID="txtAdmissionFee" runat="server" CssClass="form-control" placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Registration Fee (₹)"></asp:Label>
                                            <asp:TextBox ID="txtRegistrationFee" runat="server" CssClass="form-control" placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Security Deposit (₹)"></asp:Label>
                                            <asp:TextBox ID="txtSecurityDeposit" runat="server" CssClass="form-control" placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Annual Charges (₹)"></asp:Label>
                                            <asp:TextBox ID="txtAnnualCharges" runat="server" CssClass="form-control" placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Fee Components -->
                        <div class="card mb-4">
                            <div class="card-header">
                                <h5 class="mb-0"> Fee Components</h5>
                            </div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Tuition Fee (₹)"></asp:Label>
                                            <asp:TextBox ID="txtTuitionFee" runat="server" CssClass="form-control" 
                                                placeholder="0.00" TextMode="Number" step="0.01">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Development Fee (₹)"></asp:Label>
                                            <asp:TextBox ID="txtDevelopmentFee" runat="server" CssClass="form-control" 
                                                placeholder="0.00" TextMode="Number" step="0.01">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Library Fee (₹)"></asp:Label>
                                            <asp:TextBox ID="txtLibraryFee" runat="server" CssClass="form-control" 
                                                placeholder="0.00" TextMode="Number" step="0.01">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Computer Fee (₹)"></asp:Label>
                                            <asp:TextBox ID="txtComputerFee" runat="server" CssClass="form-control" 
                                                placeholder="0.00" TextMode="Number" step="0.01">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Sports Fee (₹)"></asp:Label>
                                            <asp:TextBox ID="txtSportsFee" runat="server" CssClass="form-control" 
                                                placeholder="0.00" TextMode="Number" step="0.01">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Transport Fee (₹)"></asp:Label>
                                            <asp:TextBox ID="txtTransportFee" runat="server" CssClass="form-control" 
                                                placeholder="0.00" TextMode="Number" step="0.01">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Lab Fee (₹)"></asp:Label>
                                            <asp:TextBox ID="txtLabFee" runat="server" CssClass="form-control" 
                                                placeholder="0.00" TextMode="Number" step="0.01">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Misc. Fee (₹)"></asp:Label>
                                            <asp:TextBox ID="txtMiscFee" runat="server" CssClass="form-control" 
                                                placeholder="0.00" TextMode="Number" step="0.01">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        
                        <!-- Additional Components -->
                        <div class="card mb-4">
                            <div class="card-header">
                                <h5 class="mb-0">Additional Charges</h5>
                            </div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-4">
                                        <asp:Label runat="server" Text="Late Fee (₹)"></asp:Label>
                                        <asp:TextBox ID="txtLateFee" runat="server" CssClass="form-control" placeholder="0.00" TextMode="Number"></asp:TextBox>
                                    </div>
                                    <div class="col-md-4">
                                        <asp:Label runat="server" Text="Total Fee (₹)"></asp:Label>
                                        <asp:TextBox ID="txtTotalFee" runat="server" CssClass="form-control" placeholder="0.00" TextMode="Number" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Payment Schedule Type -->
                        <div class="form-group mb-4">
                            <asp:Label runat="server" AssociatedControlID="ddlPaymentScheduleType" Text="Payment Schedule Type"></asp:Label>
                            <asp:DropDownList ID="ddlPaymentScheduleType" runat="server" CssClass="form-control" OnChange="togglePaymentSchedule()">
                                <asp:ListItem Text="Select Payment Schedule" Value="" />
                                <asp:ListItem Text="Quarterly" Value="Quarterly" />
                                <asp:ListItem Text="Half-Yearly" Value="HalfYearly" />
                                <asp:ListItem Text="Annual" Value="Annual" />
                            </asp:DropDownList>
                        </div>

                        <!-- Quarterly Payment Schedule -->
                        <div id="quarterlySchedule" class="card mb-4" style="display: none;">
                            <div class="card-header">
                                <h5 class="mb-0">Quarterly Payment Schedule</h5>
                            </div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-3">
                                        <asp:Label runat="server" Text="First Quarter Due Date"></asp:Label>
                                        <asp:TextBox ID="txtFirstQuarter" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                    <div class="col-md-3">
                                        <asp:Label runat="server" Text="Second Quarter Due Date"></asp:Label>
                                        <asp:TextBox ID="txtSecondQuarter" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                    <div class="col-md-3">
                                        <asp:Label runat="server" Text="Third Quarter Due Date"></asp:Label>
                                        <asp:TextBox ID="txtThirdQuarter" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                    <div class="col-md-3">
                                        <asp:Label runat="server" Text="Fourth Quarter Due Date"></asp:Label>
                                        <asp:TextBox ID="txtFourthQuarter" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Half-Yearly Payment Schedule -->
                        <div id="halfYearlySchedule" class="card mb-4" style="display: none;">
                            <div class="card-header">
                                <h5 class="mb-0">Half-Yearly Payment Schedule</h5>
                            </div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-6">
                                        <asp:Label runat="server" Text="First Half Due Date"></asp:Label>
                                        <asp:TextBox ID="txtFirstHalf" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                    <div class="col-md-6">
                                        <asp:Label runat="server" Text="Second Half Due Date"></asp:Label>
                                        <asp:TextBox ID="txtSecondHalf" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Annual Payment Schedule -->
                        <div id="annualSchedule" class="card mb-4" style="display: none;">
                            <div class="card-header">
                                <h5 class="mb-0">Annual Payment Schedule</h5>
                            </div>
                            <div class="card-body">
                                <div class="form-group">
                                    <asp:Label runat="server" Text="Annual Due Date"></asp:Label>
                                    <asp:TextBox ID="txtAnnualDueDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                            </div>
                        </div>


                        <div class="card-footer text-center">
                            <asp:Button ID="btnSubmit" runat="server" Text="Save Fee Structure" CssClass="btn btn-success" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" CausesValidation="false" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
