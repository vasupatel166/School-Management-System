<%@ Page Title="Fee Master" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FeeMaster.aspx.cs" Inherits="Schoolnest.Admin.FeeMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="formFeeMaster" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <h5 class="card-title">Fee Master</h5>
                    </div>

                    <div class="card-body">
                        <!-- Standard Dropdown -->
                        <div class="row mb-4">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlStandard" Text="Select Standard" />
                                    <asp:DropDownList ID="ddlStandard" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlStandard_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:CustomValidator ID="cvStandard" runat="server"
                                        ControlToValidate="ddlStandard"
                                        Display="Dynamic"
                                        CssClass="text-danger" />
                                </div>
                            </div>
                        </div>

                        <!-- Fee Repeater -->
                        <asp:Repeater ID="rptFee" runat="server" OnItemCommand="rptFee_ItemCommand" OnItemDataBound="rptFee_ItemDataBound">
                            <ItemTemplate>
                                <div class="row align-items-center mb-2">
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <asp:TextBox ID="txtFeeName" runat="server" CssClass="form-control" Placeholder="Fee Name" />
                                            <asp:CustomValidator ID="cvFeeName" runat="server"
                                                ControlToValidate="txtFeeName"
                                                Display="Dynamic"
                                                CssClass="text-danger" />
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:TextBox ID="txtFeeAmount" runat="server" CssClass="form-control" Placeholder="Amount" TextMode="Number" />
                                            <asp:CustomValidator ID="cvFeeAmount" runat="server"
                                                ControlToValidate="txtFeeAmount"
                                                Display="Dynamic"
                                                CssClass="text-danger" />
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:DropDownList ID="ddlTermType" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="Select Term" Value="" />
                                                <asp:ListItem Text="First" Value="First" />
                                                <asp:ListItem Text="Second" Value="Second" />
                                                <asp:ListItem Text="Both" Value="Both" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-md-2">
                                        <div class="form-group">
                                            <asp:Button ID="btnRemove" runat="server" Text="Remove" CssClass="btn btn-danger btn-sm" CausesValidation="false" CommandName="Remove" CommandArgument='<%# Container.ItemIndex %>' />
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                        <!-- Add Fee Row -->
                        <div class="row mt-4">
                            <div class="col-md-6">
                                <asp:Button ID="btnAddRow" runat="server" Text="Add Fee" CssClass="btn btn-primary" OnClick="btnAddRow_Click" CausesValidation="false" />
                            </div>
                        </div>

                        <!-- Save and Cancel Buttons -->
                        <div class="card-footer text-center mt-4">
                            <asp:Button ID="btnSubmit" runat="server" Text="Save Fee" CssClass="btn btn-success" OnClick="btnSubmit_Click" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" CausesValidation="false" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
