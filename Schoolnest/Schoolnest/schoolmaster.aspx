<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="schoolmaster.aspx.cs" Inherits="Schoolnest.schoolmaster" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Insert or Update School</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Insert or Update School</h2>
            
            <!-- Hidden field to store SchoolID -->
            <asp:TextBox ID="txtSchoolID" runat="server" Visible="false"></asp:TextBox><br />

            <asp:Label ID="lblSchoolName" runat="server" Text="School Name:"></asp:Label>
            <asp:TextBox ID="txtSchoolName" runat="server"></asp:TextBox><br /><br />

            <asp:Label ID="lblSchoolAdd1" runat="server" Text="Address 1:"></asp:Label>
            <asp:TextBox ID="txtSchoolAdd1" runat="server"></asp:TextBox><br /><br />

            <asp:Label ID="lblSchoolAdd2" runat="server" Text="Address 2:"></asp:Label>
            <asp:TextBox ID="txtSchoolAdd2" runat="server"></asp:TextBox><br /><br />

            <asp:Label ID="lblSchoolAdd3" runat="server" Text="Address 3:"></asp:Label>
            <asp:TextBox ID="txtSchoolAdd3" runat="server"></asp:TextBox><br /><br />

            <asp:Label ID="lblPhoneNo" runat="server" Text="Phone No:"></asp:Label>
            <asp:TextBox ID="txtPhoneNo" runat="server"></asp:TextBox><br /><br />

            <asp:Label ID="lblSchoolEmail" runat="server" Text="Email:"></asp:Label>
            <asp:TextBox ID="txtSchoolEmail" runat="server"></asp:TextBox><br /><br />

            <asp:Label ID="lblSchoolWebsite" runat="server" Text="Website:"></asp:Label>
            <asp:TextBox ID="txtSchoolWebsite" runat="server"></asp:TextBox><br /><br />

            <asp:Label ID="lblSchoolType" runat="server" Text="School Type:"></asp:Label>
            <asp:TextBox ID="txtSchoolType" runat="server"></asp:TextBox><br /><br />

            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
        </div>
    </form>
</body>
</html>