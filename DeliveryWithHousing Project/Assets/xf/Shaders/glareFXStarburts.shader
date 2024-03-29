    Shader "Hidden/glareFxStarburts" {
    Properties {
        _MainTex ("Input", RECT) = "white" {}
        _OrgTex ("Input", RECT) = "white" {}
        _lensDirt ("Input", RECT) = "white" {}
        _starTex ("Input", RECT) = "white" {}
        
        _blurSamples ("", Float) = 5
        _threshold ("", Float) = 0.5
        _int ("", Float) = 1.0
        _starint ("", Float) = 1.0
        _haloint ("", Float) = 1.0
    }
        SubShader {
            Pass {
                ZTest Always Cull Off ZWrite Off
                Fog { Mode off }
           
        Program "vp" {
// Vertex combos: 1
//   opengl - ALU: 11 to 11
//   d3d9 - ALU: 17 to 17, FLOW: 1 to 1
SubProgram "opengl " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
"3.0-!!ARBvp1.0
# 11 ALU
PARAM c[9] = { { 0, 1 },
		state.matrix.mvp,
		state.matrix.texture[0] };
TEMP R0;
TEMP R1;
MOV R1.zw, c[0].x;
MOV R1.xy, vertex.texcoord[0];
DP4 R0.x, R1, c[5];
DP4 R0.y, R1, c[6];
MOV result.texcoord[0].xy, R0;
MOV result.texcoord[1].xy, R0;
ADD result.texcoord[2].xy, -R0, c[0].y;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 11 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [glstate_matrix_texture0]
Vector 8 [_OrgTex_TexelSize]
"vs_3_0
; 17 ALU, 1 FLOW
dcl_position o0
dcl_texcoord0 o1
dcl_texcoord1 o2
dcl_texcoord2 o3
def c9, 1.00000000, 0.00000000, 0, 0
dcl_position0 v0
dcl_texcoord0 v1
mov r0.zw, c9.y
mov r0.xy, v1
dp4 r1.x, r0, c4
dp4 r1.y, r0, c5
mov r0.xy, r1
mov r2.z, c9.y
mov r0.zw, r1.xyxy
add r2.xy, -r1, c9.x
dp4 r1.w, v0, c3
dp4 r1.z, v0, c2
dp4 r1.y, v0, c1
dp4 r1.x, v0, c0
if_lt c8.y, r2.z
add r0.y, -r0, c9.x
endif
mov o0, r1
mov o1.xy, r0
mov o2.xy, r0.zwzw
mov o3.xy, r2
"
}

}
Program "fp" {
// Fragment combos: 1
//   opengl - ALU: 197 to 197, TEX: 34 to 34
//   d3d9 - ALU: 163 to 163, TEX: 34 to 34
SubProgram "opengl " {
Keywords { }
Matrix 0 [StarMatrix]
Float 4 [_threshold]
Float 5 [_int]
Float 6 [_starint]
Vector 7 [_chromatic]
Float 8 [_haloint]
SetTexture 0 [_OrgTex] 2D
SetTexture 1 [_MainTex] 2D
SetTexture 2 [_lensDirt] 2D
SetTexture 3 [_starTex] 2D
"3.0-!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 197 ALU, 34 TEX
PARAM c[13] = { program.local[0..8],
		{ 1, -0.5, 0, 0.1 },
		{ 0.5, 0.29890001, 0.58660001, 0.1145 },
		{ 2, 3, 4, 5 },
		{ 6, 7, 8, 9 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
ADD R0.xy, fragment.texcoord[0], c[9].y;
MUL R4.xy, R0, c[9].w;
MUL R0.xy, R4, R4;
ADD R0.x, R0, R0.y;
ADD R3.xy, R4, -fragment.texcoord[0];
ADD R3.xy, R3, c[9].x;
RSQ R0.x, R0.x;
MUL R0.xy, R0.x, R4;
MUL R1.xy, R0, c[10].x;
ADD R0.xy, R1, -fragment.texcoord[0];
ADD R1.zw, R0.xyxy, c[9].x;
MAD R0.xy, R1, c[7].y, R1.zwzw;
MAD R0.zw, R1.xyxy, c[7].z, R1;
MAD R1.xy, R1, c[7].x, R1.zwzw;
TEX R0.y, R0, texture[1], 2D;
TEX R0.x, R1, texture[1], 2D;
MUL R1.z, R0.y, c[10];
TEX R0.z, R0.zwzw, texture[1], 2D;
MAD R1.x, R0, c[10].y, R1.z;
MAD_SAT R0.w, R0.z, c[10], R1.x;
ADD R1.xyz, R0.w, -R0;
MAD R0.xyz, R1, c[4].x, R0;
MOV R0.w, c[9].x;
ADD R0.w, R0, -c[4].x;
RCP R0.w, R0.w;
ADD R1.xyz, R0, -c[4].x;
ADD R2.xy, -fragment.texcoord[0], c[9].x;
TEX R0.xyz, R2, texture[1], 2D;
MUL R1.w, R0.y, c[10].z;
MAD R1.w, R0.x, c[10].y, R1;
MAD_SAT R1.w, R0.z, c[10], R1;
ADD R2.xyz, R1.w, -R0;
MAD R0.xyz, R2, c[4].x, R0;
ADD R2.xyz, R0, -c[4].x;
MAD R0.xy, R4, c[7].y, R3;
MAD R3.zw, R4.xyxy, c[7].z, R3.xyxy;
TEX R0.y, R0, texture[1], 2D;
MAD R3.xy, R4, c[7].x, R3;
MUL_SAT R2.xyz, R0.w, R2;
MUL_SAT R1.xyz, R1, R0.w;
MAD R1.xyz, R1, c[8].x, R2;
MUL R2.xy, R4, c[11].x;
ADD R2.zw, -fragment.texcoord[0].xyxy, R2.xyxy;
ADD R2.zw, R2, c[9].x;
TEX R0.x, R3, texture[1], 2D;
MUL R0.z, R0.y, c[10];
MAD R1.w, R0.x, c[10].y, R0.z;
TEX R0.z, R3.zwzw, texture[1], 2D;
MAD_SAT R1.w, R0.z, c[10], R1;
ADD R3.xyz, R1.w, -R0;
MAD R0.xyz, R3, c[4].x, R0;
ADD R0.xyz, R0, -c[4].x;
MUL_SAT R0.xyz, R0.w, R0;
ADD R1.xyz, R1, R0;
MAD R0.xy, R2, c[7].y, R2.zwzw;
MAD R3.xy, R2, c[7].z, R2.zwzw;
TEX R0.y, R0, texture[1], 2D;
MAD R2.xy, R2, c[7].x, R2.zwzw;
TEX R0.x, R2, texture[1], 2D;
MUL R0.z, R0.y, c[10];
MAD R1.w, R0.x, c[10].y, R0.z;
TEX R0.z, R3, texture[1], 2D;
ADD R2.xy, -fragment.texcoord[0], c[10].x;
MUL R4.zw, R2.xyxy, c[9].w;
MUL R3.xy, R4.zwzw, c[11].y;
ADD R3.zw, fragment.texcoord[0].xyxy, R3.xyxy;
MAD_SAT R1.w, R0.z, c[10], R1;
ADD R2.xyz, R1.w, -R0;
MAD R0.xyz, R2, c[4].x, R0;
ADD R2.xyz, R0, -c[4].x;
MAD R0.xy, R3, c[7].y, R3.zwzw;
MAD R5.xy, R3, c[7].z, R3.zwzw;
MUL_SAT R2.xyz, R0.w, R2;
TEX R0.y, R0, texture[1], 2D;
MAD R3.xy, R3, c[7].x, R3.zwzw;
ADD R1.xyz, R1, R2;
MUL R2.xy, R4, c[11].z;
ADD R2.zw, -fragment.texcoord[0].xyxy, R2.xyxy;
ADD R2.zw, R2, c[9].x;
TEX R0.x, R3, texture[1], 2D;
MUL R0.z, R0.y, c[10];
MAD R1.w, R0.x, c[10].y, R0.z;
TEX R0.z, R5, texture[1], 2D;
MAD_SAT R1.w, R0.z, c[10], R1;
ADD R3.xyz, R1.w, -R0;
MAD R0.xyz, R3, c[4].x, R0;
ADD R0.xyz, R0, -c[4].x;
MUL_SAT R0.xyz, R0.w, R0;
ADD R1.xyz, R1, R0;
MAD R0.xy, R2, c[7].y, R2.zwzw;
MAD R3.xy, R2, c[7].z, R2.zwzw;
TEX R0.y, R0, texture[1], 2D;
MAD R2.xy, R2, c[7].x, R2.zwzw;
TEX R0.x, R2, texture[1], 2D;
MUL R0.z, R0.y, c[10];
MAD R1.w, R0.x, c[10].y, R0.z;
TEX R0.z, R3, texture[1], 2D;
MUL R3.xy, R4.zwzw, c[11].w;
ADD R3.zw, fragment.texcoord[0].xyxy, R3.xyxy;
MAD_SAT R1.w, R0.z, c[10], R1;
ADD R2.xyz, R1.w, -R0;
MAD R2.xyz, R2, c[4].x, R0;
MAD R0.xy, R3, c[7].y, R3.zwzw;
MAD R5.xy, R3, c[7].z, R3.zwzw;
TEX R0.y, R0, texture[1], 2D;
MAD R3.xy, R3, c[7].x, R3.zwzw;
ADD R2.xyz, R2, -c[4].x;
MUL_SAT R2.xyz, R0.w, R2;
ADD R1.xyz, R1, R2;
MUL R2.xy, R4.zwzw, c[12].x;
ADD R2.zw, fragment.texcoord[0].xyxy, R2.xyxy;
TEX R0.x, R3, texture[1], 2D;
MUL R0.z, R0.y, c[10];
MAD R1.w, R0.x, c[10].y, R0.z;
TEX R0.z, R5, texture[1], 2D;
MAD_SAT R1.w, R0.z, c[10], R1;
ADD R3.xyz, R1.w, -R0;
MAD R0.xyz, R3, c[4].x, R0;
ADD R0.xyz, R0, -c[4].x;
MUL_SAT R0.xyz, R0.w, R0;
ADD R1.xyz, R1, R0;
MAD R0.xy, R2, c[7].y, R2.zwzw;
MAD R3.xy, R2, c[7].z, R2.zwzw;
TEX R0.y, R0, texture[1], 2D;
MAD R2.xy, R2, c[7].x, R2.zwzw;
TEX R0.x, R2, texture[1], 2D;
MUL R0.z, R0.y, c[10];
MAD R1.w, R0.x, c[10].y, R0.z;
TEX R0.z, R3, texture[1], 2D;
MUL R3.xy, R4, c[12].y;
MAD_SAT R1.w, R0.z, c[10], R1;
ADD R2.xyz, R1.w, -R0;
MAD R0.xyz, R2, c[4].x, R0;
ADD R2.xyz, R0, -c[4].x;
MUL_SAT R2.xyz, R0.w, R2;
ADD R1.xyz, R1, R2;
MUL R2.xy, R4.zwzw, c[12].z;
ADD R3.zw, -fragment.texcoord[0].xyxy, R3.xyxy;
ADD R3.zw, R3, c[9].x;
MAD R0.xy, R3, c[7].y, R3.zwzw;
MAD R5.xy, R3, c[7].z, R3.zwzw;
TEX R0.y, R0, texture[1], 2D;
MAD R3.xy, R3, c[7].x, R3.zwzw;
TEX R0.x, R3, texture[1], 2D;
MUL R0.z, R0.y, c[10];
MAD R1.w, R0.x, c[10].y, R0.z;
TEX R0.z, R5, texture[1], 2D;
MAD_SAT R1.w, R0.z, c[10], R1;
ADD R3.xyz, R1.w, -R0;
MAD R0.xyz, R3, c[4].x, R0;
ADD R2.zw, fragment.texcoord[0].xyxy, R2.xyxy;
ADD R0.xyz, R0, -c[4].x;
MUL_SAT R0.xyz, R0.w, R0;
ADD R1.xyz, R1, R0;
MAD R3.xy, R2, c[7].y, R2.zwzw;
TEX R0.y, R3, texture[1], 2D;
MAD R3.xy, R2, c[7].x, R2.zwzw;
TEX R0.x, R3, texture[1], 2D;
MUL R0.z, R0.y, c[10];
MUL R3.xy, R4, c[12].w;
MAD R2.zw, R2.xyxy, c[7].z, R2;
MAD R1.w, R0.x, c[10].y, R0.z;
TEX R0.z, R2.zwzw, texture[1], 2D;
ADD R2.xy, -fragment.texcoord[0], R3;
ADD R3.zw, R2.xyxy, c[9].x;
MAD R2.xy, R3, c[7].y, R3.zwzw;
MAD R2.zw, R3.xyxy, c[7].z, R3;
MAD R3.xy, R3, c[7].x, R3.zwzw;
TEX R2.y, R2, texture[1], 2D;
TEX R2.x, R3, texture[1], 2D;
MUL R3.z, R2.y, c[10];
TEX R2.z, R2.zwzw, texture[1], 2D;
MAD R3.x, R2, c[10].y, R3.z;
MAD_SAT R2.w, R2.z, c[10], R3.x;
ADD R4.xyz, R2.w, -R2;
MAD_SAT R1.w, R0.z, c[10], R1;
ADD R3.xyz, R1.w, -R0;
MAD R0.xyz, R3, c[4].x, R0;
MAD R2.xyz, R4, c[4].x, R2;
ADD R0.xyz, R0, -c[4].x;
MUL_SAT R0.xyz, R0.w, R0;
ADD R0.xyz, R1, R0;
ADD R2.xyz, R2, -c[4].x;
MUL_SAT R2.xyz, R0.w, R2;
ADD R0.xyz, R0, R2;
MUL_SAT R2.xyz, R0, c[9].w;
MOV R1.zw, c[9].x;
MOV R1.xy, fragment.texcoord[0];
TEX R0, fragment.texcoord[0], texture[2], 2D;
DP4 R3.x, R1, c[0];
DP4 R3.y, R1, c[1];
TEX R1, R3, texture[3], 2D;
MAD R1, R1, c[6].x, R0;
MOV R2.w, c[9].z;
TEX R0, fragment.texcoord[0], texture[0], 2D;
MUL R1, R2, R1;
MAD result.color, R1, c[5].x, R0;
END
# 197 instructions, 6 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Matrix 0 [StarMatrix]
Float 4 [_threshold]
Float 5 [_int]
Float 6 [_starint]
Vector 7 [_chromatic]
Float 8 [_haloint]
SetTexture 0 [_OrgTex] 2D
SetTexture 1 [_MainTex] 2D
SetTexture 2 [_lensDirt] 2D
SetTexture 3 [_starTex] 2D
"ps_3_0
; 163 ALU, 34 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
dcl_2d s3
def c9, -0.50000000, 0.10000000, 0.50000000, 0.58660001
def c10, 0.29890001, 0.11450000, 1.00000000, 2.00000000
def c11, 3.00000000, 4.00000000, 5.00000000, 6.00000000
def c12, 7.00000000, 8.00000000, 9.00000000, 0.00000000
dcl_texcoord0 v0.xy
add r0.xy, v0, c9.x
mul r4.xy, r0, c9.y
mul_pp r0.xy, r4, r4
add_pp r0.x, r0, r0.y
add r3.xy, r4, -v0
add r3.xy, r3, c10.z
rsq_pp r0.x, r0.x
mul_pp r0.xy, r0.x, r4
mul r0.zw, r0.xyxy, c9.z
add r0.xy, r0.zwzw, -v0
add r1.zw, r0.xyxy, c10.z
mad r0.xy, r0.zwzw, c7.y, r1.zwzw
mad r2.xy, r0.zwzw, c7.x, r1.zwzw
texld r0.y, r0, s1
mad r1.xy, r0.zwzw, c7.z, r1.zwzw
texld r0.x, r2, s1
mul r0.z, r0.y, c9.w
mad r0.w, r0.x, c10.x, r0.z
texld r0.z, r1, s1
mad_sat r0.w, r0.z, c10.y, r0
add r1.xyz, r0.w, -r0
mad r1.xyz, r1, c4.x, r0
add r0.xy, -v0, c10.z
texld r0.xyz, r0, s1
mul r1.w, r0.y, c9
mad r1.w, r0.x, c10.x, r1
mad_sat r1.w, r0.z, c10.y, r1
add r2.xyz, r1.w, -r0
mad r0.xyz, r2, c4.x, r0
add r2.xyz, r0, -c4.x
mad r0.xy, r4, c7.y, r3
mad r5.xy, r4, c7.z, r3
texld r0.y, r0, s1
mad r3.xy, r4, c7.x, r3
mov r0.w, c4.x
add r0.w, c10.z, -r0
rcp r0.w, r0.w
add r1.xyz, r1, -c4.x
mul_sat r2.xyz, r0.w, r2
mul_sat r1.xyz, r1, r0.w
mad r1.xyz, r1, c8.x, r2
mul r2.xy, r4, c10.w
add r2.zw, -v0.xyxy, r2.xyxy
add r2.zw, r2, c10.z
texld r0.x, r3, s1
mul r0.z, r0.y, c9.w
mad r1.w, r0.x, c10.x, r0.z
texld r0.z, r5, s1
mad_sat r1.w, r0.z, c10.y, r1
add r3.xyz, r1.w, -r0
mad r0.xyz, r3, c4.x, r0
add r0.xyz, r0, -c4.x
mul_sat r0.xyz, r0.w, r0
add r1.xyz, r1, r0
mad r0.xy, r2, c7.y, r2.zwzw
mad r3.xy, r2, c7.z, r2.zwzw
texld r0.y, r0, s1
mad r2.xy, r2, c7.x, r2.zwzw
texld r0.x, r2, s1
mul r0.z, r0.y, c9.w
mad r1.w, r0.x, c10.x, r0.z
texld r0.z, r3, s1
add_pp r2.xy, -v0, c9.z
mul r4.zw, r2.xyxy, c9.y
mul r3.xy, r4.zwzw, c11.x
add_pp r3.zw, v0.xyxy, r3.xyxy
mad_sat r1.w, r0.z, c10.y, r1
add r2.xyz, r1.w, -r0
mad r0.xyz, r2, c4.x, r0
add r2.xyz, r0, -c4.x
mad r0.xy, r3, c7.y, r3.zwzw
mad r5.xy, r3, c7.z, r3.zwzw
mul_sat r2.xyz, r0.w, r2
texld r0.y, r0, s1
mad r3.xy, r3, c7.x, r3.zwzw
add r1.xyz, r1, r2
mul r2.xy, r4, c11.y
add r2.zw, -v0.xyxy, r2.xyxy
add r2.zw, r2, c10.z
texld r0.x, r3, s1
mul r0.z, r0.y, c9.w
mad r1.w, r0.x, c10.x, r0.z
texld r0.z, r5, s1
mad_sat r1.w, r0.z, c10.y, r1
add r3.xyz, r1.w, -r0
mad r0.xyz, r3, c4.x, r0
add r0.xyz, r0, -c4.x
mul_sat r0.xyz, r0.w, r0
add r1.xyz, r1, r0
mad r0.xy, r2, c7.y, r2.zwzw
mad r3.xy, r2, c7.z, r2.zwzw
texld r0.y, r0, s1
mad r2.xy, r2, c7.x, r2.zwzw
texld r0.x, r2, s1
mul r0.z, r0.y, c9.w
mad r1.w, r0.x, c10.x, r0.z
texld r0.z, r3, s1
mul r3.xy, r4.zwzw, c11.z
add_pp r3.zw, v0.xyxy, r3.xyxy
mad_sat r1.w, r0.z, c10.y, r1
add r2.xyz, r1.w, -r0
mad r2.xyz, r2, c4.x, r0
mad r0.xy, r3, c7.y, r3.zwzw
mad r5.xy, r3, c7.z, r3.zwzw
texld r0.y, r0, s1
mad r3.xy, r3, c7.x, r3.zwzw
add r2.xyz, r2, -c4.x
mul_sat r2.xyz, r0.w, r2
add r1.xyz, r1, r2
mul r2.xy, r4.zwzw, c11.w
add_pp r2.zw, v0.xyxy, r2.xyxy
texld r0.x, r3, s1
mul r0.z, r0.y, c9.w
mad r1.w, r0.x, c10.x, r0.z
texld r0.z, r5, s1
mad_sat r1.w, r0.z, c10.y, r1
add r3.xyz, r1.w, -r0
mad r0.xyz, r3, c4.x, r0
add r0.xyz, r0, -c4.x
mul_sat r0.xyz, r0.w, r0
add r1.xyz, r1, r0
mad r0.xy, r2, c7.y, r2.zwzw
mad r3.xy, r2, c7.z, r2.zwzw
texld r0.y, r0, s1
mad r2.xy, r2, c7.x, r2.zwzw
texld r0.x, r2, s1
mul r0.z, r0.y, c9.w
mad r1.w, r0.x, c10.x, r0.z
texld r0.z, r3, s1
mul r3.xy, r4, c12.x
mad_sat r1.w, r0.z, c10.y, r1
add r2.xyz, r1.w, -r0
mad r0.xyz, r2, c4.x, r0
add r2.xyz, r0, -c4.x
mul_sat r2.xyz, r0.w, r2
add r1.xyz, r1, r2
mul r2.xy, r4.zwzw, c12.y
add r3.zw, -v0.xyxy, r3.xyxy
add r3.zw, r3, c10.z
mad r0.xy, r3, c7.y, r3.zwzw
texld r0.y, r0, s1
mad r5.xy, r3, c7.z, r3.zwzw
mad r3.xy, r3, c7.x, r3.zwzw
add_pp r2.zw, v0.xyxy, r2.xyxy
texld r0.x, r3, s1
mul r0.z, r0.y, c9.w
mad r1.w, r0.x, c10.x, r0.z
texld r0.z, r5, s1
mad_sat r1.w, r0.z, c10.y, r1
add r3.xyz, r1.w, -r0
mad r0.xyz, r3, c4.x, r0
add r0.xyz, r0, -c4.x
mul_sat r0.xyz, r0.w, r0
add r1.xyz, r1, r0
mad r3.xy, r2, c7.y, r2.zwzw
texld r0.y, r3, s1
mad r3.xy, r2, c7.x, r2.zwzw
texld r0.x, r3, s1
mul r0.z, r0.y, c9.w
mul r3.xy, r4, c12.z
mad r2.xy, r2, c7.z, r2.zwzw
mad r1.w, r0.x, c10.x, r0.z
texld r0.z, r2, s1
add r2.zw, -v0.xyxy, r3.xyxy
add r2.zw, r2, c10.z
mad r2.xy, r3, c7.y, r2.zwzw
mad r4.xy, r3, c7.z, r2.zwzw
mad r3.xy, r3, c7.x, r2.zwzw
texld r2.y, r2, s1
texld r2.x, r3, s1
mad_sat r1.w, r0.z, c10.y, r1
add r3.xyz, r1.w, -r0
mul r2.z, r2.y, c9.w
mad r2.w, r2.x, c10.x, r2.z
texld r2.z, r4, s1
mad_sat r2.w, r2.z, c10.y, r2
add r4.xyz, r2.w, -r2
mad r0.xyz, r3, c4.x, r0
mad r2.xyz, r4, c4.x, r2
add r0.xyz, r0, -c4.x
mul_sat r0.xyz, r0.w, r0
add r0.xyz, r1, r0
add r2.xyz, r2, -c4.x
mul_sat r2.xyz, r0.w, r2
add r0.xyz, r0, r2
mul_sat r2.xyz, r0, c9.y
mov_pp r0.zw, c10.z
mov_pp r0.xy, v0
texld r1, v0, s2
dp4 r3.x, r0, c0
dp4 r3.y, r0, c1
texld r0, r3, s3
mad r1, r0, c6.x, r1
mov r2.w, c12
texld r0, v0, s0
mul r1, r2, r1
mad oC0, r1, c5.x, r0
"
}

}

#LINE 171

            }
        }
    }