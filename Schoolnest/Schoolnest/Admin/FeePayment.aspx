<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FeePayment.aspx.cs" Inherits="Schoolnest.Admin.FeePayment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <style>
        table#MainContent_gvFeeMaster > tr > td {
            padding: 0 !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form runat="server" id="form1" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Fee Payment</div>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlStandard" Text="Standard"></asp:Label>
                                    <asp:DropDownList ID="ddlStandard" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlStandard_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ControlToValidate="ddlStandard" runat="server" ErrorMessage="Standard is required" CssClass="text-danger" Display="Dynamic" />
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlDivision" Text="Division"></asp:Label>
                                    <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ControlToValidate="ddlDivision" runat="server" ErrorMessage="Division is required" CssClass="text-danger" Display="Dynamic" />
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlStudent" Text="Student"></asp:Label>
                                    <asp:DropDownList ID="ddlStudent" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlStudent_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ControlToValidate="ddlStudent" runat="server" ErrorMessage="Student is required" CssClass="text-danger" Display="Dynamic" />
                                </div>
                            </div>
                        </div>
                        <div class="row mt-5">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label class="control-label" for="txtTotalDue">Total Due</label>
                                    <asp:TextBox ID="txtTotalDue" runat="server" CssClass="form-control fs-3" ReadOnly="true"></asp:TextBox>
                                    <p class="mt-2 fs-6 text-danger" id="LateFeeNote" visible="false" runat="server"></p>
                                    <p class="mt-4 fs-6">Note: Recent payments may not be reflected on your account. Please allow up to 2 business days for payments to appear in the payment summary.</p>
                                </div>
                            </div>
                            <div class="col-md-8">
                                <div class="card">
                                    <div class="card-header">
                                        <div class="card-title">Fees Due Dates</div>
                                    </div>
                                    <div class="card-body">
                                        <div class="table-responsive">
                                            <table class="table table-bordered">
                                                <thead>
                                                    <tr>
                                                        <th scope="col">Term</th>
                                                        <th scope="col">Due Date</th>
                                                        <th scope="col">Amount</th>
                                                        <th scope="col">Status</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <tr>
                                                        <td>Term 1</td>
                                                        <td runat="server" id="FirstTermDate"></td>
                                                        <td runat="server" id="FirstTermAmount"></td>
                                                        <td runat="server" id="FirstTermFeeStatus"></td>
                                                    </tr>
                                                    <tr>
                                                        <td>Term 2</td>
                                                        <td runat="server" id="SecondTermDate"></td>
                                                        <td runat="server" id="SecondTermAmount"></td>
                                                        <td runat="server" id="SecondTermFeeStatus"></td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" ID="lblFeeTerm" AssociatedControlID="ddlTerm" Text="Fee Term"></asp:Label>
                                    <asp:DropDownList ID="ddlTerm" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddlTerm_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                                <div class="row mb-3">
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <label class="control-label" for="ddlPaymentMethod" id="lblPaymentMethod" runat="server">Payment Method</label>
                                            <asp:DropDownList ID="ddlPaymentMethod" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddlPaymentMethod_SelectedIndexChanged">
                                                <asp:ListItem Text="Cash" Value="Cash" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Cheque" Value="Cheque"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-md-8" id="chequeNumberContainer" runat="server" visible="false">
                                        <div class="form-group">
                                            <label class="control-label" for="txtChequeNumber" id="lblChequeNumber" runat="server">Cheque Number</label>
                                            <asp:TextBox ID="txtChequeNumber" runat="server" CssClass="form-control"></asp:TextBox>
                                            <asp:RequiredFieldValidator ControlToValidate="txtChequeNumber" runat="server" ErrorMessage="Cheque Number is required" CssClass="text-danger" Display="Dynamic" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row mt-2" id="chequeDateContainer" runat="server" visible="false">
                                    <div class="col-md-12">
                                        <div class="form-group">
                                            <label class="control-label" for="txtChequeDate" id="lblChequeDate" runat="server">Cheque Date</label>
                                            <asp:TextBox ID="txtChequeDate" TextMode="Date" runat="server" CssClass="form-control"></asp:TextBox>
                                            <asp:RequiredFieldValidator ControlToValidate="txtChequeDate" runat="server" ErrorMessage="Cheque Date is required" CssClass="text-danger" Display="Dynamic" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row mb-3">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label class="control-label" for="amountTextBox" id="lblPayAmount" runat="server">Pay Amount (INR)</label>
                                            <asp:TextBox ID="amountTextBox" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                            <asp:Button ID="payButton" runat="server" Text="Pay Now" CssClass="btn btn-success mt-4" OnClick="PayButton_Click" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row p-3">
                                    <div class="col-md-8">
                                        <asp:Label ID="lblError" runat="server" CssClass="text-danger"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-8">
                                <asp:GridView ID="gvFeeMaster" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered" OnRowDataBound="gvFeeMaster_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="SRNo" HeaderText="SR. NO" SortExpression="SRNo" />
                                        <asp:BoundField DataField="FeeName" HeaderText="Fee Name" SortExpression="FeeName" />
                                        <asp:BoundField DataField="FeeAmount" HeaderText="Amount" SortExpression="FeeAmount" />
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:HiddenField runat="server" ID="hfFeeMasterID" Value='<%# Eval("FeeMasterID") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
