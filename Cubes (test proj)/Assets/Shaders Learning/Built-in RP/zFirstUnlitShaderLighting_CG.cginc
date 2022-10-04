            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            //#define USE_LIGHTING //key difinition
            //#define BASE_PASS //key difinition

            #define TAU 6.28318530718

            float4 _ColorBase;
            float4 _ColorSecond;
            float4 _AmbientLight;

            float _waveAmp;
            float _pulseAmp;
            float _distortionAmp;
            float _pulseDist;

            float _Gloss;
            float _NormalIntensity;
            float _DispStrenght;

            float _UOffset;
            float _VOffset;

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NormalTex;
            sampler2D _HeightTex;

            struct MeshData
            {
                
                float4 vertex : POSITION;
                float3 normals : NORMAL; //local space normal direction
                float2 uv0 : TEXCOORD0; //uv0 diffuse/normal map texture

                //float4 uv1 : TEXCOORD1; //uv1 coordinates lightmap coordinates
                //float4 uv2 : TEXCOORD1; //uv2 coordinates lightmap coordinates
                float4 tangent : TANGENT; // tangent dirrection = (xyz) tangent sign = (w)
                //float4 color : COLOR; //vertex colors
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal :TEXCOORD1;
                float3 worldPos :TEXCOORD2;
                LIGHTING_COORDS(3,4)
                float3 tangent: TEXCOORD5;
                float3 bitangent: TEXCOORD6;
                


                //UNITY_FOG_COORDS(1)
                
            };

            Interpolators vert (MeshData v)
{
                //float wave = sin((v.uv0.x * 8 - _Time.y * 0.2) * TAU);
                //float wave2 = sin(((1-v.uv0.y) * 8 - _Time.y * 0.2) * TAU);
                //v.vertex.y += wave * wave2 * _waveAmp;
                
                //v.vertex.y += cos(v.uv0.x * 8 + _Time.y ) *0.1;
                //v.vertex.xyz += v.normals * cos(v.uv0.x * 8 + _Time.y) *0.05;
                
                Interpolators o;
                //o.uv = v.uv0;
                o.uv = TRANSFORM_TEX(v.uv0, _MainTex);


                //height texture
                float height = tex2Dlod(_HeightTex,float4( o.uv,0 ,0)).x *2 - 1; //zero heighest lod level// *2 increase heigh -1 press down
                v.vertex.xyz += v.normals * height * _DispStrenght;

                
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.vertex = v.vertex;
                o.normal = UnityObjectToWorldNormal(v.normals);
                o.tangent = UnityObjectToWorldDir(v.tangent.xyz);
                o.bitangent = cross(o.normal, o.tangent);
                o.bitangent *= v.tangent.w * unity_WorldTransformParams.w; //correctly handle flipping and mirroring when minus scaling in unity

                
                o.worldPos =mul(UNITY_MATRIX_M, float4( v.vertex.xyz,1)); //unity_ObjectToWorld = UNITY_MATRIX_M
                
                TRANSFER_VERTEX_TO_FRAGMENT(o); //lighting

                
                //UNITY_TRANSFER_FOG(o,o.vertex);

                return o;
            }

            //v<=a return 0 (saturate) or negative, v>=b return 1
            float InverseLerp (float a, float b, float v){
                return (v-a)/(b-a);
            }


            float FadeShadows (Interpolators i, float attenuation) {
                float viewZ = dot(_WorldSpaceCameraPos - i.worldPos, UNITY_MATRIX_V[2].xyz);
                float shadowFadeDistance = UnityComputeShadowFadeDistance(i.worldPos, viewZ);
                float shadowFade = UnityComputeShadowFade(shadowFadeDistance);
                attenuation = saturate(attenuation + shadowFade);
                return attenuation;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                float4 col;

                i.uv.x += _UOffset;
                if(i.uv.x > 1) i.uv.x -=1;
                i.uv.y += _VOffset;
                if(i.uv.y > 1) i.uv.y -=1;
                

                //Texture
                float3 albedo = tex2D(_MainTex,i.uv).rgb;
                float3 surfaceColor = albedo * _ColorSecond;
                
                //fresnel effect
                //float3 N = normalize( i.normal); //normolize normals to fix explicit normal interpolation
                //float3 V = normalize(_WorldSpaceCameraPos - i.worldPos);
                //float fresnel = dot(V,N)* 2.2;
                //fresnel *= cos(TAU + _Time.y*2)*0.1+0.9;
                //fresnel = 1 - fresnel;
                //return fresnel;
                
                
                //Texture world-wide
                //float2 topDownProjection = i.worldPos.xz;
                //topDownProjection.x+=_Time.y;
                //topDownProjection.y=1-topDownProjection.y;          //vetical tex flip 
                //col = tex2D(_MainTex, frac(topDownProjection));     // sample the texture
                //return col;

                
                //texture
                float4 albedoTex = tex2D(_MainTex, i.uv);

                
                // UNITY_APPLY_FOG(i.fogCoord, col);                 // apply fog

                //float bottomRemover = abs(i.normal.y) < 0.999;
                

                //maps
                //float4 normal = float4(i.normal,1);
                //float4 uv = float4(i.uv,0,1);

                
                
                //pulsation
                float2 uvCenter = i.uv * 2-1;
                float radialDist = saturate(length(uvCenter));
                float pulseDistortion =sin(i.uv.x * 12 * TAU) * cos(i.uv.y * 12 * TAU) * _distortionAmp;
                float pulse = sin((radialDist * 8 + pulseDistortion - _Time.y * 0.2) * TAU * _pulseAmp);
                _pulseDist = 1 - _pulseDist;
                pulse *= saturate(1 - radialDist - _pulseDist);
                //radialDist=frac(saturate(0.8-radialDist));

                //return float4(pulse.xxx,1);


                //wave
                float distortion = cos(i.uv.y * 12 * TAU) * _distortionAmp;
                float wave = sin((i.uv.x * 8 + distortion - _Time.y * 0.2) * TAU *_waveAmp);
                

                col = lerp(_ColorBase,_ColorSecond, saturate(pulse) );
                //clip(col-0.001);
                //col*=1-i.uv.x; //gradient/shadow to x axis


            #ifdef BASE_PASS
                
                //return float4( diffuseLight * col + specularLight,1);
                //return float4(radialDist.xxx,1) ;
                return col /*+ _AmbientLight*/;

                
            #else

                
                #ifdef USE_LIGHTING  //if using key
                

                //textures normal
                //float3 normalMap = UnpackNormal(tex2D(_NormalTex,i.uv)) *0.5+0.5; //looks like normal map
                float3 tangentSpaceNormal = UnpackNormal(tex2D(_NormalTex,i.uv));
                tangentSpaceNormal = lerp(float3(0,0,1),tangentSpaceNormal,_NormalIntensity);

                float3x3 mtxTangToWorld = {
                    i.tangent.x, i.bitangent.x, i.normal.x,
                    i.tangent.y, i.bitangent.y, i.normal.y,
                    i.tangent.z, i.bitangent.z, i.normal.z,
                };
                float3 N = mul(mtxTangToWorld, tangentSpaceNormal);
                // float3 N = tangentSpaceNormal;
                //return float4 ( N,0);
                
                
                //pulse clipping
                //clip(pulse.xxxx-0.01);
                
                
            
                //Diffuse Lighting
                //float3 N = normalize( i.normal); //normolize normals to fix explicit normal interpolation 
                float3 L = normalize( UnityWorldSpaceLightDir(i.worldPos)); //***//direction light //_WorldSpaceLightPos0.w=0;
                float attenuation = LIGHT_ATTENUATION(i);   //***//attenuation of spot light, use LIGHTING_COORDS(,) value
                //UNITY_LIGHT_ATTENUATION(attenuation,i,i.worldPos.xyz); 
                //attenuation = FadeShadows(i,attenuation);

                
                float3 lambert = saturate( dot( N, L ) );
                float3 lightColor = _LightColor0.xyz *1.00;
                float3 diffuseLight = (lambert * attenuation) * lightColor;



                //specular lighting
                float3 V = normalize(_WorldSpaceCameraPos - i.worldPos);
                //1 specular lighting phong
                //float3 R = reflect (-L,N);
                //2 specular blin-phong
                float3 H = normalize(L+V);

                
                //float3 specularLight = saturate(dot(V,R)); //1
                float3 specularLight = saturate(dot(H,N)) * (lambert > 0); //2
                //specularLight *= lambert > 0;  //2 remove back Lighting

                float specularExponent = exp2(_Gloss * 11) + 2;
                specularLight = pow(specularLight, specularExponent); //specular exponent
                specularLight *= _Gloss; //uncorrect but energy conservation
                specularLight *= _LightColor0.xyz;
                specularLight *= attenuation;

                return float4(diffuseLight * _ColorSecond + specularLight, 1)  * pulse;

                
                #else

                
                #endif
            #endif
                return float4(0,0,0,0);
            }
            