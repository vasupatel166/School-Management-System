<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FeePaymentHistory.aspx.cs" Inherits="Schoolnest.Student.FeePaymentHistory" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label class="control-label">Standard:</label>
                                    <asp:Label ID="lblStandard" runat="server" CssClass="form-control-static"></asp:Label>
                                    <asp:Label ID="lblStandardID" runat="server" CssClass="form-control-static"></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label class="control-label">Division:</label>
                                    <asp:Label ID="lblDivision" runat="server" CssClass="form-control-static"></asp:Label>
                                    <asp:Label ID="lblDivisionID" runat="server" CssClass="form-control-static"></asp:Label>
                                </div>
                            </div>
                        </div>

                        <!-- Fee Payment History GridView -->
                            <div class="row">
                                <asp:GridView ID="gvFeePaymentHistory" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered">
                                    <Columns>
                                        <asp:BoundField DataField="FeeRecordID" HeaderText="Fee Record ID" SortExpression="FeeRecordID" />
                                        <asp:BoundField DataField="ReceiptNumber" HeaderText="Receipt Number" SortExpression="ReceiptNumber" />
                                        <asp:BoundField DataField="TermType" HeaderText="Term Type" SortExpression="TermType" />
                                        <asp:BoundField DataField="PaidAmount" HeaderText="Paid Amount" SortExpression="PaidAmount" />
                                        <asp:BoundField DataField="AmountInWords" HeaderText="Amount In Words" SortExpression="AmountInWords" />
                                        <asp:BoundField DataField="PaymentDate" HeaderText="Payment Date" DataFormatString="{0:dd/MM/yyyy}" SortExpression="PaymentDate" />

                                         <asp:TemplateField HeaderText="Action">
                                            <ItemTemplate>
                                                <a href='<%# "Invoice.aspx?FeeRecordID=" + Eval("FeeRecordID") %>' class="btn btn-primary btn-sm">View Invoice</a>
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
