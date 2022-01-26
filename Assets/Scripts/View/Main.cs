using System;
using UnityEngine;

namespace Task
{
    public class Main : MonoBehaviour
    {
        private void Awake()
        {
            TaskCfg.Instance.StartLoad();
        }

        // Start is called before the first frame update
        void Start()
        {
            GlobalObj.s_canvasTrans = GameObject.Find("Canvas").transform;
            

            TaskLogic.Instance.GetTaskData(() =>
            {
                TaskPanel.Show();
            });
        }
    }

    public class GlobalObj
    {
        public static Transform s_canvasTrans;
    }
}