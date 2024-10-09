<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StudentList.aspx.cs" Inherits="Schoolnest.Admin.StudentList" OnInit="Page_Init" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/select2/4.0.13/css/select2.min.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/select2/4.0.13/js/select2.min.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="container mt-4">
        <div class="card">
            <div class="card-header">
                <h3 class="card-title">Student Entry Form</h3>
            </div>
            <div class="card-body">
                <ul class="nav nav-tabs" id="myTab" role="tablist">
                    <li class="nav-item">
                        <a class="nav-link active" id="personal-tab" data-toggle="tab" href="#personal" role="tab">Personal Information</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" id="academic-tab" data-toggle="tab" href="#academic" role="tab">Academic Information</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" id="contact-tab" data-toggle="tab" href="#contact" role="tab">Contact Information</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" id="additional-tab" data-toggle="tab" href="#additional" role="tab">Additional Information</a>
                    </li>
                </ul>
                <div class="tab-content mt-3" id="myTabContent">
                    <div class="tab-pane fade show active" id="personal" role="tabpanel">
                            <div class="col-md-6 form-group">
                                <asp:DropDownList ID="ddlStudents" runat="server" AutoPostBack="true" CssClass="form-control select2" OnSelectedIndexChanged="ddlStudents_SelectedIndexChanged"></asp:DropDownList>
                            </div>

                        <div class="row">
                           
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtStudentID" Text="Student ID"></asp:Label>
                                <asp:TextBox ID="txtStudentID" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                            </div>
                          <div class="row">
                              <div class="col-md-4 form-group">
                                  <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name"></asp:Label>
                                  <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" AutoPostBack="True" OnTextChanged="txtName_TextChanged"></asp:TextBox>
                                  <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" ErrorMessage="First Name is required." ForeColor="Red"></asp:RequiredFieldValidator>
                              </div>
                              <div class="col-md-4 form-group">
                                  <asp:Label runat="server" AssociatedControlID="txtMiddleName" Text="Middle Name"></asp:Label>
                                  <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-control" AutoPostBack="True" OnTextChanged="txtName_TextChanged" ></asp:TextBox>
                              </div>
                              <div class="col-md-4 form-group">
                                  <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name"></asp:Label>
                                  <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" AutoPostBack="True" OnTextChanged="txtName_TextChanged" ></asp:TextBox>
                                  <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" ErrorMessage="Last Name is required." ForeColor="Red"></asp:RequiredFieldValidator>
                              </div>
                          </div>
                               <div class="row">
                              <div class="col-md-4 form-group">
                                  <asp:Label runat="server" AssociatedControlID="txtFullName" Text="Full Name"></asp:Label>
                                  <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" ReadOnly="True" ></asp:TextBox>
                              </div>

                                   <div class="col-md-4 form-group">
                                       <asp:Label runat="server" AssociatedControlID="ddlGender" Text="Gender"></asp:Label>
                                       <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control">
                                           <asp:ListItem Value="">Select Gender</asp:ListItem>
                                           <asp:ListItem Value="M">Male</asp:ListItem>
                                           <asp:ListItem Value="F">Female</asp:ListItem>
                                           <asp:ListItem Value="O">Other</asp:ListItem>
                                       </asp:DropDownList>
                                   </div>
                          </div>
                            
                        </div>
                        <div class="row">
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtDateOfBirth" Text="Date of Birth"></asp:Label>
                                <asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvDOB" runat="server" ControlToValidate="txtDateOfBirth" ErrorMessage="Date of Birth is required." ForeColor="Red"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtPlaceOfBirth" Text="Place of Birth"></asp:Label>
                                <asp:TextBox ID="txtPlaceOfBirth" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="ddlReligion" Text="Religion"></asp:Label>
                                <asp:DropDownList ID="ddlReligion" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="--Select Religion--" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Hindu" Value="H"></asp:ListItem>
                                    <asp:ListItem Text="Muslim" Value="M"></asp:ListItem>
                                    <asp:ListItem Text="Christian" Value="C"></asp:ListItem>
                                    <asp:ListItem Text="Sikh" Value="S"></asp:ListItem>
                                    <asp:ListItem Text="Buddhist" Value="B"></asp:ListItem>
                                    <asp:ListItem Text="Jain" Value="J"></asp:ListItem>
                                    <asp:ListItem Text="Other" Value="O"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="ddlCaste" Text="Caste"></asp:Label>
                                <asp:DropDownList ID="ddlCaste" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="Select Caste" Value=""></asp:ListItem>
                                    <asp:ListItem Text="General" Value="G"></asp:ListItem>
                                    <asp:ListItem Text="OBC" Value="OBC"></asp:ListItem>
                                    <asp:ListItem Text="SC" Value="SC"></asp:ListItem>
                                    <asp:ListItem Text="ST" Value="ST"></asp:ListItem>
                                    <asp:ListItem Text="Other" Value="O"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="ddlBloodGroup" Text="Blood Group"></asp:Label>
                                <asp:DropDownList ID="ddlBloodGroup" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="--Select Blood Group--" Value=""></asp:ListItem>
                                    <asp:ListItem Text="A+" Value="A+"></asp:ListItem>
                                    <asp:ListItem Text="A-" Value="A-"></asp:ListItem>
                                    <asp:ListItem Text="B+" Value="B+"></asp:ListItem>
                                    <asp:ListItem Text="B-" Value="B-"></asp:ListItem>
                                    <asp:ListItem Text="AB+" Value="AB+"></asp:ListItem>
                                    <asp:ListItem Text="AB-" Value="AB-"></asp:ListItem>
                                    <asp:ListItem Text="O+" Value="O+"></asp:ListItem>
                                    <asp:ListItem Text="O-" Value="O-"></asp:ListItem>
                                    <asp:ListItem Text="Unknown" Value="U"></asp:ListItem>
                                </asp:DropDownList>
                            </div>

                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="ddlBusRoute" Text="Bus Route"></asp:Label>
                                  <asp:DropDownList ID="ddlBusRoute" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>

                        <!-- Image Upload Section -->
                        <div class="row">
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="fileUploadStudentImage" Text="Upload Student Image"></asp:Label>
                                <asp:FileUpload ID="fileUploadStudentImage" runat="server" CssClass="form-control" />
                                <asp:Button ID="btnUploadImage" runat="server" Text="Upload" CssClass="btn btn-primary" OnClick="btnUploadImage_Click" CausesValidation="False" />
                                <asp:Button ID="btnDeleteImage" runat="server" Text="Delete Image" CssClass="btn btn-danger" OnClick="btnDeleteImage_Click" Visible="false" CausesValidation="False" />
                                <asp:HiddenField ID="hfFilePath" runat="server" />
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Image ID="imgStudent" runat="server" CssClass="img-thumbnail" Height="150" Width="150" Visible="false" />
                            </div>
                        </div>
                        <!-- Tab Navigation Buttons -->
                        <div class="row mt-3">
                            <div class="col-md-12 text-right">
                                <button type="button" class="btn btn-primary" onclick="$('#academic-tab').tab('show')">Next</button>
                            </div>
                        </div>
                    </div>
                    <div class="tab-pane fade" id="academic" role="tabpanel">
                        <div class="row">
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="ddlStandard" Text="Standard"></asp:Label>
                                <asp:DropDownList ID="ddlStandard" runat="server" CssClass="form-control"></asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddlStandard" ErrorMessage="Please Select Standard." ForeColor="Red"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="ddlDivision" Text="Division"></asp:Label>
                                <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-control" ></asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvDivision" runat="server" ControlToValidate="ddlDivision" ErrorMessage="Please Select Division." ForeColor="Red"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="ddlSection" Text="Section"></asp:Label>
                                <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-control" ></asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvSection" runat="server" ControlToValidate="ddlSection" ErrorMessage="Please Select Section." ForeColor="Red"></asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtGRNumber" Text="GR Number"></asp:Label>
                                <asp:TextBox ID="txtGRNumber" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvGrNumber" runat="server" ControlToValidate="txtGrNumber" ErrorMessage="GR Number is required." ForeColor="Red"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revGrNumber" runat="server" ControlToValidate="txtGrNumber" ErrorMessage="GR Number must be numeric and max length 10 digits." ValidationExpression="^\d{1,10}$" ForeColor="Red"></asp:RegularExpressionValidator>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtDateOfAdmission" Text="Date of Admission"></asp:Label>
                                <asp:TextBox ID="txtDateOfAdmission" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvAdmissionDate" runat="server" ControlToValidate="txtDateOfAdmission" ErrorMessage="Date of Admission is required." ForeColor="Red"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="ddlStatus" Text="Status"></asp:Label>
                                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">Select Status</asp:ListItem>
                                    <asp:ListItem Value="A">Active</asp:ListItem>
                                    <asp:ListItem Value="I">Inactive</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtLastSchoolAttended" Text="Last School Attended"></asp:Label>
                                <asp:TextBox ID="txtLastSchoolAttended" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtMotherTongue" Text="Mother Tongue"></asp:Label>
                                <asp:TextBox ID="txtMotherTongue" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <!-- Tab Navigation Buttons -->
                        <div class="row mt-3">
                            <div class="col-md-6">
                                <button type="button" class="btn btn-secondary" onclick="$('#personal-tab').tab('show')">Previous</button>
                            </div>
                            <div class="col-md-6 text-right">
                                <button type="button" class="btn btn-primary" onclick="$('#contact-tab').tab('show')">Next</button>
                            </div>
                        </div>
                    </div>
                    <div class="tab-pane fade" id="contact" role="tabpanel">
                        <div class="row">
                            <div class="col-md-6 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtEmailID" Text="Email ID"></asp:Label>
                                <asp:TextBox ID="txtEmailID" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmailID" ErrorMessage="Email ID is required." ForeColor="Red"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmailID" ErrorMessage="Invalid Email ID." ValidationExpression="^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$" ForeColor="Red"></asp:RegularExpressionValidator>
                            </div>
                            <div class="col-md-6 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtMobileNumber" Text="Mobile Number"></asp:Label>
                                <asp:TextBox ID="txtMobileNumber" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvMobile" runat="server" ControlToValidate="txtMobileNumber" ErrorMessage="Mobile number is required." ForeColor="Red"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revMobile" runat="server" ControlToValidate="txtMobileNumber" ErrorMessage="Mobile number must be 10 digits." ValidationExpression="^\d{10}$" ForeColor="Red"></asp:RegularExpressionValidator>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtAddress1" Text="Address Line 1"></asp:Label>
                                <asp:TextBox ID="txtAddress1" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtAddress2" Text="Address Line 2"></asp:Label>
                                <asp:TextBox ID="txtAddress2" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtAddress3" Text="Address Line 3"></asp:Label>
                                <asp:TextBox ID="txtAddress3" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <!-- Tab Navigation Buttons -->
                        <div class="row mt-3">
                            <div class="col-md-6">
                                <button type="button" class="btn btn-secondary" onclick="$('#academic-tab').tab('show')">Previous</button>
                            </div>
                            <div class="col-md-6 text-right">
                               <button type="button" class="btn btn-primary" onclick="$('#additional-tab').tab('show')">Next</button>
                            </div>
                        </div>
                    </div>
                    <div class="tab-pane fade" id="additional" role="tabpanel">
                        <div class="row">
                            <div class="col-md-6 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtFatherName" Text="Father's Name"></asp:Label>
                                <asp:TextBox ID="txtFatherName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-6 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtMotherName" Text="Mother's Name"></asp:Label>
                                <asp:TextBox ID="txtMotherName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 form-group">
                                <asp:CheckBox ID="chkLastSchoolAttended" runat="server" Text="Last School Attended" AutoPostBack="true" OnCheckedChanged="chkLastSchoolAttended_CheckedChanged" />
                            </div>
                        </div>
                        <div id="lastSchoolDetails" runat="server" style="display: none;">
                            <div class="row">
                                <div class="col-md-6 form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtLastSchoolName" Text="Last School Name"></asp:Label>
                                    <asp:TextBox ID="txtLastSchoolName" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6 form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtLastSchoolAddress" Text="Last School Address"></asp:Label>
                                    <asp:TextBox ID="txtLastSchoolAddress" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12 form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtLastSchoolRemarks" Text="Last School Remarks"></asp:Label>
                                    <asp:TextBox ID="txtLastSchoolRemarks" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtLastYearPercentage" Text="Last Year Percentage"></asp:Label>
                                <asp:TextBox ID="txtLastYearPercentage" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtGrade" Text="Grade"></asp:Label>
                                <asp:TextBox ID="txtGrade" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-4 form-group">
                                <asp:Label runat="server" AssociatedControlID="txtRemarks" Text="Remarks"></asp:Label>
                                <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         <!-- Tab Navigation Button -->
                        <div class="row mt-3">
                            <div class="col-md-6">
                                <button type="button" class="btn btn-secondary" onclick="$('#contact-tab').tab('show')">Previous</button>
                                <asp:Button ID="btnSubmit" runat="server" Text="Save" CssClass="btn btn-secondary" OnClick="btnSubmit_Click"  />
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClick="btnCancel_Click" CausesValidation="False" />
                                <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-secondary" OnClick="btnReset_Click" CausesValidation="False"  />
                                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-secondary" OnClick="btnSearch_Click" CausesValidation="False"/>
                            </div>
                        </div>
                    </div>
                </div>
          
        </div>
    </form>
   <script>
       $(document).ready(function () {
           $('.nav-tabs a').on('click', function (e) {
               e.preventDefault();
               $(this).tab('show');
           });

           // Initialize the Select2 plugin on the dropdown
           $('.select2').select2({
               placeholder: "-- Select Student --",
               allowClear: true
           });

           // Capture the dropdown change event to show student details
           $('#<%= ddlStudents.ClientID %>').change(function () {
                var selectedValue = $(this).val();
                if (selectedValue) {
                    // Trigger the postback to load the student details
                    __doPostBack('<%= ddlStudents.UniqueID %>', '');
                }
            });
       });

        
   </script>

</asp:Content>
