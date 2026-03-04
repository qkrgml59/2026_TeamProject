// Copyright (c) 2025 CriticalVFX
// This script is part of the Stylized Critical Slash FX package.
// For licensing information, please refer to the included LICENSE file.

using UnityEditor;
using UnityEngine;

namespace CriticalVFX.Editor
{
    enum ImageAlignment
    {
        Left,
        Center,
        Right
    }

    public class SplashShaderInspector : ShaderGUI
    {
        #region Surface Options 변수
        MaterialProperty surfaceType;
        MaterialProperty blendingMode;
        MaterialProperty renderFace;
        MaterialProperty depthWrite;
        MaterialProperty depthTest;
        MaterialProperty castShadows;
        //bool showSurfaceType = true;
        bool showSurfaceOptions = true;

        string[] surfaceTypeOptions = new string[] 
        { 
            "Opaque",
            "Transparent" 
        };

        string[] blendingModeOptions = new string[] 
        { 
            "Alpha",
            "Premultiply",
            "Additive",
            "Multiply" 
        };

        string[] renderFaceOptions = new string[] 
        { 
            "Both",
            "Back",
            "Front" 
        };

        string[] depthWriteOptions = new string[] 
        { 
            /* "Auto", */
            "ForceEnabled",
            "ForceDisabled" 
        };

        string[] depthTestOptions = new string[] 
        { 
            "Never",     // 1
            "Less",      // 2
            "Equal",     // 3
            "LEqual",    // 4
            "Greater",   // 5
            "NotEqual",  // 6
            "GEqual",    // 7
            "Always"     // 8
        };

        #endregion

        #region Advenced Options 변수
        MaterialProperty queueControl;
        MaterialProperty soatingPriority;
        MaterialProperty renderQuque;
        string[] queueControlOptions = new string[] 
        { 
            "Auto",
            "UserOverride" 
        };

        string[] renderQueueOptions = new string[] 
        { 
            "From Shader",
            "Geometry",
            "AlphaTest",
            "Transparent", 
        };
        bool showAdvancedOptions = true;

        #endregion
        
        #region Custom Optios 변수
        MaterialProperty mainTex;
        MaterialProperty brightness;
        MaterialProperty useScreenPos;

        MaterialProperty useGradient;
        MaterialProperty gradientTex;

        MaterialProperty useMask;
        MaterialProperty maskTex;

        MaterialProperty useDistortion;
        MaterialProperty distortionTex;

        MaterialProperty useDissolve;
        MaterialProperty dissolveTex;
        MaterialProperty dissolveType;
        MaterialProperty softEdge;
        MaterialProperty edgeThinkness;
        MaterialProperty edgeColor;
        MaterialProperty edgeBrightness;

        MaterialProperty useFaceSetting;
        MaterialProperty frontFaceColor;
        MaterialProperty backFaceColor;

        MaterialProperty useVertexOffset;
        MaterialProperty noiseScale;
        MaterialProperty intencity;
        MaterialProperty speed;

        MaterialProperty useDepthFade;
        MaterialProperty fadeDistance;

        MaterialProperty customData1_x;
        MaterialProperty customData1_y;
        MaterialProperty customData1_z;
        MaterialProperty customData1_w;
        MaterialProperty customData2_x;
        MaterialProperty customData2_y;
        MaterialProperty customData2_z;
        MaterialProperty customData2_w;

        bool showCustomDataSetting = true;

        string[] options = new string[] 
        { 
            "None", 
            "MainTex_Tile_X", 
            "MainTex_Tile_Y", 
            "MainTex_Offset_X", 
            "MainTex_Offset_Y",

            "GradientTex_Tile_X", 
            "GradientTex_Tile_Y", 
            "GradientTex_Offset_X", 
            "GradientTex_Offset_Y",        

            "MaskTex_Tile_X", 
            "MaskTex_Tile_Y", 
            "MaskTex_Offset_X", 
            "MaskTex_Offset_Y",

            "DistortionTex_Tile_X", 
            "DistortionTex_Tile_Y", 
            "DistortionTex_Offset_X", 
            "DistortionTex_Offset_Y",        

            "DissolveTex_Tile_X", 
            "DissolveTex_Tile_Y", 
            "DissolveTex_Offset_X", 
            "DissolveTex_Offset_Y", 

            "Distortion_Intencity",
            "Dissolve_Intencity"
        };

