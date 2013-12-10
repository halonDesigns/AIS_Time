<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="AIS_Time.Contact" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title">
        <h1><%: Title %>.</h1>
        <h2>Contact Us.</h2>
    </hgroup>

    <section class="contact">
        <header>
            <h3>Phone:</h3>
        </header>
        <p>
            <span class="label">Main:</span>
            <span>403.247.7666</span>
        </p>
        <p>
            <span class="label">After Hours:</span>
            <span>403.867.5309</span>
        </p>
    </section>

    <section class="contact">
        <header>
            <h3>Email:</h3>
        </header>
        <p>
            <span class="label">Support:</span>
            <span><a href="mailto:Support@athenaindustrial.com">Support@athenaindustrial.com</a></span>
        </p>
        <p>
            <span class="label">Marketing:</span>
            <span><a href="mailto:Marketing@athenaindustrial.com">Marketing@athenaindustrial.com</a></span>
        </p>
        <p>
            <span class="label">General:</span>
            <span><a href="mailto:General@athenaindustrial.com">General@athenaindustrial.com</a></span>
        </p>
    </section>

    <section class="contact">
        <header>
            <h3>Address:</h3>
        </header>
        <p>
            554 Hurricane Dr<br />
            Calgary, AB 
        </p>
    </section>
</asp:Content>