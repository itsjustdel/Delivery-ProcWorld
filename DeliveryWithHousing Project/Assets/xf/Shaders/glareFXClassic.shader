    Shader "Hidden/glareFxClassic" {
    Properties {
        _MainTex ("Input", RECT) = "white" {}
        _OrgTex ("Input", RECT) = "white" {}
        _lensDirt ("Input", RECT) = "white" {}
        
        _threshold ("", Float) = 0.5
        _int ("", Float) = 1.0
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

#define SHADER_API_GLES 1
#define SHADER_API_MOBILE 1
float xll_saturate( float x) {
  return clamp( x, 0.0, 1.0);
}
vec2 xll_saturate( vec2 x) {
  return clamp( x, 0.0, 1.0);
}
vec3 xll_saturate( vec3 x) {
  return clamp( x, 0.0, 1.0);
}
vec4 xll_saturate( vec4 x) {
  return clamp( x, 0.0, 1.0);
}
mat2 xll_saturate(mat2 m) {
  return mat2( clamp(m[0], 0.0, 1.0), clamp(m[1], 0.0, 1.0));
}
mat3 xll_saturate(mat3 m) {
  return mat3( clamp(m[0], 0.0, 1.0), clamp(m[1], 0.0, 1.0), clamp(m[2], 0.0, 1.0));
}
mat4 xll_saturate(mat4 m) {
  return mat4( clamp(m[0], 0.0, 1.0), clamp(m[1], 0.0, 1.0), clamp(m[2], 0.0, 1.0), clamp(m[3], 0.0, 1.0));
}
#line 136
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 172
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 166
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 325
struct v2f_img2 {
    highp vec4 pos;
    mediump vec2 uv;
    mediump vec2 uv2;
    mediump vec2 uv2flip;
};
uniform sampler2D _MainTex;
uniform sampler2D _OrgTex;
uniform mediump float _haloint;
uniform mediump float _int;
uniform sampler2D _lensDirt;
uniform mediump float _threshold;
highp vec4 highpass( in highp vec3 sample, in highp float threshold );
highp vec3 thresholdColor( in highp vec3 c );
mediump vec4 frag( in v2f_img2 i );
#line 309
highp vec4 highpass( in highp vec3 sample, in highp float threshold ) {
    highp vec3 luminanceFilter = vec3(0.298900, 0.586600, 0.114500);
    highp float normalizationFactor;
    highp float greyLevel;
    highp vec3 desaturated;
    #line 315
    normalizationFactor = (1.00000 / (1.00000 - threshold));
    greyLevel = float( xll_saturate((sample * mat3( luminanceFilter))));
    desaturated = mix( sample, vec3( greyLevel), vec3( threshold));
    return vec4( xll_saturate(((desaturated - threshold) * normalizationFactor)), 1.00000);
}
#line 320
highp vec3 thresholdColor( in highp vec3 c ) {
    c.xyz = vec3( highpass( c.xyz, _threshold));
    return c;
}
#line 342
mediump vec4 frag( in v2f_img2 i ) {
    highp vec4 color;
    mediump float side[10];
    mediump vec2 sample_vector;
    mediump vec2 sample_vector2;
    mediump vec2 sample_vector3;
    mediump vec2 halo_vector;
    highp vec3 result;
    highp int n = 0;
    mediump vec2 _offset;
    mediump vec2 _offset2;
    highp vec3 t;
    highp vec3 t2;
    highp vec3 t3;
    color = texture2D( _OrgTex, i.uv);
    #line 346
    side[0] = 0.000000;
    side[1] = 0.000000;
    side[2] = 0.000000;
    side[3] = 1.00000;
    #line 350
    side[4] = 0.000000;
    side[5] = 1.00000;
    side[6] = 1.00000;
    side[7] = 0.000000;
    #line 354
    side[8] = 1.00000;
    side[9] = 0.000000;
    sample_vector = ((vec2( 0.500000, 0.500000) - i.uv2flip) / 10.0000);
    sample_vector2 = ((vec2( 0.500000, 0.500000) - i.uv2) / 10.0000);
    #line 358
    sample_vector3 = (vec2( 0.500000, 0.500000) - i.uv2flip);
    halo_vector = (normalize(sample_vector) * 0.500000);
    result = (thresholdColor( texture2D( _MainTex, (i.uv2flip + halo_vector)).xyz) * _haloint);
    for ( ; (n < 10); (n++)) {
        #line 365
        _offset = (sample_vector * float(n));
        _offset2 = (sample_vector2 * float(n));
        t = thresholdColor( texture2D( _MainTex, (i.uv2flip + _offset)).xyz);
        t2 = thresholdColor( texture2D( _MainTex, (i.uv2 + _offset2)).xyz);
        #line 369
        t3 = mix( t, t2, vec3( side[n]));
        result += t3;
    }
    result *= 0.100000;
    #line 373
    result = xll_saturate(result);
    return (color + ((vec4( result.xyz, 0.000000) * texture2D( _lensDirt, i.uv2)) * _int));
}
varying mediump vec2 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD2;
void main() {
    mediump vec4 xl_retval;
    v2f_img2 xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.uv = vec2( xlv_TEXCOORD0);
    xlt_i.uv2 = vec2( xlv_TEXCOORD1);
    xlt_i.uv2flip = vec2( xlv_TEXCOORD2);
    xl_retval = frag( xlt_i);
    gl_FragData[0] = vec4( xl_retval);
}
/* NOTE: GLSL optimization failed
0:64(44): error: too few components to construct `mat3'
0:64(66): error: Operands to arithmetic operators must be numeric
0:0(0): error: no matching function for call to `xll_saturate()'
0:0(0): error: candidates are: float xll_saturate(float)
0:0(0): error:                 vec2 xll_saturate(vec2)
0:0(0): error:                 vec3 xll_saturate(vec3)
0:0(0): error:                 vec4 xll_saturate(vec4)
0:0(0): error:                 mat2 xll_saturate(mat2)
0:0(0): error:                 mat3 xll_saturate(mat3)
0:0(0): error:                 mat4 xll_saturate(mat4)
0:64(14): error: cannot construct `float' from a non-numeric data type
*/


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

#define SHADER_API_GLES 1
#define SHADER_API_DESKTOP 1
float xll_saturate( float x) {
  return clamp( x, 0.0, 1.0);
}
vec2 xll_saturate( vec2 x) {
  return clamp( x, 0.0, 1.0);
}
vec3 xll_saturate( vec3 x) {
  return clamp( x, 0.0, 1.0);
}
vec4 xll_saturate( vec4 x) {
  return clamp( x, 0.0, 1.0);
}
mat2 xll_saturate(mat2 m) {
  return mat2( clamp(m[0], 0.0, 1.0), clamp(m[1], 0.0, 1.0));
}
mat3 xll_saturate(mat3 m) {
  return mat3( clamp(m[0], 0.0, 1.0), clamp(m[1], 0.0, 1.0), clamp(m[2], 0.0, 1.0));
}
mat4 xll_saturate(mat4 m) {
  return mat4( clamp(m[0], 0.0, 1.0), clamp(m[1], 0.0, 1.0), clamp(m[2], 0.0, 1.0), clamp(m[3], 0.0, 1.0));
}
#line 136
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 172
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 166
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 328
struct v2f_img2 {
    highp vec4 pos;
    mediump vec2 uv;
    mediump vec2 uv2;
    mediump vec2 uv2flip;
};
uniform sampler2D _MainTex;
uniform sampler2D _OrgTex;
uniform mediump float _haloint;
uniform mediump float _int;
uniform sampler2D _lensDirt;
uniform mediump float _threshold;
highp vec4 highpass( in highp vec3 sample, in highp float threshold );
highp vec3 thresholdColor( in highp vec3 c );
mediump vec4 frag( in v2f_img2 i );
#line 312
highp vec4 highpass( in highp vec3 sample, in highp float threshold ) {
    highp vec3 luminanceFilter = vec3(0.298900, 0.586600, 0.114500);
    highp float normalizationFactor;
    highp float greyLevel;
    highp vec3 desaturated;
    #line 318
    normalizationFactor = (1.00000 / (1.00000 - threshold));
    greyLevel = float( xll_saturate((sample * mat3( luminanceFilter))));
    desaturated = mix( sample, vec3( greyLevel), vec3( threshold));
    return vec4( xll_saturate(((desaturated - threshold) * normalizationFactor)), 1.00000);
}
#line 323
highp vec3 thresholdColor( in highp vec3 c ) {
    c.xyz = vec3( highpass( c.xyz, _threshold));
    return c;
}
#line 345
mediump vec4 frag( in v2f_img2 i ) {
    highp vec4 color;
    mediump float side[10];
    mediump vec2 sample_vector;
    mediump vec2 sample_vector2;
    mediump vec2 sample_vector3;
    mediump vec2 halo_vector;
    highp vec3 result;
    highp int n = 0;
    mediump vec2 _offset;
    mediump vec2 _offset2;
    highp vec3 t;
    highp vec3 t2;
    highp vec3 t3;
    color = texture2D( _OrgTex, i.uv);
    #line 349
    side[0] = 0.000000;
    side[1] = 0.000000;
    side[2] = 0.000000;
    side[3] = 1.00000;
    #line 353
    side[4] = 0.000000;
    side[5] = 1.00000;
    side[6] = 1.00000;
    side[7] = 0.000000;
    #line 357
    side[8] = 1.00000;
    side[9] = 0.000000;
    sample_vector = ((vec2( 0.500000, 0.500000) - i.uv2flip) / 10.0000);
    sample_vector2 = ((vec2( 0.500000, 0.500000) - i.uv2) / 10.0000);
    #line 361
    sample_vector3 = (vec2( 0.500000, 0.500000) - i.uv2flip);
    halo_vector = (normalize(sample_vector) * 0.500000);
    result = (thresholdColor( texture2D( _MainTex, (i.uv2flip + halo_vector)).xyz) * _haloint);
    for ( ; (n < 10); (n++)) {
        #line 368
        _offset = (sample_vector * float(n));
        _offset2 = (sample_vector2 * float(n));
        t = thresholdColor( texture2D( _MainTex, (i.uv2flip + _offset)).xyz);
        t2 = thresholdColor( texture2D( _MainTex, (i.uv2 + _offset2)).xyz);
        #line 372
        t3 = mix( t, t2, vec3( side[n]));
        result += t3;
    }
    result *= 0.100000;
    #line 376
    result = xll_saturate(result);
    return (color + ((vec4( result.xyz, 0.000000) * texture2D( _lensDirt, i.uv2)) * _int));
}
varying mediump vec2 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD2;
void main() {
    mediump vec4 xl_retval;
    v2f_img2 xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.uv = vec2( xlv_TEXCOORD0);
    xlt_i.uv2 = vec2( xlv_TEXCOORD1);
    xlt_i.uv2flip = vec2( xlv_TEXCOORD2);
    xl_retval = frag( xlt_i);
    gl_FragData[0] = vec4( xl_retval);
}
/* NOTE: GLSL optimization failed
0:64(44): error: too few components to construct `mat3'
0:64(66): error: Operands to arithmetic operators must be numeric
0:0(0): error: no matching function for call to `xll_saturate()'
0:0(0): error: candidates are: float xll_saturate(float)
0:0(0): error:                 vec2 xll_saturate(vec2)
0:0(0): error:                 vec3 xll_saturate(vec3)
0:0(0): error:                 vec4 xll_saturate(vec4)
0:0(0): error:                 mat2 xll_saturate(mat2)
0:0(0): error:                 mat3 xll_saturate(mat3)
0:0(0): error:                 mat4 xll_saturate(mat4)
0:64(14): error: cannot construct `float' from a non-numeric data type
*/


#endif"
}

}
Program "fp" {
// Fragment combos: 1
//   opengl - ALU: 134 to 134, TEX: 13 to 13
//   d3d9 - ALU: 121 to 121, TEX: 13 to 13
SubProgram "opengl " {
Keywords { }
Float 0 [_threshold]
Float 1 [_int]
Float 2 [_haloint]
SetTexture 0 [_OrgTex] 2D
SetTexture 1 [_MainTex] 2D
SetTexture 2 [_lensDirt] 2D
"3.0-!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 134 ALU, 13 TEX
PARAM c[7] = { program.local[0..2],
		{ 0.1, 0.5, 0.29890001, 0.58660001 },
		{ 0.1145, 1, 2, 3 },
		{ 4, 5, 6, 7 },
		{ 8, 9, 0 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
ADD R0.xy, -fragment.texcoord[2], c[3].y;
MUL R5.xy, R0, c[3].x;
ADD R0.zw, fragment.texcoord[2].xyxy, R5.xyxy;
TEX R1.xyz, R0.zwzw, texture[1], 2D;
MUL R0.xy, R5, R5;
ADD R0.x, R0, R0.y;
MUL R0.z, R1.y, c[3].w;
MAD R0.z, R1.x, c[3], R0;
MAD_SAT R0.z, R1, c[4].x, R0;
ADD R2.xyz, R0.z, -R1;
MAD R1.xyz, R2, c[0].x, R1;
ADD R2.xyz, R1, -c[0].x;
TEX R1.xyz, fragment.texcoord[2], texture[1], 2D;
RSQ R0.x, R0.x;
MUL R0.xy, R0.x, R5;
MUL R0.xy, R0, c[3].y;
ADD R0.xy, fragment.texcoord[2], R0;
TEX R0.xyz, R0, texture[1], 2D;
MUL R0.w, R0.y, c[3];
MAD R0.w, R0.x, c[3].z, R0;
MAD_SAT R0.w, R0.z, c[4].x, R0;
ADD R3.xyz, R0.w, -R0;
MUL R1.w, R1.y, c[3];
MAD R0.w, R1.x, c[3].z, R1;
MAD R0.xyz, R3, c[0].x, R0;
MAD_SAT R0.w, R1.z, c[4].x, R0;
ADD R3.xyz, R0.w, -R1;
MAD R1.xyz, R3, c[0].x, R1;
ADD R3.xy, -fragment.texcoord[1], c[3].y;
MOV R0.w, c[4].y;
ADD R0.w, R0, -c[0].x;
RCP R0.w, R0.w;
ADD R0.xyz, R0, -c[0].x;
ADD R1.xyz, R1, -c[0].x;
MUL R5.zw, R3.xyxy, c[3].x;
MUL_SAT R1.xyz, R0.w, R1;
MUL_SAT R0.xyz, R0, R0.w;
MAD R0.xyz, R0, c[2].x, R1;
MUL_SAT R1.xyz, R0.w, R2;
ADD R2.xyz, R0, R1;
MUL R0.xy, R5, c[4].z;
MUL R1.xy, R5.zwzw, c[4].w;
ADD R0.xy, fragment.texcoord[2], R0;
TEX R0.xyz, R0, texture[1], 2D;
MUL R1.w, R0.y, c[3];
MAD R1.w, R0.x, c[3].z, R1;
MAD_SAT R1.w, R0.z, c[4].x, R1;
ADD R3.xyz, R1.w, -R0;
MAD R0.xyz, R3, c[0].x, R0;
ADD R1.xy, fragment.texcoord[1], R1;
TEX R1.xyz, R1, texture[1], 2D;
MUL R2.w, R1.y, c[3];
MAD R2.w, R1.x, c[3].z, R2;
MAD_SAT R2.w, R1.z, c[4].x, R2;
ADD R4.xyz, R2.w, -R1;
MAD R1.xyz, R4, c[0].x, R1;
ADD R0.xyz, R0, -c[0].x;
MUL_SAT R0.xyz, R0.w, R0;
ADD R1.xyz, R1, -c[0].x;
MUL_SAT R1.xyz, R0.w, R1;
ADD R0.xyz, R2, R0;
ADD R2.xyz, R0, R1;
MUL R0.xy, R5, c[5].x;
MUL R1.xy, R5.zwzw, c[5].y;
ADD R0.xy, fragment.texcoord[2], R0;
TEX R0.xyz, R0, texture[1], 2D;
MUL R1.w, R0.y, c[3];
MAD R1.w, R0.x, c[3].z, R1;
MAD_SAT R1.w, R0.z, c[4].x, R1;
ADD R3.xyz, R1.w, -R0;
MAD R0.xyz, R3, c[0].x, R0;
ADD R1.xy, fragment.texcoord[1], R1;
TEX R1.xyz, R1, texture[1], 2D;
MUL R2.w, R1.y, c[3];
MAD R2.w, R1.x, c[3].z, R2;
MAD_SAT R2.w, R1.z, c[4].x, R2;
ADD R4.xyz, R2.w, -R1;
MAD R1.xyz, R4, c[0].x, R1;
ADD R0.xyz, R0, -c[0].x;
MUL_SAT R0.xyz, R0.w, R0;
ADD R1.xyz, R1, -c[0].x;
MUL_SAT R1.xyz, R0.w, R1;
ADD R0.xyz, R2, R0;
ADD R2.xyz, R0, R1;
MUL R0.xy, R5.zwzw, c[5].z;
MUL R1.xy, R5, c[5].w;
ADD R0.xy, fragment.texcoord[1], R0;
TEX R0.xyz, R0, texture[1], 2D;
MUL R1.w, R0.y, c[3];
MAD R1.w, R0.x, c[3].z, R1;
MAD_SAT R1.w, R0.z, c[4].x, R1;
ADD R3.xyz, R1.w, -R0;
MAD R0.xyz, R3, c[0].x, R0;
ADD R1.xy, fragment.texcoord[2], R1;
TEX R1.xyz, R1, texture[1], 2D;
MUL R2.w, R1.y, c[3];
MAD R2.w, R1.x, c[3].z, R2;
MAD_SAT R2.w, R1.z, c[4].x, R2;
ADD R4.xyz, R2.w, -R1;
MAD R1.xyz, R4, c[0].x, R1;
ADD R0.xyz, R0, -c[0].x;
MUL_SAT R0.xyz, R0.w, R0;
ADD R1.xyz, R1, -c[0].x;
MUL_SAT R1.xyz, R0.w, R1;
ADD R0.xyz, R2, R0;
ADD R0.xyz, R0, R1;
MUL R1.zw, R5.xyxy, c[6].y;
ADD R2.xy, fragment.texcoord[2], R1.zwzw;
TEX R2.xyz, R2, texture[1], 2D;
MUL R2.w, R2.y, c[3];
MAD R2.w, R2.x, c[3].z, R2;
MAD_SAT R2.w, R2.z, c[4].x, R2;
ADD R4.xyz, R2.w, -R2;
MUL R1.xy, R5.zwzw, c[6].x;
ADD R1.xy, fragment.texcoord[1], R1;
TEX R1.xyz, R1, texture[1], 2D;
MUL R1.w, R1.y, c[3];
MAD R1.w, R1.x, c[3].z, R1;
MAD_SAT R1.w, R1.z, c[4].x, R1;
ADD R3.xyz, R1.w, -R1;
MAD R1.xyz, R3, c[0].x, R1;
MAD R2.xyz, R4, c[0].x, R2;
ADD R1.xyz, R1, -c[0].x;
MUL_SAT R1.xyz, R0.w, R1;
ADD R0.xyz, R0, R1;
ADD R2.xyz, R2, -c[0].x;
MUL_SAT R2.xyz, R0.w, R2;
ADD R0.xyz, R0, R2;
MUL_SAT R2.xyz, R0, c[3].x;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[1], texture[2], 2D;
MOV R2.w, c[6].z;
MUL R1, R2, R1;
MAD result.color, R1, c[1].x, R0;
END
# 134 instructions, 6 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Float 0 [_threshold]
Float 1 [_int]
Float 2 [_haloint]
SetTexture 0 [_OrgTex] 2D
SetTexture 1 [_MainTex] 2D
SetTexture 2 [_lensDirt] 2D
"ps_3_0
; 121 ALU, 13 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c3, 0.50000000, 0.10000000, 1.00000000, 0.58660001
def c4, 0.29890001, 0.11450000, 2.00000000, 3.00000000
def c5, 4.00000000, 5.00000000, 6.00000000, 7.00000000
def c6, 8.00000000, 9.00000000, 0.00000000, 0
dcl_texcoord0 v0.xy
dcl_texcoord1 v1.xy
dcl_texcoord2 v2.xy
add_pp r0.xy, -v2, c3.x
mul r5.xy, r0, c3.y
add_pp r0.xy, v2, r5
mul_pp r0.zw, r5.xyxy, r5.xyxy
texld r1.xyz, r0, s1
add_pp r0.z, r0, r0.w
rsq_pp r0.x, r0.z
mul r0.z, r1.y, c3.w
mad r0.z, r1.x, c4.x, r0
mad_sat r0.z, r1, c4.y, r0
add r2.xyz, r0.z, -r1
mad r1.xyz, r2, c0.x, r1
add r2.xyz, r1, -c0.x
texld r1.xyz, v2, s1
mul_pp r0.xy, r0.x, r5
mul r0.xy, r0, c3.x
add_pp r0.xy, v2, r0
texld r0.xyz, r0, s1
mul r0.w, r0.y, c3
mad r0.w, r0.x, c4.x, r0
mad_sat r0.w, r0.z, c4.y, r0
add r3.xyz, r0.w, -r0
mul r1.w, r1.y, c3
mad r0.w, r1.x, c4.x, r1
mad r0.xyz, r3, c0.x, r0
mad_sat r0.w, r1.z, c4.y, r0
add r3.xyz, r0.w, -r1
mad r1.xyz, r3, c0.x, r1
add_pp r3.xy, -v1, c3.x
mov r0.w, c0.x
add r0.w, c3.z, -r0
rcp r0.w, r0.w
add r0.xyz, r0, -c0.x
add r1.xyz, r1, -c0.x
mul r5.zw, r3.xyxy, c3.y
mul_sat r1.xyz, r0.w, r1
mul_sat r0.xyz, r0, r0.w
mad r0.xyz, r0, c2.x, r1
mul_sat r1.xyz, r0.w, r2
add r2.xyz, r0, r1
mul r0.xy, r5, c4.z
mul r1.xy, r5.zwzw, c4.w
add_pp r0.xy, v2, r0
texld r0.xyz, r0, s1
mul r1.w, r0.y, c3
mad r1.w, r0.x, c4.x, r1
mad_sat r1.w, r0.z, c4.y, r1
add r3.xyz, r1.w, -r0
mad r0.xyz, r3, c0.x, r0
add_pp r1.xy, v1, r1
texld r1.xyz, r1, s1
mul r2.w, r1.y, c3
mad r2.w, r1.x, c4.x, r2
mad_sat r2.w, r1.z, c4.y, r2
add r4.xyz, r2.w, -r1
mad r1.xyz, r4, c0.x, r1
add r0.xyz, r0, -c0.x
mul_sat r0.xyz, r0.w, r0
add r1.xyz, r1, -c0.x
mul_sat r1.xyz, r0.w, r1
add r0.xyz, r2, r0
add r2.xyz, r0, r1
mul r0.xy, r5, c5.x
mul r1.xy, r5.zwzw, c5.y
add_pp r0.xy, v2, r0
texld r0.xyz, r0, s1
mul r1.w, r0.y, c3
mad r1.w, r0.x, c4.x, r1
mad_sat r1.w, r0.z, c4.y, r1
add r3.xyz, r1.w, -r0
mad r0.xyz, r3, c0.x, r0
add_pp r1.xy, v1, r1
texld r1.xyz, r1, s1
mul r2.w, r1.y, c3
mad r2.w, r1.x, c4.x, r2
mad_sat r2.w, r1.z, c4.y, r2
add r4.xyz, r2.w, -r1
mad r1.xyz, r4, c0.x, r1
add r0.xyz, r0, -c0.x
mul_sat r0.xyz, r0.w, r0
add r1.xyz, r1, -c0.x
mul_sat r1.xyz, r0.w, r1
add r0.xyz, r2, r0
add r2.xyz, r0, r1
mul r0.xy, r5.zwzw, c5.z
mul r1.xy, r5, c5.w
add_pp r0.xy, v1, r0
texld r0.xyz, r0, s1
mul r1.w, r0.y, c3
mad r1.w, r0.x, c4.x, r1
mad_sat r1.w, r0.z, c4.y, r1
add r3.xyz, r1.w, -r0
mad r0.xyz, r3, c0.x, r0
add_pp r1.xy, v2, r1
texld r1.xyz, r1, s1
mul r2.w, r1.y, c3
mad r2.w, r1.x, c4.x, r2
mad_sat r2.w, r1.z, c4.y, r2
add r4.xyz, r2.w, -r1
mad r1.xyz, r4, c0.x, r1
add r0.xyz, r0, -c0.x
mul_sat r0.xyz, r0.w, r0
add r1.xyz, r1, -c0.x
mul_sat r1.xyz, r0.w, r1
add r0.xyz, r2, r0
add r0.xyz, r0, r1
mul r1.zw, r5.xyxy, c6.y
add_pp r2.xy, v2, r1.zwzw
texld r2.xyz, r2, s1
mul r2.w, r2.y, c3
mad r2.w, r2.x, c4.x, r2
mad_sat r2.w, r2.z, c4.y, r2
add r4.xyz, r2.w, -r2
mul r1.xy, r5.zwzw, c6.x
add_pp r1.xy, v1, r1
texld r1.xyz, r1, s1
mul r1.w, r1.y, c3
mad r1.w, r1.x, c4.x, r1
mad_sat r1.w, r1.z, c4.y, r1
add r3.xyz, r1.w, -r1
mad r1.xyz, r3, c0.x, r1
mad r2.xyz, r4, c0.x, r2
add r1.xyz, r1, -c0.x
mul_sat r1.xyz, r0.w, r1
add r0.xyz, r0, r1
add r2.xyz, r2, -c0.x
mul_sat r2.xyz, r0.w, r2
add r0.xyz, r0, r2
mul_sat r2.xyz, r0, c3.y
texld r1, v0, s0
texld r0, v1, s2
mov r2.w, c6.z
mul r0, r2, r0
mad oC0, r0, c1.x, r1
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

#LINE 153

            }
        }
    }