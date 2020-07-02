
using System.Collections.Generic;
using XLua;

public static class WrapDefines
{
    [LuaCallCSharp]
    public static List<System.Type> WrapList
    {
        get
        {
            return new List<System.Type>()
            {
                typeof(System.Object),
                typeof(System.Type),
#if !UNITY_2018_3_OR_NEWER
                typeof(UnityEngine.WWW),
#endif
                typeof(UnityEngine.Object),
                typeof(UnityEngine.Shader),
                typeof(UnityEngine.Renderer),
                typeof(UnityEngine.Collider),
                typeof(UnityEngine.Texture),
                typeof(UnityEngine.GameObject),
                typeof(UnityEngine.Transform),
                typeof(UnityEngine.RectTransform),
                typeof(UnityEngine.Component),
                typeof(UnityEngine.Behaviour),
                typeof(UnityEngine.MonoBehaviour),
                typeof(UnityEngine.Camera),
                typeof(UnityEngine.Animator),
                typeof(UnityEngine.Animation),
                typeof(UnityEngine.Material),
                typeof(UnityEngine.Projector),
                
                //Unity Static
                typeof(UnityEngine.Time),
                typeof(UnityEngine.Application),
                typeof(UnityEngine.PlayerPrefs),
                typeof(UnityEngine.SystemInfo),
                typeof(UnityEngine.Screen),
                typeof(UnityEngine.RenderSettings),
                typeof(UnityEngine.QualitySettings),
                
                //Unity NavMesh
                typeof(UnityEngine.AI.NavMeshAgent),
                typeof(UnityEngine.AI.NavMeshObstacle),
                
                //UGUI Base
                typeof(UnityEngine.Canvas),
                typeof(UnityEngine.CanvasGroup),
                typeof(UnityEngine.EventSystems.UIBehaviour),
                typeof(UnityEngine.EventSystems.BaseEventData),
                typeof(UnityEngine.EventSystems.PointerEventData),
                typeof(UnityEngine.UI.Graphic),
                typeof(UnityEngine.UI.MaskableGraphic),
                typeof(UnityEngine.UI.Text),
                typeof(UnityEngine.UI.Image),
                typeof(UnityEngine.UI.RawImage),
                typeof(UnityEngine.UI.Selectable),
                typeof(UnityEngine.UI.Button),
                typeof(UnityEngine.UI.Toggle),
                typeof(UnityEngine.UI.Slider),
                typeof(UnityEngine.UI.Dropdown),
                typeof(UnityEngine.UI.Dropdown.OptionData),
                typeof(UnityEngine.UI.ScrollRect),
                typeof(UnityEngine.UI.Scrollbar),
                typeof(UnityEngine.UI.InputField)
                
                // UGUI Override
                
                //Tween
                
                //Other
            };
        }
    }
    
    [BlackList]
    public static List<List<string>> BlackList = new List<List<string>>()
    {
        new List<string>(){"System.Type", "IsSZArray"},
        
        new List<string>() { "UnityEngine.MonoBehaviour", "runInEditMode" },
        new List<string>() { "UnityEngine.Texture", "imageContentsHash" },
        new List<string>() { "UnityEngine.UI.Graphic", "OnRebuildRequested" },
        new List<string>() { "UnityEngine.UI.Text", "OnRebuildRequested" },
        new List<string>() { "UnityEngine.WWW", "GetMovieTexture" },
        new List<string>() { "UnityEngine.QualitySettings", "streamingMipmapsRenderersPerFrame" },
    };
}
