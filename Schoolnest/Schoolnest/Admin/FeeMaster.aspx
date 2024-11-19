<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FeeMaster.aspx.cs" Inherits="Schoolnest.Admin.FeeMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="formFeeMaster" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Fee Master</div>
                    </div>

                    <div class="card-body">
                        <div class="row mb-4">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlStandard" Text="Standard"></asp:Label>
                                    <asp:DropDownList ID="ddlStandard" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlStandard_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:CustomValidator ID="cvStandard" runat="server" 
                                        ControlToValidate="ddlStandard"
                                        OnServerValidate="ValidateStandard"
                                        Display="Dynamic"
                                        ValidationGroup="ValidateFee"
                                        CssClass="text-danger">
                                    </asp:CustomValidator>
                                </div>
                            </div>
                        </div>

                        <asp:Repeater ID="rptFee" runat="server" OnItemCommand="rptFee_ItemCommand">
                            <ItemTemplate>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <asp:TextBox ID="txtFeeName" runat="server" CssClass="form-control" Placeholder="Fee Name"></asp:TextBox>
                                            <asp:CustomValidator ID="cvFeeName" runat="server" 
                                                ControlToValidate="txtFeeName"
                                                OnServerValidate="ValidateFeeName"
                                                Display="Dynamic"
                                                ValidationGroup="ValidateFee"
                                                CssClass="text-danger">
                                            </asp:CustomValidator>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:TextBox ID="txtFeeAmount" runat="server" CssClass="form-control" Placeholder="Amount" TextMode="Number"></asp:TextBox>
                                            <asp:CustomValidator ID="cvFeeAmount" runat="server" 
                                                ControlToValidate="txtFeeAmount"
                                                OnServerValidate="ValidateFeeAmount"
                                                Display="Dynamic"
                                                ValidationGroup="ValidateFee"
                                                CssClass="text-danger">
                                            </asp:CustomValidator>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:DropDownList ID="ddlTermType" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="First" Value="First"></asp:ListItem>
                                                <asp:ListItem Text="Second" Value="Second"></asp:ListItem>
                                                <asp:ListItem Text="Both" Value="Both"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-md-2">
                                        <div class="form-group">
                                            <asp:Button ID="btnRemove" runat="server" Text="Remove" CssClass="btn btn-danger" CausesValidation="false" CommandName="Remove" CommandArgument='<%# Container.ItemIndex %>' />
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Button ID="btnAddRow" runat="server" Text="Add Fee" CssClass="btn btn-primary" OnClick="btnAddRow_Click" CausesValidation="false" />
                                </div>
                            </div>
                        </div>

                        <div class="card-footer text-center mt-4 pt-4">
                            <asp:Button ID="btnSubmit" runat="server" Text="Save Fee" CssClass="btn btn-success" OnClick="btnSubmit_Click" ValidationGroup="ValidateFee" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" CausesValidation="false" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>