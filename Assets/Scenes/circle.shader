Shader "circle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AngleStart("angleStart",range(0,360))=0
        _AngleEnd("angleEnd",range(0,360))=360
        _Distance("Distance",Float) = 5
    	_Width("Width",Float) = 3
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always 
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 worldPosition : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _LineLength;
            float _AngleStart;
            float _AngleEnd;
            float _Distance;
            float _Width;
            fixed4 frag (v2f i) : SV_Target
            {
                float2 myuv=( i.uv  + float2(-0.5,-0.5));
                float R = length(myuv);
            	float A = atan2(myuv.y,myuv.x);
                if(A<0){
                    A+=2*3.1415926;
                }
            	float2 polar = float2(R,A);
                //定义圆环们的区域
            	if(R>0.5||R<0.25)return fixed4(1,1,1,1);
                if(_AngleEnd!=_AngleStart){
                    
                    if(A>_AngleStart/180*3.1415926&&A<_AngleEnd/180*3.1415926){
                        fixed4 col = tex2D(_MainTex,float2( -polar.y/3.1415926,polar.x*4));
                        return col;
                    }
                    
                }
                return fixed4(1,1,1,1);
            }
            ENDCG
        }
    }
}
