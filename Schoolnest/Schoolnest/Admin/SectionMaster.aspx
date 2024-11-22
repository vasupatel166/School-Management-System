<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SectionMaster.aspx.cs" Inherits="Schoolnest.Admin.SectionMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Section Master</div>
                    </div>

                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6">
                                <asp:DropdownList ID="ddlSearchSection" runat="server" CssClass="form-control" AutoPostBack="true" Visible="false" OnSelectedIndexChanged="ddlSearchSection_SelectedIndexChanged">
                                </asp:DropdownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtSectionCode" Text="Section Code"></asp:Label>
                                    <asp:TextBox ID="txtSectionCode" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtSectionName" Text="Section Name"></asp:Label>
                                    <asp:TextBox ID="txtSectionName" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtSectionDesc" Text="Section Description"></asp:Label>
                                    <asp:TextBox ID="txtSectionDesc" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Submit Button -->
                    <div class="card-footer">
                        <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                        <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-primary" CausesValidation="False" OnClick="btnReset_Click" />
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" CausesValidation="False" OnClick="btnSearch_Click" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
