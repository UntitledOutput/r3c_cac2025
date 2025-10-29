
#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

#ifndef SHADERGRAPH_PREVIEW
    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
    #if (SHADERPASS != SHADERPASS_FORWARD)
        #undef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
    #endif
#endif


struct CustomLightingData {
    // Surface attributes
    float3 albedo;
    float3 normalWS;
    float3 positionWS;

    float3 viewDirectionWS;
    float4 shadowCoord;
    
    float specular;
};


#ifndef SHADERGRAPH_PREVIEW
float GetSmoothnessPower(float rawSmoothness) {
    return exp2(10 * rawSmoothness + 1);
}

void CustomLightHandling(CustomLightingData d, Light light, out float3 diffuse, out float specular) {

    float3 radiance = light.color * (light.distanceAttenuation * light.shadowAttenuation);

    float _d = saturate(dot(d.normalWS, light.direction));
    float specularDot = max(dot(d.normalWS, normalize(light.direction + d.viewDirectionWS)), 0);
    float s  = pow(specularDot, GetSmoothnessPower(d.specular)) * _d;

    diffuse += radiance * _d;
}

#endif

void GetAdditionalLighting_float(float3 WorldPos, float3 WorldNormal, float3 ViewDirection, float Specular,
    out float OutSpecular, out float3 OutDiffuse) {



    
#ifndef SHADERGRAPH_PREVIEW

    CustomLightingData d;
    d.positionWS = WorldPos;
    d.normalWS = WorldNormal;
    d.specular = Specular;
    d.viewDirectionWS = ViewDirection;

    OutSpecular = 0;
    OutDiffuse = float3(0,0,0);

    // Shade additional cone and point lights. Functions in URP/ShaderLibrary/Lighting.hlsl
    uint numAdditionalLights = GetAdditionalLightsCount();
    for (uint lightI = 0; lightI < numAdditionalLights; lightI++) {
        Light light = GetAdditionalLight(lightI, d.positionWS, 1);
        CustomLightHandling(d, light,OutDiffuse,OutSpecular);
    }
#else
    OutDiffuse = 1;
    OutSpecular = 1;
#endif
    
}

#endif