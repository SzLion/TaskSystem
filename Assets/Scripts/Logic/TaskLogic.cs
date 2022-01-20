using System;
using System.Collections.Generic;
using LitJson;
using QFramework;

namespace Task
{
    public class TaskLogic : Singleton<TaskLogic>
    {
        private TaskData m_taskData;

        public List<TaskDataItem> taskData
        {
            get { return m_taskData.taskDatas; }
        }

        public TaskLogic()
        {
            m_taskData = new TaskData();
        }

        /// <summary>
        /// 获取任务数据
        /// </summary>
        /// <param name="cb"></param>
        public void GetTaskData(Action cb)
        {
            m_taskData.GetTaskDataFromDB(cb);
        }


        /// <summary>
        /// 更新任务进度
        /// 使用Action<int, bool>回调是为了模拟实际场景中与服务端通信（异步），
        /// 处理结果会有个返回码ErrorCode（回调函数第一个参数），
        /// 客户端需判断ErrorCode的值来进行处理，
        /// 一般约定ErrorCode为0表示成功，回调函数第二个参数是是否任务进度已达成，如果任务达成，客户端需要显示领奖按钮，
        /// </summary>
        /// <param name="chainId"></param>
        /// <param name="subId"></param>
        /// <param name="deltaProgress"></param>
        /// <param name="cb"></param>
        public void AddProgress(int chainId, int subId, int deltaProgress, Action<int, bool> cb)
        {
            var data = m_taskData.GetData(chainId, subId);
            if (data != null)
            {
                data.progress += deltaProgress;
                m_taskData.AddOrUpdateData(data);

                var cfg = TaskCfg.Instance.GetCfgItem(chainId, subId);
                if (cfg != null)
                    cb(0, deltaProgress >= cfg.target_amount);
                else
                    cb(-1, false);
            }
            else
            {
                cb(-1, false);
            }
        }

        /// <summary>
        /// 领取任务奖励
        /// </summary>
        /// <param name="chainId"></param>
        /// <param name="subId"></param>
        /// <param name="cb"></param>
        public void GetAward(int chainId, int subId, Action<int, string> cb)
        {
            var data = m_taskData.GetData(chainId, subId);
            if (data == null)
            {
                cb(-1, "{}");
            }

            if (data.award_is_get == 0)
            {
                data.award_is_get = 1;
                m_taskData.AddOrUpdateData(data);
                GoNext(chainId, subId);
                var cfg = TaskCfg.Instance.GetCfgItem(data.task_chain_id, data.task_sub_id);
                cb(1, cfg.award);
            }
            else
            {
                cb(-2, "{}");
            }
        }

        /// <summary>
        /// 一键领取所有奖励
        /// </summary>
        /// <param name="cb"></param>
        public void OneKeyGetAward(Action<int, string> cb)
        {
            int totalGold = 0;
            var tempTaskDatas = new List<TaskDataItem>(m_taskData.taskDatas);
            for (int i = 0, cnt = tempTaskDatas.Count; i < cnt; ++i)
            {
                var oneTask = tempTaskDatas[i];
                var cfg = TaskCfg.Instance.GetCfgItem(oneTask.task_chain_id, oneTask.task_sub_id);
                if (oneTask.progress >= cfg.target_amount && oneTask.award_is_get == 0)
                {
                    oneTask.award_is_get = 1;
                    m_taskData.AddOrUpdateData(oneTask);
                    var awardJd = JsonMapper.ToObject(cfg.award);
                    totalGold += int.Parse(awardJd["gold"].ToString());
                    GoNext(oneTask.task_chain_id, oneTask.task_sub_id);
                }

                if (totalGold > 0)
                {
                    JsonData totalAward = new JsonData();
                    totalAward["gold"] = totalGold;
                    cb(0, JsonMapper.ToJson(totalAward));
                }
                else
                {
                    cb(-1, null);
                }
            }
        }

        /// <summary>
        /// 进入下一个任务（含有支线）
        /// </summary>
        /// <param name="chainId"></param>
        /// <param name="subId"></param>
        private void GoNext(int chainId, int subId)
        {
            var data = m_taskData.GetData(chainId, subId);
            var cfg = TaskCfg.Instance.GetCfgItem(data.task_chain_id, data.task_sub_id);
            var nextCfg = TaskCfg.Instance.GetCfgItem(data.task_chain_id, data.task_sub_id + 1);

            if (1 == data.award_is_get)
            {
                m_taskData.RemoveData(chainId, subId);

                if (null != nextCfg)
                {
                    TaskDataItem dataItem = new TaskDataItem();
                    dataItem.task_chain_id = nextCfg.task_chain_id;
                    dataItem.task_sub_id = nextCfg.task_sub_id;
                    dataItem.progress = 0;
                    dataItem.award_is_get = 0;
                    m_taskData.AddOrUpdateData(dataItem);
                }

                if (!string.IsNullOrEmpty(cfg.open_chain))
                {
                    var chain = cfg.open_chain.Split(',');
                    for (int i = 0, len = chain.Length; i < len; ++i)
                    {
                        var task = chain[i].Split('|');
                        TaskDataItem subChainDataItem = new TaskDataItem();
                        subChainDataItem.task_chain_id = int.Parse(task[0]);
                        subChainDataItem.task_sub_id = int.Parse(task[1]);
                        subChainDataItem.award_is_get = 0;
                        subChainDataItem.progress = 0;
                        m_taskData.AddOrUpdateData(subChainDataItem);
                    }
                }
            }
        }

        public void ResetAll(Action cb)
        {
            m_taskData.ResetData(cb);
        }
    }
}