// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShades/BareBone"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
    }

        Subshader
    {
        Pass
        {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                uniform half4 _Color;
                struct vertexInput 
                {
                    float4 vertex : POSITION;
                };
                
                struct vertexOutput
                {
                    float4 pos: SV_POSITION;
                };

                vertexOutput(vertexInput v)
                {
                    vertexOutput o;
                    o.pos = UnityObjectToClipPos(v.vertex)
                    return o;
                }

                half4 frag(vertexOutput i) : COLOR
                {
                    return _Color;
                }
                ENDCG
        }
    }
}
