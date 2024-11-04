<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BudgetCategories.aspx.cs" Inherits="Schoolnest.Admin.BudgetCategories" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .form-group {
            margin-bottom: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Set Budget Categories</div>
                    </div>
                    
                    <!-- Dropdown for Budget Year -->
                    <div class="card-body">
                        <div class="form-group">
                            <asp:Label AssociatedControlID="ddlBudgetYear" runat="server" Text="Select Budget Year"></asp:Label>
                            <asp:DropDownList ID="ddlBudgetYear" runat="server" CssClass="form-control" AutoPostBack="true">
                            </asp:DropDownList>
                        </div>
                    </div>

                    <!-- Dynamic fields container -->
                    <div class="card-body" id="categoryContainer" runat="server">
                        <!-- Dynamic rows will be added here -->
                    </div>

                    <div class="card-footer text-center mt-4 pt-4">
                        <asp:Button ID="btnAddCategory" runat="server" Text="Add Category" CssClass="btn btn-primary" OnClick="btnAddCategory_Click" />
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-success" OnClick="btnSubmit_Click" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" CausesValidation="false" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