        bool showCustomOptions = true;
        string msg00 = "This is the base main texture. You can control its tiling and offset by mapping them to custom data. Enabling the 'Use ScreenPosition' option allows you to use screen-space UV coordinates.";
        string msg01 = "Enabling this option adds a gradient color. The gradient texture’s tiling and offset can be controlled by mapping them to custom data.";
        string msg02 = "Enabling this option allows the use of a mask. The mask texture’s tiling and offset can be controlled by mapping them to custom data.";
        string msg03 = "Enabling this option allows the use of distortion. When this option is active, you cannot simultaneously control the main texture’s tiling and offset.";
        string msg04 = "Enabling this option allows the use of dissolve. When the dissolve type is set to 'Step', you can apply a color to the edges. The dissolve fades from 1 to 0. The dissolve texture’s tiling and offset can be controlled by mapping them to custom data.";
        string msg05 = "Enabling this option allows you to assign different colors to the front and back sides of the mesh.";
        string msg06 = "Enabling this option allows the use of vertex offset. The noise scroll of the vertex offset moves based on the Time value.";
        string msg07 = "You can freely map shader features using particle custom data. This shader uses Custom Vertex Streams (UV1, UV2, Custom1, Custom2).";
        string msg08 = "Enabling this option allows the use of depth fade. Depth fade works when colliding with opaque shaders.";

        #endregion

        Color pointColor = new Color(0.1529412f, 0.7490196f, 1f);
        //Color pointColor = new Color(1f, .5f, 0);

        Color colorA = new Color(0.3f, 0.3f, 0.3f);
        Color colorB = new Color(0.15f, 0.15f, 0.15f);

