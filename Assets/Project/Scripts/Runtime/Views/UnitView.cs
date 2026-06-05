using UnityEngine;

public class UnitView : MonoBehaviour
{
    [SerializeField] private GameObject m_selectionRing;

    public UnitModel Model { get; private set; }

    public void Initialize(UnitModel _model)
    {
        Model = _model;

        gameObject.name = $"Unit_{_model.Owner}_{_model.Role}_{_model.Id}";
        transform.position = GridUtility.GridToUnitWorld(_model.Position);

        SetSelected(false);
    }

    public void SetSelected(bool _selected)
    {
        if (m_selectionRing != null)
        {
            m_selectionRing.SetActive(_selected);
        }
    }

    public void SyncPosition()
    {
        if (Model == null)
            return;

        transform.position = GridUtility.GridToUnitWorld(Model.Position);
    }
}
