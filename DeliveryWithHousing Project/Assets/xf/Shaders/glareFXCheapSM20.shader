    Shader "Hidden/glareFxCheapSM20" {
    Properties {
        _MainTex ("Input", RECT) = "white" {}
        _OrgTex ("Input", RECT) = "white" {}
        _lensDirt ("Input", RECT) = "white" {}
        
        _threshold ("", Float) = 0.5
        _int ("", Float) = 1.0
    }
        SubShader {
            Pass {
                ZTest Always Cull Off ZWrite Off
                Fog { Mode off }
           
        Program "vp" {
// Vertex combos: 1
//   opengl - ALU: 11 to 11
//   d3d9 - ALU: 20 to 20
SubProgram "opengl " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
"!!ARBvp1.0
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
"vs_2_0
; 20 ALU
def c9, 0.00000000, 1.00000000, 0, 0
dcl_position0 v0
dcl_texcoord0 v1
mov r0.x, c9
slt r0.x, c8.y, r0
max r0.z, -r0.x, r0.x
slt r1.z, c9.x, r0
mov r0.xy, v1
mov r0.zw, c9.x
dp4 r1.xy, r0, c5
dp4 r0.x, r0, c4
mov r0.y, r1
add r1.w, -r1.z, c9.y
mul r1.w, r1.x, r1
add r1.x, -r1, c9.y
mad oT0.y, r1.z, r1.x, r1.w
mov oT1.xy, r0
add oT2.xy, -r0, c9.y
mov oT0.x, r0
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_TextureMatrix0 glstate_matrix_texture0
uniform mat4 glstate_matrix_texture0;

varying mediump vec2 xlv_TEXCOORD2;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;


attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  mediump vec2 tmpvar_1;
  mediump vec2 tmpvar_2;
  mediump vec2 tmpvar_3;
  highp vec2 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.zw = vec2(0.0, 0.0);
  tmpvar_5.x = _glesMultiTexCoord0.x;
  tmpvar_5.y = _glesMultiTexCoord0.y;
  tmpvar_4 = (gl_TextureMatrix0 * tmpvar_5).xy;
  tmpvar_1 = tmpvar_4;
  highp vec2 tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.zw = vec2(0.0, 0.0);
  tmpvar_7.x = _glesMultiTexCoord0.x;
  tmpvar_7.y = _glesMultiTexCoord0.y;
  tmpvar_6 = (gl_TextureMatrix0 * tmpvar_7).xy;
  tmpvar_2 = tmpvar_6;
  highp vec4 tmpvar_8;
  tmpvar_8.zw = vec2(0.0, 0.0);
  tmpvar_8.x = _glesMultiTexCoord0.x;
  tmpvar_8.y = _glesMultiTexCoord0.y;
  highp vec2 tmpvar_9;
  tmpvar_9 = (-((gl_TextureMatrix0 * tmpvar_8).xy) + 1.0);
  tmpvar_3 = tmpvar_9;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

varying mediump vec2 xlv_TEXCOORD0;
uniform mediump float _threshold;
uniform sampler2D _lensDirt;
uniform mediump float _int;
uniform sampler2D _OrgTex;
uniform sampler2D _MainTex;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 _offset2;
  mediump vec2 _offset;
  highp vec3 result;
  mediump vec2 sample_vector2;
  mediump vec2 sample_vector;
  mediump float side[7];
  highp vec4 color;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture2D (_OrgTex, xlv_TEXCOORD0);
  color = tmpvar_2;
  side[0] = 0.0;
  side[1] = 0.0;
  side[2] = 1.0;
  side[3] = 0.0;
  side[4] = 1.0;
  side[5] = 1.0;
  side[6] = 0.0;
  highp vec2 texcoords;
  texcoords = xlv_TEXCOORD0;
  highp vec2 tmpvar_3;
  tmpvar_3 = ((vec2(0.5, 0.5) - (-(texcoords) + 1.0)) / 7.0);
  sample_vector = tmpvar_3;
  sample_vector2 = ((vec2(0.5, 0.5) - xlv_TEXCOORD0) / 7.0);
  mediump vec2 tmpvar_4;
  tmpvar_4 = (normalize (sample_vector) * 0.5);
  highp vec2 tmpvar_5;
  highp vec2 texcoords_i0;
  texcoords_i0 = xlv_TEXCOORD0;
  tmpvar_5 = (-(texcoords_i0) + 1.0);
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, (tmpvar_5 + tmpvar_4));
  highp vec3 c;
  c = tmpvar_6.xyz;
  highp float tr;
  mediump float tmpvar_7;
  tmpvar_7 = pow (_threshold, 4.0);
  tr = tmpvar_7;
  c = (c - vec3(tr));
  highp vec2 tmpvar_8;
  highp vec2 texcoords_i0_i1;
  texcoords_i0_i1 = xlv_TEXCOORD0;
  tmpvar_8 = (-(texcoords_i0_i1) + 1.0);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_MainTex, tmpvar_8);
  highp vec3 c_i0;
  c_i0 = tmpvar_9.xyz;
  highp float tr_i0;
  mediump float tmpvar_10;
  tmpvar_10 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_10;
  c_i0 = (c_i0 - vec3(tr_i0));
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_MainTex, xlv_TEXCOORD0);
  highp vec3 c_i0;
  c_i0 = tmpvar_11.xyz;
  highp float tr_i0;
  mediump float tmpvar_12;
  tmpvar_12 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_12;
  c_i0 = (c_i0 - vec3(tr_i0));
  mediump vec3 tmpvar_13;
  tmpvar_13 = vec3(side[0]);
  result = (c + mix (c_i0, c_i0, tmpvar_13));
  highp vec2 tmpvar_14;
  tmpvar_14 = sample_vector;
  _offset = tmpvar_14;
  highp vec2 tmpvar_15;
  tmpvar_15 = sample_vector2;
  _offset2 = tmpvar_15;
  highp vec2 tmpvar_16;
  highp vec2 texcoords_i0_i1;
  texcoords_i0_i1 = xlv_TEXCOORD0;
  tmpvar_16 = (-(texcoords_i0_i1) + 1.0);
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture2D (_MainTex, (tmpvar_16 + _offset));
  highp vec3 c_i0;
  c_i0 = tmpvar_17.xyz;
  highp float tr_i0;
  mediump float tmpvar_18;
  tmpvar_18 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_18;
  c_i0 = (c_i0 - vec3(tr_i0));
  lowp vec4 tmpvar_19;
  tmpvar_19 = texture2D (_MainTex, (xlv_TEXCOORD0 + _offset2));
  highp vec3 c_i0;
  c_i0 = tmpvar_19.xyz;
  highp float tr_i0;
  mediump float tmpvar_20;
  tmpvar_20 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_20;
  c_i0 = (c_i0 - vec3(tr_i0));
  mediump vec3 tmpvar_21;
  tmpvar_21 = vec3(side[1]);
  result = (result + mix (c_i0, c_i0, tmpvar_21));
  highp vec2 tmpvar_22;
  tmpvar_22 = (sample_vector * 2.0);
  _offset = tmpvar_22;
  highp vec2 tmpvar_23;
  tmpvar_23 = (sample_vector2 * 2.0);
  _offset2 = tmpvar_23;
  highp vec2 tmpvar_24;
  highp vec2 texcoords_i0_i1;
  texcoords_i0_i1 = xlv_TEXCOORD0;
  tmpvar_24 = (-(texcoords_i0_i1) + 1.0);
  lowp vec4 tmpvar_25;
  tmpvar_25 = texture2D (_MainTex, (tmpvar_24 + _offset));
  highp vec3 c_i0;
  c_i0 = tmpvar_25.xyz;
  highp float tr_i0;
  mediump float tmpvar_26;
  tmpvar_26 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_26;
  c_i0 = (c_i0 - vec3(tr_i0));
  lowp vec4 tmpvar_27;
  tmpvar_27 = texture2D (_MainTex, (xlv_TEXCOORD0 + _offset2));
  highp vec3 c_i0;
  c_i0 = tmpvar_27.xyz;
  highp float tr_i0;
  mediump float tmpvar_28;
  tmpvar_28 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_28;
  c_i0 = (c_i0 - vec3(tr_i0));
  mediump vec3 tmpvar_29;
  tmpvar_29 = vec3(side[2]);
  result = (result + mix (c_i0, c_i0, tmpvar_29));
  highp vec2 tmpvar_30;
  tmpvar_30 = (sample_vector * 3.0);
  _offset = tmpvar_30;
  highp vec2 tmpvar_31;
  tmpvar_31 = (sample_vector2 * 3.0);
  _offset2 = tmpvar_31;
  highp vec2 tmpvar_32;
  highp vec2 texcoords_i0_i1;
  texcoords_i0_i1 = xlv_TEXCOORD0;
  tmpvar_32 = (-(texcoords_i0_i1) + 1.0);
  lowp vec4 tmpvar_33;
  tmpvar_33 = texture2D (_MainTex, (tmpvar_32 + _offset));
  highp vec3 c_i0;
  c_i0 = tmpvar_33.xyz;
  highp float tr_i0;
  mediump float tmpvar_34;
  tmpvar_34 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_34;
  c_i0 = (c_i0 - vec3(tr_i0));
  lowp vec4 tmpvar_35;
  tmpvar_35 = texture2D (_MainTex, (xlv_TEXCOORD0 + _offset2));
  highp vec3 c_i0;
  c_i0 = tmpvar_35.xyz;
  highp float tr_i0;
  mediump float tmpvar_36;
  tmpvar_36 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_36;
  c_i0 = (c_i0 - vec3(tr_i0));
  mediump vec3 tmpvar_37;
  tmpvar_37 = vec3(side[3]);
  result = (result + mix (c_i0, c_i0, tmpvar_37));
  highp vec2 tmpvar_38;
  tmpvar_38 = (sample_vector * 4.0);
  _offset = tmpvar_38;
  highp vec2 tmpvar_39;
  tmpvar_39 = (sample_vector2 * 4.0);
  _offset2 = tmpvar_39;
  highp vec2 tmpvar_40;
  highp vec2 texcoords_i0_i1;
  texcoords_i0_i1 = xlv_TEXCOORD0;
  tmpvar_40 = (-(texcoords_i0_i1) + 1.0);
  lowp vec4 tmpvar_41;
  tmpvar_41 = texture2D (_MainTex, (tmpvar_40 + _offset));
  highp vec3 c_i0;
  c_i0 = tmpvar_41.xyz;
  highp float tr_i0;
  mediump float tmpvar_42;
  tmpvar_42 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_42;
  c_i0 = (c_i0 - vec3(tr_i0));
  lowp vec4 tmpvar_43;
  tmpvar_43 = texture2D (_MainTex, (xlv_TEXCOORD0 + _offset2));
  highp vec3 c_i0;
  c_i0 = tmpvar_43.xyz;
  highp float tr_i0;
  mediump float tmpvar_44;
  tmpvar_44 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_44;
  c_i0 = (c_i0 - vec3(tr_i0));
  mediump vec3 tmpvar_45;
  tmpvar_45 = vec3(side[4]);
  result = (result + mix (c_i0, c_i0, tmpvar_45));
  highp vec2 tmpvar_46;
  tmpvar_46 = (sample_vector * 5.0);
  _offset = tmpvar_46;
  highp vec2 tmpvar_47;
  tmpvar_47 = (sample_vector2 * 5.0);
  _offset2 = tmpvar_47;
  highp vec2 tmpvar_48;
  highp vec2 texcoords_i0_i1;
  texcoords_i0_i1 = xlv_TEXCOORD0;
  tmpvar_48 = (-(texcoords_i0_i1) + 1.0);
  lowp vec4 tmpvar_49;
  tmpvar_49 = texture2D (_MainTex, (tmpvar_48 + _offset));
  highp vec3 c_i0;
  c_i0 = tmpvar_49.xyz;
  highp float tr_i0;
  mediump float tmpvar_50;
  tmpvar_50 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_50;
  c_i0 = (c_i0 - vec3(tr_i0));
  lowp vec4 tmpvar_51;
  tmpvar_51 = texture2D (_MainTex, (xlv_TEXCOORD0 + _offset2));
  highp vec3 c_i0;
  c_i0 = tmpvar_51.xyz;
  highp float tr_i0;
  mediump float tmpvar_52;
  tmpvar_52 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_52;
  c_i0 = (c_i0 - vec3(tr_i0));
  mediump vec3 tmpvar_53;
  tmpvar_53 = vec3(side[5]);
  result = (result + mix (c_i0, c_i0, tmpvar_53));
  highp vec2 tmpvar_54;
  tmpvar_54 = (sample_vector * 6.0);
  _offset = tmpvar_54;
  highp vec2 tmpvar_55;
  tmpvar_55 = (sample_vector2 * 6.0);
  _offset2 = tmpvar_55;
  highp vec2 tmpvar_56;
  highp vec2 texcoords_i0_i1;
  texcoords_i0_i1 = xlv_TEXCOORD0;
  tmpvar_56 = (-(texcoords_i0_i1) + 1.0);
  lowp vec4 tmpvar_57;
  tmpvar_57 = texture2D (_MainTex, (tmpvar_56 + _offset));
  highp vec3 c_i0;
  c_i0 = tmpvar_57.xyz;
  highp float tr_i0;
  mediump float tmpvar_58;
  tmpvar_58 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_58;
  c_i0 = (c_i0 - vec3(tr_i0));
  lowp vec4 tmpvar_59;
  tmpvar_59 = texture2D (_MainTex, (xlv_TEXCOORD0 + _offset2));
  highp vec3 c_i0;
  c_i0 = tmpvar_59.xyz;
  highp float tr_i0;
  mediump float tmpvar_60;
  tmpvar_60 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_60;
  c_i0 = (c_i0 - vec3(tr_i0));
  mediump vec3 tmpvar_61;
  tmpvar_61 = vec3(side[6]);
  result = (result + mix (c_i0, c_i0, tmpvar_61));
  highp vec3 tmpvar_62;
  tmpvar_62 = clamp ((result * 0.142857), 0.0, 1.0);
  result = tmpvar_62;
  highp vec4 tmpvar_63;
  tmpvar_63.w = 0.0;
  tmpvar_63.xyz = tmpvar_62;
  lowp vec4 tmpvar_64;
  tmpvar_64 = texture2D (_lensDirt, xlv_TEXCOORD0);
  tmpvar_1 = (color + ((tmpvar_63 * tmpvar_64) * _int));
  gl_FragData[0] = tmpvar_1;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_TextureMatrix0 glstate_matrix_texture0
uniform mat4 glstate_matrix_texture0;

varying mediump vec2 xlv_TEXCOORD2;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;


attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  mediump vec2 tmpvar_1;
  mediump vec2 tmpvar_2;
  mediump vec2 tmpvar_3;
  highp vec2 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.zw = vec2(0.0, 0.0);
  tmpvar_5.x = _glesMultiTexCoord0.x;
  tmpvar_5.y = _glesMultiTexCoord0.y;
  tmpvar_4 = (gl_TextureMatrix0 * tmpvar_5).xy;
  tmpvar_1 = tmpvar_4;
  highp vec2 tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.zw = vec2(0.0, 0.0);
  tmpvar_7.x = _glesMultiTexCoord0.x;
  tmpvar_7.y = _glesMultiTexCoord0.y;
  tmpvar_6 = (gl_TextureMatrix0 * tmpvar_7).xy;
  tmpvar_2 = tmpvar_6;
  highp vec4 tmpvar_8;
  tmpvar_8.zw = vec2(0.0, 0.0);
  tmpvar_8.x = _glesMultiTexCoord0.x;
  tmpvar_8.y = _glesMultiTexCoord0.y;
  highp vec2 tmpvar_9;
  tmpvar_9 = (-((gl_TextureMatrix0 * tmpvar_8).xy) + 1.0);
  tmpvar_3 = tmpvar_9;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

varying mediump vec2 xlv_TEXCOORD0;
uniform mediump float _threshold;
uniform sampler2D _lensDirt;
uniform mediump float _int;
uniform sampler2D _OrgTex;
uniform sampler2D _MainTex;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 _offset2;
  mediump vec2 _offset;
  highp vec3 result;
  mediump vec2 sample_vector2;
  mediump vec2 sample_vector;
  mediump float side[7];
  highp vec4 color;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture2D (_OrgTex, xlv_TEXCOORD0);
  color = tmpvar_2;
  side[0] = 0.0;
  side[1] = 0.0;
  side[2] = 1.0;
  side[3] = 0.0;
  side[4] = 1.0;
  side[5] = 1.0;
  side[6] = 0.0;
  highp vec2 texcoords;
  texcoords = xlv_TEXCOORD0;
  highp vec2 tmpvar_3;
  tmpvar_3 = ((vec2(0.5, 0.5) - (-(texcoords) + 1.0)) / 7.0);
  sample_vector = tmpvar_3;
  sample_vector2 = ((vec2(0.5, 0.5) - xlv_TEXCOORD0) / 7.0);
  mediump vec2 tmpvar_4;
  tmpvar_4 = (normalize (sample_vector) * 0.5);
  highp vec2 tmpvar_5;
  highp vec2 texcoords_i0;
  texcoords_i0 = xlv_TEXCOORD0;
  tmpvar_5 = (-(texcoords_i0) + 1.0);
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, (tmpvar_5 + tmpvar_4));
  highp vec3 c;
  c = tmpvar_6.xyz;
  highp float tr;
  mediump float tmpvar_7;
  tmpvar_7 = pow (_threshold, 4.0);
  tr = tmpvar_7;
  c = (c - vec3(tr));
  highp vec2 tmpvar_8;
  highp vec2 texcoords_i0_i1;
  texcoords_i0_i1 = xlv_TEXCOORD0;
  tmpvar_8 = (-(texcoords_i0_i1) + 1.0);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_MainTex, tmpvar_8);
  highp vec3 c_i0;
  c_i0 = tmpvar_9.xyz;
  highp float tr_i0;
  mediump float tmpvar_10;
  tmpvar_10 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_10;
  c_i0 = (c_i0 - vec3(tr_i0));
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_MainTex, xlv_TEXCOORD0);
  highp vec3 c_i0;
  c_i0 = tmpvar_11.xyz;
  highp float tr_i0;
  mediump float tmpvar_12;
  tmpvar_12 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_12;
  c_i0 = (c_i0 - vec3(tr_i0));
  mediump vec3 tmpvar_13;
  tmpvar_13 = vec3(side[0]);
  result = (c + mix (c_i0, c_i0, tmpvar_13));
  highp vec2 tmpvar_14;
  tmpvar_14 = sample_vector;
  _offset = tmpvar_14;
  highp vec2 tmpvar_15;
  tmpvar_15 = sample_vector2;
  _offset2 = tmpvar_15;
  highp vec2 tmpvar_16;
  highp vec2 texcoords_i0_i1;
  texcoords_i0_i1 = xlv_TEXCOORD0;
  tmpvar_16 = (-(texcoords_i0_i1) + 1.0);
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture2D (_MainTex, (tmpvar_16 + _offset));
  highp vec3 c_i0;
  c_i0 = tmpvar_17.xyz;
  highp float tr_i0;
  mediump float tmpvar_18;
  tmpvar_18 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_18;
  c_i0 = (c_i0 - vec3(tr_i0));
  lowp vec4 tmpvar_19;
  tmpvar_19 = texture2D (_MainTex, (xlv_TEXCOORD0 + _offset2));
  highp vec3 c_i0;
  c_i0 = tmpvar_19.xyz;
  highp float tr_i0;
  mediump float tmpvar_20;
  tmpvar_20 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_20;
  c_i0 = (c_i0 - vec3(tr_i0));
  mediump vec3 tmpvar_21;
  tmpvar_21 = vec3(side[1]);
  result = (result + mix (c_i0, c_i0, tmpvar_21));
  highp vec2 tmpvar_22;
  tmpvar_22 = (sample_vector * 2.0);
  _offset = tmpvar_22;
  highp vec2 tmpvar_23;
  tmpvar_23 = (sample_vector2 * 2.0);
  _offset2 = tmpvar_23;
  highp vec2 tmpvar_24;
  highp vec2 texcoords_i0_i1;
  texcoords_i0_i1 = xlv_TEXCOORD0;
  tmpvar_24 = (-(texcoords_i0_i1) + 1.0);
  lowp vec4 tmpvar_25;
  tmpvar_25 = texture2D (_MainTex, (tmpvar_24 + _offset));
  highp vec3 c_i0;
  c_i0 = tmpvar_25.xyz;
  highp float tr_i0;
  mediump float tmpvar_26;
  tmpvar_26 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_26;
  c_i0 = (c_i0 - vec3(tr_i0));
  lowp vec4 tmpvar_27;
  tmpvar_27 = texture2D (_MainTex, (xlv_TEXCOORD0 + _offset2));
  highp vec3 c_i0;
  c_i0 = tmpvar_27.xyz;
  highp float tr_i0;
  mediump float tmpvar_28;
  tmpvar_28 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_28;
  c_i0 = (c_i0 - vec3(tr_i0));
  mediump vec3 tmpvar_29;
  tmpvar_29 = vec3(side[2]);
  result = (result + mix (c_i0, c_i0, tmpvar_29));
  highp vec2 tmpvar_30;
  tmpvar_30 = (sample_vector * 3.0);
  _offset = tmpvar_30;
  highp vec2 tmpvar_31;
  tmpvar_31 = (sample_vector2 * 3.0);
  _offset2 = tmpvar_31;
  highp vec2 tmpvar_32;
  highp vec2 texcoords_i0_i1;
  texcoords_i0_i1 = xlv_TEXCOORD0;
  tmpvar_32 = (-(texcoords_i0_i1) + 1.0);
  lowp vec4 tmpvar_33;
  tmpvar_33 = texture2D (_MainTex, (tmpvar_32 + _offset));
  highp vec3 c_i0;
  c_i0 = tmpvar_33.xyz;
  highp float tr_i0;
  mediump float tmpvar_34;
  tmpvar_34 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_34;
  c_i0 = (c_i0 - vec3(tr_i0));
  lowp vec4 tmpvar_35;
  tmpvar_35 = texture2D (_MainTex, (xlv_TEXCOORD0 + _offset2));
  highp vec3 c_i0;
  c_i0 = tmpvar_35.xyz;
  highp float tr_i0;
  mediump float tmpvar_36;
  tmpvar_36 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_36;
  c_i0 = (c_i0 - vec3(tr_i0));
  mediump vec3 tmpvar_37;
  tmpvar_37 = vec3(side[3]);
  result = (result + mix (c_i0, c_i0, tmpvar_37));
  highp vec2 tmpvar_38;
  tmpvar_38 = (sample_vector * 4.0);
  _offset = tmpvar_38;
  highp vec2 tmpvar_39;
  tmpvar_39 = (sample_vector2 * 4.0);
  _offset2 = tmpvar_39;
  highp vec2 tmpvar_40;
  highp vec2 texcoords_i0_i1;
  texcoords_i0_i1 = xlv_TEXCOORD0;
  tmpvar_40 = (-(texcoords_i0_i1) + 1.0);
  lowp vec4 tmpvar_41;
  tmpvar_41 = texture2D (_MainTex, (tmpvar_40 + _offset));
  highp vec3 c_i0;
  c_i0 = tmpvar_41.xyz;
  highp float tr_i0;
  mediump float tmpvar_42;
  tmpvar_42 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_42;
  c_i0 = (c_i0 - vec3(tr_i0));
  lowp vec4 tmpvar_43;
  tmpvar_43 = texture2D (_MainTex, (xlv_TEXCOORD0 + _offset2));
  highp vec3 c_i0;
  c_i0 = tmpvar_43.xyz;
  highp float tr_i0;
  mediump float tmpvar_44;
  tmpvar_44 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_44;
  c_i0 = (c_i0 - vec3(tr_i0));
  mediump vec3 tmpvar_45;
  tmpvar_45 = vec3(side[4]);
  result = (result + mix (c_i0, c_i0, tmpvar_45));
  highp vec2 tmpvar_46;
  tmpvar_46 = (sample_vector * 5.0);
  _offset = tmpvar_46;
  highp vec2 tmpvar_47;
  tmpvar_47 = (sample_vector2 * 5.0);
  _offset2 = tmpvar_47;
  highp vec2 tmpvar_48;
  highp vec2 texcoords_i0_i1;
  texcoords_i0_i1 = xlv_TEXCOORD0;
  tmpvar_48 = (-(texcoords_i0_i1) + 1.0);
  lowp vec4 tmpvar_49;
  tmpvar_49 = texture2D (_MainTex, (tmpvar_48 + _offset));
  highp vec3 c_i0;
  c_i0 = tmpvar_49.xyz;
  highp float tr_i0;
  mediump float tmpvar_50;
  tmpvar_50 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_50;
  c_i0 = (c_i0 - vec3(tr_i0));
  lowp vec4 tmpvar_51;
  tmpvar_51 = texture2D (_MainTex, (xlv_TEXCOORD0 + _offset2));
  highp vec3 c_i0;
  c_i0 = tmpvar_51.xyz;
  highp float tr_i0;
  mediump float tmpvar_52;
  tmpvar_52 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_52;
  c_i0 = (c_i0 - vec3(tr_i0));
  mediump vec3 tmpvar_53;
  tmpvar_53 = vec3(side[5]);
  result = (result + mix (c_i0, c_i0, tmpvar_53));
  highp vec2 tmpvar_54;
  tmpvar_54 = (sample_vector * 6.0);
  _offset = tmpvar_54;
  highp vec2 tmpvar_55;
  tmpvar_55 = (sample_vector2 * 6.0);
  _offset2 = tmpvar_55;
  highp vec2 tmpvar_56;
  highp vec2 texcoords_i0_i1;
  texcoords_i0_i1 = xlv_TEXCOORD0;
  tmpvar_56 = (-(texcoords_i0_i1) + 1.0);
  lowp vec4 tmpvar_57;
  tmpvar_57 = texture2D (_MainTex, (tmpvar_56 + _offset));
  highp vec3 c_i0;
  c_i0 = tmpvar_57.xyz;
  highp float tr_i0;
  mediump float tmpvar_58;
  tmpvar_58 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_58;
  c_i0 = (c_i0 - vec3(tr_i0));
  lowp vec4 tmpvar_59;
  tmpvar_59 = texture2D (_MainTex, (xlv_TEXCOORD0 + _offset2));
  highp vec3 c_i0;
  c_i0 = tmpvar_59.xyz;
  highp float tr_i0;
  mediump float tmpvar_60;
  tmpvar_60 = pow (_threshold, 4.0);
  tr_i0 = tmpvar_60;
  c_i0 = (c_i0 - vec3(tr_i0));
  mediump vec3 tmpvar_61;
  tmpvar_61 = vec3(side[6]);
  result = (result + mix (c_i0, c_i0, tmpvar_61));
  highp vec3 tmpvar_62;
  tmpvar_62 = clamp ((result * 0.142857), 0.0, 1.0);
  result = tmpvar_62;
  highp vec4 tmpvar_63;
  tmpvar_63.w = 0.0;
  tmpvar_63.xyz = tmpvar_62;
  lowp vec4 tmpvar_64;
  tmpvar_64 = texture2D (_lensDirt, xlv_TEXCOORD0);
  tmpvar_1 = (color + ((tmpvar_63 * tmpvar_64) * _int));
  gl_FragData[0] = tmpvar_1;
}



