using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcClothes : MonoBehaviour
{

    //can be called to create meshes for procedural characters

    public static Mesh Mesh(BindPoseExample bpe)
    {
        

        bool shoes = (Random.value > 0.5f);
        bool shorts = (Random.value < 0.5f * bpe.curveAmount);
        if (bpe.skirt)
            shorts = false;

        bool hotPants = (Random.value < 0.5f * bpe.curveAmount);
        
        bool noPants = false;        

        bool highWaisted =  (Random.value > 0.5f);
        //if (bpe.skirt)
        //    highWaisted = true;

        bool britneySlit = (Random.value < 0.5f * bpe.curveAmount);
        bool boobTube = (Random.value < 0.5f * bpe.curveAmount);
        bool gloves = (Random.value > 0.5f);
        bool topLess = false;
        bool cropTop =  (Random.value < 0.5f * bpe.curveAmount);
        
        bool tShirt =  (Random.value > 0.5f);

        //dont make ankles baggy if they are skin and bone
        //if (shorts || hotPants)
        if(bpe.ankleWidthTrouser > bpe.footWidth)
            bpe.ankleWidthTrouser = bpe.footWidth;

        //we can make parachute pants by infalting kee and ankle size
        // if (!shorts && !hotPants)
        {

            //  kneeWidth = Random.Range(upperBodyGeneralWeight*0.5f, upperBodyGeneralWeight * 2);
            //  ankleWidthTrouser= Random.Range(upperBodyGeneralWeight*0.5f , upperBodyGeneralWeight * 2);

            //if we do this, we should make them high waisted? They look like pjs otherwise (useful?)
        }

        List<Vector3> vertices = new List<Vector3>();
        List<int> trianglesShoes = new List<int>();
        List<int> trianglesSkin = new List<int>();
        List<int> trianglesSkirt = new List<int>();
        List<BoneWeight> boneWeights = new List<BoneWeight>();
        
        //triangles
        //front
        vertices = Feet(vertices, bpe);
        vertices = Shin(vertices, bpe);
        vertices = Thigh(vertices, bpe);

        int currentVertice = vertices.Count;
       
        //start of trouser

        Vector3 waistFR = bpe.spine[1].transform.position + Vector3.forward * (bpe.belly * 0.83f + bpe.upperBodyGeneralWeight * .5f ) + Vector3.right * (bpe.waistWidth3 * 0.5f - bpe.belly * .1f) - Vector3.up * bpe.belly * .1f;
        Vector3 waistRR = bpe.spine[1].transform.position - Vector3.forward * (bpe.booty * .33f + bpe.upperBodyGeneralWeight * .4f) + Vector3.right * (bpe.waistWidth3 * 0.5f - bpe.booty*.5f)  + Vector3.up * bpe.booty * .11f;
        //Vector3 waistRL = spine[1].transform.position - Vector3.forward * thighWidth * 0.5f - Vector3.right * waistWidth3 * 0.5f;

        Vector3 centerWaist = bpe.spine[1].transform.position + Vector3.forward * (bpe.belly * 0.99f + bpe.upperBodyGeneralWeight * .66f) - Vector3.up * bpe.belly * .1f;
        Vector3 rearWaist = bpe.spine[1].transform.position - Vector3.forward * (bpe.booty * 0.66f + bpe.upperBodyGeneralWeight * .42f) + Vector3.up * bpe.booty * 0.2f;
        //Vector3 waistFL = spine[1].transform.position + Vector3.forward * belly - Vector3.right * waistWidth3 * 0.5f;

        vertices.Add(waistRR);
        vertices.Add(waistFR);
        vertices.Add(centerWaist);
        vertices.Add(rearWaist);

        Vector3 upperBellyRR = bpe.spine[2].transform.position - Vector3.forward * (-bpe.boob * .0f + +bpe.upperBodyGeneralWeight * .38f) + Vector3.right * (bpe.waistWidth4 * 0.5f + bpe.boob * 0.0f) - Vector3.up * (bpe.boob * .0f - bpe.perk * .44f + bpe.belly * 0.05f - bpe.booty * 0.05f);
        Vector3 upperBellyFR = bpe.spine[2].transform.position + Vector3.forward * (+bpe.belly * 0.66f + bpe.upperBodyGeneralWeight * .5f - bpe.perk * 0.1f) + Vector3.right * (bpe.waistWidth4) * 0.5f - Vector3.up * (-bpe.perk * .44f + bpe.boob * .0f + bpe.belly * .1f);
        Vector3 upperBellyCenter = bpe.spine[2].transform.position + Vector3.forward * (+bpe.belly * 0.83f + bpe.boob * .0f + bpe.upperBodyGeneralWeight * .66f - bpe.perk * 0.2f) - Vector3.up * (-bpe.perk * .5f + bpe.boob * .0f + bpe.belly * .1f);// + belly * 0.99f);
        Vector3 upperBellyRear = bpe.spine[2].transform.position - Vector3.forward * (bpe.booty * 0.4f + bpe.upperBodyGeneralWeight * .4f) + Vector3.up * (bpe.booty * 0.1f - bpe.boob * 0.0f);
        vertices.Add(upperBellyRR);
        vertices.Add(upperBellyFR);
        vertices.Add(upperBellyCenter);
        vertices.Add(upperBellyRear);



        Vector3 chestRR = bpe.spine[3].transform.position - Vector3.forward * (-bpe.boob * .0f + bpe.upperBodyGeneralWeight * .4f) + Vector3.right * (bpe.waistWidth5 + bpe.boob * 0.0f) * 0.5f + Vector3.up * (-bpe.boob * 0.0f + bpe.perk * 0.33f);
        Vector3 chestRF = bpe.spine[3].transform.position + Vector3.forward * (+bpe.belly * 0.05f + bpe.boob * .88f + bpe.upperBodyGeneralWeight * .5f - bpe.perk * 0.2f) + Vector3.right * ((bpe.waistWidth5) * 0.5f - bpe.boob * .0f - bpe.perk*0.5f) + Vector3.up * (bpe.perk * .44f - bpe.boob * .0f - bpe.belly * .05f);
        Vector3 chestFront = bpe.spine[3].transform.position + Vector3.forward * (bpe.belly * .1f + bpe.boob * .66f + +bpe.upperBodyGeneralWeight * .66f - bpe.perk * .2f) + Vector3.up * (bpe.perk * .44f - bpe.boob * .0f - bpe.belly * .1f);// + belly * 0.99f);
        Vector3 chestRear = bpe.spine[3].transform.position - Vector3.forward * (bpe.booty * 0.2f + bpe.upperBodyGeneralWeight * .4f) + Vector3.up * -bpe.boob * .0f;
        vertices.Add(chestRR);
        vertices.Add(chestRF);
        vertices.Add(chestFront);
        vertices.Add(chestRear);


        //mid chest

        chestRR = bpe.spine[4].transform.position - Vector3.forward * (-bpe.boob * .0f + bpe.upperBodyGeneralWeight * .38f) + Vector3.right * (bpe.waistWidth6 + bpe.boob * 0.0f) * 0.5f - Vector3.up * (-bpe.perk * .2f + bpe.boob * 0f);
        chestRF = bpe.spine[4].transform.position + Vector3.forward * (bpe.boob * 1f + bpe.upperBodyGeneralWeight * .5f - bpe.perk * 0.18f) + Vector3.right * ((bpe.waistWidth6) * .5f - bpe.boob * .0f) - Vector3.up * (-bpe.perk * .25f + bpe.boob * .00f);
        chestFront = bpe.spine[4].transform.position + Vector3.forward * (bpe.boob * .66f + +bpe.upperBodyGeneralWeight * .66f - bpe.perk * 0.2f) - Vector3.up * (-bpe.perk * .33f + bpe.boob * .0f);// + belly * 0.99f);
        chestRear = bpe.spine[4].transform.position - Vector3.forward * (bpe.booty * 0.1f + bpe.upperBodyGeneralWeight * .4f) - Vector3.up * bpe.boob * .00f;
        vertices.Add(chestRR);
        vertices.Add(chestRF);
        vertices.Add(chestFront);
        vertices.Add(chestRear);


        //top chest
        //waistWidth7 = waistWidth4 + upperBodyGeneralWeight*0.1f;
        chestRR = bpe.spine[5].transform.position - Vector3.forward * (bpe.upperBodyGeneralWeight * .33f) + Vector3.right * (bpe.waistWidth7) * 0.5f;
        chestRF = bpe.spine[5].transform.position + Vector3.forward * (bpe.boob * .0f + bpe.upperBodyGeneralWeight * .5f) + Vector3.right * (bpe.waistWidth7 * 0.5f - bpe.perk*0.5f);
        chestFront = bpe.spine[5].transform.position + Vector3.forward * (bpe.boob * .0f + bpe.upperBodyGeneralWeight * .66f);// + belly * 0.99f);
        chestRear = bpe.spine[5].transform.position - Vector3.forward * (bpe.booty * 0.05f + bpe.upperBodyGeneralWeight * .33f);
        vertices.Add(chestRR);
        vertices.Add(chestRF);
        vertices.Add(chestFront);
        vertices.Add(chestRear);


        //shoulder

        Vector3 shoulderFR = bpe.rightShoulder.transform.position + Vector3.forward * ( bpe.shoulderBallWidth) + Vector3.right * (bpe.upperBodyGeneralWeight * .0f + bpe.shoulderBallWidth) - Vector3.up * (bpe.upperBodyGeneralWeight * 0.0f + bpe.shoulderBallWidth);
        Vector3 shoulderRR = bpe.rightShoulder.transform.position - Vector3.forward * ( bpe.shoulderBallWidth) + Vector3.right * (bpe.upperBodyGeneralWeight * .0f + bpe.shoulderBallWidth) - Vector3.up * (bpe.upperBodyGeneralWeight * 0.0f + bpe.shoulderBallWidth); ;
        vertices.Add(shoulderFR);
        vertices.Add(shoulderRR);//45

        //elbow
        Vector3 elbowFR = bpe.rightElbow.transform.position + Vector3.forward * (bpe.elbowWidth) + Vector3.right * (bpe.elbowWidth);
        Vector3 elbowRR = bpe.rightElbow.transform.position - Vector3.forward * (bpe.elbowWidth) + Vector3.right * (bpe.elbowWidth);
        Vector3 elbowFL = bpe.rightElbow.transform.position + Vector3.forward * (bpe.elbowWidth) - Vector3.right * (bpe.elbowWidth);
        Vector3 elbowRL = bpe.rightElbow.transform.position - Vector3.forward * (bpe.elbowWidth) - Vector3.right * (bpe.elbowWidth);

        vertices.Add(elbowFR);//46
        vertices.Add(elbowRR);//47
        vertices.Add(elbowFL);//48
        vertices.Add(elbowRL);//49


        //wrist

        Vector3 wristFR = bpe.rightWrist.transform.position + Vector3.forward * (bpe.wristWidth) + Vector3.right * (bpe.wristWidth);
        Vector3 wristRR = bpe.rightWrist.transform.position - Vector3.forward * (bpe.wristWidth) + Vector3.right * (bpe.wristWidth);
        Vector3 wristFL = bpe.rightWrist.transform.position + Vector3.forward * (bpe.wristWidth) - Vector3.right * (bpe.wristWidth);
        Vector3 wristRL = bpe.rightWrist.transform.position - Vector3.forward * (bpe.wristWidth) - Vector3.right * (bpe.wristWidth);

        vertices.Add(wristFR);//50
        vertices.Add(wristRR);//51        
        vertices.Add(wristFL);//52
        vertices.Add(wristRL);//53

        //hand
        float handSize0 = bpe.wristWidth * 1f;

        Vector3 handFR = bpe.rightWrist.transform.position + Vector3.forward * handSize0 + Vector3.right * handSize0 - Vector3.up * handSize0;
        Vector3 handRR = bpe.rightWrist.transform.position - Vector3.forward * handSize0 + Vector3.right * handSize0 - Vector3.up * handSize0;
        Vector3 handFL = bpe.rightWrist.transform.position + Vector3.forward * handSize0 - Vector3.right * handSize0 - Vector3.up * handSize0;
        Vector3 handRL = bpe.rightWrist.transform.position - Vector3.forward * handSize0 - Vector3.right * handSize0 - Vector3.up * handSize0;

        vertices.Add(handFR);//54
        vertices.Add(handRR);//55        
        vertices.Add(handFL);//56
        vertices.Add(handRL);//57


        Vector3 neckFR = bpe.spine[6].transform.position + Vector3.right * (bpe.neckWidth) + Vector3.forward * (bpe.neckWidth);
        Vector3 neckRR = bpe.spine[6].transform.position + Vector3.right * (bpe.neckWidth) - Vector3.forward * (bpe.neckWidth);
        Vector3 neckFront = bpe.spine[6].transform.position + Vector3.forward * (bpe.neckWidth);
        Vector3 neckRear = bpe.spine[6].transform.position - Vector3.forward * (bpe.neckWidth);

        vertices.Add(neckRR);//58
        vertices.Add(neckFR);//59
        vertices.Add(neckFront);//60
        vertices.Add(neckRear);//61


        neckFR = bpe.spine[7].transform.position + Vector3.right * (bpe.neckWidth) + Vector3.forward * (bpe.neckWidth)+ Vector3.up*0.1f;//.1f to lift it inside head
        neckRR = bpe.spine[7].transform.position + Vector3.right * (bpe.neckWidth) - Vector3.forward * (bpe.neckWidth) + Vector3.up * 0.1f;
        neckFront = bpe.spine[7].transform.position + Vector3.forward * (bpe.neckWidth);
        neckRear = bpe.spine[7].transform.position - Vector3.forward * (bpe.neckWidth);

        vertices.Add(neckRR);//62
        vertices.Add(neckFR);//63
        vertices.Add(neckFront);//64
        vertices.Add(neckRear);//65

        if (shoes)
        {
            trianglesShoes = FeetTriangles();
        }
        else
        {
            trianglesSkin = FeetTriangles();
        }

        List<int> trianglesLegs = new List<int>();
        //shin
        //front
        if (shorts && !(hotPants && Random.value > 0.5f) || bpe.skirt)//sometimes adds socks for hotpants
        {
            trianglesSkin = ShinTriangles(trianglesSkin);
        }
        else
        {
            trianglesLegs = ShinTriangles(trianglesLegs);
        }

        // thigh
       // if (skirt)
        {
            
        }
        //else
        {
            if (hotPants || bpe.skirt)
            {
                trianglesSkin = ThighTriangles(trianglesSkin);
            }
            else
            {
                trianglesLegs = ThighTriangles(trianglesLegs);
            }
        }
        if (bpe.skirt)
        {

        }
        else
        {
            if (noPants)
            {

                trianglesSkin = CrotchTriangles(trianglesSkin);
            }
            else
            {
                trianglesLegs = CrotchTriangles(trianglesLegs);
            }
        }

        List<int> trianglesTop = new List<int>();
        if (highWaisted)
        {
            trianglesLegs = LowerWaistTriangles(trianglesLegs);
        }
        else if(!highWaisted && !cropTop)
        {
            trianglesTop = LowerWaistTriangles(trianglesTop);
        }
        else
            trianglesSkin = LowerWaistTriangles(trianglesSkin);


        //upper belly /below chest

        if (cropTop)
        {
            trianglesSkin = UpperWaistTriangles(trianglesSkin);
        }
        else
        {
            trianglesTop = UpperWaistTriangles(trianglesTop);
        }

        //front chest

        if (topLess)
        {
            trianglesSkin = LowerChestTriangles(trianglesSkin);
        }
        else
        {
            trianglesTop = LowerChestTriangles(trianglesTop);
        }

        bool lowCutTop = false;
        if (lowCutTop)
        {
            trianglesSkin = UpperChestTriangles(trianglesSkin);
        }
        else
        {
            trianglesTop = UpperChestTriangles(trianglesTop);
        }

        if (boobTube)
        {
            trianglesSkin = CollarBoneTriangles(trianglesSkin);

        }
        else
        {
            trianglesTop = CollarBoneTriangles(trianglesTop);
        }

        if (tShirt)
        {
            trianglesTop = UpperArmTriangles(trianglesTop);
        }
        else
        {
            trianglesSkin = UpperArmTriangles(trianglesSkin);
        }
        //use this for breast plate (britney slit)
        if (britneySlit || boobTube)
        {
            trianglesSkin = BritneySlitTriangles(trianglesSkin);
        }
        else
        {
            trianglesTop = BritneySlitTriangles(trianglesTop);
        }

        //hand
        if (!gloves)
        {

            trianglesSkin = HandTriangles(trianglesSkin);
        }
        else
        {
            trianglesTop = HandTriangles(trianglesTop);
        }

        //neckF
        trianglesSkin = NeckTriangles(trianglesSkin);

      

        //mirror
        List<Vector3> temp = new List<Vector3>(vertices);

        for (int i = 0; i < temp.Count; i++)
        {
            Vector3 t = temp[i];
            t.x *= -1;

            vertices.Add(t);
        }

        foreach (Vector3 v3 in vertices)
        {
            //GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //c.transform.position = v3;
            //c.transform.localScale *= 0.05f;
        }
       

        //add and reverse normals for tris( when we moved the vertices over we inverted their position, so in turn, we ahve to invert the triangles
        List<int> tempTris = new List<int>(trianglesShoes);
        for (int i = tempTris.Count - 1; i >= 0; i--)
        {
            trianglesShoes.Add(tempTris[i] + temp.Count);
        }
        tempTris = new List<int>(trianglesLegs);
        for (int i = tempTris.Count - 1; i >= 0; i--)
        {

            trianglesLegs.Add(tempTris[i] + temp.Count);
        }


        tempTris = new List<int>(trianglesTop);
        for (int i = tempTris.Count - 1; i >= 0; i--)
        {

            trianglesTop.Add(tempTris[i] + temp.Count);
        }

        tempTris = new List<int>(trianglesSkin);
        for (int i = tempTris.Count - 1; i >= 0; i--)
        {

            trianglesSkin.Add(tempTris[i] + temp.Count);
        }


        bool weld = true;
        if (weld)
        {

            //find any duplciate vertices and attach any triangel set to them to the same vertice -stops seams
            for (int i = 0; i < vertices.Count; i++)
            {
                for (int j = 0; j < vertices.Count; j++)
                {
                    if (vertices[i] == vertices[j])
                    {
                        for (int a = 0; a < trianglesLegs.Count; a++)
                        {
                            if (trianglesLegs[a] == i)
                            {
                                trianglesLegs[a] = j;
                            }
                        }

                        for (int a = 0; a < trianglesTop.Count; a++)
                        {
                            if (trianglesTop[a] == i)
                            {
                                trianglesTop[a] = j;
                            }
                        }
                        for (int a = 0; a < trianglesSkin.Count; a++)
                        {
                            if (trianglesSkin[a] == i)
                            {
                                trianglesSkin[a] = j;
                            }
                        }
                    }
                }
            }
        }
        float skirtWidth = Random.Range(1f, 2f);//how floaty
        //skirtWidth *= skirtWidth;
        //skirtWidth /= 10;

        skirtWidth *= bpe.gaitWidth;
        skirtWidth += bpe.kneeWidth;

      
            //start skirt from waist
            vertices.Add(vertices[20]);//- fr
            vertices.Add(vertices[21]);//- f
            vertices.Add(vertices[22]);//- rr
            vertices.Add(vertices[23]);//- b -9

            //add groin area again, but push out a litt to stop z fighting
            float skirtZHelp = 0.02f;
           
            Vector3 fr = vertices[16] + Vector3.forward * skirtZHelp + Vector3.right * skirtZHelp;
            Vector3 f = vertices[17] + Vector3.forward * skirtZHelp;
            Vector3 r = vertices[18] - Vector3.forward * skirtZHelp;
            Vector3 rightR = vertices[19] - Vector3.forward * skirtZHelp + Vector3.right * skirtZHelp;
            vertices.Add(fr);//- fr
            vertices.Add(f);//- f
            vertices.Add(r);//- b
            vertices.Add(rightR);//- rr -5 

            //we didnt set triangles on the legs for where the skirt will be, create a skirt in the space now
            float slitSize = bpe.waistWidth/4;//consider
            Vector3 front = Vector3.up * (bpe.footHeight + bpe.shinLength) + Vector3.forward * (skirtWidth) + Vector3.right * slitSize;
            front.x = bpe.rightKnee.transform.position.x;
            Vector3 back = Vector3.up * (bpe.footHeight + bpe.shinLength) - Vector3.forward * (skirtWidth) + Vector3.right * slitSize;
            Vector3 rf = bpe.rightKnee.transform.position + Vector3.right * (skirtWidth) + Vector3.forward * (skirtWidth*.5f);
            Vector3 rr = bpe.rightKnee.transform.position + Vector3.right * (skirtWidth) - Vector3.forward * (skirtWidth*.5f);

            //shorten skirt? // only if short skirt - if long, set to knees
            float shorten = bpe.skirtShorten;
            if (bpe.longSkirt)//we will use the shorten variable on the part of skirt between ankles and knees
                shorten = 0f;

            front = Vector3.Lerp(front, vertices[17], shorten);
            back= Vector3.Lerp(back, vertices[18], shorten);
            rf = Vector3.Lerp(rf, vertices[16], shorten);
            rr = Vector3.Lerp(rr, vertices[19], shorten);

            vertices.Add(front);//-4
            vertices.Add(back);//-3
            vertices.Add(rf);//-2
            vertices.Add(rr);//-1

        if (bpe.skirt)
        {
            trianglesSkirt.Add(vertices.Count - 11);
            trianglesSkirt.Add(vertices.Count - 8);
            trianglesSkirt.Add(vertices.Count - 12);

            trianglesSkirt.Add(vertices.Count - 11);
            trianglesSkirt.Add(vertices.Count - 7);
            trianglesSkirt.Add(vertices.Count - 8);

            trianglesSkirt.Add(vertices.Count - 10);
            trianglesSkirt.Add(vertices.Count - 12);
            trianglesSkirt.Add(vertices.Count - 8);

            trianglesSkirt.Add(vertices.Count - 10);//rr high
            trianglesSkirt.Add(vertices.Count - 8);//
            trianglesSkirt.Add(vertices.Count - 5);//mid r?

            trianglesSkirt.Add(vertices.Count - 10);//rr high
            trianglesSkirt.Add(vertices.Count - 5);//mid r?
            trianglesSkirt.Add(vertices.Count - 6);//mid r?

            trianglesSkirt.Add(vertices.Count - 9);
            trianglesSkirt.Add(vertices.Count - 10);
            trianglesSkirt.Add(vertices.Count - 6);

            trianglesSkirt.Add(vertices.Count - 3);
            trianglesSkirt.Add(vertices.Count - 6);
            trianglesSkirt.Add(vertices.Count - 1);

            trianglesSkirt.Add(vertices.Count - 8);
            trianglesSkirt.Add(vertices.Count - 4);
            trianglesSkirt.Add(vertices.Count - 2);

            trianglesSkirt.Add(vertices.Count - 8);
            trianglesSkirt.Add(vertices.Count - 7);
            trianglesSkirt.Add(vertices.Count - 4);

            trianglesSkirt.Add(vertices.Count - 8);
            trianglesSkirt.Add(vertices.Count - 2);
            trianglesSkirt.Add(vertices.Count - 5);

            trianglesSkirt.Add(vertices.Count - 5);
            trianglesSkirt.Add(vertices.Count - 2);
            trianglesSkirt.Add(vertices.Count - 1);

            trianglesSkirt.Add(vertices.Count - 6);
            trianglesSkirt.Add(vertices.Count - 5);
            trianglesSkirt.Add(vertices.Count - 1);

            trianglesSkirt.Add(vertices.Count - 3);
            trianglesSkirt.Add(vertices.Count - 6);
            trianglesSkirt.Add(vertices.Count - 1);
        }

            front = Vector3.forward * (skirtWidth) + Vector3.right * slitSize;
            front.x = bpe.rightAnkle.transform.position.x;
            back = bpe.rightAnkle.transform.position - Vector3.forward * (skirtWidth) + Vector3.right * slitSize;
            rf = bpe.rightAnkle.transform.position + Vector3.right * (skirtWidth ) + Vector3.forward * (skirtWidth*.5f);
            rr = bpe.rightAnkle.transform.position + Vector3.right * (skirtWidth ) - Vector3.forward * ( skirtWidth*.5f);

            //shorten skirt? // only if short skirt - if long, set to knees
            shorten = bpe.skirtShorten;

            front = Vector3.Lerp(front, vertices[140], shorten);
            back = Vector3.Lerp(back, vertices[141], shorten);
            rf = Vector3.Lerp(rf, vertices[142], shorten);
            rr = Vector3.Lerp(rr, vertices[143], shorten);

            vertices.Add(front);//-4
            vertices.Add(back);//-3
            vertices.Add(rf);//-2
            vertices.Add(rr);//-1

        if (bpe.longSkirt && bpe.skirt)
        {
            trianglesSkirt.Add(vertices.Count - 1);
            trianglesSkirt.Add(vertices.Count - 5);
            trianglesSkirt.Add(vertices.Count - 2);

            trianglesSkirt.Add(vertices.Count - 2);
            trianglesSkirt.Add(vertices.Count - 5);
            trianglesSkirt.Add(vertices.Count - 6);

            trianglesSkirt.Add(vertices.Count - 2);
            trianglesSkirt.Add(vertices.Count - 8);
            trianglesSkirt.Add(vertices.Count - 4);

            trianglesSkirt.Add(vertices.Count - 2);
            trianglesSkirt.Add(vertices.Count - 6);
            trianglesSkirt.Add(vertices.Count - 8);

            trianglesSkirt.Add(vertices.Count - 1);
            trianglesSkirt.Add(vertices.Count - 3);
            trianglesSkirt.Add(vertices.Count - 5);

            trianglesSkirt.Add(vertices.Count - 3);
            trianglesSkirt.Add(vertices.Count - 7);
            trianglesSkirt.Add(vertices.Count - 5);


        }


        
        //mirror skirt vertices
        temp = new List<Vector3>(vertices);

        for (int i = temp.Count-16; i < temp.Count; i++)//- 12, how many vertiuces are in skirt
        {
            Vector3 t = temp[i];
            t.x *= -1;

            vertices.Add(t);
          //  GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
         //   c.transform.localScale *= 0.1f;
         //   c.transform.position = t;
        }

        tempTris = new List<int>(trianglesSkirt);
        for (int i = tempTris.Count - 1; i >= 0; i--)
        {
            trianglesSkirt.Add(tempTris[i] + 16);//-12 how many vertices are in skirt
        }
        
        if (weld)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                for (int j = 0; j < vertices.Count; j++)
                {
                    if (vertices[i] == vertices[j])
                    {                        
                        for (int a = 0; a < trianglesSkirt.Count; a++)
                        {
                            if (trianglesSkirt[a] == i)
                            {
                              //  trianglesSkirt[a] = j;
                            }
                        }
                    }
                }
            }
        }
        if (bpe.skirt)
        {
            //add skirt flap - front
            trianglesSkirt.Add(140);
            trianglesSkirt.Add(153);
            trianglesSkirt.Add(156);

            if(bpe.longSkirt)
            {

                trianglesSkirt.Add(160);
                trianglesSkirt.Add(144);
                trianglesSkirt.Add(156);

                trianglesSkirt.Add(144);
                trianglesSkirt.Add(140);
                trianglesSkirt.Add(156);

            }
        }

        //rear        
        if (bpe.skirt)
        {
            trianglesSkirt.Add(154);
            trianglesSkirt.Add(141);
            trianglesSkirt.Add(157);
            if(bpe.longSkirt)
            {

                trianglesSkirt.Add(141);
                trianglesSkirt.Add(161);
                trianglesSkirt.Add(157);

                trianglesSkirt.Add(141);
                trianglesSkirt.Add(145);
                trianglesSkirt.Add(161);
            }
        }



        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.subMeshCount = 5;
        mesh.SetTriangles(trianglesShoes.ToArray(), 0);
        mesh.SetTriangles(trianglesLegs.ToArray(), 1);
        mesh.SetTriangles(trianglesTop.ToArray(), 2);
        mesh.SetTriangles(trianglesSkin.ToArray(), 3);
        mesh.SetTriangles(trianglesSkirt.ToArray(), 4);
        //mesh.boneWeights = boneWeights.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();




        return mesh;

    }

    public static Mesh Skirt(BindPoseExample bpe)
    {

        List<Vector3> vertices = new List<Vector3>();
        List<int> trianglesSkirt = new List<int>();
        List<int> trianglesSkin = new List<int>();
        List<int> trianglesLegs = new List<int>();
        List<int> trianglesTop = new List<int>();
        List<int> trianglesShoes = new List<int>();

        List<BoneWeight> boneWeights = new List<BoneWeight>();

        vertices = Feet(vertices, bpe);
        trianglesSkin = FeetTriangles();
        vertices = Shin(vertices, bpe);
        trianglesSkin = ShinTriangles(trianglesSkin);
        vertices = Thigh(vertices, bpe);
        trianglesSkin = ThighTriangles(trianglesSkin);

        //mirror
        List<Vector3> temp = new List<Vector3>(vertices);

        for (int i = 0; i < temp.Count; i++)
        {
            Vector3 t = temp[i];
            t.x *= -1;

            vertices.Add(t);
        }

        foreach (Vector3 v3 in vertices)
        {
            //GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //c.transform.position = v3;
            //c.transform.localScale *= 0.05f;
        }

        //add and reverse normals for tris( when we moved the vertices over we inverted their position, so in turn, we ahve to invert the triangles
        List<int> tempTris = new List<int>(trianglesShoes);
        for (int i = tempTris.Count - 1; i >= 0; i--)
        {
            trianglesShoes.Add(tempTris[i] + temp.Count);
        }

        tempTris = new List<int>(trianglesLegs);
        for (int i = tempTris.Count - 1; i >= 0; i--)
        {
            trianglesLegs.Add(tempTris[i] + temp.Count);
        }

        tempTris = new List<int>(trianglesTop);
        for (int i = tempTris.Count - 1; i >= 0; i--)
        {
            trianglesTop.Add(tempTris[i] + temp.Count);
        }

        tempTris = new List<int>(trianglesSkin);
        for (int i = tempTris.Count - 1; i >= 0; i--)
        {
            trianglesSkin.Add(tempTris[i] + temp.Count);
        }

        int verticeCountForLegs = vertices.Count;
        //make circle point and work towards waist creating new rings as we go

        int step = 10;//doesn't work with 5 - something to do with the way I check if to set tris***
        float skirtWidth = bpe.gaitWidth * Random.Range(1f, 3f);
        //float skirtWidth2 = bpe.gaitWidth * Random.Range(0f, 0f);
        int start = 1;//0 is ankle,1 knee,2pelvis,3 waist
        int end = 3;//
        for (int y = start; y < end; y++)
        {
            //foot, knee, hip height - only using three because I can't figure out how to give bones partial weigths through code
            float yHeight = 0f;
            if (y == 1)
                yHeight = bpe.footHeight + bpe.shinLength;
            if (y == 2)
                yHeight = bpe.spine[0].transform.position.y;// bpe.footHeight + bpe.shinLength + bpe.thighLength;
            if (y == 3)
                yHeight = bpe.spine[1].transform.position.y;// bpe.footHeight + bpe.shinLength + bpe.thighLength;

            // y == 0
            float width = skirtWidth;
            if (y == 1)
                width = bpe.waistWidth*.5f;
            if (y == 2)
                width = bpe.waistWidth*0.5f;
            if (y == 3)
                width = bpe.waistWidth * 0.5f;

            for (int i = 0; i < 360; i += step)
            {
                Vector3 d = Quaternion.Euler(0, i, 0) * (Vector3.forward * width) + Vector3.up * yHeight;

                vertices.Add(d);

                if (i % 2 == 0 && i != 0 && y > start)//** this if statement doesnt work if step is 5
                {
                    //start from second ring and grab vertice from previous ring
                   
                    trianglesSkirt.Add(vertices.Count - 1);
                    trianglesSkirt.Add(vertices.Count - 2);
                    trianglesSkirt.Add(vertices.Count - 1 - (360 / step));

                    trianglesSkirt.Add(vertices.Count - 1 - (360 / step));
                    trianglesSkirt.Add(vertices.Count - 2);
                    trianglesSkirt.Add(vertices.Count - 2 - (360 / step));

                }

                //do bones now too
                BoneWeight bw = new BoneWeight();
                if (y == 0 && i < 180)
                {
                    //right ankle
                    bw.boneIndex0 = 1;
                    bw.weight0 = .5f;
                    bw.boneIndex1 = 1;
                    bw.weight1 = .5f;
                    boneWeights.Add(bw);
                }
                else if (y == 0 && i >= 180)
                {
                    bw.boneIndex0 = 5;
                    bw.weight0 = .5f;
                    bw.boneIndex1 = 5;
                    bw.weight1 = .5f;
                    boneWeights.Add(bw);
                }
                else if (y == 1 && i < 180)
                {
                    //right knee
                    bw.boneIndex0 = 2;
                    bw.weight0 = 1f;
                    boneWeights.Add(bw);
                }
                else if (y == 1 && i >= 180)
                {
                    bw.boneIndex0 = 6;
                    bw.weight0 = 1f;
                    boneWeights.Add(bw);
                }
                else
                {
                    bw.boneIndex0 = 12;
                    bw.weight0 = 1f;
                    boneWeights.Add(bw);
                }
            }
        }
        //join front and end triangles
        for (int y = 0; y < end-start-1; y++)//was2
        {
            int ring = 360 / step;
            //start from second ring and grab vertice from previous ring
            trianglesSkirt.Add(ring * y + verticeCountForLegs);
            trianglesSkirt.Add((ring * (y + 1)) + verticeCountForLegs);
            trianglesSkirt.Add(ring * (y + 1) - 1 + verticeCountForLegs);

            trianglesSkirt.Add(ring * (y + 1) - 1 + verticeCountForLegs);
            trianglesSkirt.Add(ring * (y + 1) + verticeCountForLegs);
            trianglesSkirt.Add(ring * (y + 2) - 1 + verticeCountForLegs);

        }
        
        Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.subMeshCount = 4;
        mesh.SetTriangles(trianglesSkirt.ToArray(), 1);
        mesh.SetTriangles(trianglesSkin.ToArray(), 3);
        //mesh.boneWeights = boneWeights.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();



        return mesh;
    }


    public static BoneWeight[] SkirtWeights(Mesh mesh)
    {
        BoneWeight[] weights = new BoneWeight[mesh.vertexCount];

        int step = 10;
        for (int y = 0; y < 3; y++)
        {
            for (int i = 0; i < 360; i += step)
            {
                if (y == 0)
                {

                }
            }
        }

        return weights;
    }

    public static List<Vector3> Feet(List<Vector3> vertices, BindPoseExample bpe)
    {

        //start in one corner of the foot and extrude round in a square
        Vector3 pinkyToe = bpe.rightFoot.transform.position + Vector3.right * bpe.footWidth * 0.5f;// Vector3.right * waistWidth + Vector3.forward * footLength;
        Vector3 bigToe = bpe.rightFoot.transform.position - Vector3.right * bpe.footWidth * 0.5f;// pinkyToe - Vector3.right * ankleWidth;
        Vector3 heelInside = bpe.rightAnkle.transform.position - Vector3.right * bpe.footWidth * 0.5f - Vector3.forward*bpe.footWidth*.5f;// Vector3.right * (waistWidth - ankleWidth);
        Vector3 heelOutside = bpe.rightAnkle.transform.position + Vector3.right * bpe.footWidth * 0.5f - Vector3.forward * bpe.footWidth*.5f;// Vector3.right * waistWidth;

        //upper layer of foot
        Vector3 insideHeelUpper = heelInside + Vector3.up * bpe.footHeight;
        Vector3 outsideHeelUpper = heelOutside + Vector3.up * bpe.footHeight;

        Vector3 outsideBridge = outsideHeelUpper + Vector3.forward * bpe.footWidth;
        Vector3 insideBridge = insideHeelUpper + Vector3.forward * bpe.footWidth;

        //foot
        vertices.Add(pinkyToe);//0
        vertices.Add(bigToe);//1
        vertices.Add(heelInside);//2
        vertices.Add(heelOutside);//3
        vertices.Add(outsideBridge);//4
        vertices.Add(insideBridge);//5
        vertices.Add(insideHeelUpper);//6
        vertices.Add(outsideHeelUpper);//7

        return vertices;
    }

    public static List<Vector3> Shin(List<Vector3> vertices, BindPoseExample bpe)
    {
        Vector3 trouserFR = bpe.rightAnkle.transform.position + bpe.ankleWidthTrouser * .5f * Vector3.forward + bpe.ankleWidthTrouser * 0.5f * Vector3.right + Vector3.up * bpe.footHeight;
        Vector3 trouserFL = bpe.rightAnkle.transform.position + bpe.ankleWidthTrouser * .5f * Vector3.forward - bpe.ankleWidthTrouser * 0.5f * Vector3.right + Vector3.up * bpe.footHeight;
        Vector3 trouserRR = bpe.rightAnkle.transform.position - bpe.ankleWidthTrouser * 0.5f * Vector3.forward + bpe.ankleWidthTrouser * 0.5f * Vector3.right + Vector3.up * bpe.footHeight;
        Vector3 trouserRL = bpe.rightAnkle.transform.position - bpe.ankleWidthTrouser * 0.5f * Vector3.forward - bpe.ankleWidthTrouser * 0.5f * Vector3.right + Vector3.up * bpe.footHeight;

        vertices.Add(trouserFR);//8
        vertices.Add(trouserFL);//9
        vertices.Add(trouserRL);//10
        vertices.Add(trouserRR);//11
        //right knee

        Vector3 outsideKneeFront = bpe.rightKnee.transform.position + Vector3.right * bpe.kneeWidth * 0.5f + Vector3.forward * ( bpe.ankleWidthTrouser * .5f);//  outsideBridge + Vector3.up * shinLength;
        Vector3 insideKneeFront = bpe.rightKnee.transform.position - Vector3.right * bpe.kneeWidth * 0.5f + Vector3.forward * ( bpe.ankleWidthTrouser*.5f);
        Vector3 insideKneeBack = bpe.rightKnee.transform.position - Vector3.right * bpe.kneeWidth * 0.5f - Vector3.forward * (bpe.kneeWidth * 0.5f);
        Vector3 outsideKneeBack = bpe.rightKnee.transform.position + Vector3.right * bpe.kneeWidth * 0.5f - Vector3.forward * (bpe.kneeWidth * 0.5f);

        //knee
        vertices.Add(outsideKneeFront);//12
        vertices.Add(insideKneeFront);//13
        vertices.Add(insideKneeBack);//14
        vertices.Add(outsideKneeBack);//15

        return vertices;
    }

    public static List<Vector3> Thigh(List<Vector3> vertices, BindPoseExample bpe)
    {

        //thigh
        //right hip


        Vector3 outsideHipFront = bpe.rightPelvis.transform.position + Vector3.forward * (bpe.upperBodyGeneralWeight * .5f);// + Vector3.right *thighWidth*0.5f;
        Vector3 insideGroinFront = Vector3.up * (bpe.footHeight + bpe.shinLength + bpe.thighLength) + Vector3.forward * (bpe.belly * 0.33f + bpe.upperBodyGeneralWeight * .66f) - Vector3.up * bpe.belly * .05f;
        Vector3 insideGroinBack = Vector3.up * (bpe.footHeight + bpe.shinLength + bpe.thighLength) - Vector3.forward * (bpe.booty * .74f + bpe.upperBodyGeneralWeight * .5f) + Vector3.up * bpe.booty * .33f;
        Vector3 outsideHipBack = bpe.rightPelvis.transform.position - Vector3.forward * (bpe.booty * .5f + bpe.upperBodyGeneralWeight * .5f) + Vector3.up * bpe.booty * .2f - Vector3.right*bpe.booty*.5f;

        //hip/groin
        vertices.Add(outsideHipFront);
        vertices.Add(insideGroinFront);
        vertices.Add(insideGroinBack);//14
        vertices.Add(outsideHipBack);//15

        Vector3 hipBoneHighFront = bpe.spine[0].transform.position + Vector3.right * (bpe.waistWidth2 - bpe.belly * 0.6f) * 0.5f + Vector3.forward * (bpe.belly * .66f + bpe.upperBodyGeneralWeight * .5f) - Vector3.up * bpe.belly * .1f;
        Vector3 hipBoneHighBack = bpe.spine[0].transform.position + Vector3.right * (bpe.waistWidth2 * 0.5f - bpe.booty*0.33f) - Vector3.forward * (bpe.booty * 0.66f + bpe.upperBodyGeneralWeight * .5f) + Vector3.up * bpe.booty * .23f;

        Vector3 insideGroinFrontHigh = bpe.spine[0].transform.position + Vector3.forward * (bpe.belly * .88f + bpe.upperBodyGeneralWeight * .66f) - Vector3.up * bpe.belly * .15f;
        Vector3 insideGroinBackHigh = bpe.spine[0].transform.position - Vector3.forward * (bpe.booty * .8f + bpe.upperBodyGeneralWeight * .5f) + Vector3.up * bpe.booty * .33f;

        vertices.Add(hipBoneHighFront);//16       
        vertices.Add(insideGroinFrontHigh);//17
        vertices.Add(hipBoneHighBack);//18
        vertices.Add(insideGroinBackHigh);//19
        return vertices;
    }

    public static List<int> FeetTriangles()
    {
        List<int> triangles = new List<int>();

        triangles.Add(0);
        triangles.Add(4);
        triangles.Add(1);

        triangles.Add(4);
        triangles.Add(5);
        triangles.Add(1);
        //outside
        triangles.Add(0);
        triangles.Add(3);
        triangles.Add(4);

        triangles.Add(7);
        triangles.Add(4);
        triangles.Add(3);

        //back
        triangles.Add(7);
        triangles.Add(3);
        triangles.Add(6);

        triangles.Add(6);
        triangles.Add(3);
        triangles.Add(2);

        //inside
        triangles.Add(1);
        triangles.Add(6);
        triangles.Add(2);

        triangles.Add(1);
        triangles.Add(5);
        triangles.Add(6);

        //bottom
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);

        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(3);

        return triangles;
    }

    public static List<int> ShinTriangles(List<int> triangles)
    {

        triangles.Add(5 + 4);
        triangles.Add(4 + 4);
        triangles.Add(8 + 4);

        triangles.Add(5 + 4);
        triangles.Add(8 + 4);
        triangles.Add(9 + 4);
        //inside
        triangles.Add(9 + 4);
        triangles.Add(10 + 4);
        triangles.Add(5 + 4);

        triangles.Add(10 + 4);
        triangles.Add(6 + 4);
        triangles.Add(5 + 4);
        //rear
        triangles.Add(6 + 4);
        triangles.Add(11 + 4);
        triangles.Add(7 + 4);

        triangles.Add(10 + 4);
        triangles.Add(11 + 4);
        triangles.Add(6 + 4);
        //outside

        triangles.Add(4 + 4);
        triangles.Add(7 + 4);
        triangles.Add(8 + 4);

        triangles.Add(8 + 4);
        triangles.Add(7 + 4);
        triangles.Add(11 + 4);

        return triangles;
    }

    public static List<int> ThighTriangles(List<int> triangles)
    {

        triangles.Add(10 + 4);
        triangles.Add(9 + 4);
        triangles.Add(13 + 4);

        triangles.Add(13 + 4);
        triangles.Add(14 + 4);
        triangles.Add(10 + 4);

        //rear
        triangles.Add(11 + 4);
        triangles.Add(10 + 4);
        triangles.Add(14 + 4);

        triangles.Add(11 + 4);
        triangles.Add(14 + 4);
        triangles.Add(15 + 4);
        //outside
        triangles.Add(15 + 4);
        triangles.Add(12 + 4);
        triangles.Add(8 + 4);

        triangles.Add(11 + 4);
        triangles.Add(15 + 4);
        triangles.Add(8 + 4);

        //front thigh
        triangles.Add(9 + 4);
        triangles.Add(8 + 4);
        triangles.Add(13 + 4);

        triangles.Add(12 + 4);
        triangles.Add(13 + 4);
        triangles.Add(8 + 4);

        return triangles;
    }

    public static List<int> CrotchTriangles(List<int> triangles)
    {

        triangles.Add(12 + 4); triangles.Add(16 + 4); triangles.Add(13 + 4);
        triangles.Add(13 + 4); triangles.Add(16 + 4); triangles.Add(17 + 4);

        triangles.Add(18 + 4); triangles.Add(15 + 4); triangles.Add(19 + 4);
        triangles.Add(15 + 4); triangles.Add(14 + 4); triangles.Add(19 + 4);

        //side panels for hip
        triangles.Add(16 + 4); triangles.Add(15 + 4); triangles.Add(18 + 4);
        triangles.Add(16 + 4); triangles.Add(12 + 4); triangles.Add(15 + 4);

        return triangles;
    }
    public static List<int> LowerWaistTriangles(List<int> triangles)
    {

        triangles.Add(20); triangles.Add(25); triangles.Add(21);
        triangles.Add(21); triangles.Add(25); triangles.Add(26);
        //rear
        triangles.Add(22); triangles.Add(23); triangles.Add(27);
        triangles.Add(27); triangles.Add(24); triangles.Add(22);
        //side
        triangles.Add(22); triangles.Add(24); triangles.Add(25);
        triangles.Add(22); triangles.Add(25); triangles.Add(20);

        return triangles;
    }

    public static List<int> UpperWaistTriangles(List<int> triangles)
    {
        //side
        triangles.Add(24); triangles.Add(29); triangles.Add(25);
        triangles.Add(24); ; triangles.Add(28); triangles.Add(29);
        //front
        triangles.Add(25); triangles.Add(29); triangles.Add(26);
        triangles.Add(26); triangles.Add(29); triangles.Add(30);

        //back
        triangles.Add(31); triangles.Add(28); triangles.Add(24);
        triangles.Add(31); triangles.Add(24); triangles.Add(27);


        return triangles;
    }

    public static List<int> LowerChestTriangles(List<int> triangles)
    {

        //side
        triangles.Add(24 + 4); triangles.Add(29 + 4); triangles.Add(25 + 4);
        triangles.Add(24 + 4); triangles.Add(28 + 4); triangles.Add(29 + 4);
        //front
        triangles.Add(25 + 4); triangles.Add(29 + 4); triangles.Add(26 + 4);
        triangles.Add(26 + 4); triangles.Add(29 + 4); triangles.Add(30 + 4);

        //back
        triangles.Add(31 + 4); triangles.Add(28 + 4); triangles.Add(24 + 4);
        triangles.Add(31 + 4); triangles.Add(24 + 4); triangles.Add(27 + 4);

        //side
        triangles.Add(24 + 8); triangles.Add(29 + 8); triangles.Add(25 + 8);
        triangles.Add(24 + 8); triangles.Add(28 + 8); triangles.Add(29 + 8);
        //front
        triangles.Add(25 + 8); triangles.Add(29 + 8); triangles.Add(26 + 8);
        triangles.Add(26 + 8); triangles.Add(29 + 8); triangles.Add(30 + 8);

        //back
        triangles.Add(31 + 8); triangles.Add(28 + 8); triangles.Add(24 + 8);
        triangles.Add(31 + 8); triangles.Add(24 + 8); triangles.Add(27 + 8);

        return triangles;
    }

    public static List<int> UpperChestTriangles(List<int> triangles)
    {

        //side
        triangles.Add(24 + 12); triangles.Add(29 + 12); triangles.Add(25 + 12);
        triangles.Add(24 + 12); ; triangles.Add(28 + 12); triangles.Add(29 + 12);
        //front
        triangles.Add(25 + 12); triangles.Add(29 + 12); triangles.Add(26 + 12);
        triangles.Add(26 + 12); triangles.Add(29 + 12); triangles.Add(30 + 12);

        //back
        triangles.Add(31 + 12); triangles.Add(28 + 12); triangles.Add(24 + 12);
        triangles.Add(31 + 12); triangles.Add(24 + 12); triangles.Add(27 + 12);

        return triangles;
    }

    public static List<int> CollarBoneTriangles(List<int> triangles)
    {
    
        triangles.Add(42); triangles.Add(59); triangles.Add(60);

        triangles.Add(59); triangles.Add(41); triangles.Add(44);
        //
        //Skin shoulder
        triangles.Add(44); triangles.Add(45); triangles.Add(59);
        triangles.Add(45); triangles.Add(58); triangles.Add(59);

        //back
        triangles.Add(45); triangles.Add(40); triangles.Add(58);
        triangles.Add(40); triangles.Add(43); triangles.Add(58);
        triangles.Add(61); triangles.Add(58); triangles.Add(43);
        /*
    
     
        */



        triangles.Add(46); triangles.Add(48); triangles.Add(50);
        triangles.Add(52); triangles.Add(50); triangles.Add(48);

        triangles.Add(46 + 1); triangles.Add(50 + 1); triangles.Add(48 + 1);
        triangles.Add(52 + 1); triangles.Add(48 + 1); triangles.Add(50 + 1);

        triangles.Add(47); triangles.Add(46); triangles.Add(50);
        triangles.Add(47); triangles.Add(50); triangles.Add(51);

        triangles.Add(52); triangles.Add(48); triangles.Add(53);
        triangles.Add(49); triangles.Add(53); triangles.Add(48);

        return triangles;
    }

    public static List<int> NeckTriangles(List<int> triangles)
    {

        //side
        triangles.Add(62); triangles.Add(63); triangles.Add(59);
        triangles.Add(62); triangles.Add(59); triangles.Add(58);

        //front
        triangles.Add(64); triangles.Add(60); triangles.Add(59);
        triangles.Add(63); triangles.Add(64); triangles.Add(59);

        //back
        triangles.Add(58); triangles.Add(61); triangles.Add(62);
        triangles.Add(62); triangles.Add(61); triangles.Add(65);

        return triangles;
    }

    public static List<int> UpperArmTriangles(List<int> triangles)
    {

        //inside upper arm
        triangles.Add(40); triangles.Add(49); triangles.Add(48);
        triangles.Add(40); triangles.Add(48); triangles.Add(41);


        //back
        triangles.Add(47); triangles.Add(49); triangles.Add(40);
        triangles.Add(47); triangles.Add(40); triangles.Add(45);

        //front
        triangles.Add(48); triangles.Add(46); triangles.Add(41);
        triangles.Add(41); triangles.Add(46); triangles.Add(44);

        //outside
        triangles.Add(47); triangles.Add(44); triangles.Add(46);
        triangles.Add(47); triangles.Add(45); triangles.Add(44);

        return triangles;
    }

    public static List<int> BritneySlitTriangles (List<int> triangles)
    {
        triangles.Add(42); triangles.Add(41); triangles.Add(59);

        return triangles;
    }

    public static List<int> HandTriangles(List<int> triangles)
    {
        triangles.Add(46 + 4); triangles.Add(48 + 4); triangles.Add(50 + 4);
        triangles.Add(52 + 4); triangles.Add(50 + 4); triangles.Add(48 + 4);

        triangles.Add(46 + 1 + 4); triangles.Add(50 + 1 + 4); triangles.Add(48 + 1 + 4);
        triangles.Add(52 + 1 + 4); triangles.Add(48 + 1 + 4); triangles.Add(50 + 1 + 4);

        triangles.Add(47 + 4); triangles.Add(46 + 4); triangles.Add(50 + 4);
        triangles.Add(47 + 4); triangles.Add(50 + 4); triangles.Add(51 + 4);

        triangles.Add(52 + 4); triangles.Add(48 + 4); triangles.Add(53 + 4);
        triangles.Add(49 + 4); triangles.Add(53 + 4); triangles.Add(48 + 4);

        //bottom of hand
        triangles.Add(54); triangles.Add(56); triangles.Add(55);
        triangles.Add(55); triangles.Add(56); triangles.Add(57);

        return triangles;
    }

}
