using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalOpus.MB.Core;

namespace Dasverse.Aleo
{   
    /// <summary>
    /// 케릭터의 스킨메쉬를 하나로 합치기 위한 클래스
    /// 현재 텍스쳐 베이킹은 사용하지 않으나 추후 사용할 수 있도록 남겨둠
    /// </summary>
    public class MeshCombiner : MonoBehaviour
    {   
        protected MB3_MeshBaker meshBaker;
        //protected MB3_TextureBaker textureBaker;
        //MB3_TextureCombiner.CreateAtlasesCoroutineResult result = new MB3_TextureCombiner.CreateAtlasesCoroutineResult();

        /// <summary>
        /// 메쉬컴바이너 초기화
        /// </summary>
        public void Init(){
            meshBaker = gameObject.GetComponentInChildren<MB3_MeshBaker>();
            //textureBaker = gameObject.GetComponent<MB3_TextureBaker>();

            meshBaker.ClearMesh();
        }

        /// <summary>
        /// 메쉬 컴바인이 필요한 오브젝트들을 받아서 메쉬를 합친다.
        /// </summary>
        /// <param name="objs">콤바인할 메쉬 리스트</param>
        public void CombineMesh(List<GameObject> objs){

            // {
            //     ((MB3_MeshCombinerSingle)meshBaker.meshCombiner).SetMesh(null);
            // }

            // textureBaker.objsToMesh = objs;
            // textureBaker.textureBakeResults = ScriptableObject.CreateInstance<MB2_TextureBakeResults>();
            // textureBaker.resultMaterial = new Material(Shader.Find("Shader Graphs/Rukha93/ShaderGraph_CharacterLit"));
            
            // textureBaker.CreateAtlases();

            // textureBaker.onBuiltAtlasesSuccess = new MB3_TextureBaker.OnCombinedTexturesCoroutineSuccess(OnBuiltAtlasesSuccess);
            // StartCoroutine(textureBaker.CreateAtlasesCoroutine(null, result, false, null, .01f));

            meshBaker.AddDeleteGameObjects(objs.ToArray(), null, true);
            meshBaker.Apply();
        }

        // /// <summary>
        // /// 텍스쳐 베이킹이 완료되면 호출되는 콜백
        // /// </summary>
        // void OnBuiltAtlasesSuccess(){
        //     Debug.Log("Texture baking is complete");

        //     if(result.isFinished && result.success){
        //         meshBaker.ClearMesh();
        //         meshBaker.textureBakeResults = textureBaker.textureBakeResults;

        //         if(meshBaker.AddDeleteGameObjects(textureBaker.GetObjectsToCombine().ToArray(), null, true)){
        //             meshBaker.Apply();
        //             Debug.Log("Mesh baking is complete");
        //         }
        //     }

        //     Debug.Log("Completed baking textures on frame " + Time.frameCount);
        // }
    }
}
