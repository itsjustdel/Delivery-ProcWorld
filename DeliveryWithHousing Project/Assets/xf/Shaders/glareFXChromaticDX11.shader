    Shader "Hidden/glareFxChromaricDX11" {
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
//   d3d11 - ALU: 3 to 3, TEX: 0 to 0, FLOW: 1 to 1
//   d3d11_9x - ALU: 3 to 3, TEX: 0 to 0, FLOW: 1 to 1
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

SubProgram "xbox360 " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp] 4
Matrix 4 [glstate_matrix_texture0] 2
// Shader Timing Estimate, in Cycles/64 vertex vector:
// ALU: 12.00 (9 instructions), vertex: 32, texture: 0,
//   sequencer: 10,  3 GPRs, 31 threads,
// Performance (if enough threads): ~32 cycles per vector
// * Vertex cycle estimates are assuming 3 vfetch_minis for every vfetch_full,
//     with <= 32 bytes per vfetch_full group.

"vs_360
backbbabaaaaabcmaaaaaapeaaaaaaaaaaaaaaceaaaaaalmaaaaaaoeaaaaaaaa
aaaaaaaaaaaaaajeaaaaaabmaaaaaaihpppoadaaaaaaaaacaaaaaabmaaaaaaaa
aaaaaaiaaaaaaaeeaaacaaaaaaaeaaaaaaaaaafiaaaaaaaaaaaaaagiaaacaaae
aaacaaaaaaaaaafiaaaaaaaaghgmhdhegbhegffpgngbhehcgjhifpgnhghaaakl
aaadaaadaaaeaaaeaaabaaaaaaaaaaaaghgmhdhegbhegffpgngbhehcgjhifphe
gfhihehfhcgfdaaahghdfpddfpdaaadccodacodcdadddfddcodaaaklaaaaaaaa
aaaaaaabaaaaaaaaaaaaaaaaaaaaaabeaapmaabaaaaaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaeaaaaaaaleaacbaaacaaaaaaaaaaaaaaaaaaaabigdaaaaaaab
aaaaaaacaaaaaaadaaaaacjaaabaaaadaadafaaeaaaadafaaaabdbfbaaacdcfc
aaaabaalaaaabaamaaaabaanaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadpiaaaaa
aaaaaaaaaaaaaaaaaaaaaaaadaafcaadaaaabcaamcaaaaaaaaaaeaafaaaabcaa
meaaaaaaaaaafaajaaaaccaaaaaaaaaaafpicaaaaaaaagiiaaaaaaaaafpiaaaa
aaaaaohiaaaaaaaamiapaaabaabliiaakbacadaamiapaaabaamgiiaaklacacab
miapaaabaalbdejeklacababmiapiadoaagmaadeklacaaabmiadaaaaaagmlaaa
kbaaaeaamiadaaaaaamglalaklaaafaamiadiaaaaalalaaaocaaaaaamiadiaab
aalalaaaocaaaaaamiadiaacaelagmaakaaappaaaaaaaaaaaaaaaaaaaaaaaaaa
"
}

SubProgram "ps3 " {
Keywords { }
Matrix 256 [glstate_matrix_mvp]
Matrix 260 [glstate_matrix_texture0]
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
"sce_vp_rsx // 13 instructions using 2 registers
[Configuration]
8
0000000d01010200
[Defaults]
1
467 2
000000003f800000
[Microcode]
208
00009c6c004008080106c08360419ffc00009c6c005d30000186c08360407ffc
401f9c6c01d0300d8106c0c360403f80401f9c6c01d0200d8106c0c360405f80
401f9c6c01d0100d8106c0c360409f80401f9c6c01d0000d8106c0c360411f80
00001c6c01d0500d8286c0c360409ffc00001c6c01d0400d8286c0c360411ffc
00001c6c01d0500d8286c0c360403ffc00001c6c01d0400d8286c0c360405ffc
401f9c6c00dd302a8186c0b740219fa4401f9c6c004000080086c08360419f9c
401f9c6c004000080086c08360419fa1
"
}

SubProgram "d3d11 " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
ConstBuffer "UnityPerDraw" 336 // 64 used size, 6 vars
Matrix 0 [glstate_matrix_mvp] 4
ConstBuffer "UnityPerDrawTexMatrices" 768 // 576 used size, 5 vars
Matrix 512 [glstate_matrix_texture0] 4
BindCB "UnityPerDraw" 0
BindCB "UnityPerDrawTexMatrices" 1
// 9 instructions, 1 temp regs, 0 temp arrays:
// ALU 3 float, 0 int, 0 uint
// TEX 0 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"vs_4_0
eefiecedgomjmbjccfbnhlmjfajjiindcknbacdfabaaaaaalaacaaaaadaaaaaa
cmaaaaaaiaaaaaaaaiabaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheoiaaaaaaaaeaaaaaaaiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaheaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
heaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaaheaaaaaaacaaaaaa
aaaaaaaaadaaaaaaacaaaaaaadamaaaafdfgfpfaepfdejfeejepeoaafeeffied
epepfceeaaklklklfdeieefckaabaaaaeaaaabaagiaaaaaafjaaaaaeegiocaaa
aaaaaaaaaeaaaaaafjaaaaaeegiocaaaabaaaaaaccaaaaaafpaaaaadpcbabaaa
aaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaagfaaaaaddccabaaa
acaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaa
egiocaaaaaaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaaaaaaaaa
aaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaaaaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pccabaaaaaaaaaaaegiocaaaaaaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaa
aaaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaabaaaaaaegiecaaaabaaaaaa
cbaaaaaadcaaaaakpcaabaaaaaaaaaaaegiecaaaabaaaaaacaaaaaaaagbabaaa
abaaaaaaegaobaaaaaaaaaaadgaaaaafpccabaaaabaaaaaaegaobaaaaaaaaaaa
aaaaaaaldccabaaaacaaaaaaogakbaiaebaaaaaaaaaaaaaaaceaaaaaaaaaiadp
aaaaiadpaaaaaaaaaaaaaaaadoaaaaab"
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
  vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  mediump vec2 tmpvar_2;
  mediump vec2 tmpvar_3;
  mediump vec2 tmpvar_4;
  highp vec2 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.zw = vec2(0.000000, 0.000000);
  tmpvar_6.x = tmpvar_1.x;
  tmpvar_6.y = tmpvar_1.y;
  tmpvar_5 = (gl_TextureMatrix0 * tmpvar_6).xy;
  tmpvar_2 = tmpvar_5;
  highp vec2 tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8.zw = vec2(0.000000, 0.000000);
  tmpvar_8.x = tmpvar_1.x;
  tmpvar_8.y = tmpvar_1.y;
  tmpvar_7 = (gl_TextureMatrix0 * tmpvar_8).xy;
  tmpvar_3 = tmpvar_7;
  highp vec4 tmpvar_9;
  tmpvar_9.zw = vec2(0.000000, 0.000000);
  tmpvar_9.x = tmpvar_1.x;
  tmpvar_9.y = tmpvar_1.y;
  highp vec2 tmpvar_10;
  tmpvar_10 = (-((gl_TextureMatrix0 * tmpvar_9).xy) + 1.00000);
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_4;
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
#line 150
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 186
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 180
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 338
struct v2f_img2 {
    highp vec4 pos;
    mediump vec2 uv;
    mediump vec2 uv2;
    mediump vec2 uv2flip;
};
uniform sampler2D _MainTex;
uniform sampler2D _OrgTex;
uniform highp vec3 _chromatic;
uniform mediump float _haloint;
uniform mediump float _int;
uniform sampler2D _lensDirt;
uniform mediump float _threshold;
highp vec4 highpass( in highp vec3 sample, in highp float threshold );
highp vec3 thresholdColor( in highp vec3 c );
highp vec3 textureDistorted( in sampler2D tex, in mediump vec2 sample_center, in mediump vec2 sample_vector, in highp vec3 distortion );
highp vec2 flipTexcoords( in highp vec2 texcoords );
mediump vec4 frag( in v2f_img2 i );
#line 322
highp vec4 highpass( in highp vec3 sample, in highp float threshold ) {
    highp vec3 luminanceFilter = vec3(0.2989, 0.5866, 0.1145);
    highp float normalizationFactor;
    highp float greyLevel;
    highp vec3 desaturated;
    #line 328
    normalizationFactor = (1.0 / (1.0 - threshold));
    greyLevel = float( xll_saturate((sample * mat3( luminanceFilter))));
    desaturated = mix( sample, vec3( greyLevel), vec3( threshold));
    return vec4( xll_saturate(((desaturated - threshold) * normalizationFactor)), 1.0);
}
#line 333
highp vec3 thresholdColor( in highp vec3 c ) {
    c.xyz = vec3( highpass( c.xyz, _threshold));
    return c;
}
#line 314
highp vec3 textureDistorted( in sampler2D tex, in mediump vec2 sample_center, in mediump vec2 sample_vector, in highp vec3 distortion ) {
    return vec3( texture2D( _MainTex, (sample_center + (sample_vector * distortion.x))).x, texture2D( _MainTex, (sample_center + (sample_vector * distortion.y))).y, texture2D( _MainTex, (sample_center + (sample_vector * distortion.z))).z);
}
#line 318
highp vec2 flipTexcoords( in highp vec2 texcoords ) {
    return ((-texcoords) + 1.0);
}
#line 355
mediump vec4 frag( in v2f_img2 i ) {
    highp vec4 color;
    mediump float side[10];
    mediump vec2 sample_vector;
    mediump vec2 sample_vector2;
    mediump vec2 halo_vector;
    highp vec3 result;
    highp int n = 0;
    mediump vec2 _offset;
    mediump vec2 _offset2;
    highp vec3 t;
    highp vec3 t2;
    highp vec3 t3;
    color = texture2D( _OrgTex, i.uv);
    #line 359
    side[0] = 0.0;
    side[1] = 0.0;
    side[2] = 0.0;
    side[3] = 1.0;
    #line 363
    side[4] = 0.0;
    side[5] = 1.0;
    side[6] = 1.0;
    side[7] = 0.0;
    #line 367
    side[8] = 1.0;
    side[9] = 0.0;
    sample_vector = ((vec2( 0.5, 0.5) - flipTexcoords( i.uv)) / 10.0);
    sample_vector2 = ((vec2( 0.5, 0.5) - i.uv) / 10.0);
    #line 371
    halo_vector = (normalize(sample_vector) * 0.5);
    result = (thresholdColor( textureDistorted( _MainTex, (flipTexcoords( i.uv) + halo_vector), halo_vector, _chromatic).xyz) * _haloint);
    for ( ; (n < 10); (n++)) {
        #line 377
        _offset = (sample_vector * float(n));
        _offset2 = (sample_vector2 * float(n));
        t = thresholdColor( textureDistorted( _MainTex, (flipTexcoords( i.uv) + _offset), _offset, _chromatic).xyz);
        t2 = thresholdColor( textureDistorted( _MainTex, (i.uv + _offset2), _offset2, _chromatic).xyz);
        #line 381
        t3 = mix( t, t2, vec3( side[n]));
        result += t3;
    }
    result *= 0.1;
    #line 385
    result = xll_saturate(result);
    return (color + ((vec4( result.xyz, 0.0) * texture2D( _lensDirt, i.uv)) * _int));
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
0:330(44): error: too few components to construct `mat3'
0:330(66): error: Operands to arithmetic operators must be numeric
0:0(0): error: no matching function for call to `xll_saturate()'
0:0(0): error: candidates are: float xll_saturate(float)
0:0(0): error:                 vec2 xll_saturate(vec2)
0:0(0): error:                 vec3 xll_saturate(vec3)
0:0(0): error:                 vec4 xll_saturate(vec4)
0:0(0): error:                 mat2 xll_saturate(mat2)
0:0(0): error:                 mat3 xll_saturate(mat3)
0:0(0): error:                 mat4 xll_saturate(mat4)
0:330(14): error: cannot construct `float' from a non-numeric data type
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
  vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  mediump vec2 tmpvar_2;
  mediump vec2 tmpvar_3;
  mediump vec2 tmpvar_4;
  highp vec2 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.zw = vec2(0.000000, 0.000000);
  tmpvar_6.x = tmpvar_1.x;
  tmpvar_6.y = tmpvar_1.y;
  tmpvar_5 = (gl_TextureMatrix0 * tmpvar_6).xy;
  tmpvar_2 = tmpvar_5;
  highp vec2 tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8.zw = vec2(0.000000, 0.000000);
  tmpvar_8.x = tmpvar_1.x;
  tmpvar_8.y = tmpvar_1.y;
  tmpvar_7 = (gl_TextureMatrix0 * tmpvar_8).xy;
  tmpvar_3 = tmpvar_7;
  highp vec4 tmpvar_9;
  tmpvar_9.zw = vec2(0.000000, 0.000000);
  tmpvar_9.x = tmpvar_1.x;
  tmpvar_9.y = tmpvar_1.y;
  highp vec2 tmpvar_10;
  tmpvar_10 = (-((gl_TextureMatrix0 * tmpvar_9).xy) + 1.00000);
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_4;
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
#line 150
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 186
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 180
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 341
struct v2f_img2 {
    highp vec4 pos;
    mediump vec2 uv;
    mediump vec2 uv2;
    mediump vec2 uv2flip;
};
uniform sampler2D _MainTex;
uniform sampler2D _OrgTex;
uniform highp vec3 _chromatic;
uniform mediump float _haloint;
uniform mediump float _int;
uniform sampler2D _lensDirt;
uniform mediump float _threshold;
highp vec4 highpass( in highp vec3 sample, in highp float threshold );
highp vec3 thresholdColor( in highp vec3 c );
highp vec3 textureDistorted( in sampler2D tex, in mediump vec2 sample_center, in mediump vec2 sample_vector, in highp vec3 distortion );
highp vec2 flipTexcoords( in highp vec2 texcoords );
mediump vec4 frag( in v2f_img2 i );
#line 325
highp vec4 highpass( in highp vec3 sample, in highp float threshold ) {
    highp vec3 luminanceFilter = vec3(0.2989, 0.5866, 0.1145);
    highp float normalizationFactor;
    highp float greyLevel;
    highp vec3 desaturated;
    #line 331
    normalizationFactor = (1.0 / (1.0 - threshold));
    greyLevel = float( xll_saturate((sample * mat3( luminanceFilter))));
    desaturated = mix( sample, vec3( greyLevel), vec3( threshold));
    return vec4( xll_saturate(((desaturated - threshold) * normalizationFactor)), 1.0);
}
#line 336
highp vec3 thresholdColor( in highp vec3 c ) {
    c.xyz = vec3( highpass( c.xyz, _threshold));
    return c;
}
#line 317
highp vec3 textureDistorted( in sampler2D tex, in mediump vec2 sample_center, in mediump vec2 sample_vector, in highp vec3 distortion ) {
    return vec3( texture2D( _MainTex, (sample_center + (sample_vector * distortion.x))).x, texture2D( _MainTex, (sample_center + (sample_vector * distortion.y))).y, texture2D( _MainTex, (sample_center + (sample_vector * distortion.z))).z);
}
#line 321
highp vec2 flipTexcoords( in highp vec2 texcoords ) {
    return ((-texcoords) + 1.0);
}
#line 358
mediump vec4 frag( in v2f_img2 i ) {
    highp vec4 color;
    mediump float side[10];
    mediump vec2 sample_vector;
    mediump vec2 sample_vector2;
    mediump vec2 halo_vector;
    highp vec3 result;
    highp int n = 0;
    mediump vec2 _offset;
    mediump vec2 _offset2;
    highp vec3 t;
    highp vec3 t2;
    highp vec3 t3;
    color = texture2D( _OrgTex, i.uv);
    #line 362
    side[0] = 0.0;
    side[1] = 0.0;
    side[2] = 0.0;
    side[3] = 1.0;
    #line 366
    side[4] = 0.0;
    side[5] = 1.0;
    side[6] = 1.0;
    side[7] = 0.0;
    #line 370
    side[8] = 1.0;
    side[9] = 0.0;
    sample_vector = ((vec2( 0.5, 0.5) - flipTexcoords( i.uv)) / 10.0);
    sample_vector2 = ((vec2( 0.5, 0.5) - i.uv) / 10.0);
    #line 374
    halo_vector = (normalize(sample_vector) * 0.5);
    result = (thresholdColor( textureDistorted( _MainTex, (flipTexcoords( i.uv) + halo_vector), halo_vector, _chromatic).xyz) * _haloint);
    for ( ; (n < 10); (n++)) {
        #line 380
        _offset = (sample_vector * float(n));
        _offset2 = (sample_vector2 * float(n));
        t = thresholdColor( textureDistorted( _MainTex, (flipTexcoords( i.uv) + _offset), _offset, _chromatic).xyz);
        t2 = thresholdColor( textureDistorted( _MainTex, (i.uv + _offset2), _offset2, _chromatic).xyz);
        #line 384
        t3 = mix( t, t2, vec3( side[n]));
        result += t3;
    }
    result *= 0.1;
    #line 388
    result = xll_saturate(result);
    return (color + ((vec4( result.xyz, 0.0) * texture2D( _lensDirt, i.uv)) * _int));
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
0:333(44): error: too few components to construct `mat3'
0:333(66): error: Operands to arithmetic operators must be numeric
0:0(0): error: no matching function for call to `xll_saturate()'
0:0(0): error: candidates are: float xll_saturate(float)
0:0(0): error:                 vec2 xll_saturate(vec2)
0:0(0): error:                 vec3 xll_saturate(vec3)
0:0(0): error:                 vec4 xll_saturate(vec4)
0:0(0): error:                 mat2 xll_saturate(mat2)
0:0(0): error:                 mat3 xll_saturate(mat3)
0:0(0): error:                 mat4 xll_saturate(mat4)
0:333(14): error: cannot construct `float' from a non-numeric data type
*/


#endif"
}

