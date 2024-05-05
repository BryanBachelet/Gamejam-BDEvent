Shader "Unlit/ProgressBar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Fill Color",Color) = (1,1,1,1)
        _Fill ("Fill Value",Range(0,1)) = 1
    }
    SubShader
    {
          Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }

        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha 
        
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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Fill;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

               float InverseLerp(float a , float b ,float i)
            {
                return  (i-a)/(b-a);
            }
            

            fixed4 frag (v2f i) : SV_Target
            {
                float4 transparent  = float4(0,0,0,0);
            	float4 col = tex2D(_MainTex, i.uv);
                float colMask = col.x >0.4;
                float4 colorMask = lerp(col,transparent,colMask);

                float mask = i.uv.x < _Fill;

			
                float4 finalColor = col * colMask + transparent *(1.0f-colMask);
                float4 outColor  = lerp(colorMask, finalColor,mask);
                outColor.a =col.a* (mask + 1-col);
                return outColor;
               
            }
            ENDCG
        }
    }
}
