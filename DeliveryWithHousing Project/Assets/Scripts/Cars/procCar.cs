using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class procCar : MonoBehaviour {
    private GameObject wheelFL;
    private GameObject wheelFR;
    private GameObject wheelBL;
    private GameObject wheelBR;    
    private List<Vector3> points = new List<Vector3>();
    private float wheelRadius;
    private float width;
    private float gridSize;
    private Vector3 startPoint;

    private List<Vector3> headLights = new List<Vector3>();
    // Use this for initialization
    void Start ()
    {
        PlaceWheels();
        Body();
	}

    void OnDrawGizmos()
    {
        foreach (Vector3 v3 in points)
        {
            Vector3 p = v3;            
            Gizmos.DrawSphere(p, 0.1f);
        }
    }


    void PlaceWheels()
    {
        //grab wheels from prefab, and create a new wheelbase

        wheelFL = transform.GetChild(0).gameObject;
        wheelFR = transform.GetChild(1).gameObject;
        wheelBL = transform.GetChild(2).gameObject;
        wheelBR = transform.GetChild(3).gameObject;

        //prefab has each wheel 0.5 from centre

        //choose how much to add to wheelbase (length)
        float maxMove = 0.0f;
        float moveAmount = Random.Range(0f, maxMove);

        //choose wheel Radius - relationship with wheel base?
        float minRadius = 0.3f;
        float maxRadius= 0.3f;
        wheelRadius = Random.Range(minRadius, maxRadius);

        //set radius to wheel collider
        wheelFL.GetComponent<WheelCollider>().radius = wheelRadius;
        wheelFR.GetComponent<WheelCollider>().radius = wheelRadius;
        wheelBL.GetComponent<WheelCollider>().radius = wheelRadius;
        wheelBR.GetComponent<WheelCollider>().radius = wheelRadius;
        //set radius to visual cylinder attached to the wheel collider (Unity :/)

        wheelFR.transform.GetChild(0).transform.localScale = new Vector3(wheelRadius * 2, wheelRadius /3, wheelRadius * 2);
        wheelFL.transform.GetChild(0).transform.localScale = new Vector3(wheelRadius * 2, wheelRadius / 3, wheelRadius * 2);
        wheelBR.transform.GetChild(0).transform.localScale = new Vector3(wheelRadius * 2, wheelRadius / 3, wheelRadius * 2);
        wheelBL.transform.GetChild(0).transform.localScale = new Vector3(wheelRadius * 2, wheelRadius / 3, wheelRadius * 2);

        //extend wheel base

        //create directional vector to add to wheel existing position
        Vector3 moveForward = Vector3.forward * moveAmount;
        //apply to both front wheels
        wheelFL.transform.position += moveForward;
        wheelFR.transform.position += moveForward;

        //do the same for the rear, but now with a backward direction//could make it a different random number//

        //    Vector3 moveBackward = Vector3.back * moveAmount;
        //    wheelBL.transform.position += moveBackward;
        //    wheelBR.transform.position += moveBackward;

        //move it $ radii backwards - randomise the 4?

        Vector3 moveBackward = Vector3.back * wheelRadius;
        moveBackward *= 4;
        wheelBL.transform.position = wheelFL.transform.position + moveBackward;
        wheelBR.transform.position = wheelFR.transform.position + moveBackward;


        //now add width

        maxMove = wheelRadius ;
        moveAmount = Random.Range(maxMove, maxMove);
        width = moveAmount + 0.5f; 

        //extend wheel base

        //create directional vector to add to wheel existing position
        Vector3 moveLeft = Vector3.left * moveAmount;
        //apply to both front wheels
        wheelFL.transform.position += moveLeft;
        wheelBL.transform.position += moveLeft;

        //do the same for the rear, but now with a backward direction//could make it a different random number//

        Vector3 moveRight = Vector3.right * moveAmount;
        wheelFR.transform.position += moveRight;
        wheelBR.transform.position += moveRight;

    }

    void Body()
    {
        //body of car is split in to three distinct sections. Bottom Section, Middle Section, and Top Section
        //Bottom Section is the bottom half, which covers the wheel area.
        //Middle Section is a thin section between Bottom Section and Top section
        //Top Section covers window area

        Grid();

    }

    void Grid()
    {

        #region GridAndElevation
        //Create a grid which will cover wheel base
        //grid size is how much the wheels were stretched, plus 1 unit(the original distance between the wheels), divided by 8. We need 8 points along the car.        
        int sections = 10;
        //distance between wheels
        float wheelBase = Vector3.Distance(wheelFL.transform.position, wheelBL.transform.position);

        //car is split with 2 sections before the front wheel and 2 after the rear
        //        gridSize = wheelBase / (sections-4);
        gridSize = wheelRadius;



        //starting point is in front of front left wheel
        startPoint = transform.GetChild(0).position;
        //move down a wheelSize -- or suspension??///TODO
        startPoint -= Vector3.up * wheelRadius;
        //move start point along 3 gridsizes, we want 1 full section before the front wheel
        startPoint += Vector3.forward * (gridSize * 3);
        //Now we have our start point, work out way down the car length and add points
        //make it equal to section to include closing vertices

        //Lists to save rings of vertices, so we can esily manipulate later. Basically, ring select if you have used Blender

        List<Vector3> ringSelect0 = new List<Vector3>();
        List<Vector3> ringSelect1 = new List<Vector3>();
        List<Vector3> ringSelect2 = new List<Vector3>();
        List<Vector3> ringSelect3 = new List<Vector3>();
        List<Vector3> ringSelect4 = new List<Vector3>();
        List<Vector3> ringSelect5 = new List<Vector3>();

        float bumper = 0f;
        float randomBonnet = Random.Range(bumper + 1.8f, 2f);
        float bonnet = wheelRadius * randomBonnet;

        float randomMidBonnet = Random.Range(randomBonnet+0.5f ,randomBonnet + 0.5f);
        float midBonnet = wheelRadius * randomMidBonnet;

        float randomWindShield = Random.Range(randomMidBonnet-0.5f, randomMidBonnet + 0.5f);
        float windShieldStart = wheelRadius * randomWindShield;

        float randomRoof = Random.Range(randomWindShield+1f, randomWindShield+4f);
        float windShieldTop = wheelRadius * randomRoof;
        float roof = (wheelRadius * randomRoof) + wheelRadius*0.5f;
        float mulitiplier = 0;
        int height = 7;
        for (int y = 0; y < height; y++)
        {
            //0y plank
            //1y top of wheel arch//start of bonnet
            //2y start of windshield//boot height
            //3y top of roof

            mulitiplier++;
            for (int x = 0; x <= sections; x++)
            {

                //0x.start of bonnet
                //1x.mid point bonnet
                //2x.start windowshield
                //3x.end windowshield/ start of roof
                //4x. end of roof
                //5x. end of tail/car

                if (y == 0)
                {
                    //add a backwards direction to start point, multiplied by x
                    Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * bumper);
                    points.Add(point);

                    ringSelect0.Add(point);
                }
                if (y == 1)
                {
                    //add a backwards direction to start point, multiplied by x
                    Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * bonnet);
                    points.Add(point);

                    ringSelect1.Add(point);
                }

                //make first point on bonnet
                if (y == 2)
                {
                    if (x == 0)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * bonnet);
                        points.Add(point);

                        ringSelect2.Add(point);

                    }
                    else if (x == 1)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (Mathf.Lerp(bonnet, midBonnet, 0.5f)));
                        points.Add(point);

                        ringSelect2.Add(point);
                    }
                    else if (x == 2)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (Mathf.Lerp(bonnet,midBonnet,0.5f)));
                        points.Add(point);

                        ringSelect2.Add(point);
                    }
                    else if (x == 3)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (Mathf.Lerp(bonnet, midBonnet, 0.5f)));
                        points.Add(point);

                        ringSelect2.Add(point);
                    }

                    else
                    {
                        //add a backwards direction to start point, multiplied by x
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (Mathf.Lerp(bonnet, midBonnet, 0.5f)));
                        points.Add(point);

                        ringSelect2.Add(point);
                    }
                }

                if (y == 3)
                {
                    if (x == 0)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (Mathf.Lerp(bonnet, midBonnet, 0.5f))) + Vector3.back*0.05f;
                        points.Add(point);

                        ringSelect3.Add(point);

                    }
                    else if (x == 1)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (Mathf.Lerp(midBonnet, windShieldStart, 0.5f)));
                        points.Add(point);

                        ringSelect3.Add(point);
                    }
                    else if (x == 2)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * midBonnet);
                        points.Add(point);

                        ringSelect3.Add(point);
                    }
                    else if (x == 3)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * windShieldStart);
                        points.Add(point);

                        ringSelect3.Add(point);
                    }
                   
                    else if (x == 8 | x == 9 || x == 10)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (midBonnet));
                        points.Add(point);

                        ringSelect3.Add(point);
                    }
                    
                    else if (x == 7 || x == 8 || x == 9 || x == 10)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * windShieldStart);
                        points.Add(point);

                        ringSelect3.Add(point);
                    }

                    else
                    {
                        //add a backwards direction to start point, multiplied by x
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * windShieldStart);
                        points.Add(point);

                        ringSelect3.Add(point);
                    }
                }

                if (y == 4)
                {
                    if (x == 0)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (Mathf.Lerp(bonnet, midBonnet, 0.5f))) + Vector3.back * 0.05f;
                        points.Add(point);

                        ringSelect3.Add(point);

                    }
                    else if (x == 1)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (Mathf.Lerp(midBonnet, windShieldStart, 0.5f)));
                        points.Add(point);

                        ringSelect3.Add(point);
                    }
                    else if (x == 2)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * midBonnet);
                        points.Add(point);

                        ringSelect3.Add(point);
                    }
                    else if (x == 3)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (windShieldStart+0.2f));
                        points.Add(point);

                        ringSelect3.Add(point);
                    }

                    else if (x == 8 | x == 9 || x == 10)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (midBonnet));
                        points.Add(point);

                        ringSelect3.Add(point);
                    }

                    else if (x == 7 || x == 8 || x == 9 || x == 10)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * windShieldStart);
                        points.Add(point);

                        ringSelect3.Add(point);
                    }

                    else
                    {
                        //add a backwards direction to start point, multiplied by x
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * windShieldStart);
                        points.Add(point);

                        ringSelect3.Add(point);
                    }
                }

                if (y == 5)
                {
                    if (x == 0)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (Mathf.Lerp(bonnet, midBonnet, 0.5f))) + Vector3.back * 0.05f;
                        points.Add(point);

                        ringSelect4.Add(point);
                    }
                    else if (x == 1)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (Mathf.Lerp(midBonnet, windShieldStart, 0.5f)));
                        points.Add(point);

                        ringSelect4.Add(point);
                    }
                    else if (x == 2)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * midBonnet);
                        points.Add(point);

                        ringSelect4.Add(point);
                    }
                    else if (x == 3)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (windShieldStart + 0.2f));
                        points.Add(point);

                        ringSelect3.Add(point);
                    }
                    else if (x == 4)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * windShieldTop);
                        points.Add(point);

                        ringSelect3.Add(point);
                    }
                    else if (x == 8 | x == 9 || x == 10)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (midBonnet ));
                        points.Add(point);

                        ringSelect4.Add(point);
                    }
                    else
                    {
                        //add a backwards direction to start point, multiplied by x
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * windShieldTop);
                        points.Add(point);

                        ringSelect4.Add(point);
                    }
                    //   mulitiplier++;
                }

                if (y == 6)
                {
                    if (x == 0)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (Mathf.Lerp(bonnet, midBonnet, 0.5f))) + Vector3.back * 0.05f;
                        points.Add(point);

                        ringSelect5.Add(point);

                    }
                    else if (x == 1)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (Mathf.Lerp(midBonnet, windShieldStart, 0.5f)));
                        points.Add(point);

                        ringSelect5.Add(point);
                    }
                    else if (x == 2)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * midBonnet);
                        points.Add(point);

                        ringSelect5.Add(point);
                    }
                    else if (x == 3)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (windShieldStart + 0.2f)); //variable wingmirror height?
                        points.Add(point);

                        ringSelect3.Add(point);
                    }
                    else if (x == 4)
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * windShieldTop);
                        points.Add(point);

                        ringSelect3.Add(point);
                    }
                    else if (x == 8 | x == 9 || x == 10 )
                    {
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * (midBonnet));
                        points.Add(point);

                        ringSelect5.Add(point);
                    }
                    else
                    {
                        //add a backwards direction to start point, multiplied by x
                        Vector3 point = startPoint + (Vector3.back * gridSize * x) + (Vector3.up * roof);
                        points.Add(point);

                        ringSelect5.Add(point);
                    }                    
                }
            }
        }

        

        //make mesh from points


        List<int> triangles = new List<int>();
        

        // int height = 6;
        int length = sections;
        //one ring attached to the next
        for (int i = 0; i < height - 1; i++)
        {
            for (int j = 0; j < length; j++)
            {
               
                triangles.Add(j + (i * (length + 1)));
                triangles.Add(j + (i * (length + 1)) + (length) + 1);
                triangles.Add((j + (i * (length + 1)) + 1));

                triangles.Add(j + (i * (length + 1)) + (length) + 1);
                triangles.Add(j + (i * (length + 1)) + (length) + 2);
                triangles.Add((j + (i * (length + 1)) + 1));
               

               
            }
        }



        Mesh mesh = new Mesh();
        mesh.vertices = points.ToArray();
        //mesh.triangles = triangles.ToArray();
        mesh.SetTriangles(triangles.ToArray(), 0);
        
        #endregion

        //now we have a 2d mesh of a car

        //we have some duplicate vertices, where we dragged points on top of each other. Let's get rid of them
        mesh = AutoWeld.AutoWeldFunction(mesh, 0.001f, 100f);

        //now to extrude the mesh in to our third axis (to make it 3d) we need the edges.
        List<int> edgeVertices = FindEdges.EdgeVertices(mesh, 0.01f);

        //create a loop - the function does not return a loop
        edgeVertices.Add(edgeVertices[0]);

        //Create a new mesh, the extrusion mesh

        Vector3[] sideVertices = mesh.vertices;
        List<Vector3> extrudedVertices = new List<Vector3>();

        //stretch bonnet
        /*
        //we have our basic car outline. Alter parts of the 2d mesh to create out unique elevation
        //stretch bonnet
        //grab first edge
        List<int> firstEdgeVertices = new List<int>();
        for (int i = 0; i < sideVertices.Length; i++)
        {
            //first edge is at start pos
            if (sideVertices[i].z == startPoint.z)
                firstEdgeVertices.Add(i);

        }

        for(int i = 0; i < firstEdgeVertices.Count;i++)
        {

            //extend the front of the car. Starting to make the grid non uniform

            sideVertices[firstEdgeVertices[i]] += Vector3.forward * wheelRadius;

          //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
          ////  cube.transform.position = sideVertices[firstEdgeVertices[i]];
          //  cube.transform.localScale *= 0.1f;
        }
        */
      
            //duplicate and move over

            List<Vector3> edgeV3s = new List<Vector3>();

        for (int i = 0; i < edgeVertices.Count; i++)
        {
            edgeV3s.Add(sideVertices[edgeVertices[i]]);
        }

        //make a copy of this so we can measure from the same point each time we extrude
        List<Vector3> originals = new List<Vector3>();
        originals = edgeV3s;

        List<Vector3> vertsList = new List<Vector3>();
        List<int> trisList = new List<int>();

        //variables for width
        //0z carEdge
        //1z headlight start
        //2z headlight end
        //3z centre


        float headlightStartX = Random.Range(0.05f, 0.2f);
        float headlightFinishX = Random.Range(headlightStartX + 0.1f, headlightStartX + 0.5f) ;
        float centre = width;

        //bonnet Z is one wheel Radius forward of wheel
        float headlightStartZ = wheelRadius - (wheelRadius / 3);
        float headlightFinishZ = wheelRadius;
        float centrePullFwd = wheelRadius;

        float pinchBottomOfWindow = wheelRadius / 2;
        float pinchRoof = wheelRadius ;

        #region GlueSide
        //glue side of car to plan

        //before we extrude to create the width of the car, we need "ring select" the side of the car
        //we can then pinch these rings to add detail/depth to the side panels

        List<int> lowestRing = new List<int>();
        List<int> secondRing = new List<int>();
        List<int> thirdRing = new List<int>();
        List<int> highestRing = new List<int>();
        

        //foreach v3 in ringselect
        /*
        //for each vertices in mesh
        for(int j = 0; j < sideVertices.Length;j++)
        {
            for (int i = 0; i < ringSelect0.Count; i++)
            {
                if (ringSelect0[i] == sideVertices[j])
                {
                    if(i == 0)
                        //move the vert
                        sideVertices[j] +=  (headlightStartX)*Vector3.right;
                }
            }
            for (int i = 0; i < ringSelect1.Count; i++)
            {
                if (ringSelect1[i] == sideVertices[j])
                {
                    if(i==0)
                    //move the vert
                    sideVertices[j] += (headlightStartX) * Vector3.right;
                }
            }
            for (int i = 0; i < ringSelect2.Count; i++)
            {
                if (ringSelect2[i] == sideVertices[j])
                {
                    //ignore first vertice, this has now been welded
                    if (i == 1)
                        //move the vert
                        sideVertices[j] += headlightStartX * Vector3.right;
                    if (i == 2)
                        //move the vert
                        sideVertices[j] += headlightStartX* Vector3.right ;
                    if (i == 3)
                        //move the vert
                        sideVertices[j] += headlightStartX * Vector3.right;
                    if (i == 4)
                        sideVertices[j] += headlightStartX * Vector3.right;
                    if (i == 5 || i == 6 || i == 7)
                        sideVertices[j] += headlightStartX * Vector3.right;
                }
            }
            for (int i = 0; i < ringSelect3.Count; i++)
            {
                if (ringSelect3[i] == sideVertices[j])
                {
                    if (i == 1)
                        //move the vert
                        sideVertices[j] += pinchBottomOfWindow * Vector3.right;
                    if (i == 2)
                        //move the vert
                        sideVertices[j] += (pinchBottomOfWindow) * Vector3.right;
                    if (i == 3)
                        //move the vert
                        sideVertices[j] += (pinchBottomOfWindow) * Vector3.right;
                    if (i == 4)
                        sideVertices[j] += pinchBottomOfWindow * Vector3.right;

                    if (i == 5 || i == 6 || i == 7)
                        sideVertices[j] += pinchBottomOfWindow * Vector3.right;
                }
            }
            for (int i = 0; i < ringSelect4.Count; i++)
            {
                if (ringSelect4[i] == sideVertices[j])
                {
                    if ( i ==4 ||i == 5 || i == 6 || i == 7)
                        sideVertices[j] += (pinchRoof) * Vector3.right;
                }
            }
            for (int i = 0; i < ringSelect5.Count; i++)
            {
                if (ringSelect5[i] == sideVertices[j])
                {
                    if (i == 4 || i == 5 || i == 6)
                        sideVertices[j] += (pinchRoof) * Vector3.right;
                }
            }
            //if v3 equals vertice, move mesh vertice
        }
        */
        mesh.vertices = sideVertices;
        #endregion
        #region Mirror
        //mirror this side section we have just built for other side of the car

        Mesh mirrorMesh = new Mesh();        
        Vector3[] mirrorVertices = mesh.vertices;
        //move vertices a width over
        for(int i = 0; i < mirrorVertices.Length;i++)
        {
            mirrorVertices[i] += width*2 * Vector3.right;
        }

        mirrorMesh.vertices = mirrorVertices;

        int[] mirrorTriangles = new int[mesh.triangles.Length];
        mirrorTriangles = mesh.triangles;
        
        //reverse triangles
        
        for (int i = 0; i < mirrorTriangles.Length; i += 3)
        {
            int temp = mirrorTriangles[i + 0];
            mirrorTriangles[i + 0] = mirrorTriangles[i + 1];
            mirrorTriangles[i + 1] = temp;
        }
        mirrorMesh.triangles = mirrorTriangles;
        mirrorMesh.RecalculateBounds();
        mirrorMesh.RecalculateNormals();



        for (int i = 0; i < edgeVertices.Count;i++)
        {
            vertsList.Add(sideVertices[edgeVertices[i]]);
        }

       
        #endregion

        #region Extrude
        //3 extrusions towards centre
        int extrusions = 6;
        for (int z = 0; z < extrusions; z++)
        {
            List<Vector3> tempVertices = new List<Vector3>();

            float distance = 0f;            

            for (int i = 0; i < edgeV3s.Count; i++)
            {
                float pullFwd = 0f;
                float pinch = 0f;
                float pullUp = 0f;
                //front section //0 is bumper bottom, 1 is corner of bonnet
                #region front
                if (i == 0 || i == 1)
                {
                    if (z == 0)
                    {
                        distance = headlightStartX;// + (pinchBottomOfWindow/2); //X
                        pullFwd = headlightStartZ;
                        //pullUp = -0.1f;
                    }
                    if (z == 1)
                    {
                        distance = headlightFinishX;// + (pinchBottomOfWindow / 2);
                        pullFwd = headlightFinishZ;
                       // pullUp = -0.2f;
                    }
                    if (z == 2)
                    {
                        distance = centre;
                        pullFwd = centrePullFwd;
                       // pullUp = -0.3f;
                    }

                    if (z == 3)
                    {
                        distance = (width * 2) - headlightFinishX;
                        pullFwd = headlightFinishZ;
                      //  pullUp = -0.2f;
                    }

                    if (z == 4)
                    {
                        distance = (width * 2) - headlightStartX;
                        pullFwd = headlightStartZ;
                      //  pullUp = -0.1f;
                    }

                    if (z == 5)
                    {
                        distance = (width * 2);
                       // pullFwd = 0f;
                    }
                }
                #endregion

                #region Bonnet
                if(i == 2 || i == 3 )// i == 4)
                {
                    if (z == 0)
                    {//1st extrusion
                        pullFwd = headlightStartZ;
                    }

                    if (z == 1)
                    {
                        pullFwd = headlightFinishZ;
                    }

                    if (z == 2)
                    {
                        pullFwd = centrePullFwd;
                        
                    }
                    if (z == 3)
                    {
                        pullFwd = headlightFinishZ;
                    }

                    if (z == 4)
                    {
                        pullFwd = headlightStartZ;
                    }

                    if (z == 5)
                    {
                        //dont pull fwd
                    }
                 }

                //4 above wheel arch

                //5 bottom of window

                //top of windshield
                if (i == 6)
                {
                    if (z == 0)
                    {//1st extrusion
                        pullFwd = headlightStartZ / 8;
                    }

                    if (z == 1)
                    {
                        pullFwd = headlightFinishZ / 4;
                    }

                    if (z == 2)
                    {
                        pullFwd = centrePullFwd / 4;

                    }
                    if (z == 3)
                    {
                        pullFwd = headlightFinishZ / 4;
                    }

                    if (z == 4)
                    {
                        pullFwd = headlightStartZ / 8;
                    }

                    if (z == 5)
                    {
                        //dont pull fwd
                    }
                }

                //roof and back

                //above windshield, pull edge forward so we have extra geometry for windowshield frame. Frame will be i == 6
                //the first ring edge needs pulled forward. maybe do it there instead? Can use this for aggressive detailing?
                //the way it si no, cretes a smooth cornering edge-perhaps leave as is and randomise /2
                if( i == 7)
                {
                    if (z == 0)
                    {//1st extrusion
                        pullFwd = gridSize/2;
                    }

                    if (z == 1)
                    {
                        pullFwd = gridSize/2;
                    }

                    if (z == 2)
                    {
                        pullFwd = gridSize/2;

                    }
                    if (z == 3)
                    {
                        pullFwd = gridSize/2;
                    }

                    if (z == 4)
                    {
                        pullFwd = gridSize/2;
                    }

                    if (z == 5)
                    {
                        //dont pull fwd
                    }
                }

                if ( i >8)
                {
                    if (z == 0)
                    {//1st extrusion
                        pullFwd = -headlightStartZ / 8;
                    }

                    if (z == 1)
                    {
                        pullFwd = -headlightFinishZ / 4;
                    }

                    if (z == 2)
                    {
                        pullFwd = -centrePullFwd / 4;

                    }
                    if (z == 3)
                    {
                        pullFwd = -headlightFinishZ / 4;
                    }

                    if (z == 4)
                    {
                        pullFwd = -headlightStartZ / 8;
                    }

                    if (z == 5)
                    {
                        //dont pull fwd
                    }
                }
                #endregion
                #region pinch
                //work in the front elevation. "pinching" in the sides of the car as it builds upwards
                if (z == 0)
                {
                    /*
                    //first extrusion
                    if (i == 2)
                    {
                        //we can "pinch" in
                        pinch = pinchBottomOfWindow;
                    }
                    //top of windshield
                    if (i == 3)
                    {
                        pinch = pinchRoof;
                    }
                    //end of roof
                    if (i == 4)
                    {
                        pinch = pinchRoof;
                    }
                    if (i == 5)
                    {
                        pinch = pinchRoof;
                    }
                    */
                    
                }
                //second extrusion
                if (z == 1)
                {
                    /*
                    //first extrusion
                    if (i == 2)
                    {
                        //we can "pinch" in
                        pinch = pinchBottomOfWindow/2;
                    }
                    //top of windshield
                    if (i == 3)
                    {
                        pinch = pinchRoof/2;
                    }
                    //end of roof
                    if (i == 4)
                    {
                        pinch = pinchRoof/2;
                    }
                    if (i == 5)
                    {
                        pinch = pinchRoof/2;
                    }
                    */

                }
                #endregion
                #region Y
                if(i == 0)
                {

                }

                if (i == 1)
                {
                    //first extrusion
                    //how much we lower the bonnet edge. For example, sporty cars will have a lower edge, than a family car
                    //headlight height
                    if (z == 0)
                    {
                      //  pullUp = -wheelRadius / 8;
                    }
                    //headlight height
                    if (z == 1)
                    {
                   //     pullUp = -wheelRadius / 4;
                    }
                    //centre of bonnet edge
                    if (z == 2)
                    {
                    //    pullUp = -wheelRadius / 4;
                    }
                    if (z == 3)
                    {
                    //    pullUp = -wheelRadius/4;
                    }
                    if (z == 4)
                    {
                     //   pullUp = -wheelRadius / 8;
                    }
                }
                //above wheel
                if (i == 2)
                {
                    //first extrusion
                    if (z == 0)
                    {
                      //  pullUp = -wheelRadius / 8;
                    }
                    if (z == 1)
                    {
                     //   pullUp = -wheelRadius / 4;
                    }
                    if (z == 2)
                    {
                     //   pullUp = -wheelRadius / 4;
                    }
                    if (z == 3)
                    {
                      //  pullUp = -wheelRadius / 4;
                    }
                    if (z == 4)
                    {
                     //   pullUp = -wheelRadius / 8;
                    }
                    
                }
                
                if (i == 3)
                {
                    //first extrusion
                    if (z == 0)
                    {
                        pullUp =  wheelRadius / 8;
                    }
                    if (z == 1)
                    {
                        pullUp =  wheelRadius / 4;
                    }
                    if (z == 2)
                    {
                        pullUp =  wheelRadius / 4;
                    }
                    if (z == 3)
                    {
                        pullUp =  wheelRadius / 4;
                    }
                    if (z == 4)
                    {
                        pullUp =  wheelRadius / 8;
                    }
                }

                if (i == 4)
                {
                    //first extrusion
                    if (z == 0)
                    {
                        pullUp = wheelRadius / 8;
                    }
                    if (z == 1)
                    {
                        pullUp = wheelRadius / 4;
                    }
                    if (z == 2)
                    {
                        pullUp = wheelRadius / 4;
                    }
                    if (z == 3)
                    {
                        pullUp = wheelRadius / 4;
                    }
                    if (z == 4)
                    {
                        pullUp = wheelRadius / 8;
                    }
                   
                }
                //bottom of windshield shape
                if (i == 5)
                {
                    if (z == 0)
                    {
                        pullUp = -wheelRadius / 8;
                        
                    }
                    if (z == 1)
                    {
                        pullUp = -wheelRadius / 4;
                        
                    }
                    if (z == 2)
                    {
                        pullUp = -wheelRadius / 4;
                       
                    }
                    if (z == 3)
                    {
                        pullUp = -wheelRadius / 4;
                        
                    }
                    if (z == 4)
                    {
                        pullUp = -wheelRadius / 8;
                        
                    }
                }
                if ( i == 6 || i == 7 || i ==8 || i == 9)
                {
                    //first extrusion
                    if (z == 0)
                    {
                        pullUp = wheelRadius / 8;
                    }
                    if (z == 1)
                    {
                        pullUp = wheelRadius / 4;
                    }
                    if (z == 2)
                    {
                        pullUp = wheelRadius / 4;
                    }
                    if (z == 3)
                    {
                        pullUp = wheelRadius / 4;
                    }
                    if (z == 4)
                    {
                        pullUp = wheelRadius / 8;
                    }

                }


                #endregion
                Vector3 point = originals[i] + (Vector3.right * distance) + (Vector3.right * pinch) + (Vector3.forward * pullFwd) + Vector3.up * pullUp;
                vertsList.Add(point);

                tempVertices.Add(point);

                //save feature points

                
                //headlights
                if (z == 0 && i == 1)
                {
                    headLights.Add(point);
                }
                if (z == 1 && i == 1)
                {
                    headLights.Add(point);
                }
                if (z == 0 && i == 2)
                {
                    headLights.Add(point);
                }
                if (z == 1 && i == 2)
                {
                    headLights.Add(point);
                }

                //headlights left
                if (z == 3 && i == 1)
                {
                    headLights.Add(point);
                }
                if (z == 4 && i == 1)
                {
                    headLights.Add(point);
                }
                if (z == 3 && i == 2)
                {
                    headLights.Add(point);
                }
                if (z == 4 && i == 2)
                {
                    headLights.Add(point);
                }

            }                    

            //create loop and copy
           // tempVertices.Add(tempVertices[0]);

            edgeV3s = new List<Vector3>();
            edgeV3s = tempVertices;
        }

        #region Headlights
        Debug.Log(headLights.Count);
        for(int i =0; i < headLights.Count; i+=4)
        {
            Vector3 bottomRight = headLights[i];
            Vector3 bottomLeft= headLights[i + 1];
            Vector3 topRight = headLights[i + 2];
            Vector3 topLeft = headLights[i + 3];

            //create quad with these points
            List<Vector3> headlightVertices = new List<Vector3>();

            headlightVertices.Add(headLights[i]);
            headlightVertices.Add(headLights[i+1]);
            headlightVertices.Add(headLights[i+2]);
            headlightVertices.Add(headLights[i+3]);            

            List<int> tris = new List<int>();

            tris.Add(0);
            tris.Add(2);
            tris.Add(1);

            tris.Add(1);
            tris.Add(2);
            tris.Add(3);
            
            Mesh headlightMesh = new Mesh();
            headlightMesh.vertices = headlightVertices.ToArray();
            headlightMesh.triangles = tris.ToArray();

            headlightMesh.RecalculateNormals();

            GameObject headlight = new GameObject();
            headlight.transform.parent = transform;
            headlight.transform.position += Vector3.forward * 0.01f;
            MeshRenderer hlmr = headlight.AddComponent<MeshRenderer>();
            
            hlmr.sharedMaterial = Resources.Load("Lights") as Material;
            MeshFilter hlmf = headlight.AddComponent<MeshFilter>();
            hlmf.mesh = headlightMesh;
            
        }
        #endregion

        List<int> extrusionTriangles = new List<int>();
        List<int> trianglesTransparent = new List<int>();


        int amountInARing = edgeVertices.Count;
        //one ring attached to the next
        for (int i = 0; i < edgeVertices.Count-1; i++)
        {
            for (int j = 0; j < extrusions; j++)
            {
                if ((j == 1 || j == 2 || j == 3 || j == 4)  && i == 5)
                {

                    trianglesTransparent.Add(i + (j * amountInARing));
                    trianglesTransparent.Add(i + ((j + 1) * amountInARing));
                    trianglesTransparent.Add(i + (j * amountInARing) + 1);

                    trianglesTransparent.Add(i + (j * amountInARing) + 1);
                    trianglesTransparent.Add(i + ((j + 1) * amountInARing));
                    trianglesTransparent.Add(i + ((j + 1) * amountInARing) + 1);

                }

                else
                {
                    extrusionTriangles.Add(i + (j * amountInARing));
                    extrusionTriangles.Add(i + ((j + 1) * amountInARing));
                    extrusionTriangles.Add(i + (j * amountInARing) + 1);

                    extrusionTriangles.Add(i + (j * amountInARing) + 1);
                    extrusionTriangles.Add(i + ((j + 1) * amountInARing));
                    extrusionTriangles.Add(i + ((j + 1) * amountInARing) + 1);

                }

            }
        }

        
        foreach (Vector3 v3 in vertsList)
        {
            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
          //  cube.transform.position = v3;
        //    cube.transform.localScale *= 0.1f;
        }

        #endregion

        #region GameObjects
        Mesh extrusionMesh = new Mesh();

        //extrusionMesh.vertices = vertsList.ToArray();
        extrusionMesh.SetVertices(vertsList);
        extrusionMesh.subMeshCount = 2;
        extrusionMesh.SetTriangles(extrusionTriangles, 0);
        extrusionMesh.SetTriangles(trianglesTransparent, 1);
        extrusionMesh.RecalculateNormals();
        

        GameObject extrusion = new GameObject();
        extrusion.transform.parent = transform;
        MeshRenderer mrE = extrusion.AddComponent<MeshRenderer>();
        List<Material> materials = new List<Material>();
        materials.Add(Resources.Load("Blue") as Material);
        materials.Add(Resources.Load("White") as Material);
        mrE.sharedMaterials = materials.ToArray();
        MeshFilter mfE = extrusion.AddComponent<MeshFilter>();
        mfE.mesh = extrusionMesh;
        


        mesh.RecalculateNormals();



        GameObject grid = new GameObject();
        grid.transform.parent = transform;
        MeshRenderer mr = grid.AddComponent<MeshRenderer>();
        
        mr.sharedMaterial = (Resources.Load("Blue") as Material);

        MeshFilter mf = grid.AddComponent<MeshFilter>();
        mf.mesh = mesh;

        GameObject mirror = new GameObject();
        mirror.transform.parent = transform;
        MeshRenderer mrM = mirror.AddComponent<MeshRenderer>();
        mrM.sharedMaterial = Resources.Load("Blue") as Material;
        MeshFilter mfM = mirror.AddComponent<MeshFilter>();
        mfM.mesh = mirrorMesh;
        #endregion

        #region AdjustWheels
        //adjust wheels out now we have our grid size - maybe not the best way to do this, but it is this way atm
        //this way the wheels sit nicely in the grid, and doors have two grids each (if saloon)
        wheelFL.transform.position += Vector3.forward * (gridSize*1.5f);
        wheelFR.transform.position += Vector3.forward * (gridSize * 1.5f);
        wheelBL.transform.position += Vector3.back * (gridSize * 1);
        wheelBR.transform.position += Vector3.back  * (gridSize * 1);
        #endregion
    }


}
