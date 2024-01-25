using System;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
  public GameObject settings, menu, mainMenu, instructionsMenu, creditsMenu;
  private InputMaster input;
  [HideInInspector] public bool paused;
  [SerializeField] private Crosshair crosshair;
  private enum State {
    MAIN, SETTINGS, INSTRUCTIONS, CREDITS
  }

  private State menuState;
  private State MenuState {
    set {
      if (menuState == value) return;
      menuState = value;
      
      mainMenu.SetActive(false);
      settings.SetActive(false);
      instructionsMenu.SetActive(false);
      creditsMenu.SetActive(false);
      
      switch (menuState) {
        case State.MAIN: mainMenu.SetActive(true); break;
        case State.SETTINGS: settings.SetActive(true); break;
        case State.INSTRUCTIONS: instructionsMenu.SetActive(true); break;
        case State.CREDITS: creditsMenu.SetActive(true); break;
        default: throw new ArgumentOutOfRangeException();
      }
      
    }
  }
  private void Start() {
    input = GameManager.instance.player.input;
    MenuState = State.MAIN;
    input.Menu.Pause.performed += _ => Pause();
    menu.SetActive(paused);
  }

  private void Pause() {
    paused = !paused;
    if (paused) {
      menu.SetActive(true);
      Time.timeScale = 0;
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
      MenuState = State.MAIN;
    }
    else {
      menu.SetActive(false);
      Time.timeScale = 1;
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
    }
  }

  public void Settings() => MenuState = State.SETTINGS;
  public void Instructions() => MenuState = State.INSTRUCTIONS;
  public void Credits() => MenuState = State.CREDITS;
  public void MainMenu() => MenuState = State.MAIN;

  public void CenterSize(float value) {
    Crosshair.CrosshairValues values = crosshair.Values;
    values.centerSize = value;
    crosshair.Values = values;
  }
  public void CenterOutlineThickness(float value) {
    Crosshair.CrosshairValues values = crosshair.Values;
    values.centerOutlineThickness = value;
    crosshair.Values = values;
  }
  public void LineOutlineThickness(float value) {
    Crosshair.CrosshairValues values = crosshair.Values;
    values.lineOutlineThickness = value;
    crosshair.Values = values;
    
  }
  public void LineLength(float value) {
    Crosshair.CrosshairValues values = crosshair.Values;
    values.lineLength = value;
    crosshair.Values = values;
    
  }
  public void LineWidth(float value) {
    Crosshair.CrosshairValues values = crosshair.Values;
    values.lineWidth = value;
    crosshair.Values = values;
    
  }
  public void LineDistance(float value) {
    Crosshair.CrosshairValues values = crosshair.Values;
    values.lineDistance = value;
    crosshair.Values = values;
    
  }
  public void MovementMultiplier(float value) {
    Crosshair.CrosshairValues values = crosshair.Values;
    values.movementMultiplier = value;
    crosshair.Values = values;
    
  }

  public void Sensitivity(float value) {
    GameManager.instance.mainCamera.GetComponent<CameraLook>().sensitivity = value;
  }

  public void Volume(float value) => GameManager.instance.soundManager.SetVolume(value);

  public void Exit() => Application.Quit();
}
