<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UpcomingEvents.aspx.cs" Inherits="Schoolnest.Student.UpcomingEvents" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
    #eventContainer {
        display: flex;
        flex-wrap: wrap;
        justify-content: center;
    }

    .event-card {
        background-color: #f8f9fa;
        border: 1px solid #dee2e6;
        border-radius: 8px;
        padding: 16px;
        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        display: flex;
        flex-direction: column;
        align-items: center;
        text-align: center;
        transition: transform 0.3s ease, box-shadow 0.3s ease;
        width: 100%;
    }

    .event-card:hover {
        transform: translateY(-5px);
        box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
    }

    .event-card h4 {
        font-size: 1.2rem;
        margin-bottom: 8px;
        color: #007bff;
    }

    .event-card p {
        margin: 0;
        font-size: 0.9rem;
        color: #6c757d;
    }

    .event-card .event-date-time {
        font-weight: bold;
        margin-top: 10px;
        font-size: 1rem;
        color: #495057;
    }

    .no-events {
        text-align: center;
        font-size: 1.2rem;
        color: #6c757d;
        margin-top: 20px;
    }
</style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form class="w-100" id="form1" runat="server">

    <div class="row">
        <div class="col-md-12">
            <h2 class="mb-4 text-center">Upcoming Events</h2>
            
            <div class="row" id="eventContainer" runat="server">
                <!-- Dynamic Event Cards will be added here by the backend -->
            </div>
        </div>
    </div>
    </form>
</asp:Content>

