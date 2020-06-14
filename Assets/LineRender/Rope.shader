Shader "LineRenderSys/Rope"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Angle("Angle",float) =0
        _TintColor("TintColor",Color)= (1,1,1,1)
        _FlipRate("FlipRate",float) = 3
        [Toggle(ENABLEFLIP)]_EnableFlipXAni("EnableFlipX", float) = 0
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
            #pragma multi_compile  __ ENABLEFLIP
            
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

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            //没有卵用
            // UNITY_DECLARE_TEX2D_NOSAMPLER(_SecondTex);

            float _FlipRate;
            fixed4 _TintColor;
            float _Angle;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color*_TintColor;
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

                fixed4 col_1 = (0,0,0,0);

                #ifdef ENABLEFLIP
                    //如果 time =0  就进行一次动画
                    
                    float time =  fmod( floor(_Time.y*_FlipRate), 2);

                    if(time==0)
                    {
                         i.uv.x = 1 - i.uv.x ;
                    }
                    
                #endif

                col_1 = tex2D(_MainTex, i.uv);
                
                return col_1*i.color;
            }
            ENDCG
        }
    }
}
