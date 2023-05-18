using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Mmi
{

public class MmiService : MonoBehaviour
{
     [DllImport("mmi64")]
    private static extern void mmi_start();

    [DllImport("mmi64")]
    private static extern void mmi_stop();

    [DllImport("mmi64")]
    private static extern void mmi_subscribe(StringBuilder str);

    [DllImport("mmi64")]
    private static extern void mmi_unsubscribe(StringBuilder str);

    [DllImport("mmi64")]
    private static extern int mmi_get_message(StringBuilder str, int str_len);

    [DllImport("mmi64")]
    private static extern void mmi_configure(StringBuilder key, StringBuilder value, int mp_type);

    enum ModalityProviderType : int
    {
        NONE = 0,
        VOICE = 10,
        WATCH = 20,
        HAND = 30,
        POSE = 40,
        FILE = 50,
        TOTAL_NUM
    };

    void mmi_subscribe(string str)
    {
        mmi_subscribe(new StringBuilder(str));
    }

    void mmi_unsubscribe(string str)
    {
        mmi_unsubscribe(new StringBuilder(str));
    }

    void mmi_configure(string key, string value, ModalityProviderType mp_type)
    {
        mmi_configure(new StringBuilder(key), new StringBuilder(value), (int)mp_type);
    }


    /*
     * ========================================================================
     */


    const string subscription_1 = "[" +
        "{ \"id\":1002, \"intent\":\"CLOSE\" }," +
        "{ \"id\":1003, \"intent\":\"DROP\" }," +
        "{ \"id\":1004, \"intent\":\"GRAB\" }," +
        "{ \"id\":1005, \"intent\":\"IGNORE\" }," +
        "{ \"id\":1006, \"intent\":\"OPEN\" }," +
        "{ \"id\":1007, \"intent\":\"SELECT\" }," +
        "{ \"id\":1008, \"intent\":\"MOVE\" }," +
        "{ \"id\":1009, \"intent\":\"RIGHT\" }," +
        "{ \"id\":1010, \"intent\":\"LEFT\" }," +
        "{ \"id\":1011, \"intent\":\"UP\" }," +
        "{ \"id\":1012, \"intent\":\"DOWN\" }," +
        "{ \"id\":1013, \"intent\":\"UNDO\" }," +
        "{ \"id\":1014, \"intent\":\"REDO\" }," +
        "{ \"id\":1015, \"intent\":\"TAKE\" }," +
        "{ \"id\":1016, \"intent\":\"ZOOM\" }," +
        "{ \"id\":1017, \"intent\":\"THIS\" }," +
        "{ \"id\":1018, \"intent\":\"AND\" }," +
        "{ \"id\":1019, \"intent\":\"HERE\" }," +
        "{ \"id\":1020, \"intent\":\"ROTATE\" }," +
        "{ \"id\":1021, \"intent\":\"RESIZE\" }" +
        "]";

    const string subscription_2 = "[" +
        "{" +
        "\"id\":2001," +
        "\"intent\":\"OPEN\"," +
        "\"cognition_type\":\"MERGE\"," +
        "\"fusion_type\":\"FIRST_RAY\"," +
        "\"confidence\":0.8" +
        "}," +
        "{" +
        "\"id\":2002," +
        "\"intent\":\"OPEN\"," +
        "\"cognition_type\":\"MERGE\"," +
        "\"fusion_type\":\"HAND\"," +
        "\"confidence\":0.8" +
        "}," +
        "{" +
        "\"id\":2003," +
        "\"intent\":\"OPEN\"," +
        "\"cognition_type\":\"SIMPLE\"," +
        "\"fusion_type\":\"POSE\"," +
        "\"confidence\":0.8" +
        "}" +
        "]";


    /*
     * ========================================================================
     */

    private const int STRING_LENGTH = 8 * 1024;

    private Thread mainThread = null;
    private Thread messageThread = null;
    private bool isAlive;

    void Awake()
    {
        mainThread = Thread.CurrentThread;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    void config()
    {
        // TIMEOUTS
        string time_out = "1000";
        mmi_configure("TIMEOUT", time_out, ModalityProviderType.VOICE);
        mmi_configure("TIMEOUT", time_out, ModalityProviderType.POSE);
        mmi_configure("TIMEOUT", time_out, ModalityProviderType.HAND);
        mmi_configure("TIMEOUT", time_out, ModalityProviderType.WATCH);

        // VOICE
        string voice_event = "select";
        mmi_configure("EVENT", voice_event, ModalityProviderType.VOICE);

        //string hand_event = "OPEN";
        //mmi_configure("EVENT", hand_event, ModalityProviderType.HAND);

        //mmi_configure("x", "1.1", ModalityProviderType.POSE);
        //mmi_configure("y", "2.2", ModalityProviderType.POSE);
        //mmi_configure("z", "3.3", ModalityProviderType.POSE);

        //string watch_event = "LEFT";
        //mmi_configure("EVENT", watch_event, ModalityProviderType.WATCH);

        //string file_path = "../../../mmi_app/mock_file";
        //mmi_configure("PATH", file_path, ModalityProviderType.FILE);
    }

    public void OnStartService()
    {
        config();

        mmi_start();
        print("mmi start");

        mmi_subscribe(subscription_1);
        print("mmi subscribe: " + subscription_1);
        
        isAlive = true;
        messageThread = new Thread(GetMessage);
        messageThread.IsBackground = true;
        messageThread.Start();
    }

    public void OnStopService()
    {
        isAlive = false;
        messageThread.Join();
        print("messageThread finished");

        mmi_unsubscribe(subscription_1);
        print("mmi_unsubscribe: " + subscription_1);

        mmi_stop();
        print("mmi stop");
    }

    void GetMessage()
    {
        while (isAlive)
        {
            StringBuilder str = new StringBuilder(STRING_LENGTH);
            int len = mmi_get_message(str, STRING_LENGTH);
            if (isAlive)
                ShowText(str.ToString());
        }
    }

    private void ShowText(string text)
    {
        if (Thread.CurrentThread != mainThread)
        {
            MainThread.Dispatcher.AddJob(new Action(()=> { ShowText(text); }));
            return;
        }

        print(text);
    }  
}

}