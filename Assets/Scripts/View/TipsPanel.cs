using System;
using UnityEngine;
using UnityEngine.UI;

namespace Task
{
    public class TipsPanel : MonoBehaviour
    {
        public Button closeBtn;

        public Button addProgressBtn;

        public Button oneKeyBtn;

        private int m_taskChainId;

        private int m_taskSubId;

        private Action updateTaskDataCb;

        void Start()
        {
            closeBtn.onClick.AddListener(() =>
            {
                Destroy(gameObject);
            }); 
            
            addProgressBtn.onClick.AddListener((() =>
            {
                Destroy(gameObject);
                
                TaskLogic.Instance.AddProgress(m_taskChainId,m_taskSubId,1, (errorcode, finishTask) =>
                {
                    updateTaskDataCb();
                });
            }));
            
            oneKeyBtn.onClick.AddListener((() =>
            {
                Destroy(gameObject);

                var cfg = TaskCfg.Instance.GetCfgItem(m_taskChainId, m_taskSubId);
                
                TaskLogic.Instance.AddProgress(m_taskChainId,m_taskSubId,cfg.target_amount, (errorcode,finishTask) =>
                {
                    updateTaskDataCb();
                });

            }));
        }

        private static GameObject s_tipPanelPrefab;

        public static void Show(int chainId, int subId, Action cb)
        {
            if (null == s_tipPanelPrefab)
                s_tipPanelPrefab = Resources.Load<GameObject>("TipsPanel");
            var panelObj = Instantiate(s_tipPanelPrefab);
            panelObj.transform.SetParent(GlobalObj.s_canvasTrans,false);
            var panelBhv = panelObj.GetComponent<TipsPanel>();
            panelBhv.Init(chainId,subId,cb);
        }

        private void Init(int chainId, int subId, Action cb)
        {
            m_taskChainId = chainId;

            m_taskSubId = subId;

            updateTaskDataCb = cb;

        }

    }
    
    
}