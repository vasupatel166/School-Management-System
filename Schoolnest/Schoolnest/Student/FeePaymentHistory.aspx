<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FeePaymentHistory.aspx.cs" Inherits="Schoolnest.Student.FeePaymentHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function openInvoicePDF(feeRecordID) {
            // Open a new window/tab for the PDF
            window.open(`StudentFeeInvoice.aspx?FeeRecordID=${feeRecordID}`, '_blank');
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Fee Payment History</div>
                    </div>
                    <div class="card-body">

                        <!-- Display student Standard and Division -->
                        <div class="row mb-3">
                            <div class="col-md-3">
                                <div class="form-group">
                                    <label class="control-label fs-4 fw-bold">Standard : </label>
                                    <asp:Label ID="lblStandard" runat="server" CssClass="form-control-static fs-5 fw-light"></asp:Label>
                                    <label class="control-label fs-4 fw-bold">Division : </label>
                                    <asp:Label ID="lblDivision" runat="server" CssClass="form-control-static fs-5 fw-light"></asp:Label>
                                </div>
                            </div>
                        </div>

                        <!-- Fee Payment History GridView -->
                        <div class="row">
                            <asp:GridView ID="gvFeePaymentHistory" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered">
                                <Columns>
                                    <asp:BoundField DataField="FeeDetail_FeeDetailID" HeaderText="Document Number" SortExpression="FeeDetail_FeeDetailID" />
                                    <asp:BoundField DataField="ReceiptNumber" HeaderText="Receipt Number" SortExpression="ReceiptNumber" />
                                    <asp:BoundField DataField="TermType" HeaderText="Term" SortExpression="Term" />
                                    <asp:BoundField DataField="PaidAmount" HeaderText="Amount Paid" SortExpression="PaidAmount" />
                                    <asp:BoundField DataField="AmountInWords" HeaderText="Amount In Words" SortExpression="AmountInWords" />
                                    <asp:BoundField DataField="PaymentDate" HeaderText="Payment Date" DataFormatString="{0:dd/MM/yyyy}" SortExpression="PaymentDate" />
                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnViewInvoice" runat="server" CssClass="btn btn-primary btn-sm"
                                                OnClientClick='<%# $"return openInvoicePDF(\"{Eval("FeeRecordID")}\");" %>'
                                                Text="View Invoice" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
