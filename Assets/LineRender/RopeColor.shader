Shader "LineRenderSys/RopeColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SecondTex("SecondTex",2D) = "white"{}
        _Angle("Angle" , float) =  90
        _Height("Height",range(-0.5,1.2)) = 1
        _FadeFactor("FadeFactor",range(0,0.15)) = 0
        
        _ColorIndex("ColorIndex",float) = 1
        _TimeSpeed ("TimeSpeed",float) = 1

        _TintColor("TintColot" , Color)=  (1,1,1,1)
        
        
        [Toggle(ENABLESECOND)] _EnableSecond("EnableSecond", float) = 0
    }
    SubShader
    {
        Tags 
        {
            "RenderType"="Transparent"
            "Queue"  ="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
        }

        Lighting off
        Zwrite off
        Cull off

        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile  __ ENABLESECOND
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color:COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color:COLOR;

            };
            //声明带有采样器的
            UNITY_DECLARE_TEX2D(_MainTex);
            float4 _MainTex_TexelSize;
            //没有卵用
            // UNITY_DECLARE_TEX2D_NOSAMPLER(_SecondTex);
            
            sampler2D _SecondTex;
            fixed4 _TintColor;
            float _Angle;
            float _Height;
            float _FadeFactor;
            float _ColorIndex;
            float _TimeSpeed;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            static fixed fadeCol = (0,0,0,0);
            fixed4 frag (v2f i) : SV_Target
            {
                float outS;
                float outC;
                sincos(_Angle,  outS,outC );
                float2x2 rotate = 
                {
                    outC,-outS,
                    outS,outC,
                } ;
                i.uv = mul(rotate,i.uv );

                
                fixed4 col_1 = UNITY_SAMPLE_TEX2D(_MainTex, i.uv);
                fixed4 col_2 = tex2D(_SecondTex,i.uv);
                float2 uv_2 =  i.uv;
                
                // float isNearTop =  step(_Height, uv_2.y);

                float isNearTop =smoothstep(_Height-_FadeFactor, _Height+_FadeFactor,uv_2.y);
                
                


                fixed4 finalcol = lerp(fadeCol ,  col_1, isNearTop) ;

                ///开启当松开一个按钮的时候 应该选择哪种颜色

                #ifdef ENABLESECOND
                    //_TimeSpeed += unity_DeltaTime.x;
                    float tempTime = _TimeSpeed;
                    float time =  tempTime;
                    float isNearTop_1 =smoothstep(time-_FadeFactor, time+_FadeFactor,uv_2.y);
                    fixed4 col_3 = lerp(col_1,  col_2,_ColorIndex);
                    finalcol  = lerp( col_3,   finalcol,isNearTop_1);
                #endif


                return finalcol*i.color;
            }
            ENDCG
        }
    }
}
