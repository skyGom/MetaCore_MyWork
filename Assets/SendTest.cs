using Newtonsoft.Json;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SendTest : MonoBehaviour
{
    private QueueData _queueSendData = new QueueData();

    public int RoomId;
    public int ObjectId;
    public int ObjectTableId;
    public int Combo;
    public int Time;

    public Button ConvertBtn;
    public Button ReapeatingBtn;
    public TMP_InputField[] InputValues;

    private minigame.BattlePointRequest battlePointRequest;

    Thread _thread = null;

    private void Start()
    {
        battlePointRequest = new minigame.BattlePointRequest();

        battlePointRequest.roomId = RoomId;
        battlePointRequest.objectId = ObjectId;
        battlePointRequest.objectTableId = ObjectTableId;
        battlePointRequest.combo = Combo;
        battlePointRequest.time = Time;

        ConvertBtn.onClick.AddListener(() => ConvertAndSend());
        ReapeatingBtn.onClick.AddListener(() => InvokeConvert());

        InputValues[0].onEndEdit.AddListener(delegate { OnEndEditRoomId(InputValues[0]); });
        InputValues[1].onEndEdit.AddListener(delegate { OnEndEditObjectId(InputValues[1]); });
        InputValues[2].onEndEdit.AddListener(delegate { OnEndEditTableId(InputValues[2]); });
        InputValues[3].onEndEdit.AddListener(delegate { OnEndEditCombo(InputValues[3]); });
        InputValues[4].onEndEdit.AddListener(delegate { OnEndEditTime(InputValues[4]); });

        _thread = new Thread(new ThreadStart(Run));
        _thread.Start();
    }

    void Run()
    {
        //Debug.LogFormat("Thread#{0}: ����", Thread.CurrentThread.ManagedThreadId);

        //while (true)
        //{
        //    try
        //    {
        //        string data = _queueSendData.GetData();

        //        if (data != null)
        //        {
        //            if (!data.Equals(string.Empty))
        //            {
        //                Debug.LogFormat("Send : {0} ", data);
        //            }
        //        }
        //        else
        //        {
        //            Debug.Log("Error_Send Data is Null");
        //            CancelInvokeReapeating();
        //        }

        //    }
        //    catch (System.Exception e)
        //    {
        //        Debug.Log($"Error_{e.Message}");
        //        CancelInvokeReapeating();
        //    }

        //    Thread.Sleep(1);
        //}

        //Debug.LogFormat("Thread#{0}: ����", Thread.CurrentThread.ManagedThreadId);
    }

    public void InvokeConvert()
    {
        InvokeRepeating("ConvertAndSend", 1, 0.1f);
    }

    public void CancelInvokeReapeating()
    {
        CancelInvoke();
    }

    public void ChangePacketValue()
    {
        battlePointRequest.roomId = RoomId;
        battlePointRequest.objectId = ObjectId;
        battlePointRequest.objectTableId = ObjectTableId;
        battlePointRequest.combo = Combo;
        battlePointRequest.time = Time;

        ConvertAndSend();
    }

    public void OnEndEditRoomId(TMP_InputField tMP_InputField)
    {
        RoomId = int.Parse(tMP_InputField.text);
        ChangePacketValue();
    }

    public void OnEndEditObjectId(TMP_InputField tMP_InputField)
    {
        ObjectId = int.Parse(tMP_InputField.text);
        ChangePacketValue();
    }

    public void OnEndEditTableId(TMP_InputField tMP_InputField)
    {
        ObjectTableId = int.Parse(tMP_InputField.text);
        ChangePacketValue();
    }

    public void OnEndEditCombo(TMP_InputField tMP_InputField)
    {
        Combo = int.Parse(tMP_InputField.text);
        ChangePacketValue();
    }

    public void OnEndEditTime(TMP_InputField tMP_InputField)
    {
        Time = int.Parse(tMP_InputField.text);
        ChangePacketValue();
    }

    public void ConvertAndSend()
    {
        string msg = JsonConvert.SerializeObject(battlePointRequest);
        _queueSendData.PushData(msg);
        Debug.Log($"Convert : {msg}");
    }

    private void OnApplicationQuit()
    {
        CancelInvokeReapeating();
        _thread?.Abort();
    }
}
