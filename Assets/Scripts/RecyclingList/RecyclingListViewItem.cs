using UnityEngine;

/// <summary>
/// �б�item�����Լ�д���б�item��Ҫ�̳и���
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class RecyclingListViewItem : MonoBehaviour
{

    private RecyclingListView parentList;

    /// <summary>
    /// ѭ���б�
    /// </summary>
    public RecyclingListView ParentList
    {
        get => parentList;
    }

    private int currentRow;
    /// <summary>
    /// �к�
    /// </summary>
    public int CurrentRow
    {
        get => currentRow;
    }

    private RectTransform rectTransform;
    public RectTransform RectTransform
    {
        get
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            return rectTransform;
        }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// item�����¼���Ӧ����
    /// </summary>
    public virtual void NotifyCurrentAssignment(RecyclingListView v, int row)
    {
        parentList = v;
        currentRow = row;
    }


}
