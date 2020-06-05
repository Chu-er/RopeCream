Shader "Custom/Rope"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SecondTex("SecondTex",2D) = "white"{}
        _Angle("Angle" , float) =  90
        _Height("Height",range(-0.5,1.2)) = 1
        _FadeFactor("FadeFactor",range(0,0.15)) = 0

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

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

                fixed4 col = (1,1,1,1);
                
                 col = UNITY_SAMPLE_TEX2D(_MainTex, i.uv);

                #ifdef ENABLESECOND
                //使用重用 会无法采样正确的颜色
                    // fixed4 col_2 = UNITY_SAMPLE_TEX2D_SAMPLER(_SecondTex,_MainTex,i.uv);
                    float2 uv_2 =  i.uv;
                    fixed4 col_2 = tex2D(_SecondTex,uv_2);

                    float topDist = _MainTex_TexelSize.w -_Height;

                    // float isNearTop =  step(_Height, uv_2.y);
                    float isNearTop =smoothstep(_Height-_FadeFactor, _Height+_FadeFactor,uv_2.y);
                    


                    col = lerp(col_2 ,  col, isNearTop) ;
                #endif
                return col*i.color;
            }
            ENDCG
        }
    }
}
