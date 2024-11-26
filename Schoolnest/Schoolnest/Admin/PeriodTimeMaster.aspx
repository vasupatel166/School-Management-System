<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PeriodTimeMaster.aspx.cs" Inherits="Schoolnest.Admin.PeriodTimeMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <form id="periodForm" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header">
                        <div class="card-title">Period Details</div>
                    </div>

                    <div class="card-body">
                        <!-- Period Time -->
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlPeriodTime" Text="Period Time"></asp:Label>
                                    <asp:DropDownList ID="ddlPeriodTime" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvPeriodTime" runat="server" ControlToValidate="ddlPeriodTime" InitialValue="" Display="Dynamic" ErrorMessage="Please select a Period Time" CssClass="text-danger" />
                                </div>
                            </div>

                            <!-- Is Break Time -->
                            <div class="col-md-6">
                                <div class="form-check">
                                    <asp:CheckBox ID="chkIsBreakTime" runat="server" CssClass="form-check-input border-0" AutoPostBack="true" />
                                    <asp:Label runat="server" AssociatedControlID="chkIsBreakTime" Text="Is Break Time" CssClass="form-check-label"></asp:Label>
                                </div>
                            </div>     
                        </div>

                        <!-- Buttons -->
                        <div class="card-footer text-center mt-4 pt-4">
                            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-success" OnClick="btnSave_Click" />
                            <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-danger" CausesValidation="False" OnClick="btnReset_Click" />
                        </div>

                        <div class="row mt-4">
                            <div class="col-md-12">
                                <asp:GridView ID="gvPeriodTime" runat="server" AutoGenerateColumns="False" 
                                    CssClass="table table-striped table-bordered" OnRowCommand="gvPeriodTime_RowCommand">
                                    <Columns>
                                        <asp:BoundField DataField="PeriodTimeID" HeaderText="ID" />
                                        <asp:BoundField DataField="PeriodTime" HeaderText="Period Time" />
                                        <asp:TemplateField HeaderText="Break Time">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkGridIsBreakTime" runat="server" Enabled="false"
                                                    Checked='<%# Convert.ToBoolean(Eval("IsBreakTime")) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Actions">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkEdit" runat="server" CssClass="btn btn-primary btn-sm"
                                                    CommandName="EditPeriod" CommandArgument='<%# Eval("PeriodTimeID") %>' CausesValidation="false">
                                                    <i class="fas fa-edit"></i> Edit
                                                </asp:LinkButton>
                                                <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-danger btn-sm"
                                                    CommandName="DeletePeriod" CommandArgument='<%# Eval("PeriodTimeID") %>'
                                                    OnClientClick="return confirm('Are you sure you want to delete this period?');" CausesValidation="false">
                                                    <i class="fas fa-trash"></i> Delete
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>

</asp:Content>
