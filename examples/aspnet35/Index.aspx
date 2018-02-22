<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Bugsnag.Sample.AspNet35.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
          <h1>Bugsnag ASP.NET Sample Application!</h1>
          <asp:Button runat="server" ID="Button1" onClick="Unhandled" Text="Unhandled Exception" />
          <asp:Button runat="server" ID="Button2" onClick="Handled" Text="Handled Exception" />
        </div>
    </form>
</body>
</html>
