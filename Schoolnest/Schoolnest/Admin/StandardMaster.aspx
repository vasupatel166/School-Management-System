<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StandardMaster.aspx.cs" Inherits="Schoolnest.Admin.StandardMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .btn-primary {
            height: 29px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div class="card-title">Standard Master</div>                 
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6">
                                 <asp:DropDownList ID="ddlSearchStandard" runat="server" CssClass="form-control" AutoPostBack="true" Visible="false" OnSelectedIndexChanged="ddlSearchStandard_SelectedIndexChanged">
                                 </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtStandardID" Text="Standard ID"></asp:Label>
                                    <asp:TextBox ID="txtStandardID" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtStandardName" Text="Standard Name"></asp:Label>
                                    <asp:TextBox ID="txtStandardName" runat="server" CssClass="form-control"></asp:TextBox> 
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtStandardDesc" Text="Standard Desc"></asp:Label>
                                    <asp:TextBox ID="txtStandardDesc" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <!-- Submit Button -->
                        <div class="card-footer text-center mt-4 pt-4">
                            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-success" OnClick="btnSave_Click" />
                            <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-success" CausesValidation="False" OnClick="btnReset_Click"/>
                            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-success" CausesValidation="False" OnClick="btnSearch_Click"/>
                        </div>
                    </div>
                </div>
            </div>
       </div>
    </form>
</asp:Content>
