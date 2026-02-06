using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class MenuGameOver : MonoBehaviour
{
   [SerializeField] private GameObject menuGameOver;
   private CombateJugador combateJugador;

   private void Start()
   {
      combateJugador = GameObject.FindGameObjectWithTag("Player").GetComponent<CombateJugador>();
      combateJugador.MuerteJugador += ActivarMenu;
   }

   private void ActivarMenu(object sender, EventArgs e)
   {
      menuGameOver.SetActive(true);
      Time.timeScale = 0;
   }
   
   public void Reiniciar()
   {
      Time.timeScale = 1;
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
   }

   public void MenuInicial()
   {
      Time.timeScale = 1;
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
   }

   public void Salir()
   {
      UnityEditor.EditorApplication.isPlaying = false;
      Application.Quit();
   }
   
}