#endif"
}

SubProgram "flash " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [glstate_matrix_texture0]
"agal_vs
c8 0.0 1.0 0.0 0.0
[bc]
aaaaaaaaabaaamacaiaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.zw, c8.x
aaaaaaaaabaaadacadaaaaoeaaaaaaaaaaaaaaaaaaaaaaaa mov r1.xy, a3
bdaaaaaaaaaaabacabaaaaoeacaaaaaaaeaaaaoeabaaaaaa dp4 r0.x, r1, c4
bdaaaaaaaaaaacacabaaaaoeacaaaaaaafaaaaoeabaaaaaa dp4 r0.y, r1, c5
aaaaaaaaaaaaadaeaaaaaafeacaaaaaaaaaaaaaaaaaaaaaa mov v0.xy, r0.xyyy
aaaaaaaaabaaadaeaaaaaafeacaaaaaaaaaaaaaaaaaaaaaa mov v1.xy, r0.xyyy
bfaaaaaaaaaaadacaaaaaafeacaaaaaaaaaaaaaaaaaaaaaa neg r0.xy, r0.xyyy
abaaaaaaacaaadaeaaaaaafeacaaaaaaaiaaaaffabaaaaaa add v2.xy, r0.xyyy, c8.y
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.zw, c0
aaaaaaaaabaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.zw, c0
aaaaaaaaacaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.zw, c0
"
}

}
Program "fp" {
// Fragment combos: 1
//   opengl - ALU: 54 to 54, TEX: 10 to 10
//   d3d9 - ALU: 48 to 48, TEX: 10 to 10
SubProgram "opengl " {
Keywords { }
Float 0 [_threshold]
Float 1 [_int]
SetTexture 0 [_OrgTex] 2D
SetTexture 1 [_MainTex] 2D
SetTexture 2 [_lensDirt] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 54 ALU, 10 TEX
PARAM c[5] = { program.local[0..1],
		{ -0.5, 0, 0.14285715, 0.5 },
		{ 1, 4, 2, 3 },
		{ 5, 6 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
TEMP R6;
TEMP R7;
TEMP R8;
TEMP R9;
ADD R0.xy, fragment.texcoord[0], c[2].x;
MUL R0.xy, R0, c[2].z;
MUL R0.zw, R0.xyxy, R0.xyxy;
ADD R0.z, R0, R0.w;
ADD R1.xy, R0, -fragment.texcoord[0];
ADD R2.zw, R1.xyxy, c[3].x;
MAD R1.xy, R0, c[3].w, -fragment.texcoord[0];
ADD R4.xy, R1, c[3].x;
RSQ R0.z, R0.z;
MUL R0.zw, R0.z, R0.xyxy;
MAD R0.zw, R0, c[2].w, -fragment.texcoord[0].xyxy;
ADD R2.xy, R0.zwzw, c[3].x;
MAD R0.xy, R0, c[4].y, -fragment.texcoord[0];
ADD R5.zw, R0.xyxy, c[3].x;
ADD R0.zw, -fragment.texcoord[0].xyxy, c[2].w;
MUL R0.zw, R0, c[2].z;
MUL R1.zw, R0, c[3].z;
ADD R3.zw, fragment.texcoord[0].xyxy, R1;
MUL R1.zw, R0, c[3].y;
ADD R4.zw, fragment.texcoord[0].xyxy, R1;
MUL R0.zw, R0, c[4].x;
ADD R5.xy, fragment.texcoord[0], R0.zwzw;
ADD R3.xy, -fragment.texcoord[0], c[3].x;
TEX R9.xyz, R5.zwzw, texture[1], 2D;
TEX R8.xyz, R5, texture[1], 2D;
TEX R5.xyz, R3.zwzw, texture[1], 2D;
TEX R7.xyz, R4.zwzw, texture[1], 2D;
TEX R6.xyz, R4, texture[1], 2D;
TEX R4.xyz, R2.zwzw, texture[1], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[0], texture[2], 2D;
TEX R2.xyz, R2, texture[1], 2D;
TEX R3.xyz, R3, texture[1], 2D;
MOV R2.w, c[3].y;
POW R2.w, c[0].x, R2.w;
ADD R3.xyz, -R2.w, R3;
ADD R2.xyz, R2, -R2.w;
ADD R2.xyz, R2, R3;
ADD R3.xyz, -R2.w, R4;
ADD R2.xyz, R2, R3;
ADD R4.xyz, -R2.w, R5;
ADD R2.xyz, R2, R4;
ADD R3.xyz, -R2.w, R6;
ADD R2.xyz, R2, R3;
ADD R4.xyz, -R2.w, R7;
ADD R2.xyz, R2, R4;
ADD R3.xyz, -R2.w, R8;
ADD R4.xyz, -R2.w, R9;
ADD R2.xyz, R2, R3;
ADD R2.xyz, R2, R4;
MUL_SAT R2.xyz, R2, c[2].z;
MOV R2.w, c[2].y;
MUL R1, R2, R1;
MAD result.color, R1, c[1].x, R0;
END
# 54 instructions, 10 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Float 0 [_threshold]
Float 1 [_int]
SetTexture 0 [_OrgTex] 2D
SetTexture 1 [_MainTex] 2D
SetTexture 2 [_lensDirt] 2D
"ps_2_0
; 48 ALU, 10 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c2, -0.50000000, 0.14285715, 0.50000000, 1.00000000
def c3, 4.00000000, 6.00000000, 2.00000000, 3.00000000
def c4, 5.00000000, 0.00000000, 0, 0
dcl t0.xy
add r0.xy, t0, c2.x
mul r1.xy, r0, c2.y
mul_pp r0.xy, r1, r1
add_pp r0.x, r0, r0.y
add r4.xy, r1, -t0
add r8.xy, r4, c2.w
mad r4.xy, r1, c3.w, -t0
add r6.xy, r4, c2.w
rsq_pp r0.x, r0.x
mul_pp r0.xy, r0.x, r1
mad r0.xy, r0, c2.z, -t0
add r0.xy, r0, c2.w
add_pp r2.xy, -t0, c2.z
mul r2.xy, r2, c2.y
mul r3.xy, r2, c3.z
add_pp r7.xy, t0, r3
mul r3.xy, r2, c3.x
add_pp r5.xy, t0, r3
mul r2.xy, r2, c4.x
add_pp r4.xy, t0, r2
mad r1.xy, r1, c3.y, -t0
add r3.xy, r1, c2.w
add r9.xy, -t0, c2.w
mov r0.w, c4.y
texld r10, r0, s1
texld r2, t0, s2
texld r1, t0, s0
texld r3, r3, s1
texld r4, r4, s1
texld r5, r5, s1
texld r6, r6, s1
texld r7, r7, s1
texld r8, r8, s1
texld r9, r9, s1
mov_pp r0.x, c3
pow_pp r11.x, c0.x, r0.x
mov_pp r0.x, r11.x
add r9.xyz, -r0.x, r9
add r10.xyz, r10, -r0.x
add r8.xyz, -r0.x, r8
add r9.xyz, r10, r9
add r7.xyz, -r0.x, r7
add r8.xyz, r9, r8
add r6.xyz, -r0.x, r6
add r7.xyz, r8, r7
add r5.xyz, -r0.x, r5
add r6.xyz, r7, r6
add r4.xyz, -r0.x, r4
add r0.xyz, -r0.x, r3
add r5.xyz, r6, r5
add r3.xyz, r5, r4
add r0.xyz, r3, r0
mul_sat r0.xyz, r0, c2.y
mul r0, r0, r2
mad r0, r0, c1.x, r1
mov_pp oC0, r0
"
}

SubProgram "gles " {
Keywords { }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES"
}

}

#LINE 140

            }
        }
    }