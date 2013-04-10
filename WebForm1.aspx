<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="verifyemailsolution.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form name="form1" id="form1" runat="server">
    <article>
        <p>        
            Get MX Record from EMail address, then verify with server.
        </p>

        <p>        
           
        </p>

        <p>        
            Email Address : <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
&nbsp;
            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Verify" />
         </p>
        <p>

             Return on EMail Address : <asp:Label ID="errorLabel" runat="server" ></asp:Label>&nbsp 
        </p>
        <p>        
            Domain Name : <asp:Label ID="domainLabel" runat="server" ></asp:Label>
        </p>
        <p>        
            MX Address : <asp:Label ID="mxLabel" runat="server" ></asp:Label>
        </p>
        <p>        
            &nbsp;</p>
        <p>        
            Results:
            <asp:Label ID="Label1" runat="server"  Width="885px"></asp:Label>
        </p>
        <p>        
            &nbsp;</p>
    </article>
    </form>
</body>
</html>
