<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PaymentReport.aspx.cs" Inherits="Schoolnest.Admin.PaymentReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="formPaymentReport" runat="server" class="w-100">
        <div class="row">
            <div class="col-md-12">
                <div class="card mb-0">
                    <div class="card-header">
                        <div class="card-title">Fee Payment Report</div>
                    </div>

                    <div class="card-body">
                        <!-- Filters Section -->
                        <div class="card mb-4">
                            <div class="card-header">
                                <h5 class="mb-0">Filter Options</h5>
                            </div>
                            <div class="card-body">
                                <div class="row">
                                    <!-- Basic Filters -->
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="ddlSchool" Text="School"></asp:Label>
                                            <asp:DropDownList ID="ddlSchool" runat="server" CssClass="form-control" AutoPostBack="true">
                                                <asp:ListItem Text="Select School" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="ddlAcademicYear" Text="Academic Year"></asp:Label>
                                            <asp:DropDownList ID="ddlAcademicYear" runat="server" CssClass="form-control" AutoPostBack="true">
                                                <asp:ListItem Text="Select Academic Year" Value=""></asp:ListItem>
                                                <asp:ListItem Text="2024-2025" Value="2024-2025"></asp:ListItem>
                                                <asp:ListItem Text="2023-2024" Value="2023-2024"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="ddlStandard" Text="Standard"></asp:Label>
                                            <asp:DropDownList ID="ddlStandard" runat="server" CssClass="form-control" AutoPostBack="true">
                                                <asp:ListItem Text="All Standards" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="ddlSection" Text="Section"></asp:Label>
                                            <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="All Sections" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div class="row mt-3">
                                    <!-- Advanced Filters -->
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="ddlPaymentStatus" Text="Payment Status"></asp:Label>
                                            <asp:DropDownList ID="ddlPaymentStatus" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="All Status" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Paid" Value="Paid"></asp:ListItem>
                                                <asp:ListItem Text="Pending" Value="Pending"></asp:ListItem>
                                                <asp:ListItem Text="Partial" Value="Partial"></asp:ListItem>
                                                <asp:ListItem Text="Overdue" Value="Overdue"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="ddlQuarter" Text="Quarter"></asp:Label>
                                            <asp:DropDownList ID="ddlQuarter" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="All Quarters" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Quarter 1" Value="Q1"></asp:ListItem>
                                                <asp:ListItem Text="Quarter 2" Value="Q2"></asp:ListItem>
                                                <asp:ListItem Text="Quarter 3" Value="Q3"></asp:ListItem>
                                                <asp:ListItem Text="Quarter 4" Value="Q4"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="txtFromDate" Text="From Date"></asp:Label>
                                            <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="txtToDate" Text="To Date"></asp:Label>
                                            <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="row mt-3">
                                    <!-- Search Options -->
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="txtSearchStudent" Text="Student Name/ID"></asp:Label>
                                            <asp:TextBox ID="txtSearchStudent" runat="server" CssClass="form-control" 
                                                placeholder="Enter Student Name or ID">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="txtReceiptNo" Text="Receipt Number"></asp:Label>
                                            <asp:TextBox ID="txtReceiptNo" runat="server" CssClass="form-control" 
                                                placeholder="Enter Receipt Number">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="ddlPaymentMode" Text="Payment Mode"></asp:Label>
                                            <asp:DropDownList ID="ddlPaymentMode" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="All Modes" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Cash" Value="Cash"></asp:ListItem>
                                                <asp:ListItem Text="Cheque" Value="Cheque"></asp:ListItem>
                                                <asp:ListItem Text="Online" Value="Online"></asp:ListItem>
                                                <asp:ListItem Text="UPI" Value="UPI"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-md-3 d-flex align-items-end">
                                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary me-2" />
                                        <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-secondary" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Summary Cards -->
                        <div class="row mb-4">
                            <div class="col-md-3">
                                <div class="card bg-primary text-white">
                                    <div class="card-body">
                                        <h6 class="card-title">Total Expected</h6>
                                        <h4 class="mb-0">₹<asp:Label ID="lblTotalExpected" runat="server" Text="0.00"></asp:Label></h4>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="card bg-success text-white">
                                    <div class="card-body">
                                        <h6 class="card-title">Total Collected</h6>
                                        <h4 class="mb-0">₹<asp:Label ID="lblTotalCollected" runat="server" Text="0.00"></asp:Label></h4>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="card bg-danger text-white">
                                    <div class="card-body">
                                        <h6 class="card-title">Total Outstanding</h6>
                                        <h4 class="mb-0">₹<asp:Label ID="lblTotalOutstanding" runat="server" Text="0.00"></asp:Label></h4>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="card bg-warning text-white">
                                    <div class="card-body">
                                        <h6 class="card-title">Collection Percentage</h6>
                                        <h4 class="mb-0"><asp:Label ID="lblCollectionPercentage" runat="server" Text="0"></asp:Label>%</h4>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Report Grid -->
                        <div class="card">
                            <div class="card-header d-flex justify-content-between align-items-center">
                                <h5 class="mb-0">Payment Details</h5>
                                <div>
                                    <asp:Button ID="btnExportExcel" runat="server" Text="Export to Excel" CssClass="btn btn-success btn-sm me-2" />
                                    <asp:Button ID="btnExportPDF" runat="server" Text="Export to PDF" CssClass="btn btn-danger btn-sm" />
                                </div>
                            </div>
                            <div class="card-body">
                                <asp:GridView ID="gvPaymentReport" runat="server" CssClass="table table-striped table-bordered" 
                                    AutoGenerateColumns="false" AllowPaging="true" PageSize="10">
                                    <Columns>
                                        <asp:BoundField DataField="ReceiptNo" HeaderText="Receipt No." />
                                        <asp:BoundField DataField="PaymentDate" HeaderText="Payment Date" DataFormatString="{0:dd-MM-yyyy}" />
                                        <asp:BoundField DataField="StudentID" HeaderText="Student ID" />
                                        <asp:BoundField DataField="StudentName" HeaderText="Student Name" />
                                        <asp:BoundField DataField="Standard" HeaderText="Standard" />
                                        <asp:BoundField DataField="Section" HeaderText="Section" />
                                        <asp:BoundField DataField="Quarter" HeaderText="Quarter" />
                                        <asp:BoundField DataField="TotalAmount" HeaderText="Total Amount" DataFormatString="₹{0:N2}" />
                                        <asp:BoundField DataField="PaidAmount" HeaderText="Paid Amount" DataFormatString="₹{0:N2}" />
                                        <asp:BoundField DataField="Balance" HeaderText="Balance" DataFormatString="₹{0:N2}" />
                                        <asp:BoundField DataField="PaymentMode" HeaderText="Payment Mode" />
                                        <asp:BoundField DataField="Status" HeaderText="Status" />
                                    </Columns>
                                    <PagerStyle CssClass="pagination-ys" />
                                    <EmptyDataTemplate>
                                        <div class="text-center">No records found</div>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>