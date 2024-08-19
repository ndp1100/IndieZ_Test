using UnityEngine;

namespace Rowlan.AnimationPreviewPro
{
    public class PreviewClip
    {
        public AnimationClip clip;
        public string layerName;
        public string stateName;

        public string GetFullPath()
        {
            return layerName + "." + stateName;
        }

        public string GetDisplayName()
        {
            if (stateName != null)
                return stateName;

            return clip.name;
        }
    }

}