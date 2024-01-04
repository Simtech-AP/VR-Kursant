using System.Collections;
using System.Linq;
using UnityEngine;

namespace QuickOutline
{
    /// <summary>
    /// Class to add outline to object
    /// </summary>
    [DisallowMultipleComponent]
    public class Outline : MonoBehaviour
    {
        /// <summary>
        /// Array of renderers to outline
        /// </summary>
        private Renderer[] renderers;
        /// <summary>
        /// Oultine material
        /// </summary>
        private Material outlineVRMaterial;

        /// <summary>
        /// Get all renderers in object children
        /// Get material for outline
        /// </summary>
        void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
            outlineVRMaterial = Instantiate(Resources.Load<Material>(@"OutlineVR"));
            outlineVRMaterial.name = "OutlineVR (Instance)";
        }

        /// <summary>
        /// Enable outline for each renderer
        /// </summary>
        void OnEnable()
        {
            foreach (var renderer in renderers)
            {
                var materials = renderer.sharedMaterials.ToList();
                materials.Add(outlineVRMaterial);
                renderer.materials = materials.ToArray();
            }
        }

        /// <summary>
        /// Disable outline for each renderer
        /// </summary>
        void OnDisable()
        {
            foreach (var renderer in renderers)
            {
                // Remove outline shaders
                var materials = renderer.sharedMaterials.ToList();
                materials.Remove(outlineVRMaterial);
                renderer.materials = materials.ToArray();
            }
        }

        /// <summary>
        /// Clean up after destroying
        /// </summary>
        void OnDestroy()
        {
            Destroy(outlineVRMaterial);
        }

        /// <summary>
        /// Starts blinking of an outline
        /// </summary>
        public void Blink()
        {
            StartCoroutine(BlinkOutline());
        }

        /// <summary>
        /// Main coroutine for blinking outline
        /// </summary>
        /// <returns>Handle for coroutine</returns>
        private IEnumerator BlinkOutline()
        {
            while (true)
            {
                foreach (var renderer in renderers)
                {
                    renderer.sharedMaterials[1].SetFloat("_Strength", 0.5f);
                }
                yield return new WaitForSeconds(0.5f);
                foreach (var renderer in renderers)
                {
                    renderer.sharedMaterials[1].SetFloat("_Strength", 1.5f);
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}