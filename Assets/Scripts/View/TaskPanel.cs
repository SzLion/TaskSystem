using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Task
{
    public class TaskPanel : MonoBehaviour
    {
        public RecyclingListView scrollList;


        private static GameObject s_taskPanelPrefab;

        public static void Show()
        {
            if (null == s_taskPanelPrefab)
                s_taskPanelPrefab = Resources.Load<GameObject>("TaskPanel");
            var panelObj = Instantiate(s_taskPanelPrefab);
            panelObj.transform.SetParent(GlobalObj.s_canvasTrans,false);
        }

        private void Start()
        {
            scrollList.ItemCallback = PopulateItem;
            CreateList();
        }

        private void PopulateItem(RecyclingListViewItem item, int rowIndex)
        {
            var child = item as TaskItemUI;

            child.UpdateUI(TaskLogic.Instance.taskData[rowIndex]);

            child.updateListCb = () =>
            {
                //刷新整个列表
                RefreshAll();
            };
        }

        /// <summary>
        /// 刷新整个列表
        /// </summary>
        private void RefreshAll()
        {
            scrollList.RowCount = TaskLogic.Instance.taskData.Count;
            scrollList.Refresh();
        }

        private void CreateList()
        {
            scrollList.RowCount = TaskLogic.Instance.taskData.Count;
        }
    }
}