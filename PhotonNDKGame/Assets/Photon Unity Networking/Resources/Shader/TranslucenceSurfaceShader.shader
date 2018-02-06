Shader "Custom/TranslucenceSurfaceShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}

		SubShader{
		//描画順番の指定。
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 200

		//モデルを描画せずに、モデルを表示する場所のZ値だけを先に書き出し、そのZ値を参照して半透明のモデルを描画する。
		//Zテストで半透明の裏面は描画されないので綺麗な半透明が出来る。
		Pass{
		ZWrite ON	//デプスバッファ書き込みモードを設定。
		ColorMask 0	//モデルは描画せずにZ値だけ先に書き込む。
	}

		CGPROGRAM
		//半透明で描画出来るように指定。
#pragma surface surf Standard fullforwardshadows alpha:fade
#pragma target 3.0

		sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
	};

	fixed4 _Color;

	void surf(Input IN, inout SurfaceOutputStandard o)
	{
		//テクスチャの1ピクセルをフェッチ。
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Metallic = 0;
		o.Smoothness = 0;
		//カラーの透明度を設定。
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}