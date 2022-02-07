using System;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace Task
{
    public class TaskPanel : MonoBehaviour
    {
        public RecyclingListView scrollList;

        public Button oneKeyGetAwardBth;

        private static GameObject s_taskPanelPrefab;

        public static void Show()
        {
            
            //TODO 资源加载方案替换
            if (null == s_taskPanelPrefab)
                s_taskPanelPrefab = Resources.Load<GameObject>("TaskPanel");
            var panelObj = Instantiate(s_taskPanelPrefab);
            panelObj.transform.SetParent(GlobalObj.s_canvasTrans,false);
        }

        private void Start()
        {
            scrollList.ItemCallback = PopulateItem;
            CreateList();
            
            oneKeyGetAwardBth.onClick.AddListener((() =>
            {
                TaskLogic.Instance.OneKeyGetAward(((errorcode, award) =>
                {
                    if (0 == errorcode)
                    {
                        AwardPanel.Show(award);
                        RefreshAll();
                    }
                } ));
            }));
            
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
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                TaskLogic.Instance.ResetAll(() => 
                {
                    RefreshAll();
                });
            }
        }
    }
}