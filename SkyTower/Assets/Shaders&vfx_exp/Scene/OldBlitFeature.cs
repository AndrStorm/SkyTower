/*using UnityEngine; 
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlitRenderPassFeature : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {

        public RenderTargetIdentifier source;
        private Material material;
        
        private RenderTargetHandle tempRenderTargetHandle; //texture buffer

        public CustomRenderPass(Material material)
        {
            this.material = material;
            tempRenderTargetHandle.Init("_TempColorTexture");
        }
        
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer commandBuffer = CommandBufferPool.Get("Custom Blit Pass");
            
            Blit(commandBuffer,source,source, material);  //works without texture buffer
            // commandBuffer.GetTemporaryRT(tempRenderTargetHandle.id,renderingData.cameraData.cameraTargetDescriptor);
            // Blit(commandBuffer,source,tempRenderTargetHandle.Identifier(), material);
            // Blit(commandBuffer,tempRenderTargetHandle.Identifier(),source, material);

            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
               

        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    [System.Serializable]
    public class Settings {
        public Material material = null;
    }

    public Settings settings = new Settings();

    CustomRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(settings.material);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ScriptablePass.source = renderer.cameraColorTarget;
        
        renderer.EnqueuePass(m_ScriptablePass);
    }
}*/
