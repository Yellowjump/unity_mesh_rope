using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class myColumn{
    public GameObject column;
    public Vector3 columnWorldPos;
    public Vector3 startWorldPos;
    public Vector3 preTangentWorldPos;//前一个点
    public float rad;//绕柱子的弧度值  带方向
    public myColumn(GameObject c,Vector3 swp){
        this.column=c;
        this.columnWorldPos=this.column.transform.position;
        this.startWorldPos=swp;
        this.preTangentWorldPos=this.startWorldPos;
        this.rad=0;
    }
    public string  mytoString(){
        
        return this.column.name+this.columnWorldPos+this.startWorldPos+"\n"+this.preTangentWorldPos+"\t"+this.rad;
    }
}
public class ropeEnd : MonoBehaviour
{
    // Start is called before the first frame update
    public meshScript ms;
    public Transform ropeStart;
    public float lineWidth =1;
    bool flag=false;
    List <myColumn>columns=new List<myColumn>{};
    Vector3 mouseWorldPos=new Vector3(0,0,0);
    Vector3 newTangentP=new Vector3(0,1,0);
    void Start()
    {
        flag=false;
        
        this.columns.Add(new myColumn(this.ropeStart.gameObject,ropeStart.position));
    }
    private void OnMouseDown() {
        //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        flag=true;
        
        mouseWorldPos=Camera.main.ScreenToWorldPoint(Input.mousePosition)+new Vector3(0,10,0);
    }
    private void OnMouseDrag() {
        mouseWorldPos=Camera.main.ScreenToWorldPoint(Input.mousePosition)+new Vector3(0,10,0);
    }
    private void OnMouseUp() {
        flag=false;
    }
    // Update is called once per frame
    void Update()
    {
        for(var i=0;i<columns.Count;i++){
            Debug.Log(i+"\tcolumn:"+columns[i].mytoString());
        }
        ms.clear();
        if(this.flag){
            GetComponent<Rigidbody2D>().velocity=(this.mouseWorldPos-transform.position)*10;
        }
        else{
            if(Vector3.Distance(transform.position,this.columns[this.columns.Count-1].preTangentWorldPos)<1){
                this.GetComponent<Rigidbody2D>().velocity=(this.columns[this.columns.Count-1].preTangentWorldPos-transform.position)*10;
            }
            else{
                this.GetComponent<Rigidbody2D>().velocity=(this.columns[this.columns.Count-1].preTangentWorldPos-transform.position).normalized*100;
            }
        }
        RaycastHit2D[] results;
        if(this.columns.Count>1){
            
            //获取上一个柱子切点
            //var p=CalcQieDian(this.columns[this.columns.Count-1].columnWorldPos,this.transform.position,this.columns[this.columns.Count-1].column.transform.localScale.x/10,this.columns[this.columns.Count-1].preTangentWorldPos);
            //this.columns[this.columns.Count-1].preTangentWorldPos=p;
            //Debug.DrawLine(transform.position, this.columns[this.columns.Count-1].preTangentWorldPos, Color.red, Vector3.Distance(transform.position,this.columns[this.columns.Count-1].preTangentWorldPos));
            results = Physics2D.RaycastAll(transform.position, this.columns[this.columns.Count-1].preTangentWorldPos-transform.position,Vector3.Distance(transform.position,this.columns[this.columns.Count-1].preTangentWorldPos));
            //var results=cc.director.getPhysicsManager().rayCast(transform.position,this.columns[this.columns.Count-1].preTangentWorldPos,cc.RayCastType.Closest);
        }
        else{
            //Debug.DrawLine(transform.position, this.columns[this.columns.Count-1].startWorldPos, Color.red, Vector3.Distance(transform.position,this.columns[this.columns.Count-1].startWorldPos));
            results = Physics2D.RaycastAll(transform.position, this.columns[this.columns.Count-1].startWorldPos-transform.position,Vector3.Distance(transform.position,this.columns[this.columns.Count-1].startWorldPos));
            //grounded = Physics.Raycast(transform.position, this.columns[this.columns.Count-1].startWorldPos);
            //var results = cc.director.getPhysicsManager().rayCast(transform.position,this.columns[this.columns.Count-1].startWorldPos,cc.RayCastType.Closest);
        }
        for(var i=0;i<results.Length;i++){
            Debug.Log(i+"\t result:"+results[i].transform.name);
        }
        //朝切点发射射线检测
        Debug.Log(results);
        if(results.Length>1){
        
        
            //this.c0=results[0].collider.node.name;
            var col = results[1].collider.gameObject;
            if(col!=this.columns[this.columns.Count-1].column){
                //Debug.Log("radius:"+col.transform.localScale.x/10);
                //Debug.Log("point"+results[1].point);
                var sp=this.CalcQieDian(col.transform.position,this.transform.position,col.transform.localScale.x/10,results[1].point);
                Debug.Log("sp:"+sp);
                var nc=new myColumn(col,sp);
                this.columns.Add(nc);
                //nc.column.GetComponent('columnScr').onLight();
            }
            else{
                //再次碰到原来的column
                this.newTangentP=this.CalcQieDian(this.columns[this.columns.Count-1].columnWorldPos,transform.position,this.columns[this.columns.Count-1].column.transform.localScale.x/10,this.columns[this.columns.Count-1].preTangentWorldPos);
                
                
                
                
                 
                
                
                if(this.columns[this.columns.Count-1].rad*(this.columns[this.columns.Count-1].rad+this.radTwoPOnR(this.columns[this.columns.Count-1].preTangentWorldPos,this.newTangentP,this.columns[this.columns.Count-1].columnWorldPos))<0){
                    //console.log("!!!!!pop");
                    var c=this.columns[this.columns.Count-1];
                    this.columns.Remove(this.columns[this.columns.Count-1]);
                    if(this.ifOnlyColumn(c.column)==0){
                        //只有一份c.column
                        //c.column. GetComponent('columnScr').cancelLight();
                    }
                }
                else{
                    this.columns[this.columns.Count-1].rad+=this.radTwoPOnR(this.columns[this.columns.Count-1].preTangentWorldPos,this.newTangentP,this.columns[this.columns.Count-1].columnWorldPos);
                    this.columns[this.columns.Count-1].preTangentWorldPos=this.newTangentP;
                }
            }
            
        
        }
        else{
            if(this.columns.Count>1){

                this.newTangentP=this.CalcQieDian(this.columns[this.columns.Count-1].columnWorldPos,transform.position,this.columns[this.columns.Count-1].column.transform.localScale.x/10,this.columns[this.columns.Count-1].preTangentWorldPos);
                if(this.columns[this.columns.Count-1].rad*(this.columns[this.columns.Count-1].rad+this.radTwoPOnR(this.columns[this.columns.Count-1].preTangentWorldPos,this.newTangentP,this.columns[this.columns.Count-1].columnWorldPos))<0){
                    //console.log("!!!!!pop");
                    var c=this.columns[this.columns.Count-1];
                    this.columns.Remove(this.columns[this.columns.Count-1]);
                    if(this.ifOnlyColumn(c.column)==0){
                        //只有一份c.column
                        //c.column. getComponent('columnScr').cancelLight();
                    }
                }
                else{
                    
                    this.columns[this.columns.Count-1].rad+=this.radTwoPOnR(this.columns[this.columns.Count-1].preTangentWorldPos,this.newTangentP,this.columns[this.columns.Count-1].columnWorldPos);
                    this.columns[this.columns.Count-1].preTangentWorldPos=this.newTangentP;
                }
            }
        }
        if(this.columns.Count>1){
            /*for(var i=1;i<this.columns.Count-1;i++){
                this.columns[i].column.getComponent('columnScr').onLight();
            }*/
            //var pz=this.columns[this.columns.Count-1].startWorldPos-v2Tov3( rotateVec2( (this.columns[this.columns.Count-1].columnWorldPos),(this.columns[this.columns.Count-1].rad/2)))+(this.columns[this.columns.Count-1].columnWorldPos);
            var pz=v2Tov3(rotateVec2( this.columns[this.columns.Count-1].startWorldPos-(this.columns[this.columns.Count-1].columnWorldPos),(this.columns[this.columns.Count-1].rad/2)))+(this.columns[this.columns.Count-1].columnWorldPos);
            var pzno=pz-(this.columns[this.columns.Count-1].columnWorldPos);

            //以下防止绳子没从柱子上下来
            if(Mathf.Abs(this.columns[this.columns.Count-1].rad)<(Math.PI*2)-0.5){
                
                if(Vector3.Dot(pzno.normalized,((this.columns[this.columns.Count-2].preTangentWorldPos-( this.columns[this.columns.Count-1].startWorldPos)).normalized))>0){
                    //console.log("!!!!!pop   dot");
                    var c=this.columns[this.columns.Count-1];
                    this.columns.Remove(this.columns[this.columns.Count-1]);
                    if(this.ifOnlyColumn(c.column)==0){
                        //只有一份c.column
                    //c.column. getComponent('columnScr').cancelLight();
                    }
                }
            }
        }   
        this.ms.Width=this.lineWidth;
        
        
        this.ms.moveTo(transform.position);
        for(int i=this.columns.Count-1;i>0;i--){
            
            
                
                
                var nor=(this.columns[i].preTangentWorldPos-this.columns[i].columnWorldPos).normalized;
                var po=this.columns[i].preTangentWorldPos+(nor*(this.lineWidth/2));
                this.ms.lineTo(new Vector2(po.x,po.y));
            
            
            //对第i个柱子画弧
            if(Mathf.Abs( this.columns[i].rad)>=Math.PI*2){
                if(this.columns[i].rad>0){
                    //顺时针绕上柱子，但线要逆时针从结点向起始点画
                    this.ms.circle(this.columns[i].columnWorldPos,this.columns[i].column.transform.localScale.x/10+ this.lineWidth/2,0,Mathf.PI*2,false);
                }
                else{
                    this.ms.circle(this.columns[i].columnWorldPos,this.columns[i].column.transform.localScale.x/10+ this.lineWidth/2,Mathf.PI*2,0,true);
                }
                
                this.ms.moveTo(this.columns[i].startWorldPos);
            }
            else if(Mathf.Abs( this.columns[i].rad)!=0){
                var strad=this.radOnePointOnCircle(this.columns[i].preTangentWorldPos,this.columns[i].columnWorldPos);
                this.ms.circle(this.columns[i].columnWorldPos,this.columns[i].column.transform.localScale.x/10+ this.lineWidth/2,strad,strad+this.columns[i].rad,this.columns[i].rad<0);
            }
            var norSt=(this.columns[i].startWorldPos-(this.columns[i].columnWorldPos)).normalized;
            var poSt=this.columns[i].startWorldPos+(norSt*(this.lineWidth/2));
            this.ms.moveTo(poSt);
        }
        this.ms.lineTo(ropeStart.position);

        //this.ms.fillColor = cc.Color.YELLOW;
        //var q=this.CalcQieDian(this.ci.parent.convertToWorldSpaceAR(this.ci.position),transform.position,20);
        //this.ms.circle(this.node.convertToNodeSpaceAR(q).x,this.node.convertToNodeSpaceAR(q).y,5);
        this.ms.stroke();
    }
    float radOnePointOnCircle(Vector2 p,Vector2 pR){
        var ra=0.0f;
        var va=p-(pR);
        var vb=new Vector2(1,0);
        ra=Vector2.SignedAngle(vb,va)/180*Mathf.PI;
        
        return ra;
    }
    public Vector2 CalcQieDian(Vector2 ptCenter,Vector2 ptOutside,float dbRadious,Vector2 alignP)
    { 
        
        var E=new Vector2(0,0);
        var F=new Vector2(0,0);
        var G=new Vector2(0,0);
        var H=new Vector2(0,0);
        var r=dbRadious;
        //1. 坐标平移到圆心ptCenter处,求园外点的新坐标E
        E=ptOutside-ptCenter;//平移变换到E
        //2. 求园与OE的交点坐标F, 相当于E的缩放变换
        var t= r /E.magnitude;  //得到缩放比例
        F= E*t;   //缩放变换到F
        var a=0.0f;
        //3. 将E旋转变换角度a到切点G，其中cos(a)=r/OF=t, 所以a=arccos(t);
        if(this.radTwoPOnR(ptOutside,alignP,ptCenter)<0){
            a= -Mathf.Acos(t);   //得到旋转角度 角度为负
        }else{
            a= Mathf.Acos(t);   //得到旋转角度  角度为正
        }
        
        G=rotateVec2(F,a);//旋转变换到G
        
        //4. 将G平移到原来的坐标下得到新坐标H
         H=G+(ptCenter);           //平移变换到H

        //5. 返回H
        return H;
        //6. 实际应用过程中，只要一个中间变量E,其他F,G,H可以不用。
    }
    float ifOnlyColumn(GameObject c){
        float _out=0.0f;
        for(var i=1;i<this.columns.Count;i++){
            if(this.columns[this.columns.Count-1].column==c){
                _out++;
                break;
            }
        }
        return _out;
    }
    float radTwoPOnR(Vector2 pA,Vector2 pB,Vector2 pR){
        var ra=0.0f;
        var va=pA-pR;
        var vb=pB-pR;
        ra=Vector2.SignedAngle(vb,va)/180*Mathf.PI;
        
        return ra;
    }
    Vector3 v2Tov3(Vector2 v){
        return new Vector3(v.x,v.y,0);
    }
    public Vector2 rotateVec2(Vector2 v,float rad){
        Vector2 _out=new Vector2(v.x*Mathf.Cos(rad)+v.y*Mathf.Sin(rad),-v.x*Mathf.Sin(rad)+v.y*Mathf.Cos(rad));
        return _out;
    }
    
}