        #region OnGUI
        // OnGUI
        public override void OnGUI(MaterialEditor _materialEditor, MaterialProperty[] _properties)
        {
            FindProperty(_properties);
            DrawTitle();
            
            Material material = _materialEditor.target as Material;

            showSurfaceOptions = EditorGUILayout.Foldout(showSurfaceOptions, "Surface Options");
            DrawUILine(pointColor, 1);
            if (showSurfaceOptions)
            {
                DrawDropdown(surfaceType, "Surface Type", surfaceTypeOptions);
                EditorGUI.BeginChangeCheck();
                DrawDropdown(blendingMode, "Blending Mode", blendingModeOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    int blendMode = Mathf.FloorToInt(blendingMode.floatValue);
                    if (material != null)
                    {
                        switch (blendMode)
                        {
                            case 0: // Alpha
                                material.SetOverrideTag("RenderType", "Transparent");
                                material.SetOverrideTag("Queue", "Transparent");
                                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                                break;
                            case 1: // Premultiply
                                material.SetOverrideTag("RenderType", "Transparent");
                                material.SetOverrideTag("Queue", "Transparent");
                                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                                break;
                            case 2: // Additive
                                material.SetOverrideTag("RenderType", "Transparent");
                                material.SetOverrideTag("Queue", "Transparent");
                                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                                break;
                            case 3: // Multiply
                                material.SetOverrideTag("RenderType", "Transparent");
                                material.SetOverrideTag("Queue", "Transparent");
                                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                                break;
                        }
                    }
                }
                DrawDropdown(renderFace, "Render Face", renderFaceOptions);
                DrawDepthWriteDropdown(depthWrite, "Depth Write", depthWriteOptions, material);
                DrawDepthTestDropdown(depthTest, "Depth Test", depthTestOptions);

                MaterialProperty alphaClip = FindProperty("_AlphaClip", _properties);
                
                EditorGUI.BeginChangeCheck();
                bool alphaClipToggle = alphaClip != null && alphaClip.floatValue > 0.5f;
                alphaClipToggle = EditorGUILayout.Toggle("Alpha Clipping", alphaClipToggle);
                if (EditorGUI.EndChangeCheck())
                {
                    alphaClip.floatValue = alphaClipToggle ? 1.0f : 0.0f;
                    if (material != null)
                    {
                        if (alphaClipToggle)
                            material.EnableKeyword("_ALPHATEST_ON");
                        else
                            material.DisableKeyword("_ALPHATEST_ON");
                    }
                }

                EditorGUI.BeginChangeCheck();
                bool castShadowsToggle = castShadows != null && castShadows.floatValue > 0.5f;
                castShadowsToggle = EditorGUILayout.Toggle("Cast Shadows", castShadowsToggle);
                if (EditorGUI.EndChangeCheck())
                {
                    castShadows.floatValue = castShadowsToggle ? 1.0f : 0.0f;
                    
                    if (material != null)
                    {
                        material.SetShaderPassEnabled("ShadowCaster", castShadowsToggle);
                    }
                }

            }  

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, "Advanced Options");
            DrawUILine(pointColor, 1);
            if (showAdvancedOptions)
            {
                
                DrawDropdown(queueControl, "Queue Control", queueControlOptions);
                MaterialProperty queueOffsetProp = FindProperty("_QueueOffset", _properties);
                if (queueControl != null && queueControl.floatValue == 0)
                {
                    if (queueOffsetProp != null)
                    {
                        int baseQueue = material.shader.renderQueue;

                        EditorGUI.BeginChangeCheck();
                        float queueOffsetSlider = queueOffsetProp.floatValue;
                        queueOffsetSlider = EditorGUILayout.Slider("Sorting Priority", queueOffsetSlider, -50f, 50f);
                        if (EditorGUI.EndChangeCheck())
                        {
                            queueOffsetProp.floatValue = queueOffsetSlider;
                            int queueOffset = Mathf.RoundToInt(queueOffsetSlider);
                            material.renderQueue = baseQueue + queueOffset;
                        }
                    }
                }
                else if (queueControl != null && queueControl.floatValue == 1)
                {
                    EditorGUI.BeginChangeCheck();
                    int renderQueue = material.renderQueue;
                    renderQueue = EditorGUILayout.IntField("Render Queue", renderQueue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        material.renderQueue = renderQueue;
                    }
                    
                    string[] renderQueueEnumOptions = { "From Shader", "Geometry", "AlphaTest", "Transparent" };
                    int selected = 0;
                    if (renderQueue >= 3000)
                        selected = 3;
                    else if (renderQueue >= 2450 && renderQueue < 3000)
                        selected = 2;
                    else if (renderQueue >= 2000 && renderQueue < 2450)
                        selected = 1;
                    else
                        selected = 0;
                    
                    EditorGUI.BeginChangeCheck();
                    selected = EditorGUILayout.Popup(" ", selected, renderQueueEnumOptions);
                    if (EditorGUI.EndChangeCheck())
                    {
                        switch (selected)
                        {
                            case 0: 
                                material.renderQueue = -1;
                                break;
                            case 1: 
                                material.renderQueue = 2000;
                                break;
                            case 2: 
                                material.renderQueue = 2450;
                                break;
                            case 3: 
                                material.renderQueue = 3000;
                                break;
                        }
                    }
                }

                EditorGUI.BeginChangeCheck();
                bool instancing = material.enableInstancing;
                instancing = EditorGUILayout.Toggle("Enable GPU Instancing", instancing);
                if (EditorGUI.EndChangeCheck())
                {
                    material.enableInstancing = instancing;
                }
            }  

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            showCustomOptions = EditorGUILayout.Foldout(showCustomOptions, "Custom Shader Options");
            DrawUILine(pointColor, 1);
            if (showCustomOptions)
            {
                _materialEditor.ShaderProperty(mainTex, "Main Texture");
                _materialEditor.ShaderProperty(brightness, "Brightness");
                _materialEditor.ShaderProperty(useScreenPos, "Use ScreenPosition");
                if (material != null)
                {
                    if (useScreenPos != null)
                    {
                        if (useScreenPos.floatValue > 0.5f)
                        {
                            material.EnableKeyword("USE_SCREEN_POS");
                        }
                        else
                        {
                            material.DisableKeyword("USE_SCREEN_POS");
                        }
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(msg00, MessageType.Info);
                EditorGUILayout.Space();
                DrawUILine(colorA, 1);

                _materialEditor.ShaderProperty(useGradient, "Gradient");
                if (useGradient != null && useGradient.floatValue > 0)
                {
                    _materialEditor.ShaderProperty(gradientTex, "Gradient Texture");
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox(msg01, MessageType.Info);
                }
                EditorGUILayout.Space();
                DrawUILine(colorA, 1);

                _materialEditor.ShaderProperty(useMask, "Mask");
                if (useMask != null && useMask.floatValue > 0)
                {
                    _materialEditor.ShaderProperty(maskTex, "Mask Texture");
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox(msg02, MessageType.Info);
                }

                EditorGUILayout.Space();
                DrawUILine(colorA, 1);
                
                _materialEditor.ShaderProperty(useDistortion, "Distortion");
                if (useDistortion != null && useDistortion.floatValue > 0)
                {
                    _materialEditor.ShaderProperty(distortionTex, "Distortion Texture");
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox(msg03, MessageType.Info);
                }

                EditorGUILayout.Space();
                DrawUILine(colorA, 1);

                _materialEditor.ShaderProperty(useDissolve, "Dissolve");
                if (useDissolve != null && useDissolve.floatValue > 0)
                {
                    _materialEditor.ShaderProperty(dissolveTex, "Dissolve Texture");

                    int selected = (int)dissolveType.floatValue;
                    string[] options = { "Step", "SmoothStep"};
                    
                    EditorGUI.BeginChangeCheck();
                    selected = EditorGUILayout.Popup("Dissolve Type", selected, options);
                    if (EditorGUI.EndChangeCheck())
                    {
                        dissolveType.floatValue = selected;
        
                        foreach (Material mat in _materialEditor.targets)
                        {
                            SetKeyword(mat, "Step", selected == 0);
                            SetKeyword(mat, "SmoothStep", selected == 1);
                            UnityEditor.EditorUtility.SetDirty(mat);
                            var path = UnityEditor.AssetDatabase.GetAssetPath(mat);
                            var importer = UnityEditor.AssetImporter.GetAtPath(path);
                            if (importer != null)
                            {
                                importer.SaveAndReimport();
                            }
                        }
                    }
                    EditorGUI.showMixedValue = false;

                    if(selected == 0) 
                    {
                        _materialEditor.ShaderProperty(edgeThinkness, "Step Edge Thinkness");
                        _materialEditor.ShaderProperty(edgeColor, "Step Edge Color");
                        _materialEditor.ShaderProperty(edgeBrightness, "Step Edge Brightness");
                    }
                    if(selected == 1) 
                    {
                        _materialEditor.ShaderProperty(softEdge, "SmoothStep Soft Edge");
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox(msg04, MessageType.Info);
                }

                EditorGUILayout.Space();
                DrawUILine(colorA, 1);

                _materialEditor.ShaderProperty(useFaceSetting, "Face Color");
                if (useFaceSetting != null && useFaceSetting.floatValue > 0)
                {
                    _materialEditor.ShaderProperty(frontFaceColor, "Front Face Color");
                    _materialEditor.ShaderProperty(backFaceColor, "Back Face Color");
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox(msg05, MessageType.Info);
                }

                EditorGUILayout.Space();
                DrawUILine(colorA, 1);

                _materialEditor.ShaderProperty(useVertexOffset, "Vertex Offset");
                if (useVertexOffset != null && useVertexOffset.floatValue > 0)
                {
                    _materialEditor.ShaderProperty(noiseScale, "Noise Scale");
                    _materialEditor.ShaderProperty(intencity, "Intencity");
                    _materialEditor.ShaderProperty(speed, "Speed");
                    EditorGUILayout.HelpBox(msg06, MessageType.Info);
                }

                EditorGUILayout.Space();
                DrawUILine(colorA, 1);

                _materialEditor.ShaderProperty(useDepthFade, "Depth Fade");
                if (useDepthFade != null && useDepthFade.floatValue > 0)
                {
                    _materialEditor.ShaderProperty(fadeDistance, "Fade Distance");
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox(msg08, MessageType.Info);
                }

                EditorGUILayout.Space();
                DrawUILine(colorA, 1);

                EditorGUI.indentLevel++;
                showCustomDataSetting = EditorGUILayout.Foldout(showCustomDataSetting, "CustomData Setting");
                if (showCustomDataSetting)
                {
                    DrawDropdown(customData1_x, "CustomData1.x Port", options);
                    DrawDropdown(customData1_y, "CustomData1.y Port", options);
                    DrawDropdown(customData1_z, "CustomData1.z Port", options);
                    DrawDropdown(customData1_w, "CustomData1.w Port", options);
                    DrawDropdown(customData2_x, "CustomData2.x Port", options);
                    DrawDropdown(customData2_y, "CustomData2.y Port", options);
                    DrawDropdown(customData2_z, "CustomData2.z Port", options);
                    DrawDropdown(customData2_w, "CustomData2.w Port", options);
                    
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox(msg07, MessageType.Info);

                }  
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();

            }  
        }
        #endregion

        #region Helper
        void FindProperty(MaterialProperty[] properties)
        {
            mainTex = FindProperty("_MainTex", properties);
            brightness = FindProperty("_Brightness", properties);
            useScreenPos = FindProperty("_Use_ScreenPos", properties);

            useGradient = FindProperty("_Use_Gradient", properties);
            gradientTex = FindProperty("_GradientTex", properties);

            useMask = FindProperty("_Use_Mask", properties);
            maskTex = FindProperty("_MaskTex", properties);

            useDistortion = FindProperty("_Use_Distortion", properties);
            distortionTex = FindProperty("_DistortionTex", properties);

            useDissolve = FindProperty("_Use_Dissolve", properties);
            dissolveTex = FindProperty("_DissolveTex", properties);
            dissolveType = FindProperty("_DISSOLVETYPE", properties);
            softEdge = FindProperty("_SmoothStep_SoftEdge", properties);
            edgeThinkness = FindProperty("_Step_EdgeThinkness", properties);
            edgeColor = FindProperty("_Step_EdgeColor", properties);
            edgeBrightness = FindProperty("_Step_EdgeBrightness", properties);

            useFaceSetting = FindProperty("_Use_Face_Setting", properties);
            frontFaceColor = FindProperty("_FrontFace_Color", properties);
            backFaceColor = FindProperty("_BackFace_Color", properties);
            
            useVertexOffset = FindProperty("_Use_VertexOffset", properties);
            intencity = FindProperty("_Intencity", properties);
            speed = FindProperty("_Speed", properties);
            noiseScale = FindProperty("_NoiseScale", properties);

            useDepthFade = FindProperty("_Use_DepthFade", properties);
            fadeDistance = FindProperty("_FadeDistance", properties);

            customData1_x = FindProperty("_CustomData1_x_Port", properties);
            customData1_y = FindProperty("_CustomData1_y_Port", properties);
            customData1_z = FindProperty("_CustomData1_z_Port", properties);
            customData1_w = FindProperty("_CustomData1_w_Port", properties);
            customData2_x = FindProperty("_CustomData2_x_Port", properties);
            customData2_y = FindProperty("_CustomData2_y_Port", properties);
            customData2_z = FindProperty("_CustomData2_z_Port", properties);
            customData2_w = FindProperty("_CustomData2_w_Port", properties);

            castShadows = FindProperty("_CastShadows", properties);
            surfaceType = FindProperty("_Surface", properties);
            blendingMode = FindProperty("_Blend", properties);
            renderFace = FindProperty("_Cull", properties);
            depthWrite = FindProperty("_ZWrite", properties);
            depthTest = FindProperty("_ZTest", properties);

            queueControl = FindProperty("_QueueControl", properties);
        }

        void DrawDropdown(MaterialProperty property, string label, string[] options)
        {
            if (property == null)
                return;

            EditorGUI.BeginChangeCheck();
            int selected = Mathf.Clamp(Mathf.FloorToInt(property.floatValue), 0, options.Length - 1);
            selected = EditorGUILayout.Popup(label, selected, options);
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = selected;
            }
        }

        void DrawDepthTestDropdown(MaterialProperty property, string label, string[] options)
        {
            if (property == null)
                return;

            EditorGUI.BeginChangeCheck();

            int selected = Mathf.Clamp(Mathf.FloorToInt(property.floatValue) - 1, 0, options.Length - 1);
            selected = EditorGUILayout.Popup(label, selected, options);
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = selected + 1;
                Debug.Log(property.floatValue);
            }
        }

