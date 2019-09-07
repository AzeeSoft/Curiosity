// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

#ifndef UNITY_STANDARD_CORE_FORWARD_INCLUDED
#define UNITY_STANDARD_CORE_FORWARD_INCLUDED

#if defined(UNITY_NO_FULL_STANDARD_SHADER)
#   define UNITY_STANDARD_SIMPLE 1
#endif

#include "UnityStandardConfig.cginc"

/* #if UNITY_STANDARD_SIMPLE
    #include "UnityStandardCoreForwardSimple.cginc"
    VertexOutputBaseSimple vertBase (VertexInput v) { return vertForwardBaseSimple(v); }
    VertexOutputForwardAddSimple vertAdd (VertexInput v) { return vertForwardAddSimple(v); }
    half4 fragBase (VertexOutputBaseSimple i) : SV_Target { return fragForwardBaseSimpleInternal(i); }
    half4 fragAdd (VertexOutputForwardAddSimple i) : SV_Target { return fragForwardAddSimpleInternal(i); }
#else

#endif */

#include "UnityStandardCore.cginc"
VertexOutputForwardBase vertBase (VertexInput v) { return vertForwardBase(v); }
[maxvertexcount(3)] void geomBase (triangle VertexOutputForwardBase IN[3], inout TriangleStream<VertexOutputForwardBase> triStream) { geomForwardBase(IN, triStream); }
half4 fragBase (VertexOutputForwardBase i) : SV_Target { return fragForwardBaseInternal(i); }

VertexOutputForwardAdd vertAdd (VertexInput v) { return vertForwardAdd(v); }
// VertexOutputForwardAdd geomAdd (VertexOutputForwardBase i) { return vertForwardAdd(v); }
half4 fragAdd (VertexOutputForwardAdd i) : SV_Target { return fragForwardAddInternal(i); }

#endif // UNITY_STANDARD_CORE_FORWARD_INCLUDED
