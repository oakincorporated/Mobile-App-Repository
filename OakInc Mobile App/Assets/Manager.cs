using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Manager : MonoBehaviour
{
    public GameObject homePage;
    public GameObject loginPage;
    public GameObject attendancePage;
    public GameObject reportLostStickerPage;
    public GameObject incorrectCredentials;

    TextMeshProUGUI usernameEntered;
    TextMeshProUGUI passwordEntered;

    public GameObject buttonsParent;


    public string usernameEnteredText;
    public string passwordEnteredText;

    GameObject currentGameObject;
    bool loginCreds;

    public string[] users;
    public string[] usersLoginData;

    public string thisFirstName, thisSurname, thisCourse, thisSsid, thisClassId, thisUsername;

    public string thisSession, thisAttended, thisAbsentFrom, thisLateFrom;

    int UserNumber;

    TextMeshProUGUI NameText;
    TextMeshProUGUI CourseText;
    TextMeshProUGUI SSIDText;
    TextMeshProUGUI totalSessionText;
    TextMeshProUGUI attendedText;
    TextMeshProUGUI absentFromText;
    TextMeshProUGUI lateFromText;

    Transform nameObject;
    Transform courseObject;
    Transform ssidObject;
    Transform totalSessionObject;
    Transform attendedObject;
    Transform absentFromObject;
    Transform lateFromObject;

    string usernameCheck;
    string passwordCheck;

    public int userNumber;


    // - - //


    // Start is called before the first frame update
    void Start()
    {
        // Turns off the buttons on app start
        buttonsParent.SetActive(false);
        //Sets the current gameobject to be the login page.
        currentGameObject = loginPage;

        //StartCoroutine(LoadData());
        StartCoroutine(LoadLoginData());
    }



    // Check to see if the user's credentials are correct
    public void Login()
    {
        // Get the entered username and password
        GetEnteredData();

        // If the user has entered valid credentials then...
        if (loginCheck())
        {
            // Turn off the incorrect credentials text.
            incorrectCredentials.SetActive(false);

            // Move the user to the homepage
            currentGameObject.SetActive(false);
            homePage.SetActive(true);
            currentGameObject = homePage;

            // Load the data for the logged in user.
            StartCoroutine(LoadData(usernameEnteredText));
            // Load the attendance data for the logged in user.
            //StartCoroutine(LoadAttendance(thisSsid));

            // Turn on the navigation buttons (these are off whe trying to log in)
            buttonsParent.SetActive(true);
        }
        else
        {
            incorrectCredentials.SetActive(true);
        }
        
    }

    void GetEnteredData()
    {
        Transform usernameObject;
        Transform passwordObject;

        usernameObject = loginPage.transform.Find("Info/EnterUsername");
        passwordObject = loginPage.transform.Find("Info/EnterPassword");

        usernameEnteredText = usernameObject.GetComponent<TMP_InputField>().text;
        passwordEnteredText = passwordObject.GetComponent<TMP_InputField>().text;
    }

    bool loginCheck()
    {
        bool usernameCheckBool = false;
        bool passwordCheckBool = false;
        

        Debug.Log(usernameEnteredText);
        Debug.Log(passwordEnteredText);

        //Gets each of the usernames of each user
        for (int i = 0; i < (usersLoginData.Length-1); i++)
        {
            usernameCheck = GetThisUserData(usersLoginData[i], "Username:");
            passwordCheck = GetThisUserData(usersLoginData[i], "Password:");

            if(usernameEnteredText == usernameCheck)
            {
                usernameCheckBool = true;
            }
            else
            {
                usernameCheckBool = false;
            }

            if(passwordEnteredText == passwordCheck)
            {
                passwordCheckBool = true;
            }
            else
            {
                passwordCheckBool = false;
            }
            if (passwordCheckBool && usernameCheckBool)
            {
                return true;
            }
        }
        return false;

        
    }

    IEnumerator LoadLoginData()
    {
        // Load the data from the php page on our cloud site with SSL encryption.
        WWW userLoginData = new WWW("https://d-walsh.co.uk/oakinc/logindata.php");
        yield return userLoginData;

        string userLoginDataString = userLoginData.text;
        usersLoginData = userLoginDataString.Split(';');

    }

    IEnumerator LoadData(string username)
    {
        // Load the data from the php page on our cloud site with SSL encryption.
        WWW userData = new WWW("https://d-walsh.co.uk/oakinc/userdata.php");
        yield return userData;

        string userDataString = userData.text;

        users = userDataString.Split(';');

        // Loop until you find the block of data with the logged in user's username in it.
        for (int i = 0; i < users.Length; i++)
        {
            if(username == GetThisUserData(users[i], "Username:"))
            {
                thisFirstName = GetThisUserData(users[i], "First name:");
                thisSsid = GetThisUserData(users[i], "SSID:");
                thisCourse = GetThisUserData(users[i], "Course:");
                thisClassId = GetThisUserData(users[i], "Class_ID:");
                thisUsername = GetThisUserData(users[i], "Username:");
                userNumber = i;
                break;
            }
        }

        // Sets the text on screen to be the logged in user's data.
        SetHomepageText(thisFirstName, thisCourse, thisSsid);

        StartCoroutine(LoadAttendance(thisSsid));
    }

    IEnumerator LoadAttendance(string ssid)
    {
        // Load the data from the php page on our cloud site with SSL encryption.
        WWW attendanceData = new WWW("https://d-walsh.co.uk/oakinc/attendancedata.php");
        yield return attendanceData;

        string userAttendanceDataString = attendanceData.text;

        users = userAttendanceDataString.Split(';');

        // Loop until you find the block of data with the logged in user's username in it.
        for (int i = 0; i < users.Length; i++)
        {
            if (ssid == GetThisUserData(users[i], "StudentID:"))
            {
                //thisSession = GetThisUserData(users[i], "First name:");
                thisSession = "1"; // code for this does not work yet as we have not got the total sessions working in our sql database yet.
                thisAttended = GetThisUserData(users[i], "Present:");
                thisAbsentFrom = GetThisUserData(users[i], "Absent:");
                thisLateFrom = GetThisUserData(users[i], "Late:");
                break;
            }
        }

        // Sets the text on screen to be the logged in user's data.
        SetAttendanceText(thisSession, thisAttended, thisAbsentFrom, thisLateFrom);

    }

    string GetThisUserData(string data, string index)
    {
        string value = data.Substring(data.IndexOf(index)+index.Length);
        if (value.Contains("|"))
        {
            value = value.Remove(value.IndexOf("|"));
        }
        return value;
    }



    void SetAttendanceText(string totalSession, string attended, string absentFrom, string lateFrom)
    {

        Debug.Log("Setting text");
        totalSessionObject = attendancePage.transform.Find("Info/TotalSession");
        attendedObject = attendancePage.transform.Find("Info/Attended");
        absentFromObject = attendancePage.transform.Find("Info/AbsentFrom");
        lateFromObject = attendancePage.transform.Find("Info/LateFrom");


        totalSessionText = totalSessionObject.GetComponent<TextMeshProUGUI>();
        attendedText = attendedObject.GetComponent<TextMeshProUGUI>();
        absentFromText = absentFromObject.GetComponent<TextMeshProUGUI>();
        lateFromText = lateFromObject.GetComponent<TextMeshProUGUI>();

        totalSessionText.text = "Total Sessions: " + totalSession;
        attendedText.text = "Attended Sessions: "+attended;
        absentFromText.text = "Absent from(%): " + absentFrom;
        lateFromText.text = "Late from(%): " + lateFrom;

    }

    void SetHomepageText(string name, string course, string ssid)
    {
        nameObject = homePage.transform.Find("Info/Name");
        courseObject = homePage.transform.Find("Info/Course");
        ssidObject = homePage.transform.Find("Info/SSID");

        NameText = nameObject.GetComponent<TextMeshProUGUI>();
        CourseText = courseObject.GetComponent<TextMeshProUGUI>();
        SSIDText = ssidObject.GetComponent<TextMeshProUGUI>();

        NameText.text = "Hi " + name;
        CourseText.text = course;
        SSIDText.text = "SSID: " + ssid;
    }



    public void HomepageButton()
    {
        // Turns off the current gameobject and turns on the homepage one.

        currentGameObject.SetActive(false);
        homePage.SetActive(true);
        currentGameObject = homePage;
    }

    public void AttendanceButton()
    {
        // Turns off the current gameobject and turns on the attendance one.

        currentGameObject.SetActive(false);
        attendancePage.SetActive(true);
        currentGameObject = attendancePage;
    }

    public void ReportLostStickerButton()
    {
        // Turns off the current gameobject and turns on the report lost sticker one.

        currentGameObject.SetActive(false);
        reportLostStickerPage.SetActive(true);
        currentGameObject = reportLostStickerPage;
    }
}
