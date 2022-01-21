using UnityEngine.UI;

namespace Task
{
    public class TaskItemUI : RecyclingListViewItem
    {
        //描述
        public Text desText;
        //进度
        public Text progressText;
        //前进按钮
        public Button goAheadBtn;
        //领奖按钮
        public Button getAwardBtn;
        //进度条
        public Slider progressSlider;
        //任务图标
        public Image icon;
        //任务类型标记
        public Image taskType;
    }
}