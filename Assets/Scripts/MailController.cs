using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailController : MonoBehaviour
{
    public string SendToAddress = "oliversimonedward@hotmail.co.uk";


    public void SendEmail()
    {
        string email = SendToAddress;
        string subject = MyEscapeURL("Feedback");
        string body = MyEscapeURL("Feedback for presenter:");

        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }
    string MyEscapeURL(string URL)
    {
        return WWW.EscapeURL(URL).Replace("+", "%20");
    }
}
