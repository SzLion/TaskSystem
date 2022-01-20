using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace Task
{
    public class TaskData
    {
        public TaskData()
        {
            m_taskDatas = new List<TaskDataItem>();
        }

        private List<TaskDataItem> m_taskDatas;

        public List<TaskDataItem> taskDatas
        {
            get { return m_taskDatas; }
        }

        /// <summary>
        /// 模拟 从数据库读取任务数据
        /// </summary>
        /// <param name="cb"></param>
        public void GetTaskDataFromDB(Action cb)
        {
            var jsonStr = PlayerPrefs.GetString("TASK_DATA",
                "[{'task_chain_id':1,'task_sub_id':1,'progress':0,'award_is_get':0}]");
            var taskList = JsonMapper.ToObject<List<TaskDataItem>>(jsonStr);
            for (int i = 0, cnt = taskList.Count; i < cnt; ++i)
            {
                AddOrUpdateData(taskList[i]);
            }

            cb();
        }

        /// <summary>
        /// 新增任务时，需要对列表进行重新排序，确保主线任务（即task_chain_id为1）的任务排在最前面
        /// </summary>
        /// <param name="itemData"></param>
        public void AddOrUpdateData(TaskDataItem itemData)
        {
            bool isUpdate = false;
            for (int i = 0, cnt = m_taskDatas.Count; i < cnt; ++i)
            {
                var item = m_taskDatas[i];
                if (itemData.task_chain_id == item.task_chain_id && itemData.task_sub_id == item.task_sub_id)
                {
                    m_taskDatas[i] = itemData;
                    isUpdate = true;
                    break;
                }
            }

            if (!isUpdate)
                m_taskDatas.Add(itemData);
            m_taskDatas.Sort((a, b) => { return a.task_chain_id.CompareTo(b.task_chain_id); });
            SaveDataToDB();
        }

        /// <summary>
        /// 移除任务数据
        /// </summary>
        /// <param name="chainID"></param>
        /// <param name="subId"></param>
        public void RemoveData(int chainID, int subId)
        {
            for (int i = 0, cnt = m_taskDatas.Count; i < cnt; ++i)
            {
                var item = m_taskDatas[i];
                if (chainID == item.task_chain_id && subId == item.task_sub_id)
                {
                    m_taskDatas.Remove(item);
                    SaveDataToDB();
                    return;
                }
            }
        }

        /// <summary>
        /// 写数据到数据库
        /// </summary>
        private void SaveDataToDB()
        {
            var jsonStr = JsonMapper.ToJson(m_taskDatas);
            PlayerPrefs.SetString("TASK_DATA", jsonStr);
        }
        
        public void ResetData(Action cb)
        {
            PlayerPrefs.DeleteKey("TASK_DATA");
            m_taskDatas.Clear();
            GetTaskDataFromDB(cb);
        }

        /// <summary>
        /// 获取某个任务数据
        /// </summary>
        /// <param name="chainId"></param>
        /// <param name="subId"></param>
        /// <returns></returns>
        public TaskDataItem GetData(int chainId, int subId)
        {
            for (int i = 0, cnt = m_taskDatas.Count; i < cnt; ++i)
            {
                var item = m_taskDatas[i];
                if (chainId == item.task_chain_id && subId == item.task_sub_id)
                    return item;
            }

            return null;
        }
    }

    /// <summary>
    /// 任务数据
    /// </summary>
    public class TaskDataItem
    {
        //链id
        public int task_chain_id;

        //子任务id
        public int task_sub_id;

        //进度
        public int progress;

        //奖励是否被领取，0：未被领取，1：已被领取
        public short award_is_get;
    }
}