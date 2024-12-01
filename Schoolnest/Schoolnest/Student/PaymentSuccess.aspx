<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PaymentSuccess.aspx.cs" Inherits="Schoolnest.Student.PaymentSuccess" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="success-container">
        <h2>Payment Successful</h2>
        <p>Thank you for your payment. Your transaction has been completed successfully.</p>
        <asp:Label ID="lblTransactionDetails" runat="server"></asp:Label>
    </div>
</asp:Content>