SubProgram "d3d11_9x " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
ConstBuffer "UnityPerDraw" 336 // 64 used size, 6 vars
Matrix 0 [glstate_matrix_mvp] 4
ConstBuffer "UnityPerDrawTexMatrices" 768 // 576 used size, 5 vars
Matrix 512 [glstate_matrix_texture0] 4
BindCB "UnityPerDraw" 0
BindCB "UnityPerDrawTexMatrices" 1
// 9 instructions, 1 temp regs, 0 temp arrays:
// ALU 3 float, 0 int, 0 uint
// TEX 0 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"vs_4_0_level_9_3
eefiecedgemidcljioagjbcpjbkdklpcnfmkagfpabaaaaaaoaadaaaaaeaaaaaa
daaaaaaafmabaaaaaeadaaaafiadaaaaebgpgodjceabaaaaceabaaaaaaacpopp
oeaaaaaaeaaaaaaaacaaceaaaaaadmaaaaaadmaaaaaaceaaabaadmaaaaaaaaaa
aeaaabaaaaaaaaaaabaacaaaacaaafaaaaaaaaaaaaaaaaaaabacpoppfbaaaaaf
ahaaapkaaaaaiadpaaaaaaaaaaaaaaaaaaaaaaaabpaaaaacafaaaaiaaaaaapja
bpaaaaacafaaabiaabaaapjaafaaaaadaaaaapiaabaaffjaagaabekaaeaaaaae
aaaaapiaafaabekaabaaaajaaaaaoeiaacaaaaadabaaadoaaaaaolibahaaaaka
abaaaaacaaaaapoaaaaaoeiaafaaaaadaaaaapiaaaaaffjaacaaoekaaeaaaaae
aaaaapiaabaaoekaaaaaaajaaaaaoeiaaeaaaaaeaaaaapiaadaaoekaaaaakkja
aaaaoeiaaeaaaaaeaaaaapiaaeaaoekaaaaappjaaaaaoeiaaeaaaaaeaaaaadma
aaaappiaaaaaoekaaaaaoeiaabaaaaacaaaaammaaaaaoeiappppaaaafdeieefc
kaabaaaaeaaaabaagiaaaaaafjaaaaaeegiocaaaaaaaaaaaaeaaaaaafjaaaaae
egiocaaaabaaaaaaccaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaa
abaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaa
gfaaaaadmccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaagiaaaaacabaaaaaa
diaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaaaaaaaaaaabaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaaaaaaaaaaaaaaaaaagbabaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaaaaaaaaaacaaaaaa
kgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaa
aaaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaadiaaaaaipcaabaaa
aaaaaaaafgbfbaaaabaaaaaaegiecaaaabaaaaaacbaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiecaaaabaaaaaacaaaaaaaagbabaaaabaaaaaaegaobaaaaaaaaaaa
dgaaaaafpccabaaaabaaaaaaegaobaaaaaaaaaaaaaaaaaaldccabaaaacaaaaaa
ogakbaiaebaaaaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaaaaaaaaaaaaa
doaaaaabejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaa
adadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklklepfdeheoiaaaaaaa
aeaaaaaaaiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaa
heaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaaheaaaaaaabaaaaaa
aaaaaaaaadaaaaaaabaaaaaaamadaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaa
acaaaaaaadamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklkl
"
}

}
Program "fp" {
// Fragment combos: 1
//   opengl - ALU: 191 to 191, TEX: 33 to 33
//   d3d9 - ALU: 158 to 158, TEX: 33 to 33
//   d3d11 - ALU: 68 to 68, TEX: 33 to 33, FLOW: 1 to 1
//   d3d11_9x - ALU: 68 to 68, TEX: 33 to 33, FLOW: 1 to 1
SubProgram "opengl " {
Keywords { }
Float 0 [_threshold]
Float 1 [_int]
Vector 2 [_chromatic]
Float 3 [_haloint]
SetTexture 0 [_OrgTex] 2D
SetTexture 1 [_MainTex] 2D
SetTexture 2 [_lensDirt] 2D
"3.0-!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 191 ALU, 33 TEX
PARAM c[8] = { program.local[0..3],
		{ -0.5, 0, 0.1, 0.5 },
		{ 1, 0.29890001, 0.58660001, 0.1145 },
		{ 2, 3, 4, 5 },
		{ 6, 7, 8, 9 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
ADD R0.xy, fragment.texcoord[0], c[4].x;
MUL R4.xy, R0, c[4].z;
MUL R0.xy, R4, R4;
ADD R0.x, R0, R0.y;
ADD R3.xy, R4, -fragment.texcoord[0];
ADD R3.xy, R3, c[5].x;
RSQ R0.x, R0.x;
MUL R0.xy, R0.x, R4;
MUL R1.xy, R0, c[4].w;
ADD R0.xy, R1, -fragment.texcoord[0];
ADD R1.zw, R0.xyxy, c[5].x;
MAD R0.xy, R1, c[2].y, R1.zwzw;
MAD R0.zw, R1.xyxy, c[2].z, R1;
MAD R1.xy, R1, c[2].x, R1.zwzw;
TEX R0.y, R0, texture[1], 2D;
TEX R0.x, R1, texture[1], 2D;
MUL R1.z, R0.y, c[5];
TEX R0.z, R0.zwzw, texture[1], 2D;
MAD R1.x, R0, c[5].y, R1.z;
MAD_SAT R0.w, R0.z, c[5], R1.x;
ADD R1.xyz, R0.w, -R0;
MAD R0.xyz, R1, c[0].x, R0;
MOV R0.w, c[5].x;
ADD R0.w, R0, -c[0].x;
RCP R0.w, R0.w;
ADD R1.xyz, R0, -c[0].x;
ADD R2.xy, -fragment.texcoord[0], c[5].x;
TEX R0.xyz, R2, texture[1], 2D;
MUL R1.w, R0.y, c[5].z;
MAD R1.w, R0.x, c[5].y, R1;
MAD_SAT R1.w, R0.z, c[5], R1;
ADD R2.xyz, R1.w, -R0;
MAD R0.xyz, R2, c[0].x, R0;
ADD R2.xyz, R0, -c[0].x;
MAD R0.xy, R4, c[2].y, R3;
MAD R3.zw, R4.xyxy, c[2].z, R3.xyxy;
TEX R0.y, R0, texture[1], 2D;
MAD R3.xy, R4, c[2].x, R3;
MUL_SAT R2.xyz, R0.w, R2;
MUL_SAT R1.xyz, R1, R0.w;
MAD R1.xyz, R1, c[3].x, R2;
MUL R2.xy, R4, c[6].x;
ADD R2.zw, -fragment.texcoord[0].xyxy, R2.xyxy;
ADD R2.zw, R2, c[5].x;
TEX R0.x, R3, texture[1], 2D;
MUL R0.z, R0.y, c[5];
MAD R1.w, R0.x, c[5].y, R0.z;
TEX R0.z, R3.zwzw, texture[1], 2D;
MAD_SAT R1.w, R0.z, c[5], R1;
ADD R3.xyz, R1.w, -R0;
MAD R0.xyz, R3, c[0].x, R0;
ADD R0.xyz, R0, -c[0].x;
MUL_SAT R0.xyz, R0.w, R0;
ADD R1.xyz, R1, R0;
MAD R0.xy, R2, c[2].y, R2.zwzw;
MAD R3.xy, R2, c[2].z, R2.zwzw;
TEX R0.y, R0, texture[1], 2D;
MAD R2.xy, R2, c[2].x, R2.zwzw;
TEX R0.x, R2, texture[1], 2D;
MUL R0.z, R0.y, c[5];
MAD R1.w, R0.x, c[5].y, R0.z;
TEX R0.z, R3, texture[1], 2D;
ADD R2.xy, -fragment.texcoord[0], c[4].w;
MUL R4.zw, R2.xyxy, c[4].z;
MUL R3.xy, R4.zwzw, c[6].y;
ADD R3.zw, fragment.texcoord[0].xyxy, R3.xyxy;
MAD_SAT R1.w, R0.z, c[5], R1;
ADD R2.xyz, R1.w, -R0;
MAD R0.xyz, R2, c[0].x, R0;
ADD R2.xyz, R0, -c[0].x;
MAD R0.xy, R3, c[2].y, R3.zwzw;
MAD R5.xy, R3, c[2].z, R3.zwzw;
MUL_SAT R2.xyz, R0.w, R2;
TEX R0.y, R0, texture[1], 2D;
MAD R3.xy, R3, c[2].x, R3.zwzw;
ADD R1.xyz, R1, R2;
MUL R2.xy, R4, c[6].z;
ADD R2.zw, -fragment.texcoord[0].xyxy, R2.xyxy;
ADD R2.zw, R2, c[5].x;
TEX R0.x, R3, texture[1], 2D;
MUL R0.z, R0.y, c[5];
MAD R1.w, R0.x, c[5].y, R0.z;
TEX R0.z, R5, texture[1], 2D;
MAD_SAT R1.w, R0.z, c[5], R1;
ADD R3.xyz, R1.w, -R0;
MAD R0.xyz, R3, c[0].x, R0;
ADD R0.xyz, R0, -c[0].x;
MUL_SAT R0.xyz, R0.w, R0;
ADD R1.xyz, R1, R0;
MAD R0.xy, R2, c[2].y, R2.zwzw;
MAD R3.xy, R2, c[2].z, R2.zwzw;
TEX R0.y, R0, texture[1], 2D;
MAD R2.xy, R2, c[2].x, R2.zwzw;
TEX R0.x, R2, texture[1], 2D;
MUL R0.z, R0.y, c[5];
MAD R1.w, R0.x, c[5].y, R0.z;
TEX R0.z, R3, texture[1], 2D;
MUL R3.xy, R4.zwzw, c[6].w;
ADD R3.zw, fragment.texcoord[0].xyxy, R3.xyxy;
MAD_SAT R1.w, R0.z, c[5], R1;
ADD R2.xyz, R1.w, -R0;
MAD R2.xyz, R2, c[0].x, R0;
MAD R0.xy, R3, c[2].y, R3.zwzw;
MAD R5.xy, R3, c[2].z, R3.zwzw;
TEX R0.y, R0, texture[1], 2D;
MAD R3.xy, R3, c[2].x, R3.zwzw;
ADD R2.xyz, R2, -c[0].x;
MUL_SAT R2.xyz, R0.w, R2;
ADD R1.xyz, R1, R2;
MUL R2.xy, R4.zwzw, c[7].x;
ADD R2.zw, fragment.texcoord[0].xyxy, R2.xyxy;
TEX R0.x, R3, texture[1], 2D;
MUL R0.z, R0.y, c[5];
MAD R1.w, R0.x, c[5].y, R0.z;
TEX R0.z, R5, texture[1], 2D;
MAD_SAT R1.w, R0.z, c[5], R1;
ADD R3.xyz, R1.w, -R0;
MAD R0.xyz, R3, c[0].x, R0;
ADD R0.xyz, R0, -c[0].x;
MUL_SAT R0.xyz, R0.w, R0;
ADD R1.xyz, R1, R0;
MAD R0.xy, R2, c[2].y, R2.zwzw;
MAD R3.xy, R2, c[2].z, R2.zwzw;
TEX R0.y, R0, texture[1], 2D;
MAD R2.xy, R2, c[2].x, R2.zwzw;
TEX R0.x, R2, texture[1], 2D;
MUL R0.z, R0.y, c[5];
MAD R1.w, R0.x, c[5].y, R0.z;
TEX R0.z, R3, texture[1], 2D;
MUL R3.xy, R4, c[7].y;
MAD_SAT R1.w, R0.z, c[5], R1;
ADD R2.xyz, R1.w, -R0;
MAD R0.xyz, R2, c[0].x, R0;
ADD R2.xyz, R0, -c[0].x;
MUL_SAT R2.xyz, R0.w, R2;
ADD R1.xyz, R1, R2;
MUL R2.xy, R4.zwzw, c[7].z;
ADD R3.zw, -fragment.texcoord[0].xyxy, R3.xyxy;
ADD R3.zw, R3, c[5].x;
MAD R0.xy, R3, c[2].y, R3.zwzw;
MAD R5.xy, R3, c[2].z, R3.zwzw;
TEX R0.y, R0, texture[1], 2D;
MAD R3.xy, R3, c[2].x, R3.zwzw;
TEX R0.x, R3, texture[1], 2D;
MUL R0.z, R0.y, c[5];
MAD R1.w, R0.x, c[5].y, R0.z;
TEX R0.z, R5, texture[1], 2D;
MAD_SAT R1.w, R0.z, c[5], R1;
ADD R3.xyz, R1.w, -R0;
MAD R0.xyz, R3, c[0].x, R0;
ADD R2.zw, fragment.texcoord[0].xyxy, R2.xyxy;
ADD R0.xyz, R0, -c[0].x;
MUL_SAT R0.xyz, R0.w, R0;
ADD R1.xyz, R1, R0;
MAD R3.xy, R2, c[2].y, R2.zwzw;
TEX R0.y, R3, texture[1], 2D;
MAD R3.xy, R2, c[2].x, R2.zwzw;
TEX R0.x, R3, texture[1], 2D;
MUL R0.z, R0.y, c[5];
MUL R3.xy, R4, c[7].w;
MAD R2.zw, R2.xyxy, c[2].z, R2;
MAD R1.w, R0.x, c[5].y, R0.z;
TEX R0.z, R2.zwzw, texture[1], 2D;
ADD R2.xy, -fragment.texcoord[0], R3;
ADD R3.zw, R2.xyxy, c[5].x;
MAD R2.xy, R3, c[2].y, R3.zwzw;
MAD R2.zw, R3.xyxy, c[2].z, R3;
MAD R3.xy, R3, c[2].x, R3.zwzw;
TEX R2.y, R2, texture[1], 2D;
TEX R2.x, R3, texture[1], 2D;
MUL R3.z, R2.y, c[5];
TEX R2.z, R2.zwzw, texture[1], 2D;
MAD R3.x, R2, c[5].y, R3.z;
MAD_SAT R2.w, R2.z, c[5], R3.x;
ADD R4.xyz, R2.w, -R2;
MAD_SAT R1.w, R0.z, c[5], R1;
ADD R3.xyz, R1.w, -R0;
MAD R0.xyz, R3, c[0].x, R0;
MAD R2.xyz, R4, c[0].x, R2;
ADD R0.xyz, R0, -c[0].x;
MUL_SAT R0.xyz, R0.w, R0;
ADD R0.xyz, R1, R0;
ADD R2.xyz, R2, -c[0].x;
MUL_SAT R2.xyz, R0.w, R2;
ADD R0.xyz, R0, R2;
MUL_SAT R2.xyz, R0, c[4].z;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[0], texture[2], 2D;
MOV R2.w, c[4].y;
MUL R1, R2, R1;
MAD result.color, R1, c[1].x, R0;
END
# 191 instructions, 6 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Float 0 [_threshold]
Float 1 [_int]
Vector 2 [_chromatic]
Float 3 [_haloint]
SetTexture 0 [_OrgTex] 2D
SetTexture 1 [_MainTex] 2D
SetTexture 2 [_lensDirt] 2D
"ps_3_0
; 158 ALU, 33 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c4, -0.50000000, 0.10000000, 0.50000000, 0.58660001
def c5, 0.29890001, 0.11450000, 1.00000000, 2.00000000
def c6, 3.00000000, 4.00000000, 5.00000000, 6.00000000
def c7, 7.00000000, 8.00000000, 9.00000000, 0.00000000
dcl_texcoord0 v0.xy
add r0.xy, v0, c4.x
mul r4.xy, r0, c4.y
mul_pp r0.xy, r4, r4
add_pp r0.x, r0, r0.y
add r3.xy, r4, -v0
add r3.xy, r3, c5.z
rsq_pp r0.x, r0.x
mul_pp r0.xy, r0.x, r4
mul r0.zw, r0.xyxy, c4.z
add r0.xy, r0.zwzw, -v0
add r1.zw, r0.xyxy, c5.z
mad r0.xy, r0.zwzw, c2.y, r1.zwzw
mad r2.xy, r0.zwzw, c2.x, r1.zwzw
texld r0.y, r0, s1
mad r1.xy, r0.zwzw, c2.z, r1.zwzw
texld r0.x, r2, s1
mul r0.z, r0.y, c4.w
mad r0.w, r0.x, c5.x, r0.z
texld r0.z, r1, s1
mad_sat r0.w, r0.z, c5.y, r0
add r1.xyz, r0.w, -r0
mad r1.xyz, r1, c0.x, r0
add r0.xy, -v0, c5.z
texld r0.xyz, r0, s1
mul r1.w, r0.y, c4
mad r1.w, r0.x, c5.x, r1
mad_sat r1.w, r0.z, c5.y, r1
add r2.xyz, r1.w, -r0
mad r0.xyz, r2, c0.x, r0
add r2.xyz, r0, -c0.x
mad r0.xy, r4, c2.y, r3
mad r5.xy, r4, c2.z, r3
texld r0.y, r0, s1
mad r3.xy, r4, c2.x, r3
mov r0.w, c0.x
add r0.w, c5.z, -r0
rcp r0.w, r0.w
add r1.xyz, r1, -c0.x
mul_sat r2.xyz, r0.w, r2
mul_sat r1.xyz, r1, r0.w
mad r1.xyz, r1, c3.x, r2
mul r2.xy, r4, c5.w
add r2.zw, -v0.xyxy, r2.xyxy
add r2.zw, r2, c5.z
texld r0.x, r3, s1
mul r0.z, r0.y, c4.w
mad r1.w, r0.x, c5.x, r0.z
texld r0.z, r5, s1
mad_sat r1.w, r0.z, c5.y, r1
add r3.xyz, r1.w, -r0
mad r0.xyz, r3, c0.x, r0
add r0.xyz, r0, -c0.x
mul_sat r0.xyz, r0.w, r0
add r1.xyz, r1, r0
mad r0.xy, r2, c2.y, r2.zwzw
mad r3.xy, r2, c2.z, r2.zwzw
texld r0.y, r0, s1
mad r2.xy, r2, c2.x, r2.zwzw
texld r0.x, r2, s1
mul r0.z, r0.y, c4.w
mad r1.w, r0.x, c5.x, r0.z
texld r0.z, r3, s1
add_pp r2.xy, -v0, c4.z
mul r4.zw, r2.xyxy, c4.y
mul r3.xy, r4.zwzw, c6.x
add_pp r3.zw, v0.xyxy, r3.xyxy
mad_sat r1.w, r0.z, c5.y, r1
add r2.xyz, r1.w, -r0
mad r0.xyz, r2, c0.x, r0
add r2.xyz, r0, -c0.x
mad r0.xy, r3, c2.y, r3.zwzw
mad r5.xy, r3, c2.z, r3.zwzw
mul_sat r2.xyz, r0.w, r2
texld r0.y, r0, s1
mad r3.xy, r3, c2.x, r3.zwzw
add r1.xyz, r1, r2
mul r2.xy, r4, c6.y
add r2.zw, -v0.xyxy, r2.xyxy
add r2.zw, r2, c5.z
texld r0.x, r3, s1
mul r0.z, r0.y, c4.w
mad r1.w, r0.x, c5.x, r0.z
texld r0.z, r5, s1
mad_sat r1.w, r0.z, c5.y, r1
add r3.xyz, r1.w, -r0
mad r0.xyz, r3, c0.x, r0
add r0.xyz, r0, -c0.x
mul_sat r0.xyz, r0.w, r0
add r1.xyz, r1, r0
mad r0.xy, r2, c2.y, r2.zwzw
mad r3.xy, r2, c2.z, r2.zwzw
texld r0.y, r0, s1
mad r2.xy, r2, c2.x, r2.zwzw
texld r0.x, r2, s1
mul r0.z, r0.y, c4.w
mad r1.w, r0.x, c5.x, r0.z
texld r0.z, r3, s1
mul r3.xy, r4.zwzw, c6.z
add_pp r3.zw, v0.xyxy, r3.xyxy
mad_sat r1.w, r0.z, c5.y, r1
add r2.xyz, r1.w, -r0
mad r2.xyz, r2, c0.x, r0
mad r0.xy, r3, c2.y, r3.zwzw
mad r5.xy, r3, c2.z, r3.zwzw
texld r0.y, r0, s1
mad r3.xy, r3, c2.x, r3.zwzw
add r2.xyz, r2, -c0.x
mul_sat r2.xyz, r0.w, r2
add r1.xyz, r1, r2
mul r2.xy, r4.zwzw, c6.w
add_pp r2.zw, v0.xyxy, r2.xyxy
texld r0.x, r3, s1
mul r0.z, r0.y, c4.w
mad r1.w, r0.x, c5.x, r0.z
texld r0.z, r5, s1
mad_sat r1.w, r0.z, c5.y, r1
add r3.xyz, r1.w, -r0
mad r0.xyz, r3, c0.x, r0
add r0.xyz, r0, -c0.x
mul_sat r0.xyz, r0.w, r0
add r1.xyz, r1, r0
mad r0.xy, r2, c2.y, r2.zwzw
mad r3.xy, r2, c2.z, r2.zwzw
texld r0.y, r0, s1
mad r2.xy, r2, c2.x, r2.zwzw
texld r0.x, r2, s1
mul r0.z, r0.y, c4.w
mad r1.w, r0.x, c5.x, r0.z
texld r0.z, r3, s1
mul r3.xy, r4, c7.x
mad_sat r1.w, r0.z, c5.y, r1
add r2.xyz, r1.w, -r0
mad r0.xyz, r2, c0.x, r0
add r2.xyz, r0, -c0.x
mul_sat r2.xyz, r0.w, r2
add r1.xyz, r1, r2
mul r2.xy, r4.zwzw, c7.y
add r3.zw, -v0.xyxy, r3.xyxy
add r3.zw, r3, c5.z
mad r0.xy, r3, c2.y, r3.zwzw
texld r0.y, r0, s1
mad r5.xy, r3, c2.z, r3.zwzw
mad r3.xy, r3, c2.x, r3.zwzw
add_pp r2.zw, v0.xyxy, r2.xyxy
texld r0.x, r3, s1
mul r0.z, r0.y, c4.w
mad r1.w, r0.x, c5.x, r0.z
texld r0.z, r5, s1
mad_sat r1.w, r0.z, c5.y, r1
add r3.xyz, r1.w, -r0
mad r0.xyz, r3, c0.x, r0
add r0.xyz, r0, -c0.x
mul_sat r0.xyz, r0.w, r0
add r1.xyz, r1, r0
mad r3.xy, r2, c2.y, r2.zwzw
texld r0.y, r3, s1
mad r3.xy, r2, c2.x, r2.zwzw
texld r0.x, r3, s1
mul r0.z, r0.y, c4.w
mul r3.xy, r4, c7.z
mad r2.xy, r2, c2.z, r2.zwzw
mad r1.w, r0.x, c5.x, r0.z
texld r0.z, r2, s1
add r2.zw, -v0.xyxy, r3.xyxy
add r2.zw, r2, c5.z
mad r2.xy, r3, c2.y, r2.zwzw
mad r4.xy, r3, c2.z, r2.zwzw
mad r3.xy, r3, c2.x, r2.zwzw
texld r2.y, r2, s1
texld r2.x, r3, s1
mad_sat r1.w, r0.z, c5.y, r1
mul r2.z, r2.y, c4.w
mad r2.w, r2.x, c5.x, r2.z
texld r2.z, r4, s1
mad_sat r2.w, r2.z, c5.y, r2
add r4.xyz, r2.w, -r2
add r3.xyz, r1.w, -r0
mad r0.xyz, r3, c0.x, r0
mad r2.xyz, r4, c0.x, r2
add r0.xyz, r0, -c0.x
mul_sat r0.xyz, r0.w, r0
add r0.xyz, r1, r0
add r2.xyz, r2, -c0.x
mul_sat r2.xyz, r0.w, r2
add r0.xyz, r0, r2
mul_sat r2.xyz, r0, c4.y
texld r1, v0, s0
texld r0, v0, s2
mov r2.w, c7
mul r0, r2, r0
mad oC0, r0, c1.x, r1
"
}

SubProgram "xbox360 " {
Keywords { }
Vector 2 [_chromatic]
Float 3 [_haloint]
Float 1 [_int]
Float 0 [_threshold]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_OrgTex] 2D
SetTexture 2 [_lensDirt] 2D
// Shader Timing Estimate, in Cycles/64 pixel vector:
// ALU: 144.00 (108 instructions), vertex: 0, texture: 132,
//   sequencer: 50, interpolator: 12;    25 GPRs, 6 threads,
// Performance (if enough threads): ~144 cycles per vector
// * Texture cycle estimates are assuming an 8bit/component texture with no
//     aniso or trilinear filtering.
// * Warning: high GPR count may result in poorer than estimated performance.

"ps_360
backbbaaaaaaablaaaaaaiaeaaaaaaaaaaaaaaceaaaaabfmaaaaabieaaaaaaaa
aaaaaaaaaaaaabdeaaaaaabmaaaaabcfppppadaaaaaaaaahaaaaaabmaaaaaaaa
aaaaabboaaaaaakiaaadaaaaaaabaaaaaaaaaaleaaaaaaaaaaaaaameaaadaaab
aaabaaaaaaaaaaleaaaaaaaaaaaaaammaaacaaacaaabaaaaaaaaaaniaaaaaaaa
aaaaaaoiaaacaaadaaabaaaaaaaaaapeaaaaaaaaaaaaabaeaaacaaabaaabaaaa
aaaaaapeaaaaaaaaaaaaabajaaadaaacaaabaaaaaaaaaaleaaaaaaaaaaaaabbd
aaacaaaaaaabaaaaaaaaaapeaaaaaaaafpengbgjgofegfhiaaklklklaaaeaaam
aaabaaabaaabaaaaaaaaaaaafpephcghfegfhiaafpgdgihcgpgngbhegjgdaakl
aaabaaadaaabaaadaaabaaaaaaaaaaaafpgigbgmgpgjgoheaaklklklaaaaaaad
aaabaaabaaabaaaaaaaaaaaafpgjgoheaafpgmgfgohdeegjhcheaafphegihcgf
hdgigpgmgeaahahdfpddfpdaaadccodacodcdadddfddcodaaaklklklaaaaaaaa
aaaaaaahaaaaaaaaaaaaaaaaaaaaaabeabpeaadaaaaaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaamaaaaaaheebaaabiaaaaaaaaaeaaaaaaaaaaaabigdaaahaaah
aaaaaaabaaaadafaaaaadbfbaaaadcfcaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
doogggghdpggggghaaaaaaaaaaaaaaaadpljjjjkaaaaaaaadpimmmmndpiaaaaa
dnmmmmmnloemmmmndpigggggdpkmmmmndoemmmmndpjjjjjkdnemmmmnlnmmmmmn
dnemmmmndolddddddobjjjjkdommmmmndoiaaaaadojjjjjkdpaaaaaadpbjjjjk
dnmmmmmndpdddddddojjjjjkdpemmmmndnokhopkdojjajgmdpbgclgldommmmmn
eaeaaaaaeakaaaaaeamaaaaaebaaaaaaaaaagaangabdbcaabcaaaaaaaaaagabj
gabpbcaabcaaaaaaaaaagacfgaclbcaabcaaaaaaaaaagadbgadhbcaabcaaaaaa
afffgadngaedbcaabcaaafffafffgaejgaepbcaabcaaafffafffgaffdaflbcaa
bcaaaabfaaaaaaaagafomeaabcaaaaaaaaaagagegagkbcaabcaaaaaaaaaagaha
gahgbcaabcaaaaaaaaaagahmgaicbcaabcaaaaaaaaaagaiigaiobcaabcaaaaaa
aaaagajeaaaaccaaaaaaaaaamiapaaabaehagbmlilaapjpkmiapaaalaamgaaaa
cbacppaamiapaaakaeknbglmilaapmpmmiapaaamaeknbgbgilaapnplmiamaaaa
aekmblaakaaapiaamiapaaafaaknbgaakaaaplaamiapaaadaaknlmaakaaapmaa
miamaaahaenllbgmilaaphphmiamaaagaenllbgmilaaphpimiapaaacaeahgbaa
kbaapnaamiapaaaeaeakblgbilaapopkmiapaaadaeknbgaaklaapmadmiapaaaf
aeknbgaaklaapnafmiapaaajaaaklmakklamacafmiapaaaoaaaklmakklakacad
miapaaaiaabmnkaaolalabafmiapaaafaalmhnlmklamacafmiapaaanaambnkaa
olalabadmiapaaapaakalmkaklakacadmiadaaalaabklblaklaeacaemiapaabb
aahalmnkklabacacmiamaaadaakmmgagklabacacmiapaaabaanagbaakaacplaa
miapaaamaakhbgaakaacpjaamiapaaaeaaakmmkaklaeacaemiapaaakaaaklmak
klahacagmiapaaakacaahnaaoaakaaaamiapaaaeacdolmaaoaaeaaaamiadaaad
aabkgmlaklabacammiadaaacaabklblaklabacammiadaabaaabkmglaklabacam
miamaaalaakmmgagklabacammiapaaamaakalmakklabacammiabaaagaamhmhlb
nbababpimiadaaabaabkbkaaoaadacaamiapaabbaanaakaaoabbacaaleemacad
aakmmgmaiabbpipileimacbaaaagmgmbiabbpipificpabamacjogbgmoaamaaig
beapaaalacaahabloaalaaabambpabbaacaaknlboabaaaabbeapaaacacdokamg
oaacaaabamcpabbbacaaknlboaadaaabmiadaaahaalamgaakbabpmaamiadaaag
aabklaaaoaaaahaamiapaaadaakammkaklahacagmiapaaabaakklgkkklahacag
miadaaabaclagnaaoaabaaaabacihaabbpbppoiiaaaaeaaabaaigagbbpbpppmh
aaaaeaaaliaigacbbpbppohpaaaaeaaaliaigagbbpbppfppaaaaeaaaeeaidccb
bpbpppmhaaaaeaaajiaidaebbpbppohpaaaaeaaaeeaidcabbpbppfppaaaaeaaa
liaicccbbpbpppmhaaaaeaaaliaiccabbpbppohpaaaaeaaadaaicaebbpbppfpp
aaaaeaaageaieaibbpbpppmhaaaaeaaabaaiebgbbpbppohpaaaaeaaadaaieaib
bpbppfppaaaaeaaaeeajfbobbpbppppiaaaaeaaaomajfbobbpbpppmpaaaaeaaa
eeajfbkbbpbppolpaaaaeaaanmaifakbbpbpppmhaaaaeaaaiiaifakbbpbppohp
aaaaeaaaeeaifbabbpbppfppaaaaeaaabaajdbmbbpbppppiaaaaeaaaliajdbmb
bpbpppmpaaaaeaaaliajdbkbbpbppolpaaaaeaaanmaimbibbpbpppmhaaaaeaaa
iiaimbibbpbppohpaaaaeaaaomaimbgbbpbppfppaaaaeaaaeeaiobebbpbppppi
aaaaeaaaomaiobebbpbpppmpaaaaeaaaeeaioacbbpbppolpaaaaeaaabaaibbcb
bpbpppmhaaaaeaaaliaibbcbbpbppohpaaaaeaaaliaibbabbpbppfppaaaaeaaa
liajhaabbpbppoiiaaaaeaaababiaaabbpbppgiiaaaaeaaabeiaiaaaaaaaaabl
ocaaaaaabeahaaaladmagmgmiabhaaaaafbiagaiaalomabljabhpopilmbhapan
admagmebiaaoaaaalmchapbaadbfgmeciaamaaaalmehapbcadmagmediabdaaaa
lmbhaibeadmagmefiabfaaaalnciaiahabmdmaegjaagpoaalnebaiadabmdmaeh
jaadpoaaljbbbgacabmdmaefjaacpoaaljcbbgaeabmdmaegjaaepoaaljebbgaf
abmdmaehjaafpoaaljbiajajablomaeblabfpoaaljciajakablomaeclabdpoaa
ljeiajalabmdmaedlaampoaalnbbakababmdmaeblaabpoaalnceakakablomaec
laaopoaalmehakaoadmgmaedoaakaoaalibhamabadgmbfefoaababaalichambb
adblbfegoaalamaabeahaabdacblmabloaakbdaiaebhbibfadblmagmoaajbfbh
beahaaafacgmbfbloaafafaiaechbiaeadgmbflboaaeaebhbeahaaacacgmbfbl
oaacacaiaeehbiadadgmbfmgoaadadbhemihabbhacblbfgmoaahagagmiahaaal
aalegmleklbiaaalljehamalableblehobalabaamiahaaamaamagmmaklbhaaam
miahaaakaalegmlekladaaakmiahaaajaalegmleklacaaajmiahaaagaalegmle
klaeaabgmiahaaaiaamagmmaklafaaaimiahaaafaalegmleklbfaabemiahaaae
aalegmleklbdaabcmiahaaadaamagmmaklbbaabamiahaaacaalegmleklabaaap
miahaaabaalegmleklaoaaanmjahaaabaaleblaaobababaamjahaaacaaleblaa
obacabaabfahaaaeaaleblgmobaeabadapbhadafaaleblblobafababbfahaaag
aaleblgmobagabaiapbhaiajaaleblblobajababbfahaaakaalebllbobakabai
apchaiamaaleblblobamababmiahaaalaamagmleklamadalbeahaaakaamalemg
oaalakaiaoehaiajaamalebloaakajabbeahaaaiaamalelboaajaiadaochadag
aamalebloaaiagabbeahaaafaamalemgoaagafadaoehadaeaamalebloaafaeab
miahaaadaamaleaaoaaeadaamiahaaacaamaleaaoaadacaamiahaaabaamaleaa
oaacabaamjahaaabaalegmaakbabpnaamiahaaabaamamaaaobabahaamiahiaaa
aamagmmaklababaaaaaaaaaaaaaaaaaaaaaaaaaa"
}

SubProgram "d3d11 " {
Keywords { }
ConstBuffer "$Globals" 64 // 64 used size, 6 vars
Float 32 [_threshold]
Float 36 [_int]
Vector 48 [_chromatic] 3
Float 60 [_haloint]
BindCB "$Globals" 0
SetTexture 0 [_OrgTex] 2D 1
SetTexture 1 [_MainTex] 2D 0
SetTexture 2 [_lensDirt] 2D 2
// 158 instructions, 9 temp regs, 0 temp arrays:
// ALU 68 float, 0 int, 0 uint
// TEX 33 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"ps_4_0
eefiecedkcdkkgaflijjebndfodbddcmlinmedccabaaaaaagibgaaaaadaaaaaa
cmaaaaaaleaaaaaaoiaaaaaaejfdeheoiaaaaaaaaeaaaaaaaiaaaaaagiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaaheaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amaaaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaacaaaaaaadaaaaaafdfgfpfa
epfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaa
aiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfe
gbhcghgfheaaklklfdeieefchibfaaaaeaaaaaaafoafaaaafjaaaaaeegiocaaa
aaaaaaaaaeaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaa
fkaaaaadaagabaaaacaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaafibiaaae
aahabaaaabaaaaaaffffaaaafibiaaaeaahabaaaacaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacajaaaaaaaaaaaaal
pcaabaaaaaaaaaaaegbebaiaebaaaaaaabaaaaaaaceaaaaaaaaaiadpaaaaiadp
aaaaaadpaaaaaadpaaaaaaaldcaabaaaabaaaaaaegaabaiaebaaaaaaaaaaaaaa
aceaaaaaaaaaaadpaaaaaadpaaaaaaaaaaaaaaaadiaaaaakpcaabaaaacaaaaaa
egaebaaaabaaaaaaaceaaaaamnmmmmdnmnmmmmdnmnmmemdomnmmemdoapaaaaah
ecaabaaaabaaaaaaegaabaaaacaaaaaaegaabaaaacaaaaaaeeaaaaafecaabaaa
abaaaaaackaabaaaabaaaaaadiaaaaahmcaabaaaabaaaaaakgakbaaaabaaaaaa
agaebaaaacaaaaaadcaaaaamdcaabaaaadaaaaaaogakbaaaabaaaaaaaceaaaaa
aaaaaadpaaaaaadpaaaaaaaaaaaaaaaaegaabaaaaaaaaaaadiaaaaakmcaabaaa
abaaaaaakgaobaaaabaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaadpaaaaaadp
dcaaaaakmcaabaaaadaaaaaakgaobaaaabaaaaaakgikcaaaaaaaaaaaadaaaaaa
agaebaaaadaaaaaadcaaaaakpcaabaaaaeaaaaaaogaobaaaabaaaaaaagifcaaa
aaaaaaaaadaaaaaaegaebaaaadaaaaaaefaaaaajpcaabaaaadaaaaaaogakbaaa
adaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaafaaaaaa
egaabaaaaeaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaa
aeaaaaaaogakbaaaaeaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadgaaaaaf
ccaabaaaadaaaaaabkaabaaaaeaaaaaadgaaaaafbcaabaaaadaaaaaaakaabaaa
afaaaaaabacaaaakecaabaaaabaaaaaaegacbaaaadaaaaaaaceaaaaagmajjjdo
glclbgdppkhookdnaaaaaaaaaaaaaaaihcaabaaaaeaaaaaaegacbaiaebaaaaaa
adaaaaaakgakbaaaabaaaaaadcaaaaakhcaabaaaadaaaaaaagiacaaaaaaaaaaa
acaaaaaaegacbaaaaeaaaaaaegacbaaaadaaaaaaaaaaaaajhcaabaaaadaaaaaa
egacbaaaadaaaaaaagiacaiaebaaaaaaaaaaaaaaacaaaaaaaaaaaaajecaabaaa
abaaaaaaakiacaiaebaaaaaaaaaaaaaaacaaaaaaabeaaaaaaaaaiadpaoaaaaak
ecaabaaaabaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadpckaabaaa
abaaaaaadicaaaahhcaabaaaadaaaaaakgakbaaaabaaaaaaegacbaaaadaaaaaa
efaaaaajpcaabaaaaeaaaaaaegaabaaaaaaaaaaaeghobaaaabaaaaaaaagabaaa
aaaaaaaabacaaaakicaabaaaabaaaaaaegacbaaaaeaaaaaaaceaaaaagmajjjdo
glclbgdppkhookdnaaaaaaaaaaaaaaaihcaabaaaafaaaaaaegacbaiaebaaaaaa
aeaaaaaapgapbaaaabaaaaaadcaaaaakhcaabaaaaeaaaaaaagiacaaaaaaaaaaa
acaaaaaaegacbaaaafaaaaaaegacbaaaaeaaaaaaaaaaaaajhcaabaaaaeaaaaaa
egacbaaaaeaaaaaaagiacaiaebaaaaaaaaaaaaaaacaaaaaadicaaaahhcaabaaa
aeaaaaaakgakbaaaabaaaaaaegacbaaaaeaaaaaadcaaaaakhcaabaaaadaaaaaa
egacbaaaadaaaaaapgipcaaaaaaaaaaaadaaaaaaegacbaaaaeaaaaaadcaaaaam
pcaabaaaaeaaaaaaegaebaaaabaaaaaaaceaaaaamnmmmmdnmnmmmmdnmnmmemdo
mnmmemdoegaebaaaaaaaaaaadcaaaaakpcaabaaaafaaaaaaegaebaaaacaaaaaa
agifcaaaaaaaaaaaadaaaaaaegaebaaaaeaaaaaaefaaaaajpcaabaaaagaaaaaa
egaabaaaafaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaa
afaaaaaaogakbaaaafaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadgaaaaaf
ccaabaaaagaaaaaabkaabaaaafaaaaaadcaaaaakpcaabaaaafaaaaaaegaobaaa
acaaaaaakgiacaaaaaaaaaaaadaaaaaaegaobaaaaeaaaaaadcaaaaakpcaabaaa
acaaaaaaogaobaaaacaaaaaafgikcaaaaaaaaaaaadaaaaaaogaobaaaaeaaaaaa
efaaaaajpcaabaaaaeaaaaaaegaabaaaafaaaaaaeghobaaaabaaaaaaaagabaaa
aaaaaaaaefaaaaajpcaabaaaafaaaaaaogakbaaaafaaaaaaeghobaaaabaaaaaa
aagabaaaaaaaaaaadgaaaaafecaabaaaagaaaaaackaabaaaaeaaaaaabacaaaak
icaabaaaabaaaaaaegacbaaaagaaaaaaaceaaaaagmajjjdoglclbgdppkhookdn
aaaaaaaaaaaaaaaihcaabaaaaeaaaaaaegacbaiaebaaaaaaagaaaaaapgapbaaa
abaaaaaadcaaaaakhcaabaaaaeaaaaaaagiacaaaaaaaaaaaacaaaaaaegacbaaa
aeaaaaaaegacbaaaagaaaaaaaaaaaaajhcaabaaaaeaaaaaaegacbaaaaeaaaaaa
agiacaiaebaaaaaaaaaaaaaaacaaaaaadicaaaahhcaabaaaaeaaaaaakgakbaaa
abaaaaaaegacbaaaaeaaaaaaaaaaaaahhcaabaaaadaaaaaaegacbaaaadaaaaaa
egacbaaaaeaaaaaaefaaaaajpcaabaaaaeaaaaaaegaabaaaacaaaaaaeghobaaa
abaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaacaaaaaaogakbaaaacaaaaaa
eghobaaaabaaaaaaaagabaaaaaaaaaaadgaaaaafecaabaaaafaaaaaackaabaaa
acaaaaaadgaaaaafccaabaaaafaaaaaabkaabaaaaeaaaaaabacaaaakicaabaaa
abaaaaaaegacbaaaafaaaaaaaceaaaaagmajjjdoglclbgdppkhookdnaaaaaaaa
aaaaaaaihcaabaaaacaaaaaaegacbaiaebaaaaaaafaaaaaapgapbaaaabaaaaaa
dcaaaaakhcaabaaaacaaaaaaagiacaaaaaaaaaaaacaaaaaaegacbaaaacaaaaaa
egacbaaaafaaaaaaaaaaaaajhcaabaaaacaaaaaaegacbaaaacaaaaaaagiacaia
ebaaaaaaaaaaaaaaacaaaaaadicaaaahhcaabaaaacaaaaaakgakbaaaabaaaaaa
egacbaaaacaaaaaaaaaaaaahhcaabaaaacaaaaaaegacbaaaacaaaaaaegacbaaa
adaaaaaadiaaaaakpcaabaaaadaaaaaaogaobaaaaaaaaaaaaceaaaaajkjjjjdo
jkjjjjdoaaaaaadpaaaaaadpdcaaaaampcaabaaaaeaaaaaaogaobaaaaaaaaaaa
aceaaaaajkjjjjdojkjjjjdoaaaaaadpaaaaaadpegbebaaaabaaaaaadcaaaaak
pcaabaaaafaaaaaaegaebaaaadaaaaaaagifcaaaaaaaaaaaadaaaaaaegaebaaa
aeaaaaaaefaaaaajpcaabaaaagaaaaaaegaabaaaafaaaaaaeghobaaaabaaaaaa
aagabaaaaaaaaaaaefaaaaajpcaabaaaafaaaaaaogakbaaaafaaaaaaeghobaaa
abaaaaaaaagabaaaaaaaaaaadgaaaaafccaabaaaagaaaaaabkaabaaaafaaaaaa
dcaaaaakpcaabaaaafaaaaaaegaobaaaadaaaaaakgiacaaaaaaaaaaaadaaaaaa
egaobaaaaeaaaaaadcaaaaakpcaabaaaadaaaaaaogaobaaaadaaaaaafgikcaaa
aaaaaaaaadaaaaaaogaobaaaaeaaaaaaefaaaaajpcaabaaaaeaaaaaaegaabaaa
afaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaafaaaaaa
ogakbaaaafaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadgaaaaafecaabaaa
agaaaaaackaabaaaaeaaaaaabacaaaakicaabaaaabaaaaaaegacbaaaagaaaaaa
aceaaaaagmajjjdoglclbgdppkhookdnaaaaaaaaaaaaaaaihcaabaaaaeaaaaaa
egacbaiaebaaaaaaagaaaaaapgapbaaaabaaaaaadcaaaaakhcaabaaaaeaaaaaa
agiacaaaaaaaaaaaacaaaaaaegacbaaaaeaaaaaaegacbaaaagaaaaaaaaaaaaaj
hcaabaaaaeaaaaaaegacbaaaaeaaaaaaagiacaiaebaaaaaaaaaaaaaaacaaaaaa
dicaaaahhcaabaaaaeaaaaaakgakbaaaabaaaaaaegacbaaaaeaaaaaaaaaaaaah
hcaabaaaacaaaaaaegacbaaaacaaaaaaegacbaaaaeaaaaaadcaaaaampcaabaaa
aeaaaaaaegaebaaaabaaaaaaaceaaaaamnmmmmdomnmmmmdodddddddpdddddddp
egaebaaaaaaaaaaadiaaaaakpcaabaaaagaaaaaaegaebaaaabaaaaaaaceaaaaa
mnmmmmdomnmmmmdodddddddpdddddddpdcaaaaakpcaabaaaahaaaaaaegaebaaa
agaaaaaaagifcaaaaaaaaaaaadaaaaaaegaebaaaaeaaaaaaefaaaaajpcaabaaa
aiaaaaaaegaabaaaahaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaefaaaaaj
pcaabaaaahaaaaaaogakbaaaahaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaa
dgaaaaafccaabaaaaiaaaaaabkaabaaaahaaaaaadcaaaaakpcaabaaaahaaaaaa
egaobaaaagaaaaaakgiacaaaaaaaaaaaadaaaaaaegaobaaaaeaaaaaadcaaaaak
pcaabaaaaeaaaaaaogaobaaaagaaaaaafgikcaaaaaaaaaaaadaaaaaaogaobaaa
aeaaaaaaefaaaaajpcaabaaaagaaaaaaegaabaaaahaaaaaaeghobaaaabaaaaaa
aagabaaaaaaaaaaaefaaaaajpcaabaaaahaaaaaaogakbaaaahaaaaaaeghobaaa
abaaaaaaaagabaaaaaaaaaaadgaaaaafecaabaaaaiaaaaaackaabaaaagaaaaaa
bacaaaakicaabaaaabaaaaaaegacbaaaaiaaaaaaaceaaaaagmajjjdoglclbgdp
pkhookdnaaaaaaaaaaaaaaaihcaabaaaagaaaaaaegacbaiaebaaaaaaaiaaaaaa
pgapbaaaabaaaaaadcaaaaakhcaabaaaagaaaaaaagiacaaaaaaaaaaaacaaaaaa
egacbaaaagaaaaaaegacbaaaaiaaaaaaaaaaaaajhcaabaaaagaaaaaaegacbaaa
agaaaaaaagiacaiaebaaaaaaaaaaaaaaacaaaaaadicaaaahhcaabaaaagaaaaaa
kgakbaaaabaaaaaaegacbaaaagaaaaaaaaaaaaahhcaabaaaacaaaaaaegacbaaa
acaaaaaaegacbaaaagaaaaaaefaaaaajpcaabaaaagaaaaaaegaabaaaadaaaaaa
eghobaaaabaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaadaaaaaaogakbaaa
adaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadgaaaaafecaabaaaafaaaaaa
ckaabaaaadaaaaaadgaaaaafccaabaaaafaaaaaabkaabaaaagaaaaaabacaaaak
icaabaaaabaaaaaaegacbaaaafaaaaaaaceaaaaagmajjjdoglclbgdppkhookdn
aaaaaaaaaaaaaaaihcaabaaaadaaaaaaegacbaiaebaaaaaaafaaaaaapgapbaaa
abaaaaaadcaaaaakhcaabaaaadaaaaaaagiacaaaaaaaaaaaacaaaaaaegacbaaa
adaaaaaaegacbaaaafaaaaaaaaaaaaajhcaabaaaadaaaaaaegacbaaaadaaaaaa
agiacaiaebaaaaaaaaaaaaaaacaaaaaadicaaaahhcaabaaaadaaaaaakgakbaaa
abaaaaaaegacbaaaadaaaaaaaaaaaaahhcaabaaaacaaaaaaegacbaaaacaaaaaa
egacbaaaadaaaaaadiaaaaakpcaabaaaadaaaaaaogaobaaaaaaaaaaaaceaaaaa
jkjjbjdpjkjjbjdpmnmmemdpmnmmemdpdcaaaaampcaabaaaafaaaaaaogaobaaa
aaaaaaaaaceaaaaajkjjbjdpjkjjbjdpmnmmemdpmnmmemdpegbebaaaabaaaaaa
dcaaaaamdcaabaaaaaaaaaaaegaabaaaabaaaaaaaceaaaaaghggggdpghggggdp
aaaaaaaaaaaaaaaaegaabaaaaaaaaaaadiaaaaakmcaabaaaaaaaaaaaagaebaaa
abaaaaaaaceaaaaaaaaaaaaaaaaaaaaaghggggdpghggggdpdcaaaaakpcaabaaa
agaaaaaaegaebaaaadaaaaaaagifcaaaaaaaaaaaadaaaaaaegaebaaaafaaaaaa
efaaaaajpcaabaaaaiaaaaaaegaabaaaagaaaaaaeghobaaaabaaaaaaaagabaaa
aaaaaaaaefaaaaajpcaabaaaagaaaaaaogakbaaaagaaaaaaeghobaaaabaaaaaa
aagabaaaaaaaaaaadgaaaaafccaabaaaaiaaaaaabkaabaaaagaaaaaadcaaaaak
pcaabaaaagaaaaaaegaobaaaadaaaaaakgiacaaaaaaaaaaaadaaaaaaegaobaaa
afaaaaaadcaaaaakpcaabaaaadaaaaaaogaobaaaadaaaaaafgikcaaaaaaaaaaa
adaaaaaaogaobaaaafaaaaaaefaaaaajpcaabaaaafaaaaaaegaabaaaagaaaaaa
eghobaaaabaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaagaaaaaaogakbaaa
agaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadgaaaaafecaabaaaaiaaaaaa
ckaabaaaafaaaaaabacaaaakbcaabaaaabaaaaaaegacbaaaaiaaaaaaaceaaaaa
gmajjjdoglclbgdppkhookdnaaaaaaaaaaaaaaailcaabaaaabaaaaaaegaibaia
ebaaaaaaaiaaaaaaagaabaaaabaaaaaadcaaaaaklcaabaaaabaaaaaaagiacaaa
aaaaaaaaacaaaaaaegambaaaabaaaaaaegaibaaaaiaaaaaaaaaaaaajlcaabaaa
abaaaaaaegambaaaabaaaaaaagiacaiaebaaaaaaaaaaaaaaacaaaaaadicaaaah
lcaabaaaabaaaaaakgakbaaaabaaaaaaegambaaaabaaaaaaaaaaaaahlcaabaaa
abaaaaaaegambaaaabaaaaaaegaibaaaacaaaaaaefaaaaajpcaabaaaacaaaaaa
egaabaaaaeaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaa
aeaaaaaaogakbaaaaeaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadgaaaaaf
ecaabaaaahaaaaaackaabaaaaeaaaaaadgaaaaafccaabaaaahaaaaaabkaabaaa
acaaaaaabacaaaakbcaabaaaacaaaaaaegacbaaaahaaaaaaaceaaaaagmajjjdo
glclbgdppkhookdnaaaaaaaaaaaaaaaihcaabaaaacaaaaaaegacbaiaebaaaaaa
ahaaaaaaagaabaaaacaaaaaadcaaaaakhcaabaaaacaaaaaaagiacaaaaaaaaaaa
acaaaaaaegacbaaaacaaaaaaegacbaaaahaaaaaaaaaaaaajhcaabaaaacaaaaaa
egacbaaaacaaaaaaagiacaiaebaaaaaaaaaaaaaaacaaaaaadicaaaahhcaabaaa
acaaaaaakgakbaaaabaaaaaaegacbaaaacaaaaaaaaaaaaahlcaabaaaabaaaaaa
egambaaaabaaaaaaegaibaaaacaaaaaaefaaaaajpcaabaaaacaaaaaaegaabaaa
adaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaadaaaaaa
ogakbaaaadaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadgaaaaafecaabaaa
agaaaaaackaabaaaadaaaaaadgaaaaafccaabaaaagaaaaaabkaabaaaacaaaaaa
bacaaaakbcaabaaaacaaaaaaegacbaaaagaaaaaaaceaaaaagmajjjdoglclbgdp
pkhookdnaaaaaaaaaaaaaaaihcaabaaaacaaaaaaegacbaiaebaaaaaaagaaaaaa
agaabaaaacaaaaaadcaaaaakhcaabaaaacaaaaaaagiacaaaaaaaaaaaacaaaaaa
egacbaaaacaaaaaaegacbaaaagaaaaaaaaaaaaajhcaabaaaacaaaaaaegacbaaa
acaaaaaaagiacaiaebaaaaaaaaaaaaaaacaaaaaadicaaaahhcaabaaaacaaaaaa
kgakbaaaabaaaaaaegacbaaaacaaaaaaaaaaaaahlcaabaaaabaaaaaaegambaaa
abaaaaaaegaibaaaacaaaaaadcaaaaakdcaabaaaacaaaaaaogakbaaaaaaaaaaa
kgikcaaaaaaaaaaaadaaaaaaegaabaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaa
ogaobaaaaaaaaaaaagifcaaaaaaaaaaaadaaaaaaegaebaaaaaaaaaaaefaaaaaj
pcaabaaaacaaaaaaegaabaaaacaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaa
efaaaaajpcaabaaaadaaaaaaegaabaaaaaaaaaaaeghobaaaabaaaaaaaagabaaa
aaaaaaaaefaaaaajpcaabaaaaaaaaaaaogakbaaaaaaaaaaaeghobaaaabaaaaaa
aagabaaaaaaaaaaadgaaaaafccaabaaaacaaaaaabkaabaaaaaaaaaaadgaaaaaf
bcaabaaaacaaaaaaakaabaaaadaaaaaabacaaaakbcaabaaaaaaaaaaaegacbaaa
acaaaaaaaceaaaaagmajjjdoglclbgdppkhookdnaaaaaaaaaaaaaaaihcaabaaa
aaaaaaaaegacbaiaebaaaaaaacaaaaaaagaabaaaaaaaaaaadcaaaaakhcaabaaa
aaaaaaaaagiacaaaaaaaaaaaacaaaaaaegacbaaaaaaaaaaaegacbaaaacaaaaaa
aaaaaaajhcaabaaaaaaaaaaaegacbaaaaaaaaaaaagiacaiaebaaaaaaaaaaaaaa
acaaaaaadicaaaahhcaabaaaaaaaaaaakgakbaaaabaaaaaaegacbaaaaaaaaaaa
aaaaaaahhcaabaaaaaaaaaaaegacbaaaaaaaaaaaegadbaaaabaaaaaadicaaaak
hcaabaaaaaaaaaaaegacbaaaaaaaaaaaaceaaaaamnmmmmdnmnmmmmdnmnmmmmdn
aaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaabaaaaaaeghobaaaacaaaaaa
aagabaaaacaaaaaadiaaaaahhcaabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaa
abaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaa
aagabaaaabaaaaaadgaaaaaficaabaaaaaaaaaaaabeaaaaaaaaaaaaadcaaaaak
pccabaaaaaaaaaaaegaobaaaaaaaaaaafgifcaaaaaaaaaaaacaaaaaaegaobaaa
abaaaaaadoaaaaab"
}

SubProgram "gles " {
Keywords { }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES"
}

SubProgram "d3d11_9x " {
Keywords { }
ConstBuffer "$Globals" 64 // 64 used size, 6 vars
Float 32 [_threshold]
Float 36 [_int]
Vector 48 [_chromatic] 3
Float 60 [_haloint]
BindCB "$Globals" 0
SetTexture 0 [_OrgTex] 2D 1
SetTexture 1 [_MainTex] 2D 0
SetTexture 2 [_lensDirt] 2D 2
// 158 instructions, 9 temp regs, 0 temp arrays:
// ALU 68 float, 0 int, 0 uint
// TEX 33 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"ps_4_0_level_9_3
eefiecedipnbhohppaamidadlohpljefdhnbklhkabaaaaaaaeccaaaaaeaaaaaa
daaaaaaamialaaaaeicbaaaanacbaaaaebgpgodjjaalaaaajaalaaaaaaacpppp
fealaaaadmaaaaaaabaadaaaaaaadmaaaaaadmaaadaaceaaaaaadmaaabaaaaaa
aaababaaacacacaaaaaaacaaacaaaaaaaaaaaaaaabacppppfbaaaaafacaaapka
aaaaiadpaaaaaadpmnmmmmdnmnmmemdofbaaaaafadaaapkajkjjjjdoaaaaaadp
mnmmmmdodddddddpfbaaaaafaeaaapkajkjjbjdpmnmmemdpghggggdpaaaaaaaa
fbaaaaafafaaapkaaaaaaaaagmajjjdoglclbgdppkhookdnfbaaaaafagaaapka
aaaaiadpaaaaaaaaaaaaaaaaaaaaaaaabpaaaaacaaaaaaiaaaaacplabpaaaaac
aaaaaajaaaaiapkabpaaaaacaaaaaajaabaiapkabpaaaaacaaaaaajaacaiapka
acaaaaadaaaaadiaaaaaoelbacaaaakaacaaaaadaaaaamiaaaaaeeibacaaffka
afaaaaadabaacpiaaaaaooiaacaapkkafkaaaaaeacaaciiaabaaoeiaabaaoeia
afaaaakaahaaaaacacaacbiaacaappiaafaaaaadacaaadiaabaaoeiaacaaaaia
aeaaaaaeacaacmiaacaaeeiaacaaffkaaaaaeeiaafaaaaadacaacdiaacaaoeia
acaaffkaaeaaaaaeadaaadiaacaaoeiaabaaaakaacaaooiaaeaaaaaeaeaaadia
acaaoeiaabaaffkaacaaooiaaeaaaaaeacaaadiaacaaoeiaabaakkkaacaaooia
ecaaaaadadaaapiaadaaoeiaaaaioekaecaaaaadacaaapiaacaaoeiaaaaioeka
abaaaaacacaaabiaadaaaaiaecaaaaadadaaapiaaaaaoeiaaaaioekaecaaaaad
aeaaapiaaeaaoeiaaaaioekaabaaaaacacaaaciaaeaaffiaaiaaaaadacaabiia
acaaoeiaafaapjkabcaaaaaeaeaaahiaaaaaaakaacaappiaacaaoeiaacaaaaad
acaaahiaaeaaoeiaaaaaaakbabaaaaacaeaaadiaaaaaoekaacaaaaadacaaaiia
aeaaaaibacaaaakaagaaaaacacaaaiiaacaappiaafaaaaadacaabhiaacaappia
acaaoeiaaiaaaaadadaabiiaadaaoeiaafaapjkabcaaaaaeaeaaaniaaaaaaaka
adaappiaadaajeiaacaaaaadadaaahiaaeaapiiaaaaaaakbafaaaaadadaabhia
acaappiaadaaoeiaaeaaaaaeacaaahiaacaaoeiaabaappkaadaaoeiaaeaaaaae
adaacpiaaaaaooiaacaapkkaaaaaeeiaaeaaaaaeafaaadiaabaaoeiaabaaaaka
adaaoeiaaeaaaaaeagaaadiaabaaoeiaabaaffkaadaaoeiaecaaaaadafaaapia
afaaoeiaaaaioekaecaaaaadagaaapiaagaaoeiaaaaioekaabaaaaacagaaabia
afaaaaiaaeaaaaaeabaaadiaabaaoeiaabaakkkaadaaoeiaaeaaaaaeadaaadia
abaaooiaabaaaakaadaaooiaecaaaaadafaaapiaabaaoeiaaaaioekaecaaaaad
ahaaapiaadaaoeiaaaaioekaabaaaaacagaaaeiaafaakkiaaiaaaaadagaabiia
agaaoeiaafaapjkabcaaaaaeaeaaaniaaaaaaakaagaappiaagaajeiaacaaaaad
aeaaaniaaeaaoeiaaaaaaakbafaaaaadaeaabniaacaappiaaeaaoeiaacaaaaad
acaaahiaacaaoeiaaeaapiiaaeaaaaaeabaaadiaabaaooiaabaaffkaadaaooia
aeaaaaaeadaaadiaabaaooiaabaakkkaadaaooiaecaaaaadabaaapiaabaaoeia
aaaioekaecaaaaadadaaapiaadaaoeiaaaaioekaabaaaaacahaaaeiaadaakkia
abaaaaacahaaaciaabaaffiaaiaaaaadahaabiiaahaaoeiaafaapjkabcaaaaae
abaaahiaaaaaaakaahaappiaahaaoeiaacaaaaadabaaahiaabaaoeiaaaaaaakb
afaaaaadabaabhiaacaappiaabaaoeiaacaaaaadabaaahiaabaaoeiaacaaoeia
acaaaaadadaaapiaaaaaeelbacaaffkaafaaaaadafaacpiaadaaooiaadaafaka
aeaaaaaeagaacpiaadaaooiaadaafakaaaaaeelaaeaaaaaeacaaadiaafaaoeia
abaaaakaagaaoeiaaeaaaaaeahaaadiaafaaoeiaabaaffkaagaaoeiaecaaaaad
aiaaapiaacaaoeiaaaaioekaecaaaaadahaaapiaahaaoeiaaaaioekaabaaaaac
ahaaabiaaiaaaaiaaeaaaaaeacaaadiaafaaoeiaabaakkkaagaaoeiaaeaaaaae
aiaacpiaaaaaooiaadaapkkaaaaaeeiaaeaaaaaeaaaacdiaaaaaooiaaeaakkka
aaaaoeiaafaaaaadajaacpiaaaaaooiaadaapkkaafaaaaadaaaacmiaaaaaoeia
aeaakkkaaeaaaaaeafaaadiaajaaoeiaabaaaakaaiaaoeiaecaaaaadakaaapia
acaaoeiaaaaioekaecaaaaadalaaapiaafaaoeiaaaaioekaabaaaaacahaaaeia
akaakkiaaiaaaaadabaabiiaahaaoeiaafaapjkabcaaaaaeacaaahiaaaaaaaka
abaappiaahaaoeiaacaaaaadacaaahiaacaaoeiaaaaaaakbafaaaaadacaabhia
acaappiaacaaoeiaacaaaaadabaaahiaabaaoeiaacaaoeiaaeaaaaaeacaaadia
ajaaoeiaabaaffkaaiaaoeiaaeaaaaaeafaaadiaajaaoeiaabaakkkaaiaaoeia
ecaaaaadahaaapiaacaaoeiaaaaioekaecaaaaadakaaapiaafaaoeiaaaaioeka
abaaaaacalaaaciaahaaffiaabaaaaacalaaaeiaakaakkiaaiaaaaadabaabiia
alaaoeiaafaapjkabcaaaaaeacaaahiaaaaaaakaabaappiaalaaoeiaacaaaaad
acaaahiaacaaoeiaaaaaaakbafaaaaadacaabhiaacaappiaacaaoeiaacaaaaad
abaaahiaabaaoeiaacaaoeiaaeaaaaaeacaaadiaafaaooiaabaaaakaagaaooia
aeaaaaaeafaaadiaafaaooiaabaaffkaagaaooiaaeaaaaaeagaaadiaafaaooia
abaakkkaagaaooiaecaaaaadahaaapiaacaaoeiaaaaioekaecaaaaadagaaapia
agaaoeiaaaaioekaabaaaaacagaaabiaahaaaaiaafaaaaadahaacpiaadaaoeia
aeaafakaaeaaaaaeadaacpiaadaaoeiaaeaafakaaaaaeelaaeaaaaaeacaaadia
ahaaoeiaabaaaakaadaaoeiaecaaaaadafaaapiaafaaoeiaaaaioekaecaaaaad
akaaapiaacaaoeiaaaaioekaabaaaaacagaaaciaafaaffiaaiaaaaadabaabiia
agaaoeiaafaapjkabcaaaaaeacaaahiaaaaaaakaabaappiaagaaoeiaacaaaaad
acaaahiaacaaoeiaaaaaaakbafaaaaadacaabhiaacaappiaacaaoeiaacaaaaad
abaaahiaabaaoeiaacaaoeiaaeaaaaaeacaaadiaahaaoeiaabaaffkaadaaoeia
aeaaaaaeadaaadiaahaaoeiaabaakkkaadaaoeiaecaaaaadafaaapiaacaaoeia
aaaioekaecaaaaadagaaapiaadaaoeiaaaaioekaabaaaaacakaaaciaafaaffia
abaaaaacakaaaeiaagaakkiaaiaaaaadabaabiiaakaaoeiaafaapjkabcaaaaae
acaaahiaaaaaaakaabaappiaakaaoeiaacaaaaadacaaahiaacaaoeiaaaaaaakb
afaaaaadacaabhiaacaappiaacaaoeiaacaaaaadabaaahiaabaaoeiaacaaoeia
aeaaaaaeacaaadiaajaaooiaabaaaakaaiaaooiaaeaaaaaeadaaadiaajaaooia
abaaffkaaiaaooiaaeaaaaaeafaaadiaajaaooiaabaakkkaaiaaooiaecaaaaad
agaaapiaacaaoeiaaaaioekaecaaaaadafaaapiaafaaoeiaaaaioekaabaaaaac
afaaabiaagaaaaiaaeaaaaaeacaaadiaahaaooiaabaaaakaadaaooiaecaaaaad
agaaapiaadaaoeiaaaaioekaecaaaaadaiaaapiaacaaoeiaaaaioekaabaaaaac
afaaaciaagaaffiaaiaaaaadabaabiiaafaaoeiaafaapjkabcaaaaaeacaaahia
aaaaaakaabaappiaafaaoeiaacaaaaadacaaahiaacaaoeiaaaaaaakbafaaaaad
acaabhiaacaappiaacaaoeiaacaaaaadabaaahiaabaaoeiaacaaoeiaaeaaaaae
acaaadiaahaaooiaabaaffkaadaaooiaaeaaaaaeadaaadiaahaaooiaabaakkka
adaaooiaecaaaaadafaaapiaacaaoeiaaaaioekaecaaaaadadaaapiaadaaoeia
aaaioekaabaaaaacaiaaaeiaadaakkiaabaaaaacaiaaaciaafaaffiaaiaaaaad
abaabiiaaiaaoeiaafaapjkabcaaaaaeacaaahiaaaaaaakaabaappiaaiaaoeia
acaaaaadacaaahiaacaaoeiaaaaaaakbafaaaaadacaabhiaacaappiaacaaoeia
acaaaaadabaaahiaabaaoeiaacaaoeiaaeaaaaaeacaaadiaaaaaooiaabaaaaka
aaaaoeiaaeaaaaaeadaaadiaaaaaooiaabaaffkaaaaaoeiaaeaaaaaeaaaaadia
aaaaooiaabaakkkaaaaaoeiaecaaaaadafaaapiaacaaoeiaaaaioekaecaaaaad
aaaaapiaaaaaoeiaaaaioekaabaaaaacaaaaabiaafaaaaiaecaaaaadafaaapia
aaaaoelaacaioekaecaaaaadadaaapiaadaaoeiaaaaioekaabaaaaacaaaaacia
adaaffiaaiaaaaadaaaabiiaaaaaoeiaafaapjkabcaaaaaeacaaahiaaaaaaaka
aaaappiaaaaaoeiaacaaaaadaaaaahiaacaaoeiaaaaaaakbafaaaaadaaaabhia
acaappiaaaaaoeiaacaaaaadaaaaahiaaaaaoeiaabaaoeiaafaaaaadaaaabhia
aaaaoeiaacaakkkaabaaaaacaaaaaiiaaaaaffkaafaaaaadaaaaapiaafaaoeia
aaaaoeiaecaaaaadabaaapiaaaaaoelaabaioekaafaaaaadacaaapiaaeaaffia
agaaeakaaeaaaaaeaaaacpiaaaaaoeiaacaaoeiaabaaoeiaabaaaaacaaaicpia
aaaaoeiappppaaaafdeieefchibfaaaaeaaaaaaafoafaaaafjaaaaaeegiocaaa
aaaaaaaaaeaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaa
fkaaaaadaagabaaaacaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaafibiaaae
aahabaaaabaaaaaaffffaaaafibiaaaeaahabaaaacaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacajaaaaaaaaaaaaal
pcaabaaaaaaaaaaaegbebaiaebaaaaaaabaaaaaaaceaaaaaaaaaiadpaaaaiadp
aaaaaadpaaaaaadpaaaaaaaldcaabaaaabaaaaaaegaabaiaebaaaaaaaaaaaaaa
aceaaaaaaaaaaadpaaaaaadpaaaaaaaaaaaaaaaadiaaaaakpcaabaaaacaaaaaa
egaebaaaabaaaaaaaceaaaaamnmmmmdnmnmmmmdnmnmmemdomnmmemdoapaaaaah
ecaabaaaabaaaaaaegaabaaaacaaaaaaegaabaaaacaaaaaaeeaaaaafecaabaaa
abaaaaaackaabaaaabaaaaaadiaaaaahmcaabaaaabaaaaaakgakbaaaabaaaaaa
agaebaaaacaaaaaadcaaaaamdcaabaaaadaaaaaaogakbaaaabaaaaaaaceaaaaa
aaaaaadpaaaaaadpaaaaaaaaaaaaaaaaegaabaaaaaaaaaaadiaaaaakmcaabaaa
abaaaaaakgaobaaaabaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaadpaaaaaadp
dcaaaaakmcaabaaaadaaaaaakgaobaaaabaaaaaakgikcaaaaaaaaaaaadaaaaaa
agaebaaaadaaaaaadcaaaaakpcaabaaaaeaaaaaaogaobaaaabaaaaaaagifcaaa
aaaaaaaaadaaaaaaegaebaaaadaaaaaaefaaaaajpcaabaaaadaaaaaaogakbaaa
adaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaafaaaaaa
egaabaaaaeaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaa
aeaaaaaaogakbaaaaeaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadgaaaaaf
ccaabaaaadaaaaaabkaabaaaaeaaaaaadgaaaaafbcaabaaaadaaaaaaakaabaaa
afaaaaaabacaaaakecaabaaaabaaaaaaegacbaaaadaaaaaaaceaaaaagmajjjdo
glclbgdppkhookdnaaaaaaaaaaaaaaaihcaabaaaaeaaaaaaegacbaiaebaaaaaa
adaaaaaakgakbaaaabaaaaaadcaaaaakhcaabaaaadaaaaaaagiacaaaaaaaaaaa
acaaaaaaegacbaaaaeaaaaaaegacbaaaadaaaaaaaaaaaaajhcaabaaaadaaaaaa
egacbaaaadaaaaaaagiacaiaebaaaaaaaaaaaaaaacaaaaaaaaaaaaajecaabaaa
abaaaaaaakiacaiaebaaaaaaaaaaaaaaacaaaaaaabeaaaaaaaaaiadpaoaaaaak
ecaabaaaabaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadpckaabaaa
abaaaaaadicaaaahhcaabaaaadaaaaaakgakbaaaabaaaaaaegacbaaaadaaaaaa
efaaaaajpcaabaaaaeaaaaaaegaabaaaaaaaaaaaeghobaaaabaaaaaaaagabaaa
aaaaaaaabacaaaakicaabaaaabaaaaaaegacbaaaaeaaaaaaaceaaaaagmajjjdo
glclbgdppkhookdnaaaaaaaaaaaaaaaihcaabaaaafaaaaaaegacbaiaebaaaaaa
aeaaaaaapgapbaaaabaaaaaadcaaaaakhcaabaaaaeaaaaaaagiacaaaaaaaaaaa
acaaaaaaegacbaaaafaaaaaaegacbaaaaeaaaaaaaaaaaaajhcaabaaaaeaaaaaa
egacbaaaaeaaaaaaagiacaiaebaaaaaaaaaaaaaaacaaaaaadicaaaahhcaabaaa
aeaaaaaakgakbaaaabaaaaaaegacbaaaaeaaaaaadcaaaaakhcaabaaaadaaaaaa
egacbaaaadaaaaaapgipcaaaaaaaaaaaadaaaaaaegacbaaaaeaaaaaadcaaaaam
pcaabaaaaeaaaaaaegaebaaaabaaaaaaaceaaaaamnmmmmdnmnmmmmdnmnmmemdo
mnmmemdoegaebaaaaaaaaaaadcaaaaakpcaabaaaafaaaaaaegaebaaaacaaaaaa
agifcaaaaaaaaaaaadaaaaaaegaebaaaaeaaaaaaefaaaaajpcaabaaaagaaaaaa
egaabaaaafaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaa
afaaaaaaogakbaaaafaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadgaaaaaf
ccaabaaaagaaaaaabkaabaaaafaaaaaadcaaaaakpcaabaaaafaaaaaaegaobaaa
acaaaaaakgiacaaaaaaaaaaaadaaaaaaegaobaaaaeaaaaaadcaaaaakpcaabaaa
acaaaaaaogaobaaaacaaaaaafgikcaaaaaaaaaaaadaaaaaaogaobaaaaeaaaaaa
efaaaaajpcaabaaaaeaaaaaaegaabaaaafaaaaaaeghobaaaabaaaaaaaagabaaa
aaaaaaaaefaaaaajpcaabaaaafaaaaaaogakbaaaafaaaaaaeghobaaaabaaaaaa
aagabaaaaaaaaaaadgaaaaafecaabaaaagaaaaaackaabaaaaeaaaaaabacaaaak
icaabaaaabaaaaaaegacbaaaagaaaaaaaceaaaaagmajjjdoglclbgdppkhookdn
aaaaaaaaaaaaaaaihcaabaaaaeaaaaaaegacbaiaebaaaaaaagaaaaaapgapbaaa
abaaaaaadcaaaaakhcaabaaaaeaaaaaaagiacaaaaaaaaaaaacaaaaaaegacbaaa
aeaaaaaaegacbaaaagaaaaaaaaaaaaajhcaabaaaaeaaaaaaegacbaaaaeaaaaaa
agiacaiaebaaaaaaaaaaaaaaacaaaaaadicaaaahhcaabaaaaeaaaaaakgakbaaa
abaaaaaaegacbaaaaeaaaaaaaaaaaaahhcaabaaaadaaaaaaegacbaaaadaaaaaa
egacbaaaaeaaaaaaefaaaaajpcaabaaaaeaaaaaaegaabaaaacaaaaaaeghobaaa
abaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaacaaaaaaogakbaaaacaaaaaa
eghobaaaabaaaaaaaagabaaaaaaaaaaadgaaaaafecaabaaaafaaaaaackaabaaa
acaaaaaadgaaaaafccaabaaaafaaaaaabkaabaaaaeaaaaaabacaaaakicaabaaa
abaaaaaaegacbaaaafaaaaaaaceaaaaagmajjjdoglclbgdppkhookdnaaaaaaaa
aaaaaaaihcaabaaaacaaaaaaegacbaiaebaaaaaaafaaaaaapgapbaaaabaaaaaa
dcaaaaakhcaabaaaacaaaaaaagiacaaaaaaaaaaaacaaaaaaegacbaaaacaaaaaa
egacbaaaafaaaaaaaaaaaaajhcaabaaaacaaaaaaegacbaaaacaaaaaaagiacaia
ebaaaaaaaaaaaaaaacaaaaaadicaaaahhcaabaaaacaaaaaakgakbaaaabaaaaaa
egacbaaaacaaaaaaaaaaaaahhcaabaaaacaaaaaaegacbaaaacaaaaaaegacbaaa
adaaaaaadiaaaaakpcaabaaaadaaaaaaogaobaaaaaaaaaaaaceaaaaajkjjjjdo
jkjjjjdoaaaaaadpaaaaaadpdcaaaaampcaabaaaaeaaaaaaogaobaaaaaaaaaaa
aceaaaaajkjjjjdojkjjjjdoaaaaaadpaaaaaadpegbebaaaabaaaaaadcaaaaak
pcaabaaaafaaaaaaegaebaaaadaaaaaaagifcaaaaaaaaaaaadaaaaaaegaebaaa
aeaaaaaaefaaaaajpcaabaaaagaaaaaaegaabaaaafaaaaaaeghobaaaabaaaaaa
aagabaaaaaaaaaaaefaaaaajpcaabaaaafaaaaaaogakbaaaafaaaaaaeghobaaa
abaaaaaaaagabaaaaaaaaaaadgaaaaafccaabaaaagaaaaaabkaabaaaafaaaaaa
dcaaaaakpcaabaaaafaaaaaaegaobaaaadaaaaaakgiacaaaaaaaaaaaadaaaaaa
egaobaaaaeaaaaaadcaaaaakpcaabaaaadaaaaaaogaobaaaadaaaaaafgikcaaa
aaaaaaaaadaaaaaaogaobaaaaeaaaaaaefaaaaajpcaabaaaaeaaaaaaegaabaaa
afaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaafaaaaaa
ogakbaaaafaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadgaaaaafecaabaaa
agaaaaaackaabaaaaeaaaaaabacaaaakicaabaaaabaaaaaaegacbaaaagaaaaaa
aceaaaaagmajjjdoglclbgdppkhookdnaaaaaaaaaaaaaaaihcaabaaaaeaaaaaa
egacbaiaebaaaaaaagaaaaaapgapbaaaabaaaaaadcaaaaakhcaabaaaaeaaaaaa
agiacaaaaaaaaaaaacaaaaaaegacbaaaaeaaaaaaegacbaaaagaaaaaaaaaaaaaj
hcaabaaaaeaaaaaaegacbaaaaeaaaaaaagiacaiaebaaaaaaaaaaaaaaacaaaaaa
dicaaaahhcaabaaaaeaaaaaakgakbaaaabaaaaaaegacbaaaaeaaaaaaaaaaaaah
hcaabaaaacaaaaaaegacbaaaacaaaaaaegacbaaaaeaaaaaadcaaaaampcaabaaa
aeaaaaaaegaebaaaabaaaaaaaceaaaaamnmmmmdomnmmmmdodddddddpdddddddp
egaebaaaaaaaaaaadiaaaaakpcaabaaaagaaaaaaegaebaaaabaaaaaaaceaaaaa
mnmmmmdomnmmmmdodddddddpdddddddpdcaaaaakpcaabaaaahaaaaaaegaebaaa
agaaaaaaagifcaaaaaaaaaaaadaaaaaaegaebaaaaeaaaaaaefaaaaajpcaabaaa
aiaaaaaaegaabaaaahaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaefaaaaaj
pcaabaaaahaaaaaaogakbaaaahaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaa
dgaaaaafccaabaaaaiaaaaaabkaabaaaahaaaaaadcaaaaakpcaabaaaahaaaaaa
egaobaaaagaaaaaakgiacaaaaaaaaaaaadaaaaaaegaobaaaaeaaaaaadcaaaaak
pcaabaaaaeaaaaaaogaobaaaagaaaaaafgikcaaaaaaaaaaaadaaaaaaogaobaaa
aeaaaaaaefaaaaajpcaabaaaagaaaaaaegaabaaaahaaaaaaeghobaaaabaaaaaa
aagabaaaaaaaaaaaefaaaaajpcaabaaaahaaaaaaogakbaaaahaaaaaaeghobaaa
abaaaaaaaagabaaaaaaaaaaadgaaaaafecaabaaaaiaaaaaackaabaaaagaaaaaa
bacaaaakicaabaaaabaaaaaaegacbaaaaiaaaaaaaceaaaaagmajjjdoglclbgdp
pkhookdnaaaaaaaaaaaaaaaihcaabaaaagaaaaaaegacbaiaebaaaaaaaiaaaaaa
pgapbaaaabaaaaaadcaaaaakhcaabaaaagaaaaaaagiacaaaaaaaaaaaacaaaaaa
egacbaaaagaaaaaaegacbaaaaiaaaaaaaaaaaaajhcaabaaaagaaaaaaegacbaaa
agaaaaaaagiacaiaebaaaaaaaaaaaaaaacaaaaaadicaaaahhcaabaaaagaaaaaa
kgakbaaaabaaaaaaegacbaaaagaaaaaaaaaaaaahhcaabaaaacaaaaaaegacbaaa
acaaaaaaegacbaaaagaaaaaaefaaaaajpcaabaaaagaaaaaaegaabaaaadaaaaaa
eghobaaaabaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaadaaaaaaogakbaaa
adaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadgaaaaafecaabaaaafaaaaaa
ckaabaaaadaaaaaadgaaaaafccaabaaaafaaaaaabkaabaaaagaaaaaabacaaaak
icaabaaaabaaaaaaegacbaaaafaaaaaaaceaaaaagmajjjdoglclbgdppkhookdn
aaaaaaaaaaaaaaaihcaabaaaadaaaaaaegacbaiaebaaaaaaafaaaaaapgapbaaa
abaaaaaadcaaaaakhcaabaaaadaaaaaaagiacaaaaaaaaaaaacaaaaaaegacbaaa
adaaaaaaegacbaaaafaaaaaaaaaaaaajhcaabaaaadaaaaaaegacbaaaadaaaaaa
agiacaiaebaaaaaaaaaaaaaaacaaaaaadicaaaahhcaabaaaadaaaaaakgakbaaa
abaaaaaaegacbaaaadaaaaaaaaaaaaahhcaabaaaacaaaaaaegacbaaaacaaaaaa
egacbaaaadaaaaaadiaaaaakpcaabaaaadaaaaaaogaobaaaaaaaaaaaaceaaaaa
jkjjbjdpjkjjbjdpmnmmemdpmnmmemdpdcaaaaampcaabaaaafaaaaaaogaobaaa
aaaaaaaaaceaaaaajkjjbjdpjkjjbjdpmnmmemdpmnmmemdpegbebaaaabaaaaaa
dcaaaaamdcaabaaaaaaaaaaaegaabaaaabaaaaaaaceaaaaaghggggdpghggggdp
aaaaaaaaaaaaaaaaegaabaaaaaaaaaaadiaaaaakmcaabaaaaaaaaaaaagaebaaa
abaaaaaaaceaaaaaaaaaaaaaaaaaaaaaghggggdpghggggdpdcaaaaakpcaabaaa
agaaaaaaegaebaaaadaaaaaaagifcaaaaaaaaaaaadaaaaaaegaebaaaafaaaaaa
efaaaaajpcaabaaaaiaaaaaaegaabaaaagaaaaaaeghobaaaabaaaaaaaagabaaa
aaaaaaaaefaaaaajpcaabaaaagaaaaaaogakbaaaagaaaaaaeghobaaaabaaaaaa
aagabaaaaaaaaaaadgaaaaafccaabaaaaiaaaaaabkaabaaaagaaaaaadcaaaaak
pcaabaaaagaaaaaaegaobaaaadaaaaaakgiacaaaaaaaaaaaadaaaaaaegaobaaa
afaaaaaadcaaaaakpcaabaaaadaaaaaaogaobaaaadaaaaaafgikcaaaaaaaaaaa
adaaaaaaogaobaaaafaaaaaaefaaaaajpcaabaaaafaaaaaaegaabaaaagaaaaaa
eghobaaaabaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaagaaaaaaogakbaaa
agaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadgaaaaafecaabaaaaiaaaaaa
ckaabaaaafaaaaaabacaaaakbcaabaaaabaaaaaaegacbaaaaiaaaaaaaceaaaaa
gmajjjdoglclbgdppkhookdnaaaaaaaaaaaaaaailcaabaaaabaaaaaaegaibaia
ebaaaaaaaiaaaaaaagaabaaaabaaaaaadcaaaaaklcaabaaaabaaaaaaagiacaaa
aaaaaaaaacaaaaaaegambaaaabaaaaaaegaibaaaaiaaaaaaaaaaaaajlcaabaaa
abaaaaaaegambaaaabaaaaaaagiacaiaebaaaaaaaaaaaaaaacaaaaaadicaaaah
lcaabaaaabaaaaaakgakbaaaabaaaaaaegambaaaabaaaaaaaaaaaaahlcaabaaa
abaaaaaaegambaaaabaaaaaaegaibaaaacaaaaaaefaaaaajpcaabaaaacaaaaaa
egaabaaaaeaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaa
aeaaaaaaogakbaaaaeaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadgaaaaaf
ecaabaaaahaaaaaackaabaaaaeaaaaaadgaaaaafccaabaaaahaaaaaabkaabaaa
acaaaaaabacaaaakbcaabaaaacaaaaaaegacbaaaahaaaaaaaceaaaaagmajjjdo
glclbgdppkhookdnaaaaaaaaaaaaaaaihcaabaaaacaaaaaaegacbaiaebaaaaaa
ahaaaaaaagaabaaaacaaaaaadcaaaaakhcaabaaaacaaaaaaagiacaaaaaaaaaaa
acaaaaaaegacbaaaacaaaaaaegacbaaaahaaaaaaaaaaaaajhcaabaaaacaaaaaa
egacbaaaacaaaaaaagiacaiaebaaaaaaaaaaaaaaacaaaaaadicaaaahhcaabaaa
acaaaaaakgakbaaaabaaaaaaegacbaaaacaaaaaaaaaaaaahlcaabaaaabaaaaaa
egambaaaabaaaaaaegaibaaaacaaaaaaefaaaaajpcaabaaaacaaaaaaegaabaaa
adaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaadaaaaaa
ogakbaaaadaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadgaaaaafecaabaaa
agaaaaaackaabaaaadaaaaaadgaaaaafccaabaaaagaaaaaabkaabaaaacaaaaaa
bacaaaakbcaabaaaacaaaaaaegacbaaaagaaaaaaaceaaaaagmajjjdoglclbgdp
pkhookdnaaaaaaaaaaaaaaaihcaabaaaacaaaaaaegacbaiaebaaaaaaagaaaaaa
agaabaaaacaaaaaadcaaaaakhcaabaaaacaaaaaaagiacaaaaaaaaaaaacaaaaaa
egacbaaaacaaaaaaegacbaaaagaaaaaaaaaaaaajhcaabaaaacaaaaaaegacbaaa
acaaaaaaagiacaiaebaaaaaaaaaaaaaaacaaaaaadicaaaahhcaabaaaacaaaaaa
kgakbaaaabaaaaaaegacbaaaacaaaaaaaaaaaaahlcaabaaaabaaaaaaegambaaa
abaaaaaaegaibaaaacaaaaaadcaaaaakdcaabaaaacaaaaaaogakbaaaaaaaaaaa
kgikcaaaaaaaaaaaadaaaaaaegaabaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaa
ogaobaaaaaaaaaaaagifcaaaaaaaaaaaadaaaaaaegaebaaaaaaaaaaaefaaaaaj
pcaabaaaacaaaaaaegaabaaaacaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaa
efaaaaajpcaabaaaadaaaaaaegaabaaaaaaaaaaaeghobaaaabaaaaaaaagabaaa
aaaaaaaaefaaaaajpcaabaaaaaaaaaaaogakbaaaaaaaaaaaeghobaaaabaaaaaa
aagabaaaaaaaaaaadgaaaaafccaabaaaacaaaaaabkaabaaaaaaaaaaadgaaaaaf
bcaabaaaacaaaaaaakaabaaaadaaaaaabacaaaakbcaabaaaaaaaaaaaegacbaaa
acaaaaaaaceaaaaagmajjjdoglclbgdppkhookdnaaaaaaaaaaaaaaaihcaabaaa
aaaaaaaaegacbaiaebaaaaaaacaaaaaaagaabaaaaaaaaaaadcaaaaakhcaabaaa
aaaaaaaaagiacaaaaaaaaaaaacaaaaaaegacbaaaaaaaaaaaegacbaaaacaaaaaa
aaaaaaajhcaabaaaaaaaaaaaegacbaaaaaaaaaaaagiacaiaebaaaaaaaaaaaaaa
acaaaaaadicaaaahhcaabaaaaaaaaaaakgakbaaaabaaaaaaegacbaaaaaaaaaaa
aaaaaaahhcaabaaaaaaaaaaaegacbaaaaaaaaaaaegadbaaaabaaaaaadicaaaak
hcaabaaaaaaaaaaaegacbaaaaaaaaaaaaceaaaaamnmmmmdnmnmmmmdnmnmmmmdn
aaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaabaaaaaaeghobaaaacaaaaaa
aagabaaaacaaaaaadiaaaaahhcaabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaa
abaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaa
aagabaaaabaaaaaadgaaaaaficaabaaaaaaaaaaaabeaaaaaaaaaaaaadcaaaaak
pccabaaaaaaaaaaaegaobaaaaaaaaaaafgifcaaaaaaaaaaaacaaaaaaegaobaaa
abaaaaaadoaaaaabejfdeheoiaaaaaaaaeaaaaaaaiaaaaaagiaaaaaaaaaaaaaa
abaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaaaaaaaaaaaaaaaaaaadaaaaaa
abaaaaaaadadaaaaheaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamaaaaaa
heaaaaaaacaaaaaaaaaaaaaaadaaaaaaacaaaaaaadaaaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaa
caaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgf
heaaklkl"
}

}

#LINE 161

            }
        }
    }