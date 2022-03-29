using Pixelplacement.XRTools;
using UnityEngine;

namespace Pixelplacement.RoomMapperDemo
{
    public class PlacementDemo : RoomMapperDemoState
    {
        //Public Variables:
        public LineRenderer pointer;
        public Transform cursor;
        public GameObject floorContent;
        public GameObject chairContent;
        public GameObject wallContent;
        public GameObject ceilingContent;
        public OVRCameraRig oVRCameraRig;
        public GameObject[] chairs;
        public GameObject fantasyWorld;
        public OVRPassthroughLayer passthrough;

        //Startup:
        protected override void Awake()
        {
            base.Awake();
            
            //activation:
            floorContent.SetActive(false);
            wallContent.SetActive(false);
            ceilingContent.SetActive(false);
            fantasyWorld.SetActive(false);
            chairContent.SetActive(false);

            chairs = new GameObject[1];
        }

        //Loops:
        protected override void Update()
        {
            base.Update();

            Vector3 cameraPosition = oVRCameraRig.centerEyeAnchor.position;
            
            //scan:
            RaycastHit hit;
            if (Physics.Raycast(_rig.rightControllerAnchor.position, _rig.rightControllerAnchor.forward, out hit))
            {
                //pointer:
                pointer.gameObject.SetActive(true);
                pointer.SetPosition(0, _rig.rightControllerAnchor.position);
                pointer.SetPosition(1, hit.point);
            
                //cursor:
                cursor.gameObject.SetActive(true);
                cursor.position = hit.point + hit.normal * .001f; //otherwise there will be z sorting with the surface
                cursor.rotation = Quaternion.LookRotation(hit.normal); //orient to surface

                //place content based on surface orientation:
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
                {
                    //surface:
                    float surfaceDot = Vector3.Dot(hit.normal, Vector3.up);
                    
                    //floor:
                    if (surfaceDot == -1)
                    {
                        //ceiling:
                        GameObject newCeilingContent = Instantiate(ceilingContent);
                        newCeilingContent.transform.position = hit.point;
                        newCeilingContent.transform.parent = RoomAnchor.Instance.transform;
                        newCeilingContent.SetActive(true);
                    } 
                    else if (surfaceDot == 0)
                    {
                        //wall:
                        GameObject newWallContent = Instantiate(wallContent);
                        newWallContent.transform.position = hit.point;
                        newWallContent.transform.forward = hit.normal;
                        newWallContent.transform.parent = RoomAnchor.Instance.transform;
                        newWallContent.SetActive(true);

                        
                    }
                    else
                    {
                        //floor:
                        //GameObject newFloorContent = Instantiate(floorContent);
                        //newFloorContent.transform.position = hit.point;
                        //newFloorContent.transform.forward = 
                        //newFloorContent.transform.parent = RoomAnchor.Instance.transform;
                        
                        //newFloorContent.SetActive(true);

                        //wall:
                        GameObject newChairContent = Instantiate(chairContent);
                        newChairContent.transform.position = hit.point;
                        newChairContent.transform.forward = -Vector3.ProjectOnPlane(_rig.rightControllerAnchor.forward, Vector3.up).normalized; //face controller
                        newChairContent.transform.parent = RoomAnchor.Instance.transform;
                        newChairContent.SetActive(true);

                        chairs[0] = newChairContent;
                        //fantasyWorld.SetActive(true);

                    }

                    //ceiling:
                    if (surfaceDot == -1)
                    {
                        GameObject newCeilingContent = Instantiate(ceilingContent);
                        newCeilingContent.transform.position = hit.point;
                        newCeilingContent.transform.parent = RoomAnchor.Instance.transform;
                        newCeilingContent.SetActive(true);
                    }
                }
            }
            else
            {
                //disable:
                pointer.gameObject.SetActive(false);
                cursor.gameObject.SetActive(false);
            }




            if (chairs.Length > 0)
            {

                if (Mathf.Abs(chairs[0].transform.position.y - cameraPosition.y) < 1.3 &&
                    Vector3.Distance(chairs[0].transform.position, cameraPosition) < 1.3)
                {
                    fantasyWorld.SetActive(true);
                    passthrough.textureOpacity = 0.0f;
                    //passthrough.hidden = true;
                }

                else if (Vector3.Distance(chairs[0].transform.position, cameraPosition) > 1.6)
                {
                    fantasyWorld.SetActive(false);
                    passthrough.textureOpacity = 1.0f;
                    //passthrough.hidden = false;
                }
            }


        }

    }
}