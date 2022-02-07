using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
using QFramework;

namespace Task
{
    public class TaskCfg :  MonoSingleton<TaskCfg>
    {
        /// <summary>
        /// 用来存放配置数据的字典（链id：子任务id：taskcfgitem）
        /// </summary>
        private Dictionary<int, Dictionary<int, TaskCfgItem>> m_cfg;

        public void StartLoad()
        {
            StartCoroutine(nameof(LoadCfg));
        }

        /// <summary>
        /// 读取配置 此处改为UnityWebRequest异步加载
        /// </summary>
        /// <returns></returns>
        public IEnumerator LoadCfg()
        {
            m_cfg = new Dictionary<int, Dictionary<int, TaskCfgItem>>();
            var uri = new System.Uri(Path.Combine(Application.streamingAssetsPath, "task_cfg.bytes"));
            UnityWebRequest webRequest = UnityWebRequest.Get(uri.AbsolutePath);
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error While Sending: " + webRequest.error);
            }
            else
            {
                var txt = webRequest.downloadHandler.text;
               // Debug.Log(txt);
                var jd = JsonMapper.ToObject<JsonData>(txt);

                for (int i = 0, cnt = jd.Count; i < cnt; ++i)
                {
                    var itemJd = jd[i] as JsonData;
                    TaskCfgItem cfgItem = JsonMapper.ToObject<TaskCfgItem>(itemJd.ToJson());
                   // Debug.Log(cfgItem.task_chain_id);

                    if (!m_cfg.ContainsKey(cfgItem.task_chain_id))
                    {
                        m_cfg[cfgItem.task_chain_id] = new Dictionary<int, TaskCfgItem>();
                    }

                    m_cfg[cfgItem.task_chain_id].Add(cfgItem.task_sub_id, cfgItem);
                }
            }
        }

        /// <summary>
        /// 获取配置方法
        /// </summary>
        /// <param name="chainId">链id</param>
        /// <param name="taskSubId">子任务id</param>
        /// <returns></returns>
        public TaskCfgItem GetCfgItem(int chainId, int taskSubId)
        {
            if (m_cfg.ContainsKey(chainId) && m_cfg[chainId].ContainsKey(taskSubId))
                return m_cfg[chainId][taskSubId];
            return null;
        }
    }

    /// <summary>
    /// 任务配置 字段
    /// LitJson提供了一个JsonMapper.ToObject<T>(jsonString)方法，可以直接将json字符串转为类对象，前提是类的字段名要与json的字段相同
    /// </summary>
    public class TaskCfgItem
    {
        public int task_chain_id; //链ID
        public int task_sub_id; //子任务ID
        public string icon; //任务图标
        public string desc; //任务介绍
        public string task_target; //任务目标
        public int target_amount; //目标数量
        public string award; //奖励    {“gold”:1000}
        public string open_chain; //要打开的支线任务  链id|任务id，开启多个链用英文逗号隔开 例：2|1，4|1
    }
}