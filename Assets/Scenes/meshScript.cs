using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meshScript : MonoBehaviour
{
    private MeshFilter filter;
    private Mesh mesh;
    private Vector3 startPos=new Vector3(0,0,0);
    public List<Vector3> meshVert;
    public List<int> meshTriangles;//三角形
    public List<Vector2> meshUV;
    float offset=0;//保存上一段结尾的x偏移量，附加给这一段
    public float Width=1;
    private bool needStart=true;//需要添加4个点。在初始或者 moveTo时
    [SerializeField]
    private int pointNum=0;//顶点总数。按照 sr，sl，er，el添加
    void Start()
    {
        filter=GetComponent<MeshFilter>();
        mesh = new Mesh();
        filter.mesh=mesh;
        meshVert=new List<Vector3>{};
        meshTriangles=new List<int>{};
        meshUV=new List<Vector2>{};
        
    }
    public void stroke()
    {
        mesh.name = "MyMesh";
        mesh.Clear();
        // 为网格创建顶点数组
        Vector3[] vertices = meshVert.ToArray();

        

        // 通过顶点为网格创建三角形
        int[] triangles = meshTriangles.ToArray();

        
        // 为mesh设置纹理贴图坐标
        Vector2[] uv = meshUV.ToArray();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        //Debug.LogError(vertices.Length+"\tverticesLength:"+mesh.vertices.Length);
       // Debug.Log("")
    }
    // Update is called once per frame
    public void lineTo(Vector3 endPos){
        Vector3 sToEnd=endPos-startPos;
        float length=Vector3.Distance(startPos,endPos);
        Vector3 left=new Vector3(-sToEnd.y,sToEnd.x);
        Vector3 Right=new Vector3(sToEnd.y,-sToEnd.x);
        left=left.normalized*Width/2;
        Right=Right.normalized*Width/2;
        //初始添加4个点，后可只添加两个点
        if(true){
            meshVert.Add(startPos+Right);
            meshVert.Add(startPos+left);
            pointNum+=2;
            //添加UV
            meshUV.Add(new Vector2(offset,0));
            meshUV.Add(new Vector2(offset,1));
            
        }
        meshVert.Add(endPos+Right);
        meshVert.Add(endPos+left);
        meshUV.Add(new Vector2(offset+length/5,0));
        meshUV.Add(new Vector2(offset+length/5,1));
        offset=((offset*5+length)%5)/5.0f;
        //Debug.Log("!!!offset"+offset);
        pointNum+=2;
        meshTriangles.Add(pointNum-1);
        meshTriangles.Add(pointNum-2);
        meshTriangles.Add(pointNum-4);
        meshTriangles.Add(pointNum-1);
        meshTriangles.Add(pointNum-4);
        meshTriangles.Add(pointNum-3);
        startPos=endPos;
        needStart=false;
    }
    /// <summary>
    /// 添加弧线路径
    /// </summary>
    /// <param name="circlePoint">圆心位置</param>
    /// <param name="radius">弧线中点到圆心距离</param>
    /// <param name="startRad">起始角，以弧度计。（弧的圆形的三点钟位置是 0 度）。</param>
    /// <param name="endRad">结束角，以弧度计。三点钟为2pi</param>
    /// <param name="clockwise">可选。规定应该逆时针还是顺时针绘图，默认false逆指针。False = 逆时针，true = 顺时针</param>
    public void circle(Vector3 circlePoint,float radius,float startRad,float endRad,bool clockwise=false){
        float cirLength=0;
        float temOffset=offset;
        if(clockwise){//顺时针
            if(endRad>startRad){
                cirLength=(startRad+2*Mathf.PI-endRad)*radius;
            }
            else{
                cirLength=(startRad-endRad)*radius;   
            }
        }
        else{//逆时针
            if(endRad>startRad){
                cirLength=(endRad-startRad)*radius;
            }
            else{
                
                cirLength=(endRad+2*Mathf.PI-startRad)*radius;
                Debug.LogError("cirLength:"+cirLength);
            }
        }
        float rad=startRad;
                Vector3 temPoint=new Vector3(Mathf.Cos(rad),Mathf.Sin(rad),0);//临时的点
                
            
                if(clockwise)
                    {
                        meshVert.Add(temPoint*(radius-Width/2)+circlePoint);
                        meshVert.Add(temPoint*(radius+Width/2)+circlePoint);
                    }else{
                        meshVert.Add(temPoint*(radius+Width/2)+circlePoint);
                        meshVert.Add(temPoint*(radius-Width/2)+circlePoint);
                    }
                meshUV.Add(new Vector2(offset,0));
                meshUV.Add(new Vector2(offset,1));
                pointNum+=2;
                
                for(int i=0;i<cirLength-1;i++){
                    offset+=1/5.0f;
                    if(clockwise)
                    {
                        rad-=Mathf.Abs((startRad-endRad))/cirLength;
                    }else{
                        rad+=Mathf.Abs( (startRad-endRad))/cirLength;
                    }
                    
                    temPoint=new Vector3(Mathf.Cos(rad),Mathf.Sin(rad),0);
                    if(clockwise)
                    {
                        meshVert.Add(temPoint*(radius-Width/2)+circlePoint);
                        meshVert.Add(temPoint*(radius+Width/2)+circlePoint);
                    }else{
                        meshVert.Add(temPoint*(radius+Width/2)+circlePoint);
                        meshVert.Add(temPoint*(radius-Width/2)+circlePoint);
                    }
                meshUV.Add(new Vector2(offset,0));
                meshUV.Add(new Vector2(offset,1));
                pointNum+=2;
                    meshTriangles.Add(pointNum-1);
                    meshTriangles.Add(pointNum-2);
                    meshTriangles.Add(pointNum-4);
                    meshTriangles.Add(pointNum-1);
                    meshTriangles.Add(pointNum-4);
                    meshTriangles.Add(pointNum-3);

                    
                }
                if(offset!=temOffset+cirLength/5.0f){

                
                offset=temOffset+cirLength/5.0f;
                rad=endRad;
                temPoint=new Vector3(Mathf.Cos(rad),Mathf.Sin(rad),0);
                    if(clockwise)
                    {
                        meshVert.Add(temPoint*(radius-Width/2)+circlePoint);
                        meshVert.Add(temPoint*(radius+Width/2)+circlePoint);
                    }else{
                        meshVert.Add(temPoint*(radius+Width/2)+circlePoint);
                        meshVert.Add(temPoint*(radius-Width/2)+circlePoint);
                    }
                meshUV.Add(new Vector2(offset,0));
                meshUV.Add(new Vector2(offset,1));
                pointNum+=2;
                    meshTriangles.Add(pointNum-1);
                    meshTriangles.Add(pointNum-2);
                    meshTriangles.Add(pointNum-4);
                    meshTriangles.Add(pointNum-1);
                    meshTriangles.Add(pointNum-4);
                    meshTriangles.Add(pointNum-3);
                }
                    
                offset=(offset*5%5)/5.0f;
                //startPos=temPoint*radius+circlePoint;
                needStart=true;
        
    }
    public void moveTo(Vector2 endPos){
        startPos=endPos;
        needStart=true;
        //offset=0;
    }
    public void clear(){
        meshVert.Clear();
        meshTriangles.Clear();
        meshUV.Clear();
        needStart=true;
        startPos=new Vector3(0,0,0);
        pointNum=0;
        offset=0;
    }
    void Update()
    {
    }
}
