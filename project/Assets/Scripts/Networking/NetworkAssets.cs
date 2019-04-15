
  using UnityEngine;

  public  static class NetworkAssets
  {
   
      
      public static NetworkController GetController()
       {
           return GameObject.FindWithTag("NetworkController").GetComponent<NetworkController>();
       }


  }