<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BudgetCategories.aspx.cs" Inherits="Schoolnest.Admin.BudgetCategories" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label AssociatedControlID="ddlBudgetYear" runat="server" Text="Select Budget Year"></asp:Label>
                                    <asp:DropDownList ID="ddlBudgetYear" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlBudgetYear_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator runat="server" ID="rfvBUdgetYear" ValidationGroup="ValidateCategory" 
                                        ControlToValidate="ddlBudgetYear" Display="Dynamic" ErrorMessage="Select Budget Year." 
                                        CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label AssociatedControlID="txtAnnualBudget" runat="server" Text="Annual Budget"></asp:Label>
                                    <asp:TextBox ID="txtAnnualBudget" runat="server" CssClass="form-control" ReadOnly="true">
                                    </asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label AssociatedControlID="txtCategoryName" runat="server" Text="Category Name"></asp:Label>
                                    <asp:TextBox ID="txtCategoryName" runat="server" CssClass="form-control" Placeholder="Category Name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvCategoryName" runat="server" ValidationGroup="ValidateCategory" ControlToValidate="txtCategoryName" Display="Dynamic" CssClass="text-danger" ErrorMessage="Category name is required"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label AssociatedControlID="txtAmountAllocated" runat="server" Text="Amount Allocated"></asp:Label>
                                    <asp:TextBox ID="txtAmountAllocated" runat="server" CssClass="form-control" TextMode="Number" Placeholder="Amount Allocated"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvAmountAllocated" runat="server" ValidationGroup="ValidateCategory" ControlToValidate="txtAmountAllocated" Display="Dynamic" CssClass="text-danger" ErrorMessage="Amount is required"></asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rvAmountAllocated" MinimumValue="1" MaximumValue="100000000" runat="server" ControlToValidate="txtAmountAllocated" Display="Dynamic" CssClass="text-danger" ErrorMessage="Amount cannot be zero" />
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label AssociatedControlID="txtActualExpenditure" runat="server" Text="Actual Expenditure"></asp:Label>
                                    <asp:TextBox ID="txtActualExpenditure" runat="server" CssClass="form-control" TextMode="Number" Placeholder="Actual Expenditure" Enabled="false"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvActualExpenditure" runat="server" ValidationGroup="ValidateCategory" ControlToValidate="txtActualExpenditure" Display="Dynamic" CssClass="text-danger" ErrorMessage="Expenditure is required"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                    </div>


                    <div class="card-footer text-center mt-4 py-4">
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-success" OnClick="btnSubmit_Click" ValidationGroup="ValidateCategory" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" CausesValidation="false" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
        </div>
        <div class="card mt-5">
            <div class="card-header">
                <h4 class="card-title">All Budget Categories</h4>
            </div>
            <div class="card-body">
                <div class="table-responsive">
                    <asp:GridView ID="gvBudgetCategories" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-hover" OnRowCommand="gvBudgetCategories_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="BudgetCategoryName" HeaderText="Name" />
                            <asp:BoundField DataField="AmountAllocated" HeaderText="Allocated" DataFormatString="₹{0:N}" />
                            <asp:BoundField DataField="ActualExpenditure" HeaderText="Spent" DataFormatString="₹{0:N}" />
                            <asp:BoundField DataField="TotalAllocated" HeaderText="Total Budget" DataFormatString="₹{0:N}" />
                            <asp:BoundField DataField="AcademicYear" HeaderText="Year" />

                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:Button ID="btnEdit" runat="server" CommandName="EditBudget" CommandArgument='<%# Eval("BudgetCategoryID") %>' CausesValidation="false" Text="Edit" CssClass="btn btn-primary btn-sm" />
                                    <asp:Button ID="btnDelete" runat="server" CommandName="DeleteBudget" CommandArgument='<%# Eval("BudgetCategoryID") %>' CausesValidation="false" Text="Delete" CssClass="btn btn-danger btn-sm" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>

    </form>
</asp:Content>