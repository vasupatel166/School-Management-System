<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FeeMaster.aspx.cs" Inherits="Schoolnest.Admin.FeeMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="formFeeMaster" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Fee Structure Master</div>
                        <!-- Search Dropdowns -->
                        <div class="d-flex gap-2">
                            <div class="form-group mb-0">
                                <asp:DropDownList ID="ddlAcademicYear" runat="server" CssClass="form-control" AutoPostBack="true">
                                    <asp:ListItem Text="Select Academic Year" Value=""></asp:ListItem>
                                    <asp:ListItem Text="2024-2025" Value="2024-2025"></asp:ListItem>
                                    <asp:ListItem Text="2025-2026" Value="2025-2026"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="form-group mb-0">
                                <asp:DropDownList ID="ddlStandard" runat="server" CssClass="form-control" AutoPostBack="true">
                                    <asp:ListItem Text="Select Standard" Value=""></asp:ListItem>
                                </asp:DropDownList>
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
                                        <asp:ListItem Text="GURU0012024" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvSchool" runat="server" ControlToValidate="ddlSchool" 
                                        Display="Dynamic" ErrorMessage="School selection is required" CssClass="text-danger">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtFeeCode" Text="Fee Code"></asp:Label>
                                    <asp:TextBox ID="txtFeeCode" runat="server" CssClass="form-control" placeholder="Enter Fee Code"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvFeeCode" runat="server" ControlToValidate="txtFeeCode" 
                                        Display="Dynamic" ErrorMessage="Fee Code is required" CssClass="text-danger">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-check mt-4">
                                    <asp:CheckBox ID="chkIsActive" runat="server" CssClass="form-check-input border-0" Checked="true" />
                                    <label class="form-check-label">Active</label>
                                </div>
                            </div>
                        </div>

                        <!-- Fee Components -->
                        <div class="card mb-4">
                            <div class="card-header">
                                <h5 class="mb-0">Annual Fee Components</h5>
                            </div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Admission Fee (₹)"></asp:Label>
                                            <asp:TextBox ID="txtAdmissionFee" runat="server" CssClass="form-control" 
                                                placeholder="0.00" TextMode="Number" step="0.01">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Registration Fee (₹)"></asp:Label>
                                            <asp:TextBox ID="txtRegistrationFee" runat="server" CssClass="form-control" 
                                                placeholder="0.00" TextMode="Number" step="0.01">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Security Deposit (₹)"></asp:Label>
                                            <asp:TextBox ID="txtSecurityDeposit" runat="server" CssClass="form-control" 
                                                placeholder="0.00" TextMode="Number" step="0.01">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Annual Charges (₹)"></asp:Label>
                                            <asp:TextBox ID="txtAnnualCharges" runat="server" CssClass="form-control" 
                                                placeholder="0.00" TextMode="Number" step="0.01">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Quarterly Fee Components -->
                        <div class="card mb-4">
                            <div class="card-header">
                                <h5 class="mb-0">Quarterly Fee Components</h5>
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

                        <!-- Fee Payment Schedule -->
                        <div class="card mb-4">
                            <div class="card-header">
                                <h5 class="mb-0">Payment Schedule</h5>
                            </div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="First Quarter Due Date"></asp:Label>
                                            <asp:TextBox ID="txtFirstQuarter" runat="server" CssClass="form-control" 
                                                TextMode="Date">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Second Quarter Due Date"></asp:Label>
                                            <asp:TextBox ID="txtSecondQuarter" runat="server" CssClass="form-control" 
                                                TextMode="Date">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Third Quarter Due Date"></asp:Label>
                                            <asp:TextBox ID="txtThirdQuarter" runat="server" CssClass="form-control" 
                                                TextMode="Date">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Fourth Quarter Due Date"></asp:Label>
                                            <asp:TextBox ID="txtFourthQuarter" runat="server" CssClass="form-control" 
                                                TextMode="Date">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="card-footer text-center">
                            <asp:Button ID="btnSubmit" runat="server" Text="Save Fee Structure" CssClass="btn btn-success" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" CausesValidation="false" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>