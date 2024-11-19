<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BudgetMaster.aspx.cs" Inherits="Schoolnest.Admin.BudgetMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .badge-success { background-color: #28a745; color: white; padding: 5px 10px; border-radius: 5px; }
        .badge-danger { background-color: #dc3545; color: white; padding: 5px 10px; border-radius: 5px; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="budgetForm" runat="server" class="w-100">
        <div class="card mb-0">
            <div class="card-header">
                <div class="card-title">Create Budget Entry</div>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-4">
                        <div class="form-group">
                            <asp:Label AssociatedControlID="txtBudgetName" runat="server" Text="Budget Name"></asp:Label>
                            <asp:TextBox ID="txtBudgetName" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="txtBudgetName" runat="server" ErrorMessage="Budget Name is required" CssClass="text-danger" Display="Dynamic" />
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="form-group">
                            <asp:Label AssociatedControlID="txtTotalAllocated" runat="server" Text="Total Allocated"></asp:Label>
                            <asp:TextBox ID="txtTotalAllocated" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="txtTotalAllocated" runat="server" ErrorMessage="Amount is required" CssClass="text-danger" Display="Dynamic" />
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="form-group">
                            <asp:Label AssociatedControlID="txtAcademicYear" runat="server" Text="Academic Year"></asp:Label>
                            <asp:TextBox ID="txtAcademicYear" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="txtAcademicYear" runat="server" ErrorMessage="Academic Year is required" CssClass="text-danger" Display="Dynamic" />
                            <asp:RegularExpressionValidator ID="revAcademicYear" runat="server" ControlToValidate="txtAcademicYear"
                                ErrorMessage="Academic Year must be in the format yyyy-yyyy" CssClass="text-danger" 
                                ValidationExpression="^\d{4}-\d{4}$" Display="Dynamic" />
                            <asp:Label ID="lblErrorMessage" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div class="form-check mt-2">
                            <asp:CheckBox ID="chkIsActive" runat="server" CssClass="form-check-input" Checked="true" />
                            <label class="form-check-label">Active</label>
                        </div>
                    </div>
                </div>
                <div class="card-footer text-center mt-4 pt-4">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-success" OnClick="btnSubmit_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" CausesValidation="false" />
                </div>
            </div>
        </div>

        <div class="card mt-5">
            <div class="card-header">
                <h4 class="card-title">All Budgets</h4>
            </div>
            <div class="card-body">
                <div class="table-responsive">
                    <asp:GridView ID="gvBudgets" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-hover" OnRowCommand="gvBudgets_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="BudgetName" HeaderText="Name" />
                            <asp:BoundField DataField="TotalAllocated" HeaderText="Allocated" DataFormatString="₹{0:N}" />
                            <asp:BoundField DataField="TotalSpent" HeaderText="Spent" DataFormatString="₹{0:N}" />
                            <asp:BoundField DataField="RemainingAmount" HeaderText="Remaining" DataFormatString="₹{0:N}" />

                            <asp:BoundField DataField="AcademicYear" HeaderText="Year" />
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <asp:Label ID="lblStatus" runat="server" Text='<%# (bool)Eval("Status") ? "Active" : "Inactive" %>'
                                        CssClass='<%# (bool)Eval("Status") ? "badge badge-success" : "badge badge-danger" %>'>
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
        
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:Button ID="btnEdit" runat="server" CommandName="EditBudget" CommandArgument='<%# Eval("BudgetID") %>' CausesValidation="false" Text="Edit" CssClass="btn btn-primary btn-sm" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
