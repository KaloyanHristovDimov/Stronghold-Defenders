using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoiceWindow : Window
{
    public static ChoiceWindow Instance;
    public GameObject windowContainer;
    [SerializeField] private List<Transform> tileContainers;
    [SerializeField] private List<TextMeshProUGUI> towerSpacesTexts, biomeBuffTexts, biomeDebuffTexts;

    const string towerSpacesTextPrefix = "<color=#666666>Tower Spaces:</color> ",
                 biomeBuffTextPrefix = "<color=#666666>Biome Buff:</color> ",
                 biomeDebuffTextPrefix = "<color=#666666>Biome Debuff:</color> ";
    readonly Quaternion defaultRotation = Quaternion.Euler(0, 0, 0);

    public GameObject camera;

    Vector3 newPosition = new Vector3(1000f, 1000f, 1000f);

    private Transform camTf;
    private FixedPovCamera fixedPovCam;
    private Vector3 cameraOffsetApplied;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    private readonly List<GameObject> spawnedIcons = new();
    private List<TileData> tilesToChooseFrom = new List<TileData>();
    private Action<TileData> Choose;

    public void Open(List<TileData> tilesToShow, Action<TileData> onChosen)
    {
        camTf = (camera != null) ? camera.transform : (Camera.main != null ? Camera.main.transform : null);

        if (camTf != null)
        {
            if (fixedPovCam == null)
                fixedPovCam = camTf.GetComponent<FixedPovCamera>();

            cameraOffsetApplied = newPosition;

            if (fixedPovCam != null)
                fixedPovCam.AddWorldOffset(cameraOffsetApplied);
            else
                camTf.position += cameraOffsetApplied;
        }
        else
        {
            Debug.LogWarning("ChoiceWindow: No camera found. Assign 'camera' or tag your main camera as MainCamera.");
        }

        tilesToChooseFrom = tilesToShow;
        Choose = onChosen;

        for (int i = 0; i < tileContainers.Count; i++)
        {
            var iconPrefab = tilesToShow[i].icon;
            var iconInstance = Instantiate(iconPrefab, tileContainers[i]);
            spawnedIcons.Add(iconInstance);

            towerSpacesTexts[i].text = towerSpacesTextPrefix + tilesToShow[i].towerAmount.ToString();

            switch (tilesToShow[i].tileBiome)
            {
                case TileData.biomeType.Plains:
                    biomeBuffTexts[i].text = biomeBuffTextPrefix + "Cheaper than normal";
                    biomeDebuffTexts[i].text = biomeDebuffTextPrefix + "None";
                    break;
                case TileData.biomeType.Desert:
                    biomeBuffTexts[i].text = biomeBuffTextPrefix + "Higher range";
                    biomeDebuffTexts[i].text = biomeDebuffTextPrefix + "Lower attack speed";
                    break;
                case TileData.biomeType.Snow:
                    biomeBuffTexts[i].text = biomeBuffTextPrefix + "Towers slow enemies";
                    biomeDebuffTexts[i].text = biomeDebuffTextPrefix + "Lower range";
                    break;
                case TileData.biomeType.Volcanic:
                    biomeBuffTexts[i].text = biomeBuffTextPrefix + "Higher damage";
                    biomeDebuffTexts[i].text = biomeDebuffTextPrefix + "More expensive";
                    break;
            }
        }

        tileContainers.ForEach(tc => tc.transform.localRotation = Quaternion.identity);

        UICanvasController.inst.gameObject.SetActive(false);
        base.Open();
    }

    public void ChooseOption(int id)
    {
        CloseAfterChoice();
        Debug.Log($"Option {id} chosen.");
        Choose(tilesToChooseFrom[id]);
    }

    public void CloseAfterChoice()
    {
        UICanvasController.inst.gameObject.SetActive(true);
        base.Close();

        for (int i = 0; i < tileContainers.Count; i++)
        {
            if (spawnedIcons[i] != null)
                Destroy(spawnedIcons[i]);
        }
        spawnedIcons.Clear();

        if (camTf != null)
        {
            if (fixedPovCam == null)
                fixedPovCam = camTf.GetComponent<FixedPovCamera>();

            if (fixedPovCam != null)
                fixedPovCam.AddWorldOffset(-cameraOffsetApplied);
            else
                camTf.position -= cameraOffsetApplied;
        }
    }

    public override void Update()
    {
        if(!Input.GetMouseButtonDown(1))
            return;
        camTf = (camera != null) ? camera.transform : (Camera.main != null ? Camera.main.transform : null);

        if (camTf != null)
        {
            if(windowContainer.activeSelf)
            {
                if (fixedPovCam == null)
                    fixedPovCam = camTf.GetComponent<FixedPovCamera>();

                if (fixedPovCam != null)
                    fixedPovCam.AddWorldOffset(-cameraOffsetApplied);
                else
                    camTf.position -= cameraOffsetApplied;
            }
            else
            {
                if (fixedPovCam == null)
                    fixedPovCam = camTf.GetComponent<FixedPovCamera>();

                cameraOffsetApplied = newPosition;

                if (fixedPovCam != null)
                    fixedPovCam.AddWorldOffset(cameraOffsetApplied);
                else
                    camTf.position += cameraOffsetApplied;
            }
        }
        windowContainer.SetActive(!windowContainer.activeSelf);
    }
}