        void DrawDepthWriteDropdown(MaterialProperty property, string label, string[] options, Material material)
        {
            if (property == null || material == null)
                return;

            EditorGUI.BeginChangeCheck();

            int selected = 0;
            if (property.floatValue == 0f)
                selected = 1; 
            else if (property.floatValue == 1f)
                selected = 0; 

            selected = EditorGUILayout.Popup(label, selected, options);

            if (EditorGUI.EndChangeCheck())
            {
                if (selected == 0) 
                {
                    if (material.HasProperty("_ZWrite"))
                        material.SetFloat("_ZWrite", 1.0f);
                }
                else if (selected == 1)
                {
                    if (material.HasProperty("_ZWrite"))
                        material.SetFloat("_ZWrite", 0.0f);
                }

                if (selected == 0)
                    property.floatValue = 0f;
                else if (selected == 1)
                    property.floatValue = 1f;

            }
        }

        void SetKeyword(Material mat, string keyword, bool enable)
        {
            if (enable)
                mat.EnableKeyword(keyword);
            else
                mat.DisableKeyword(keyword);
        }

        void DrawUILine(Color color, int thickness = 1, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        void DrawTitle()
        {
            float targetHeight = 100f; 
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(targetHeight));
            rect.xMin = 0;
            rect.width = EditorGUIUtility.currentViewWidth;

            EditorGUI.DrawRect(rect, colorB);

            Texture2D titleImage1 = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CriticalVFX/StylizedCriticalSlashFX/Editor/Image/01.png");
            Texture2D titleImage2 = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CriticalVFX/StylizedCriticalSlashFX/Editor/Image/02.png");

            DrawImage(rect, titleImage1, targetHeight, CriticalVFX.Editor.ImageAlignment.Left);
            DrawImage(rect, titleImage2, targetHeight, CriticalVFX.Editor.ImageAlignment.Right);
        }

        void DrawImage(Rect rect, Texture2D image, float targetHeight, CriticalVFX.Editor.ImageAlignment alignment)
        {
            if (image != null)
            {
                float aspect = (float)image.width / (float)image.height;
                float width = targetHeight * aspect;
                float height = targetHeight;

                float posX = 0f;
                switch (alignment)
                {
                    case CriticalVFX.Editor.ImageAlignment.Left:
                        posX = rect.x;
                        break;
                    case CriticalVFX.Editor.ImageAlignment.Center:
                        posX = rect.x + (rect.width - width) * 0.5f;
                        break;
                    case CriticalVFX.Editor.ImageAlignment.Right:
                        posX = rect.x + rect.width - width;
                        break;
                }

                Rect imageRect = new Rect(posX, rect.y, width, height);
                GUI.DrawTexture(imageRect, image, ScaleMode.ScaleToFit);
            }
        }
        #endregion
    }
}