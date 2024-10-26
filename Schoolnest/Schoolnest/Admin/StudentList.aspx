<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StudentList.aspx.cs" Inherits="Schoolnest.Admin.StudentList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server" class="container mt-4">
        <div class="col-md-12">
			<div class="card mb-0">
				<div class="card-header d-flex justify-content-between align-items-center">
					<div class="card-title">Student Entry Form</div>
					<div class="form-group mb-0">
						<asp:DropDownList ID="ddlStudents" runat="server" AutoPostBack="true" CssClass="form-control">

						</asp:DropDownList>
					</div>
				</div>
				<div class="card-body">
					<ul class="nav nav-pills nav-black" id="pills-tab" role="tablist">
						<li class="nav-item">
							<a class="nav-link active" id="pills-personal-tab" data-bs-toggle="pill" href="#pills-personal" role="tab" aria-controls="pills-personal" aria-selected="true">Personal Information</a>
						</li>
						<li class="nav-item">
							<a class="nav-link" id="pills-academic-tab" runat="server" data-bs-toggle="pill" href="#pills-academic" role="tab" aria-controls="pills-academic" aria-selected="false">Academic Information</a>
						</li>
						<li class="nav-item">
							<a class="nav-link" id="pills-contact-tab" runat="server" data-bs-toggle="pill" href="#pills-contact" role="tab" aria-controls="pills-contact" aria-selected="false">Contact Information</a>
						</li>
						<li class="nav-item">
							<a class="nav-link" id="pills-additional-tab" runat="server" data-bs-toggle="pill" href="#pills-additional" role="tab" aria-controls="pills-additional" aria-selected="false">Additional Information</a>
						</li>
					</ul>
					<div class="tab-content mt-2 mb-3 p-2" id="pills-tabContent">
						<%--Personal Information--%>
						<div class="tab-pane fade show active" id="pills-personal" role="tabpanel" aria-labelledby="pills-personal-tab">
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
									 <asp:Label runat="server" AssociatedControlID="txtDateOfBirth" Text="Date of Birth"></asp:Label>
									 <asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
									 <asp:RequiredFieldValidator ID="rfvDOB" runat="server" ControlToValidate="txtDateOfBirth" ErrorMessage="Date of Birth is required." ForeColor="Red"></asp:RequiredFieldValidator>
								 </div>
								 <div class="col-md-4 form-group">
									 <asp:Label runat="server" AssociatedControlID="txtPlaceOfBirth" Text="Place of Birth"></asp:Label>
									 <asp:TextBox ID="txtPlaceOfBirth" runat="server" CssClass="form-control"></asp:TextBox>
								 </div>
							</div>
							<div class="row mb-4">
								<div class="col-md-4 form-group">
									<asp:Label runat="server" AssociatedControlID="ddlGender" Text="Gender"></asp:Label>
									<asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control">
										<asp:ListItem Value="">Select Gender</asp:ListItem>
										<asp:ListItem Value="M">Male</asp:ListItem>
										<asp:ListItem Value="F">Female</asp:ListItem>
										<asp:ListItem Value="O">Other</asp:ListItem>
									</asp:DropDownList>
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
							</div>
							<div class="row mb-4">
								<div class="col-md-4 form-group">
									<asp:Label runat="server" AssociatedControlID="txtMotherTongue" Text="Mother Tongue"></asp:Label>
									<asp:TextBox ID="txtMotherTongue" runat="server" CssClass="form-control"></asp:TextBox>
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
                                    <asp:DropDownList ID="ddlBusRoute" runat="server" CssClass="form-control" AutoPostBack="True"></asp:DropDownList>
								</div>								
							</div>
							<div class="row">
								<div class="col-md-4 form-group">
									<asp:Label runat="server" AssociatedControlID="fileProfileImage" Text="Profile Image (optional)"></asp:Label>
									<asp:FileUpload ID="fileProfileImage" runat="server" CssClass="form-control" />
								</div>
								<div class="col-md-4">
									<div class="form-check mt-2">
										 <asp:CheckBox
												 ID="chkIsActive"
												 runat="server"
												 CssClass="form-check-input border-0"
												 Checked="true"
												 AutoPostBack="true"
											 />
										 <label class="form-check-label" for="flexCheckDefault">
										 Active
										 </label>
									</div>
								</div>					
							</div>
						</div>

						<%--Academic Information--%>
						<div class="tab-pane fade" id="pills-academic" role="tabpanel" aria-labelledby="pills-academic-tab">
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
							</div>
						</div>

						<%--Contact Information--%>
						<div class="tab-pane fade" id="pills-contact" role="tabpanel" aria-labelledby="pills-contact-tab">
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
								 <div class="col-md-6 form-group">
									 <asp:Label runat="server" AssociatedControlID="txtAddress1" Text="Address Line 1"></asp:Label>
									 <asp:TextBox ID="txtAddress1" runat="server" CssClass="form-control"></asp:TextBox>
								 </div>
								 <div class="col-md-6 form-group">
									 <asp:Label runat="server" AssociatedControlID="txtAddress2" Text="Address Line 2"></asp:Label>
									 <asp:TextBox ID="txtAddress2" runat="server" CssClass="form-control"></asp:TextBox>
								 </div>
							 </div>
							 <div class="row">
								 <div class="col-md-4 form-group">
									<asp:Label runat="server" AssociatedControlID="ddlState" Text="State"></asp:Label>
									<asp:DropDownList ID="ddlState" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlState_SelectedIndexChanged">
									</asp:DropDownList>
									<asp:RequiredFieldValidator ID="rfvState" runat="server" ControlToValidate="ddlState" Display="Dynamic" ErrorMessage="State is required" CssClass="text-danger"></asp:RequiredFieldValidator>
								 </div>
								 <div class="col-md-4 form-group">
									<asp:Label runat="server" AssociatedControlID="ddlCity" Text="City"></asp:Label>
									<asp:DropDownList ID="ddlCity" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCity_SelectedIndexChanged" Enabled="false">
									</asp:DropDownList>
									<asp:RequiredFieldValidator ID="rfvCity" runat="server" ControlToValidate="ddlCity" Display="Dynamic" ErrorMessage="City is required" CssClass="text-danger"></asp:RequiredFieldValidator>
								 </div>
								 <div class="col-md-4 form-group">
									<asp:Label runat="server" AssociatedControlID="ddlPincode" Text="Pincode"></asp:Label>
									<asp:DropDownList ID="ddlPincode" runat="server" CssClass="form-control" Enabled="false">
									</asp:DropDownList>
									<asp:RequiredFieldValidator ID="rfvPincode" runat="server" ControlToValidate="ddlPincode" Display="Dynamic" ErrorMessage="Pincode is required" CssClass="text-danger"></asp:RequiredFieldValidator>
								 </div>
							 </div>
						</div>

						<%--Additional Information--%>
						<div class="tab-pane fade" id="pills-additional" role="tabpanel" aria-labelledby="pills-additional-tab">
							<div class="row">
								<div class="col-md-6 form-group">
									<asp:Label runat="server" AssociatedControlID="txtFatherName" Text="Father's Name"></asp:Label>
									<asp:TextBox ID="txtFatherName" runat="server" CssClass="form-control"></asp:TextBox>
								</div>
								<div class="col-md-6 form-group">
									<asp:Label runat="server" AssociatedControlID="txtMotherName" Text="Mother's Name"></asp:Label>
									<asp:TextBox ID="txtMotherName" runat="server" CssClass="form-control"></asp:TextBox>
								</div>
								<div class="col-md-4 form-group">
									<div class="form-check mt-2">
										 <asp:CheckBox ID="chkLastSchoolAttended" runat="server" CssClass="form-check-input border-0" OnCheckedChanged="chkLastSchoolAttended_CheckedChanged" Checked="false" AutoPostBack="true"/>
										 <label class="form-check-label" for="chkLastSchoolAttended">
											Last School Attended
										 </label>
									</div>
								</div>
							</div>
							<div id="lastSchoolDetails" runat="server" visible="false">
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
						</div>
					</div>

					<div class="card-footer text-center mt-4 pt-4">
						<asp:Button ID="btnSubmit" runat="server" Text="Save" CssClass="btn btn-success" OnClick="btnSubmit_Click" />
						<asp:Button ID="btnNext" runat="server" Text="Next" CssClass="btn btn-success" OnClick="btnNext_Click" />
						<asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" CausesValidation="false" OnClick="btnCancel_Click" />
					</div>
				</div>
			</div>
		</div>
    </form>
</asp:Content>
