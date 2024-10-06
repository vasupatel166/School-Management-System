using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.SuperAdmin
{
    public partial class RegisterSchool : System.Web.UI.Page
    {

        private string connectionString = Global.ConnectionString;
        private string SelectedSchoolID = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadStates(); // Load states when the page loads for the first time
                LoadSchoolSearchDropdown(); // Load the school search dropdown
            }
        }

        private void LoadSchoolSearchDropdown()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT SchoolID, SchoolName FROM SchoolMaster", conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlSearchSchool.DataSource = reader;
                    ddlSearchSchool.DataTextField = "SchoolName"; // Display SchoolName in dropdown
                    ddlSearchSchool.DataValueField = "SchoolID"; // Use SchoolID as value
                    ddlSearchSchool.DataBind();
                }
            }
            ddlSearchSchool.Items.Insert(0, new ListItem("Select School", ""));
        }

        protected void ddlSearchSchool_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected SchoolID from the dropdown
            SelectedSchoolID = ddlSearchSchool.SelectedValue;

            if (!string.IsNullOrEmpty(SelectedSchoolID))
            {
                // Load the school details and fill the form fields
                LoadSchoolDetails(SelectedSchoolID);
            }
            else
            {
                // If no school is selected, reset the form
                ResetForm();
            }
        }

        private void LoadSchoolDetails(string schoolID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM SchoolMaster WHERE SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", schoolID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Populate form fields with data
                            txtSchoolName.Text = reader["SchoolName"].ToString();
                            txtPrincipalName.Text = reader["PrincipalName"].ToString();
                            txtSchoolAdd1.Text = reader["SchoolAddress1"].ToString();
                            txtSchoolAdd2.Text = reader["SchoolAddress2"].ToString();
                            txtPhoneNo.Text = reader["SchoolPhone"].ToString();
                            txtAlterPhoneNo.Text = reader["SchoolAlternatePhone"].ToString();
                            txtSchoolEmail.Text = reader["SchoolEmail"].ToString();
                            txtSchoolWebsite.Text = reader["SchoolWebsite"].ToString();
                            ddlSchoolType.SelectedValue = reader["SchoolType"].ToString();
                            ddlBoardAffiliation.SelectedValue = reader["BoardAffiliation"].ToString();
                            txtSchoolEstablishedYear.Text = reader["SchoolEstablishedYear"].ToString();
                            ddlSchoolCategory.SelectedValue = reader["SchoolCategory"].ToString();
                            chkIsActive.Checked = Convert.ToBoolean(reader["IsActive"]);

                            // Load location details (State, City, Pincode)
                            if (!reader.IsDBNull(reader.GetOrdinal("School_LocationID")))
                            {
                                int locationID = reader.GetInt32(reader.GetOrdinal("School_LocationID"));
                                LoadLocationDetails(locationID);
                            }
                        }
                    }
                }
            }
        }

        private void LoadLocationDetails(int locationID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM LocationMaster WHERE LocationID = @LocationID", conn))
                {
                    cmd.Parameters.AddWithValue("@LocationID", locationID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Populate location dropdowns
                            ddlCity.Enabled = true;
                            ddlPincode.Enabled = true;
                            ddlState.SelectedValue = reader["State"].ToString();
                            LoadCities(reader["State"].ToString());
                            ddlCity.SelectedValue = reader["City"].ToString();
                            LoadPincodes(reader["State"].ToString(), reader["City"].ToString());

                            // Check if the pincode exists in the ddlPincode dropdown list before setting the SelectedValue
                            string pincode = reader["Pincode"].ToString();
                            string location_id = reader["LocationID"].ToString();

                            // Ensure that the pincode is available in the dropdown
                            if (ddlPincode.Items.FindByValue(location_id) != null)
                            {
                                ddlPincode.SelectedValue = location_id;
                            }
                            else
                            {
                                // If the pincode is not available in the dropdown, add it dynamically or log the issue
                                ddlPincode.Items.Insert(0, new ListItem(location_id, pincode));
                                ddlPincode.SelectedValue = location_id;
                            }
                        }
                    }
                }
            }
        }

        private void LoadStates()
        {
            // Fetch all distinct states
            DataTable dtStates = GetLocationData(null, null);
            ddlState.DataSource = dtStates;
            ddlState.DataTextField = "State";
            ddlState.DataValueField = "State";
            ddlState.DataBind();
            ddlState.Items.Insert(0, new ListItem("Select State", ""));
        }

        private void LoadCities(string state)
        {
            // Fetch all cities for the selected state
            DataTable dtCities = GetLocationData(state, null);
            ddlCity.DataSource = dtCities;
            ddlCity.DataTextField = "City";
            ddlCity.DataValueField = "City";
            ddlCity.DataBind();
            ddlCity.Items.Insert(0, new ListItem("Select City", ""));
        }

        private void LoadPincodes(string state, string city)
        {
            // Fetch all pincodes for the selected state and city
            DataTable dtPincodes = GetLocationData(state, city);
            ddlPincode.DataSource = dtPincodes;
            ddlPincode.DataTextField = "Pincode";
            ddlPincode.DataValueField = "LocationID";
            ddlPincode.DataBind();
            ddlPincode.Items.Insert(0, new ListItem("Select Pincode", ""));
        }

        protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = ddlState.SelectedValue;

            if (!string.IsNullOrEmpty(selectedState))
            {
                // Load cities for the selected state and enable city dropdown
                LoadCities(selectedState);
                ddlCity.Enabled = true;

                // Reset and disable pincode dropdown
                ddlPincode.Items.Clear();
                ddlPincode.Items.Insert(0, new ListItem("Select Pincode", ""));
                ddlPincode.Enabled = false;
            }
            else
            {
                // If no state is selected, disable both city and pincode dropdowns
                ddlCity.Enabled = false;
                ddlCity.Items.Clear();
                ddlCity.Items.Insert(0, new ListItem("Select City", ""));

                ddlPincode.Enabled = false;
                ddlPincode.Items.Clear();
                ddlPincode.Items.Insert(0, new ListItem("Select Pincode", ""));
            }
        }

        protected void ddlCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = ddlState.SelectedValue;
            string selectedCity = ddlCity.SelectedValue;

            if (!string.IsNullOrEmpty(selectedCity))
            {
                // Load pincodes for the selected city and enable pincode dropdown
                LoadPincodes(selectedState, selectedCity);
                ddlPincode.Enabled = true;
            }
            else
            {
                // If no city is selected, disable pincode dropdown
                ddlPincode.Enabled = false;
                ddlPincode.Items.Clear();
                ddlPincode.Items.Insert(0, new ListItem("Select Pincode", ""));
            }
        }

        private DataTable GetLocationData(string state, string city)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetLocationDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@State", (object)state ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", (object)city ?? DBNull.Value);

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }
            return dt;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Gather all form data
            SelectedSchoolID = ddlSearchSchool.SelectedValue;
            string schoolName = txtSchoolName.Text.Trim();
            string principalName = txtPrincipalName.Text.Trim();
            string address1 = txtSchoolAdd1.Text.Trim();
            string address2 = txtSchoolAdd2.Text.Trim();
            int locationID = int.Parse(ddlPincode.SelectedValue);
            string phoneNo = txtPhoneNo.Text.Trim();
            string alternatePhoneNo = txtAlterPhoneNo.Text.Trim();
            string email = txtSchoolEmail.Text.Trim();
            string website = txtSchoolWebsite.Text.Trim();
            string schoolType = ddlSchoolType.SelectedValue;
            string boardAffiliation = ddlBoardAffiliation.SelectedValue;
            int establishedYear = string.IsNullOrEmpty(txtSchoolEstablishedYear.Text) ? 0 : int.Parse(txtSchoolEstablishedYear.Text);
            string schoolCategory = ddlSchoolCategory.SelectedValue;
            bool isActive = chkIsActive.Checked;

            System.Diagnostics.Debug.WriteLine("On Submit",SelectedSchoolID);

            // Define the procedure parameters
            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@SchoolID", string.IsNullOrEmpty(SelectedSchoolID) ? (object)DBNull.Value : SelectedSchoolID), // Pass NULL for new records
            new SqlParameter("@SchoolName", schoolName),
            new SqlParameter("@SchoolAddress1", address1),
            new SqlParameter("@SchoolAddress2", string.IsNullOrEmpty(address2) ? (object)DBNull.Value : address2),
            new SqlParameter("@School_LocationID", locationID),
            new SqlParameter("@SchoolPhone", phoneNo),
            new SqlParameter("@SchoolAlternatePhone", string.IsNullOrEmpty(alternatePhoneNo) ? (object)DBNull.Value : alternatePhoneNo),
            new SqlParameter("@SchoolEmail", email),
            new SqlParameter("@SchoolWebsite", string.IsNullOrEmpty(website) ? (object)DBNull.Value : website),
            new SqlParameter("@SchoolType", schoolType),
            new SqlParameter("@BoardAffiliation", string.IsNullOrEmpty(boardAffiliation) ? (object)DBNull.Value : boardAffiliation),
            new SqlParameter("@SchoolEstablishedYear", establishedYear == 0 ? (object)DBNull.Value : establishedYear),
            new SqlParameter("@PrincipalName", principalName),
            new SqlParameter("@SchoolCategory", string.IsNullOrEmpty(schoolCategory) ? (object)DBNull.Value : schoolCategory),
            new SqlParameter("@IsActive", isActive ? 1 : 0)
            };

            // Execute the stored procedure
            ExecuteInsertOrUpdate("InsertUpdateSchoolMaster", parameters);
            
            Response.Redirect("~/SuperAdmin/RegisterSchool.aspx");

            // Reset the form after successful insertion
            ResetForm();
        }
        
        // Function to execute the insert or update stored procedure
        private void ExecuteInsertOrUpdate(string procedureName, SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(procedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add all parameters to the command
                    cmd.Parameters.AddRange(parameters);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery(); // Execute the stored procedure
                        ClientScript.RegisterStartupScript(this.GetType(), "Success", "alert('School details have been saved successfully.');", true);
                    }
                    catch (SqlException ex)
                    {
                        string errorDetails = $"SQL Error: {ex.Number} - {ex.Message} at Line {ex.LineNumber}";
                        System.Diagnostics.Debug.WriteLine(errorDetails);
                    }
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm(); // Reset the form fields when cancel button is clicked
        }

        private void ResetForm()
        {
            // Reset all input fields
            ddlSearchSchool.SelectedIndex = 0;
            txtSchoolName.Text = string.Empty;
            txtPrincipalName.Text = string.Empty;
            txtSchoolAdd1.Text = string.Empty;
            txtSchoolAdd2.Text = string.Empty;
            ddlState.SelectedIndex = 0;
            ddlCity.Items.Clear();
            ddlCity.Items.Insert(0, new ListItem("Select City", ""));
            ddlCity.Enabled = false;

            ddlPincode.Items.Clear();
            ddlPincode.Items.Insert(0, new ListItem("Select Pincode", ""));
            ddlPincode.Enabled = false;

            txtPhoneNo.Text = string.Empty;
            txtAlterPhoneNo.Text = string.Empty;
            txtSchoolEmail.Text = string.Empty;
            txtSchoolWebsite.Text = string.Empty;
            ddlSchoolType.SelectedIndex = 0;
            ddlBoardAffiliation.SelectedIndex = 0;
            txtSchoolEstablishedYear.Text = string.Empty;
            ddlSchoolCategory.SelectedIndex = 0;
            chkIsActive.Checked = true;

            // Reset any validation messages or error labels (if applicable)
            Page.Validators.OfType<BaseValidator>().ToList().ForEach(validator => validator.IsValid = true);

        }

        
    }
}