using UnityEngine;
using UnityEngine.EventSystems;
using MyUnityPackage.Toolkit;
using System.Collections.Generic;
using System;

namespace MyUnityPackage.Controller
{
    public class VehiculeHandler : MonoBehaviour
    {
        private enum SeatIndex
        {
            Condutor = -2,
            Passenger = -3,
            Other = -1
        }
        public Transform condutorSeat;
        public Transform passengerSeat;
        public Transform[] otherSeat;
        public GameObject vehicleInput;
        //<Controller,seat> 
        public Dictionary<Transform, Transform> occupedSeat = new Dictionary<Transform, Transform>();


        public void InteractVehicule(Transform interactTransform)
        {
            bool isInVehicle = occupedSeat.TryGetValue(interactTransform, out Transform seat);
            MUPLogger.Info(interactTransform  + " : " + isInVehicle);
            if(isInVehicle)
                ExitVehicule(ServiceLocator.GetService<PlayerController>().transform,seat);
            else
            {
                Transform currentSeat = ChooseSeat();
                if(currentSeat != null)
                    EnterVehicule(ServiceLocator.GetService<PlayerController>().transform,currentSeat);
                else
                    MUPLogger.Info("No seat available");
            }
        }
        Transform ChooseSeat()
        {
            if(!occupedSeat.ContainsKey(condutorSeat))
                return condutorSeat;
            else if(!occupedSeat.ContainsKey(passengerSeat))
                return passengerSeat;
            else
                for(int i =0; i < otherSeat.Length; i++)
                {
                    if(occupedSeat.ContainsKey(otherSeat[i]))
                        return otherSeat[i];
                }
            return null;
        }
        void EnterVehicule(Transform controllerTransform,Transform seatTransform)
        {
            MUPLogger.Info("EnterVehicule");
            //MUPLogger.Info("Pos seat " + seatTransform.position);
            PlayerController pC = ServiceLocator.GetService<PlayerController>();
            //MUPLogger.Info("Pos seat " + pC.name);

            
            if(pC)
            {
                 //Disable component
                pC.enabled = false;
                pC.GetComponent<CharacterController>().enabled = false;

                MonoBehaviour pCMono = pC.GetComponent<IPlayerMovement>() as MonoBehaviour;
                pCMono.enabled = false;

                //Set position
                pC.transform.SetParent(seatTransform);

                pC.transform.position = seatTransform.position;
                pC.transform.rotation = seatTransform.rotation;
                
                //Physics.SyncTransforms();//A regarder
                
               
                //Add in dictionnary
                occupedSeat.Add(controllerTransform, seatTransform);
                //Active Vehicle Input
                if(seatTransform == condutorSeat)
                    vehicleInput.GetComponent<IVehicleInput>().EnableVehicleInput();
            }
                
            else
                MUPLogger.Info("Pc is null" );
        }

        bool SeatIsLeft(Transform seatTransform)
        {
            return Vector3.Distance(seatTransform.position,condutorSeat.position) < Vector3.Distance(seatTransform.position,passengerSeat.position);
        }
        void ExitVehicule(Transform controllerTransform,Transform seatTransform)
        {
            MUPLogger.Info("ExitVehicule");
            //MUPLogger.Info("Pos seat " + seatTransform.transform.position);
            PlayerController pC = ServiceLocator.GetService<PlayerController>();
            //MUPLogger.Info("Pos seat " + pC.name);
            if(pC)
            {
                //Disable Vehicle Input
                if(seatTransform == condutorSeat)
                    vehicleInput.GetComponent<IVehicleInput>().DisableVehicleInput();
                
                //Set position
                Vector3 exitPosition = SeatIsLeft(seatTransform) ? new Vector3(-2,0,0) : new Vector3(2,0,0);
                pC.transform.SetParent(null);
                pC.transform.position = seatTransform.transform.position-exitPosition;
                //Physics.SyncTransforms();//A regarder
                
                pC.transform.localScale = Vector3.one;
                //Remove in dictionnary
                occupedSeat.Remove(controllerTransform);
                //Activate component
                pC.enabled = true;
                pC.GetComponent<CharacterController>().enabled = true;

                MonoBehaviour pCMono = pC.GetComponent<IPlayerMovement>() as MonoBehaviour;
                pCMono.enabled = true;
            }
                
            else
                MUPLogger.Info("Pc is null" );
        }

    }

}
