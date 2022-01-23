using System;
using UnityEngine.UI;

namespace Task
{
    public class TaskItemUI : RecyclingListViewItem
    {
        //描述
        public Text desText;

        //进度
        public Text progressText;

        //前往按钮
        public Button goAheadBtn;

        //领奖按钮
        public Button getAwardBtn;

        //进度条
        public Slider progressSlider;

        //任务图标
        public Image icon;

        //任务类型标记
        public Image taskType;


        public Action updateListCb;

        public void UpdateUI(TaskDataItem data)
        {
            var cfg = TaskCfg.Instance.GetCfgItem(data.task_chain_id, data.task_sub_id);
            if (null != cfg)
            {
                desText.text = cfg.desc;
                
                //TODO 设置图标
                //icon.sprite
                
                //TODO 设置主线/支线图标
                //taskType.sprite

                progressText.text = cfg.task_target;

                progressSlider.value = (float)data.progress / cfg.target_amount;
                
                goAheadBtn.onClick.RemoveAllListeners();
                
                goAheadBtn.onClick.AddListener(() =>
                {
                    //TODO 前往任务
                    
                });
                
                getAwardBtn.onClick.RemoveAllListeners();
                
                getAwardBtn.onClick.AddListener(() =>
                {
                    TaskLogic.Instance.GetAward(data.task_chain_id,data.task_sub_id, (errorCode, award) =>
                    {
                        if (0 == errorCode)
                        {
                            //TODO 领奖界面
                            updateListCb();
                        }
                    });
                    
                });
                
                goAheadBtn.gameObject.SetActive(data.progress < cfg.target_amount);
                
                getAwardBtn.gameObject.SetActive(data.progress >= cfg.target_amount && 0 == data.award_is_get);
            }
        }
    }
}