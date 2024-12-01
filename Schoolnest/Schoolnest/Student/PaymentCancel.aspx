<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PaymentCancel.aspx.cs" Inherits="Schoolnest.Student.PaymentCancel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cancel-container">
        <h2>Payment Cancelled</h2>
        <p>Your payment has been cancelled. If this was unintentional, please try again.</p>
        <asp:HyperLink ID="lnkReturnToPayment" runat="server" NavigateUrl="~/Student/FeePayment.aspx" CssClass="btn btn-primary">Return to Payment</asp:HyperLink>
    </div>
</asp:Content>
