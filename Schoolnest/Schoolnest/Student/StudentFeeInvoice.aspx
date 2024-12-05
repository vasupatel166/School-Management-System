<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentFeeInvoice.aspx.cs" Inherits="Schoolnest.Student.StudentFeeInvoice" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Fee Payment Invoice</title>
    <script type="text/javascript">
        function printInvoice() {
            window.print(); // Triggers the browser's print dialog
        }
    </script>
</head>
<body onload="printInvoice();">
    <form id="form1" runat="server">
        <div id="invoiceContainer" style="width: 100%; padding: 20px;">
            <!-- The PDF will be displayed here -->
        </div>
    </form>
</body>
</html>
