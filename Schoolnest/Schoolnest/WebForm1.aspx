<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="Schoolnest.WebForm1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Student List</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Student List</h2>
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="table">
                <Columns>
                    <asp:BoundField DataField="name" HeaderText="Name" />
                    <asp:BoundField DataField="email" HeaderText="Email" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>