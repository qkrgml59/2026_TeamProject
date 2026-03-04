void CustomDataSelector_float(
    float4 uv1,
    float4 uv2,
    float4 customFeatureMapping1,
    float4 customFeatureMapping2,
    out float MainTex_Tile_X,
    out float MainTex_Tile_Y,
    out float MainTex_Offset_X,
    out float MainTex_Offset_Y,
    out float GradientTex_Tile_X,
    out float GradientTex_Tile_Y,
    out float GradientTex_Offset_X,
    out float GradientTex_Offset_Y,
    out float MaskTex_Tile_X,
    out float MaskTex_Tile_Y,
    out float MaskTex_Offset_X,
    out float MaskTex_Offset_Y,
    out float DistortionTex_Tile_X,
    out float DistortionTex_Tile_Y,
    out float DistortionTex_Offset_X,
    out float DistortionTex_Offset_Y,
    out float DissolveTex_Tile_X,
    out float DissolveTex_Tile_Y,
    out float DissolveTex_Offset_X,
    out float DissolveTex_Offset_Y,
    out float Distortion_Intencity,
    out float Dissolve_Intencity
)
{
    float ports[8];
    ports[0] = uv1.x;
    ports[1] = uv1.y;
    ports[2] = uv1.z;
    ports[3] = uv1.w;

#if defined(SHADER_API_METAL) || defined(SHADER_API_D3D11) || defined(SHADER_API_D3D12)
    ports[4] = uv2.x;
    ports[5] = uv2.y;
    ports[6] = uv2.w;
    ports[7] = uv2.z;
#else
    ports[4] = uv2.x;
    ports[5] = uv2.y;
    ports[6] = uv2.z;
    ports[7] = uv2.w;
#endif

    int featureMappingPort[8];
    featureMappingPort[0] = (int)customFeatureMapping1.x;
    featureMappingPort[1] = (int)customFeatureMapping1.y;
    featureMappingPort[2] = (int)customFeatureMapping1.z;
    featureMappingPort[3] = (int)customFeatureMapping1.w;
    featureMappingPort[4] = (int)customFeatureMapping2.x;
    featureMappingPort[5] = (int)customFeatureMapping2.y;
    featureMappingPort[6] = (int)customFeatureMapping2.z;
    featureMappingPort[7] = (int)customFeatureMapping2.w;

    float Value[22];
    [unroll]
    for (int v = 0; v < 22; v++)
    {
        Value[v] = 0.0;
    }

    [unroll]
    for (int i = 0; i < 8; i++)
    {
        int idx = featureMappingPort[i] - 1;
        if (idx >= 0 && idx < 22)
        {
            Value[idx] = ports[i];
        }
    }

    MainTex_Tile_X         = Value[0];
    MainTex_Tile_Y         = Value[1];
    MainTex_Offset_X       = Value[2];
    MainTex_Offset_Y       = Value[3];
    GradientTex_Tile_X     = Value[4];
    GradientTex_Tile_Y     = Value[5];
    GradientTex_Offset_X   = Value[6];
    GradientTex_Offset_Y   = Value[7];
    MaskTex_Tile_X         = Value[8];
    MaskTex_Tile_Y         = Value[9];
    MaskTex_Offset_X       = Value[10];
    MaskTex_Offset_Y       = Value[11];
    DistortionTex_Tile_X   = Value[12];
    DistortionTex_Tile_Y   = Value[13];
    DistortionTex_Offset_X = Value[14];
    DistortionTex_Offset_Y = Value[15];
    DissolveTex_Tile_X     = Value[16];
    DissolveTex_Tile_Y     = Value[17];
    DissolveTex_Offset_X   = Value[18];
    DissolveTex_Offset_Y   = Value[19];
    Distortion_Intencity   = Value[20];
    Dissolve_Intencity     = Value[21];
}