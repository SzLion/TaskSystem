using UnityEngine;

namespace Task
{


    public class Main : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GlobalObj.s_canvasTrans = GameObject.Find("Canvas").transform;
            TaskCfg.Instance.LoadCfg();
            TaskLogic.Instance.GetTaskData(() =>
            {
                //TODO 显示任务界面
            });

        }


    }

    public class GlobalObj
    {
        public static Transform s_canvasTrans;
    }
}