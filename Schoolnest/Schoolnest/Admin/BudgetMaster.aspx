<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BudgetMaster.aspx.cs" Inherits="Schoolnest.Admin.BudgetMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <form id="budgetForm" runat="server" class="w-100">
    <div class="row">
        <div class="col-md-12">
            <div class="card mb-0">
                <div class="card-header">
                    <div class="card-title">Create Budget Entry</div>
                </div>

                <div class="card-body">
                    <!-- Category Name -->
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtCategoryName" Text="Category Name"></asp:Label>
                        <asp:TextBox ID="txtCategoryName" runat="server" CssClass="form-control" placeholder="Enter Category Name"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvCategoryName" runat="server" ControlToValidate="txtCategoryName" Display="Dynamic" ErrorMessage="Category name is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                    </div>

                     <!-- Amount (integer only) -->
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="txtAmount" Text="Amount"></asp:Label>
                            <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" placeholder="Enter Amount" TextMode="Number"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvAmount" runat="server" ControlToValidate="txtAmount" Display="Dynamic" ErrorMessage="Amount is required" CssClass="text-danger"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revAmount" runat="server" ControlToValidate="txtAmount" Display="Dynamic" ErrorMessage="Please enter a valid integer amount" CssClass="text-danger" ValidationExpression="^\d+$"></asp:RegularExpressionValidator>
                        </div>

                    <!-- Submit and Cancel Buttons -->
                    <div class="card-footer text-center mt-4 pt-4">
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-success"  />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger"  />
                    </div>
                </div>
            </div>
        </div>
    </div>
</form>

</asp:Content>
