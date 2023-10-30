using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Reflection.Emit;

public class UIManager : MonoBehaviour
{
    public static UIManager current;
    public GameObject screenSpaceDragHandler;
    public Button globalViewButton, telluricViewButton;
    public Toggle orbitsToggle, unrealisticSizeToggle, rotationToggle, playPauseToggle;
    public InputField timeSpeedField, dateField;
    public Text planetName, planetDescription;
    public InputAction rotAndPan = new InputAction();
    public InputAction playPause = new InputAction();
    public InputAction resetView = new InputAction();
    public InputAction zoomIn = new InputAction();
    public InputAction zoomOut = new InputAction();
    private bool initializedView;

    private void Awake()
    {
        if (current == null)
        {
            current = this;
        }
        else
        {
            Destroy(obj: this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        screenSpaceDragHandler.SetActive(false);
        initializedView = false;
        globalViewButton.onClick.AddListener(GlobalView);
        telluricViewButton.onClick.AddListener(TelluricView);
        orbitsToggle.onValueChanged.AddListener(ToggleOrbits);
        ToggleOrbits(orbitsToggle.isOn);
        handleChangePlanetView(false);
        CameraDriver.current.OnFollowingPlanet += handleChangePlanetView;
        unrealisticSizeToggle.onValueChanged.AddListener(ToggleRealisticSize);
        ToggleRealisticSize(unrealisticSizeToggle.isOn);
        rotationToggle.onValueChanged.AddListener(ToggleRotation);
        ToggleRotation(rotationToggle.isOn);
        timeSpeedField.onSubmit.AddListener(ChangePlaySpeed);
        timeSpeedField.text = PlanetManager.current.speed.ToString();
        playPauseToggle.onValueChanged.AddListener(TogglePlayPause);
        TogglePlayPause(playPauseToggle.isOn);
        dateField.onSubmit.AddListener(ChangeDate);
        dateField.text = PlanetManager.current.timestamp.dateTime.ToString();
        PlanetManager.current.OnTimeChange += updateDateField;
        rotAndPan.Enable();
        playPause.Enable();
        resetView.Enable();
        zoomIn.Enable();
        zoomOut.Enable();
        rotAndPan.started += ctx => screenSpaceDragHandler.SetActive(true);
        rotAndPan.canceled += ctx => screenSpaceDragHandler.SetActive(false);
        playPause.performed += ctx => playPauseToggle.isOn = !playPauseToggle.isOn;
        resetView.performed += ctx => GlobalView();
        zoomIn.performed += ctx => CameraDriver.current.zoom(1);
        zoomOut.performed += ctx => CameraDriver.current.zoom(-1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!initializedView)
        {
            GlobalView();
            initializedView = true;
        }
    }

    public event Action OnGlobalViewClick;
    public void GlobalView()
    {
        globalViewButton.interactable = false;
        telluricViewButton.interactable = true;
        OnGlobalViewClick?.Invoke();
    }

    public event Action OnTelluricViewClick;
    public void TelluricView()
    {
        globalViewButton.interactable = true;
        telluricViewButton.interactable = false;
        OnTelluricViewClick?.Invoke();
    }

    public event Action<bool> OnOrbitsToggle;
    public void ToggleOrbits(bool visible)
    {
        OnOrbitsToggle?.Invoke(visible);
    }

    public event Action<bool> OnRealisticSizeToggle;
    public void ToggleRealisticSize(bool unrealistic)
    {
        OnRealisticSizeToggle?.Invoke(unrealistic);
    }

    public event Action<bool> OnRotationToggle;
    public void ToggleRotation(bool rotation)
    {
        OnRotationToggle?.Invoke(rotation);
    }

    public event Action<int> OnPlaySpeedChanged;
    public void ChangePlaySpeed(string inputText)
    {
        try
        {
            OnPlaySpeedChanged?.Invoke(int.Parse(inputText));
        }
        catch { }
    }

    public event Action<bool> OnPlayPauseToggle;
    public void TogglePlayPause(bool playing)
    {
        timeSpeedField.interactable = playing;
        dateField.interactable = !playing;
        OnPlayPauseToggle?.Invoke(playing);
    }

    public event Action<DateTime> OnDateChanged;
    public void ChangeDate(string inputText)
    {
        try
        {
            OnDateChanged?.Invoke(DateTime.ParseExact(inputText, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
        }
        catch { }
    }

    private void updateDateField(DateTime timestamp)
    {
        dateField.text = timestamp.ToString("dd/MM/yyyy HH:mm:ss");
    }

    private void handleChangePlanetView(bool followingAPlanet)
    {
        orbitsToggle.interactable = !followingAPlanet;
        if (followingAPlanet)
        {
            globalViewButton.interactable = true;
            telluricViewButton.interactable = true;
            bool orbitsToggleWasOn = orbitsToggle.isOn;
            orbitsToggle.isOn = false;
            orbitsToggle.SetIsOnWithoutNotify(orbitsToggleWasOn);
            PlanetData.Planet planet;
            try
            {
                planet = SolarSystemController.current.whatPlanetIsThis(CameraDriver.current.planet);
            }
            catch (Exception)
            {
                return;
            }
            switch (planet)
            {
                case PlanetData.Planet.Mercury:
                    planetName.text = "<b>MERCURE</b>";
                    planetDescription.text = @"La plus petite des planètes, mais la plus proche de toutes les autres !
                    Son verrouillage gravitationnel expose toujours la même face au Soleil.

                    <b>Type</b> : <i>Tellurique/rocheuse</i>
                    <b>Lunes</b> : <i>0</i>

                    <b>Période de révolution</b> : <i>1 408 heures</i>
                    <b>Période de rotation</b> : <i>88 jours terrestres</i>

                    <b>Diamètre</b> : <i>2 440 km</i>
                    <b>Distance moyenne du Soleil</b> : <i>0,39 UA</i>";
                    break;
                case PlanetData.Planet.Venus:
                    planetName.text = "<b>VÉNUS</b>";
                    planetDescription.text = @"La planète la plus chaude du Système solaire !
                    C'est aussi celle qui orbite le plus près de la Terre.

                    <b>Type</b> : <i>Tellurique/rocheuse</i>
                    <b>Lunes</b> : <i>0</i>

                    <b>Période de révolution</b> : <i>5 832 heures</i>
                    <b>Période de rotation</b> : <i>225 jours terrestres</i>

                    <b>Diamètre</b> : <i>6 052 km</i>
                    <b>Distance moyenne du Soleil</b> : <i>0,72 UA</i>";
                    break;
                case PlanetData.Planet.Earth:
                    planetName.text = "<b>TERRE</b>";
                    planetDescription.text = @"Notre maison, et la plus grande des planètes rocheuses !
                    Porte aussi le doux nom de « planète bleue ».

                    <b>Type</b> : <i>Tellurique/rocheuse</i>
                    <b>Lunes</b> : <i>1</i>

                    <b>Période de révolution</b> : <i>24 heures</i>
                    <b>Période de rotation</b> : <i>365 jours</i>

                    <b>Diamètre</b> : <i>6 371 km</i>
                    <b>Distance moyenne du Soleil</b> : <i>1,00 UA</i>";
                    break;
                case PlanetData.Planet.Mars:
                    planetName.text = "<b>MARS</b>";
                    planetDescription.text = @"La planète rouge, souvent représentée dans la fiction.
                    Au pôle Nord, on aperçoit de la glace d'eau !

                    <b>Type</b> : <i>Tellurique/rocheuse</i>
                    <b>Lunes</b> : <i>2</i>

                    <b>Période de révolution</b> : <i>25 heures</i>
                    <b>Période de rotation</b> : <i>687 jours terrestres</i>

                    <b>Diamètre</b> : <i>3 390 km</i>
                    <b>Distance moyenne du Soleil</b> : <i>1,52 UA</i>";
                    break;
                case PlanetData.Planet.Jupiter:
                    planetName.text = "<b>JUPITER</b>";
                    planetDescription.text = @"La reine des planètes, la plus grande de toutes.
                    Sa fameuse grande tache rouge, une tempête à sa surface, est plus grande que la Terre !

                    <b>Type</b> : <i>Géante gazeuse</i>
                    <b>Lunes</b> : <i>95</i>

                    <b>Période de révolution</b> : <i>10 heures</i>
                    <b>Période de rotation</b> : <i>4 333 jours terrestres</i>

                    <b>Diamètre</b> : <i>69 911 km</i>
                    <b>Distance moyenne du Soleil</b> : <i>5,20 UA</i>";
                    break;
                case PlanetData.Planet.Saturn:
                    planetName.text = "<b>SATURNE</b>";
                    planetDescription.text = @"La plus belle des planètes, avec ses anneaux d'astéroïdes envoûtants !
                    Le grand hexagone à son pôle Nord est des plus mystérieux.

                    <b>Type</b> : <i>Géante gazeuse</i>
                    <b>Lunes</b> : <i>146</i>

                    <b>Période de révolution</b> : <i>11 heures</i>
                    <b>Période de rotation</b> : <i>10 759 jours terrestres</i>

                    <b>Diamètre</b> : <i>58 232 km</i>
                    <b>Distance moyenne du Soleil</b> : <i>9,54 UA</i>";
                    break;
                case PlanetData.Planet.Uranus:
                    planetName.text = "<b>URANUS</b>";
                    planetDescription.text = @"Une étrange planète à l'axe de rotation complètement retourné !
                    Elle aussi a un anneau, on le voit parfois.

                    <b>Type</b> : <i>Uranus is a gas giant</i>
                    <b>Lunes</b> : <i>27</i>

                    <b>Période de révolution</b> : <i>17 heures</i>
                    <b>Période de rotation</b> : <i>30 687 jours terrestres</i>

                    <b>Diamètre</b> : <i>25 362 km</i>
                    <b>Distance moyenne du Soleil</b> : <i>19,22 UA</i>";
                    break;
                case PlanetData.Planet.Neptune:
                    planetName.text = "<b>NEPTUNE</b>";
                    planetDescription.text = @"Plus bleue encore que la planète bleue, c'est la plus éloignées des huit planètes du Système solaire !

                    <b>Type</b> : <i>Géante gazeuse</i>
                    <b>Lunes</b> : <i>14</i>

                    <b>Période de révolution</b> : <i>16 heures</i>
                    <b>Période de rotation</b> : <i>60 190 jours terrestres</i>

                    <b>Diamètre</b> : <i>24 622 km</i>
                    <b>Distance moyenne du Soleil</b> : <i>30,06 UA</i>";
                    break;
            }
        }
        else
        {
            ToggleOrbits(orbitsToggle.isOn);
            planetName.text = "<b>SYSTÈME SOLAIRE</b>";
            planetDescription.text = @"<i>Pour le terme général, autour d'étoiles quelconques, on parle de <i>système planétaire</i>, et non « stellaire » !</i>

            —

            <b>Espace</b> : Basculer le défilement temporel
            <b>Échap</b> : Revenir en vue globale
            <b>Clic sur un nom</b> : Vue planète

            <b>Molette</b> : Zoom
            <b>Ctrl + clic</b> : Rotation orbitale
            <b>Ctrl + clic droit</b> : Panoramique";
        }
    }
}
